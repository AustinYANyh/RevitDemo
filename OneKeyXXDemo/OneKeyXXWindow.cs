using HWCommonFunction.CommonFunction;
using HWCommonFunction.RevitCmdWrapTool;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.UI;

namespace OneKeyXXTest
{
    public partial class OneKeyXXWindow : CmdWrapTopLeftUI
    {
        public UIApplication m_app { get; set; }
        public OneKeyXXWindow(RevitCmdWrap cmd, UIApplication app) : base(cmd)
        {
            m_app = app;
            InitializeComponent();
        }

        protected override void LayoutUI()
        {
            OneKeyXXPanel panel = new OneKeyXXPanel();
            this.Controls.Add(panel);

            string tooltip = "这是表示提示信息的内容";
            InitWindow("测试一键XX", tooltip, panel.Width, panel.Height, "一键XX");
        }

        public override bool CheckInputValid()
        {
            return true;
        }
    }
}
