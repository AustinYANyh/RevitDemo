using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HW.Collaborate.Framework
{
    /// <summary>
    /// 日志基类
    /// </summary>
    public interface ILoger
    {
        /// <summary>
        /// 输出常规日志
        /// </summary>
        /// <param name="txt"></param>
        void Log(string txt);
        /// <summary>
        /// 输出核心调试日志
        /// </summary>
        /// <param name="txt"></param>
        void CoreLog(string txt);
        /// <summary>
        /// 输出警告信息
        /// </summary>
        /// <param name="warnning"></param>
        void Warnning(string warnning, string detail = null);
        /// <summary>
        /// 输出错误
        /// </summary>
        /// <param name="error"></param>
        void Error(string error, string detail = null);
        /// <summary>
        /// 输出异常
        /// </summary>
        /// <param name="ex"></param>
        void Error(Exception ex);
    }

    class Logger : ILoger
    {
        readonly string LOG_PATH = null;

        /// <summary>
        /// 是否保存常规日志
        /// </summary>
        readonly bool _bSaveCommon = true;
        /// <summary>
        /// 是否保存核心调试日志
        /// </summary>
        readonly bool _bSaveCore = false;
        /// <summary>
        /// 当前进程ID
        /// </summary>
        readonly int _curProcessID = -1;
        Logger()
        {
            var dir = Path.GetDirectoryName(typeof(Logger).Assembly.Location);

            LOG_PATH = Path.Combine(dir, "log.txt");

            var logConfig = Path.Combine(Path.GetDirectoryName(dir), @"Config\LogConfig.ini");
            if (File.Exists(logConfig))
            {
                const string SectionName = "Config";
                const string KeyCommon = "Common";
                const string KeyCore = "Core";
            }
            _curProcessID = System.Diagnostics.Process.GetCurrentProcess().Id;
        }
        public static readonly ILoger Instance = new Logger();

        public void Log(string txt)
        {
            WriteLine(txt, _bSaveCommon);
        }
        public void CoreLog(string txt)
        {
            WriteLine(txt, _bSaveCore);
        }
        public void Warnning(string warnning, string detail = null)
        {
            if (warnning != null)
            {
                WriteLine("[WARNNING]" + warnning, true);

                if (!string.IsNullOrEmpty(detail))
                    WriteLine(detail, true);
            }
        }

        public void Error(string error, string detail = null)
        {
            if (error != null)
            {
                WriteLine("[ERROR]" + error, true);
                if (!string.IsNullOrEmpty(detail))
                    WriteLine(detail, true);
            }
        }

        public void Error(Exception ex)
        {
            if (ex != null)
            {
                WriteLine(ex.ToString(), true);
            }
        }

        protected virtual void WriteLine(string line, bool writeToFile)
        {
            var temp = string.Format("{0} [{1}] {2}{3}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), _curProcessID, (line ?? string.Empty), Environment.NewLine);

            System.Diagnostics.Trace.Write(temp);

            if (writeToFile)
                WriteToFile(temp);
        }
        /// <summary>
        /// 进程间同步
        /// </summary>
        const string MutexKey = "cd3f144b-0fae-464a-84be-0a192a4842ab";
        void WriteToFile(string line)
        {
            using (Mutex mut = new Mutex(false, MutexKey))
            {
                try
                {
                    mut.WaitOne();

                    // 如果日志文件超过3M，删除之
                    if (File.Exists(LOG_PATH))
                    {
                        var info = new FileInfo(LOG_PATH);
                        var m = info.Length / 1024 / 1024;
                        if (m > 3)
                            File.Delete(LOG_PATH);
                    }

                    File.AppendAllText(LOG_PATH, line);
                }
                //当其他进程已上锁且没有正常释放互斥锁时(譬如进程忽然关闭或退出)，则会抛出AbandonedMutexException异常
                catch (AbandonedMutexException ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.Message);
                }
                finally
                {
                    mut.ReleaseMutex();
                }
            }
        }
    }
}
