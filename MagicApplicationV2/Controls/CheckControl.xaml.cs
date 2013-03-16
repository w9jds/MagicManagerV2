using MagicApplicationV2.Windows;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
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
    /// Interaction logic for CheckControl.xaml
    /// </summary>
    public partial class CheckControl : UserControl
    {
        public delegate void CheckControlDelegate(object parent);
        public CheckControlDelegate CloseWin;

        private PopupWin Parent;

        public CheckControl(Window parent)
        {
            this.Parent = parent as PopupWin;
            InitializeComponent();
        }

        private async void Yes_Click(object sender, RoutedEventArgs e)
        {
            CreateNewODB();
            CloseWin(Parent);
        }

        private async Task CreateNewODB()
        {
            string DocLocation = System.Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Documents\\Magic Manager\\";
            
            if (!System.IO.Directory.Exists(DocLocation))
                System.IO.Directory.CreateDirectory(DocLocation);

            DocLocation += "MyCards.mmodb";

            ADOX.Catalog CreateDB = new ADOX.Catalog();
            CreateDB.Create("Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + DocLocation + "; Jet OLEDB:Engine Type=5");

            ADOX.Table CardTable = new ADOX.Table();
            CardTable.Name = "MyCards";
            CardTable.Columns.Append("MultiverseID");
            CardTable.Columns.Append("Name");
            CardTable.Columns.Append("Expansion");
            CardTable.Columns.Append("stdAmount");
            CardTable.Columns.Append("foilAmount");
            CreateDB.Tables.Append(CardTable);

            //OleDbConnection DBcon = CreateDB.ActiveConnection as OleDbConnection;
            //if (DBcon != null)
            //    DBcon.Close();

            //Marshal.ReleaseComObject(CreateDB.ActiveConnection);
            //Marshal.ReleaseComObject(CreateDB);
            //GC.Collect();
            //GC.WaitForPendingFinalizers();

            Properties.Settings.Default.OwnedDatabase = DocLocation;
            Properties.Settings.Default.Save();

        }

        private void No_Click(object sender, RoutedEventArgs e)
        {
            CloseWin(this.Parent);
        }
    }
}
