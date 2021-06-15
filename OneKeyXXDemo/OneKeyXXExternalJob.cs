/*============================================================================================
   *   Copyright (C) 2021 All rights reserved.
   *   
   *   文件名称：OneKeyXXExternalJobS.cs
   *   创 建 者：yanyunhao
   *   创建日期：2021/6/15 17:12:37
   *   描    述：
   *             
==============================================================================================*/
using HWCommonFunction.CommonFunction;
using HWCommonFunction.RevitCmdWrapTool;
using HWTransCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OneKeyXXTest
{
    public class OneKeyXXExternalJobS
    {
        private void PrepareJob()
        {

        }

        //------------------------------------------------------【DoJob( )函数】---------------------------
        //描述：OneKeyXX的外部命令Excute后执行的方法
        //------------------------------------------------------------------------------------------------------------
        public void DoJob(Autodesk.Revit.UI.UIApplication app)
        {
            RevitCmdWrap cmd = RevitCmdWrap.CreateRevitCmdWrap(app, new Autodesk.Revit.DB.Color(255, 0, 255), PressOk);
            CmdWrapTopLeftUI ui = GetShowUI(cmd,app);
            WindowTool.ShowFormModeless(ui);
            cmd.RunCommand();
        }

        protected virtual CmdWrapTopLeftUI GetShowUI(RevitCmdWrap cmd, Autodesk.Revit.UI.UIApplication app)
        {
            CmdWrapTopLeftUI ui = new OneKeyXXWindow(cmd, app);
            return ui;
        }

        //------------------------------------------------------【PressOk( )函数】---------------------------
        //描述：一键XX按钮被点击后的回调
        //------------------------------------------------------------------------------------------------------------
        private bool PressOk(RevitCmdWrap cmd)
        {
            MessageBox.Show("一键XX按钮被点击");
            return true;
        }
    }
}
