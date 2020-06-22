using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace wmsDH
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void Popup_Opened(object sender, EventArgs e)
        {
            Popup pop = (Popup)sender;
            TextBlock popupText = new TextBlock();
            popupText.Text = "Popup Text";
            popupText.Background = Brushes.LightBlue;
            popupText.Foreground = Brushes.Blue;
            pop.Child = popupText;
            //Popup.CreateRootPopup(pop, popupText);
            
        }
    }
}
