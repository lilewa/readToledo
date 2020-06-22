using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using static wmsDH.DataAccess.ExecuteFromXml;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace wmsDH
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
             
            var task = ExecuteReturnTb("Common.Common", "t_user2", new Dictionary<string, object> { { "p_czybh", "0001" } });
            await task;
            if (!string.IsNullOrWhiteSpace(task.Result.Item2))
            {
                MessageBox.Show(task.Result.Item2);
                return;
            }
            DataTable dt = task.Result.Item1;
          
        }


    }
}
