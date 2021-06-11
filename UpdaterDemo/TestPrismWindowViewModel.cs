using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AddinTest
{
    public class TestPrismWindowViewModel : NotificationObject
    {
        public Window m_window { get; set; }

        public DelegateCommand TestCommand { get; set; }

        public Action DoWork { get; set; }

        public TestPrismWindowViewModel()
        {
            TestCommand = new DelegateCommand(() =>
            {
                MessageBox.Show("测试按钮被点击");
                m_window.Close();

                DoWork?.Invoke();
            });
        }
    }
}
