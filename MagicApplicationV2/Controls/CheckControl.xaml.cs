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
        private PopupWin Parent;

        public CheckControl(Window parent)
        {
            this.Parent = parent as PopupWin;
            InitializeComponent();
        }

        /// <summary>
        /// Run the create new Database function Async so it doesn't lock up the UI (Doesn't wait) then closes window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Yes_Click(object sender, RoutedEventArgs e)
        {
            CreateNewODB();
            Parent.Close();
        }

        /// <summary>
        /// Creates a Database in the My Documents folder of your computer for the current user. (Based on Windows 8 / Windows 7)
        /// </summary>
        /// <returns></returns>
        private async Task CreateNewODB()
        {
            string DocLocation = System.Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Documents\\Magic Manager\\";
            
            if (!System.IO.Directory.Exists(DocLocation))
                System.IO.Directory.CreateDirectory(DocLocation);

            DocLocation += "MyCards.mmodb";

            ADOX.Catalog CreateDB = new ADOX.Catalog();
            CreateDB.Create("Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + DocLocation + "; Jet OLEDB:Engine Type=5");

            try
            {
                ADOX.Table CardTable = new ADOX.Table();
                CardTable.Name = "MyCards";
                CardTable.Columns.Append("MultiverseID");
                CardTable.Columns.Append("OwnedAmount", ADOX.DataTypeEnum.adInteger);
                CardTable.Columns["OwnedAmount"].Attributes = ADOX.ColumnAttributesEnum.adColNullable;
                CardTable.Columns.Append("WishOwnedAmount", ADOX.DataTypeEnum.adInteger);
                CardTable.Columns["WishOwnedAmount"].Attributes = ADOX.ColumnAttributesEnum.adColNullable;
                CardTable.Columns.Append("FoilOwnedAmount", ADOX.DataTypeEnum.adInteger);
                CardTable.Columns["FoilOwnedAmount"].Attributes = ADOX.ColumnAttributesEnum.adColNullable;

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
            catch (Exception) { }

        }

        private void No_Click(object sender, RoutedEventArgs e)
        {
            Parent.Close();
        }
    }
}
