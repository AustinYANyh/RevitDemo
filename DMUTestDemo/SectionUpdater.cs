/*============================================================================================
   *   Copyright (C) 2021 All rights reserved.
   *   
   *   文件名称：SectionUpdater.cs
   *   创 建 者：yanyunhao
   *   创建日期：2021/6/24 10:00:46
   *   描    述：
==============================================================================================*/
using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Windows;
using System.Diagnostics;
using HW.Collaborate.Framework;

namespace DMUTest
{
    /// <summary>
    /// Updater to automatically move a section in conjunction with the location of a window
    /// </summary>
    public class SectionUpdater : IUpdater
    {
        private object mutex { get; set; } = new object();
        private List<ElementId> m_id_list { get; set; } = new List<ElementId>();

        private readonly int sleep_time = 30 * 1000;

        private AutoResetEvent reset_event = new AutoResetEvent(false);
        private CancellationTokenSource source = new CancellationTokenSource();

        internal SectionUpdater(AddInId addinID)
        {
            m_updaterId = new UpdaterId(addinID, Guid.NewGuid());

            reset_event.Reset();
            CancellationToken token = source.Token;

            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (token.IsCancellationRequested) return;

                    Monitor.Enter(mutex);

                    string log = string.Join(" ",
                        DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"), "变化的Element个数: ", m_id_list.Count,"当前Revit是否响应:",
                        Process.GetCurrentProcess().Responding);
                    Logger.Instance.Log(log);

                    m_id_list.Clear();

                    Monitor.Exit(mutex);

                    reset_event.WaitOne(sleep_time);
                }
            }, token);
        }

        internal void Register()
        {
            try
            {
                UpdaterRegistry.RegisterUpdater(this);
                UpdaterRegistry.EnableUpdater(m_updaterId);

                LogicalOrFilter filter = new LogicalOrFilter(
                      new ElementIsElementTypeFilter(false),
                      new ElementIsElementTypeFilter(true));

                UpdaterRegistry.AddTrigger(m_updaterId, filter, Element.GetChangeTypeAny());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        internal void UnRegister()
        {
            reset_event.Set();
            source.Cancel();
            UpdaterRegistry.DisableUpdater(m_updaterId);
        }

        #region IUpdater members

        // The Execute method for the updater
        public void Execute(UpdaterData data)
        {
            try
            {
                Document doc = data.GetDocument();
                Action<ICollection<ElementId>> AddIdToList = (list) =>
                {
                    foreach (ElementId id in list)
                    {
                        Monitor.Enter(mutex);
                        m_id_list.Add(id);
                        Monitor.Exit(mutex);
                    }
                };
                AddIdToList(data.GetAddedElementIds());
                AddIdToList(data.GetDeletedElementIds());
                AddIdToList(data.GetModifiedElementIds());
            }
            catch (System.Exception ex)
            {
                TaskDialog.Show("Exception", ex.ToString());
            }
            return;
        }

        public UpdaterId GetUpdaterId()
        {
            return m_updaterId;
        }

        public string GetUpdaterName()
        {
            return "Associative Section Updater";
        }

        public string GetAdditionalInformation()
        {
            return "Automatically moves a section to maintain its position relative to a window";
        }

        public ChangePriority GetChangePriority()
        {
            return ChangePriority.Views;
        }

        #endregion

        private UpdaterId m_updaterId = null;
    }

}
