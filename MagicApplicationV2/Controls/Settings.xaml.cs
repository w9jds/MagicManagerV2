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
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : UserControl
    {
        public delegate void SettingsWinDelegate(object parent);
        public SettingsWinDelegate CloseWindow;
        public SettingsWinDelegate SaveCloseWindow;

        private PopupWin Popup = new PopupWin();

        public Settings(Window PopupWinIn)
        {
            Popup = PopupWinIn as PopupWin;
            InitializeComponent();
        }

        private void TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            SettingsGrid.DataContext = Properties.Settings.Default;
        }

        private void CardDBbtn_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog
            Microsoft.Win32.OpenFileDialog OFD = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension
            OFD.DefaultExt = ".mmcdb";
            OFD.Filter = "Magic Manager Card Database |*.mmcdb";

            // Display OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = OFD.ShowDialog();

            // Get the selected file name and display in a TextBox
            if (result == true)
            {
                // Open document
                string filename = OFD.FileName;
                CardDBPath.Text = filename;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (CardDBPath.Text != Properties.Settings.Default.DatabaseLocation)
            {
                Properties.Settings.Default.DatabaseLocation = CardDBPath.Text;
                Properties.Settings.Default.Save();
                SaveCloseWindow(Popup);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            CloseWindow(Popup);
        }
    }
}
