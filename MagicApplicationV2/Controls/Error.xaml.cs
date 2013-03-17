using MagicApplicationV2.Windows;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MagicApplicationV2.Controls
{
    /// <summary>
    /// Interaction logic for Error.xaml
    /// </summary>
    public partial class Error : UserControl
    {
        private PopupWin Popup = new PopupWin();

        public Error(Window Parent)
        {
            Popup = Parent as PopupWin;
            InitializeComponent();
        }

        private void Okay_Click(object sender, RoutedEventArgs e)
        {
            Popup.Close();
        }
    }
}
