using MagicApplicationV2.Controls;
using MagicApplicationV2.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private ObservableCollection<CardListData> Cards = new ObservableCollection<CardListData>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        { 
           GetCards();
        }

        private void CardsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            CardWin ViewCard = new CardWin(Cards[CardsList.SelectedIndex]);
            ViewCard.Show();
        }

        private async Task GetCards()
        {
            try
            {
                OleDbConnection DBCon = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + Properties.Settings.Default.DatabaseLocation);
                await DBCon.OpenAsync();

                OleDbDataAdapter CardDA = new OleDbDataAdapter("SELECT * FROM Cards", DBCon);
                DataSet CardDS = new DataSet();
                CardDA.Fill(CardDS);
                DBCon.Close();

                for (int i = 0; i < CardDS.Tables[0].Rows.Count; i++)
                    Cards.Add(new CardListData
                    {
                        MultiverseID = CardDS.Tables[0].Rows[i]["MultiverseID"].ToString(),
                        CardName = CardDS.Tables[0].Rows[i]["Name"].ToString(),
                        CardExpansion = CardDS.Tables[0].Rows[i]["Expansion"].ToString(),
                        CardImg = CardDS.Tables[0].Rows[i]["ImgURL"].ToString(),
                        Rarity = CardDS.Tables[0].Rows[i]["Rarity"].ToString(),
                        ConvMana = CardDS.Tables[0].Rows[i]["ConvManaCost"].ToString(),
                        Type = CardDS.Tables[0].Rows[i]["Type"].ToString(),
                        Power = CardDS.Tables[0].Rows[i]["Power"].ToString(),
                        Toughness = CardDS.Tables[0].Rows[i]["Toughness"].ToString()
                    });

                this.CardsList.ItemsSource = Cards;
            }
            catch (Exception) {
                PopupWin ErrorWin = new PopupWin();
                Error ErrorControl = new Error(ErrorWin);
                ErrorWin.ControlGrid.Children.Add(ErrorControl);
                ErrorControl.ErrorText.Text = "There was a problem connection to the cards database. Please go to File > Settings and make sure the path is correct.";
                ErrorWin.Show();
                ErrorControl.CloseWin += new Error.ErrorControlDelegate(CloseWindow);
            };
        }

        private void CardsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CardTab.DataContext = Cards[CardsList.SelectedIndex];
            CardImg.Source = new BitmapImage(new Uri(Cards[CardsList.SelectedIndex].CardImg.ToString(), UriKind.Absolute));
        }

        private void Settings_Clicked(object sender, RoutedEventArgs e)
        {
            PopupWin SettingsWin = new PopupWin();
            Settings SettingsControl = new Settings(SettingsWin);

            SettingsWin.ControlGrid.Children.Add(SettingsControl);
            SettingsWin.Show();

            SettingsControl.CloseWindow += new Settings.SettingsWinDelegate(CloseWindow);
            SettingsControl.SaveCloseWindow += new Settings.SettingsWinDelegate(UpdateCloseWindow);
        }

        private void CloseWindow(object parent)
        {
            PopupWin ParentWin = parent as PopupWin;
            ParentWin.Close();
        }

        private async void UpdateCloseWindow(object parent)
        {
            PopupWin SettingsWin = parent as PopupWin;
            SettingsWin.Close();
            GetCards();
        }

        private async void Search_Click(object sender, RoutedEventArgs e)
        {
            FilterDB();
        }

        private async Task FilterDB()
        {
            List<string> Filter = new List<string>();

            if (CardNameCheck.IsChecked == true && CardNameBox.Text != "")
                Filter.Add("%" + CardNameBox.Text + "%");
            else
                Filter.Add("%");
            
            if (CardExpanCheck.IsChecked == true && CardExpanBox.Text != "")
                Filter.Add("%" + CardExpanBox.Text + "%");
            else
                Filter.Add("%");


            try
            {
                OleDbConnection DBCon = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + Properties.Settings.Default.DatabaseLocation);
                await DBCon.OpenAsync();
                
                OleDbDataAdapter CardDA = new OleDbDataAdapter("SELECT * FROM Cards WHERE Name LIKE '" + Filter[0] + "' and Expansion LIKE '" + Filter[1] + "'", DBCon);
                DataSet CardDS = new DataSet();
                CardDA.Fill(CardDS);
                DBCon.Close();

                Cards.Clear();

                for (int i = 0; i < CardDS.Tables[0].Rows.Count; i++)
                        Cards.Add(new CardListData
                        {
                            MultiverseID = CardDS.Tables[0].Rows[i]["MultiverseID"].ToString(),
                            CardName = CardDS.Tables[0].Rows[i]["Name"].ToString(),
                            CardExpansion = CardDS.Tables[0].Rows[i]["Expansion"].ToString(),
                            CardImg = CardDS.Tables[0].Rows[i]["ImgURL"].ToString(),
                            Rarity = CardDS.Tables[0].Rows[i]["Rarity"].ToString(),
                            ConvMana = CardDS.Tables[0].Rows[i]["ConvManaCost"].ToString(),
                            Type = CardDS.Tables[0].Rows[i]["Type"].ToString(),
                            Power = CardDS.Tables[0].Rows[i]["Power"].ToString(),
                            Toughness = CardDS.Tables[0].Rows[i]["Toughness"].ToString()
                        });
            }
            catch (Exception) {
                PopupWin ErrorWin = new PopupWin();
                Error ErrorControl = new Error(ErrorWin);
                ErrorWin.ControlGrid.Children.Add(ErrorControl);
                ErrorControl.ErrorText.Text = "There was a problem connection to the cards database. Please go to File > Settings and make sure the path is correct.";
                ErrorWin.Show();
                ErrorControl.CloseWin += new Error.ErrorControlDelegate(CloseWindow);
            };


        }
    }
}
