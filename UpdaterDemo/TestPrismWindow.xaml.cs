using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AddinTest
{
    /// <summary>
    /// TestPrismWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TestPrismWindow : Window
    {
        public TestPrismWindowViewModel m_view_model { get; set; }

        public TestPrismWindow()
        {
            InitializeComponent();
            m_view_model = new TestPrismWindowViewModel() { m_window = this };
            this.DataContext = m_view_model;
        }
    }
}
