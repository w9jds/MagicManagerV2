using System;
using System.Collections.Generic;
using System.Data;
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

namespace MagicApplicationV2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {        
            CardList();
        }

        private void CardList()
        {
            OleDbConnection DBCon = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + Properties.Settings.Default.DatabaseLocation);
            DBCon.Open();

            OleDbDataAdapter CardDA = new OleDbDataAdapter("SELECT MultiverseID, Name, Expansion FROM Cards", DBCon);
            DataSet CardDS = new DataSet();
            CardDA.Fill(CardDS);

            List<CardListData> Cards = new List<CardListData>();

            for (int i = 0; i < CardDS.Tables[0].Rows.Count; i++)
                Cards.Add(new CardListData {MultiverseID = CardDS.Tables[0].Rows[i]["MultiverseID"].ToString(), CardName = CardDS.Tables[0].Rows[i]["Name"].ToString(), CardExpansion = CardDS.Tables[0].Rows[i]["Expansion"].ToString()});

            CardsList.ItemsSource = Cards;
        }
    }
}
