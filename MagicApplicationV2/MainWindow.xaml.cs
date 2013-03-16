using MagicApplicationV2.Controls;
using MagicApplicationV2.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
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
            Cards.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(UpdateCardsListView_Event);
            Properties.Settings.Default.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(UpdateEverything_Event);
        }

        private async void UpdateEverything_Event(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "OwnedDatabase")
            {


            }
            if (e.PropertyName == "DatabaseLocation")
                GetCards();

        }

        /// <summary>
        /// When the Cards List is changed it updates the Cards ListView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateCardsListView_Event(object sender, NotifyCollectionChangedEventArgs e)
        {
            //this.CardsList.Items.Clear();
            this.CardsList.ItemsSource = Cards;
        }

        /// <summary>
        /// When the window is loaded it check to see if you have a Owned Database path and if not asks if you want to set one up. 
        /// Also Gets all the cards from the database and adds to the Cards list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        { 
            GetCards();

            if (Properties.Settings.Default.OwnedDatabase == "" || System.IO.File.Exists(Properties.Settings.Default.OwnedDatabase) == false)
            {
                PopupWin NewODB = new PopupWin();
                CheckControl NewODBText = new CheckControl(NewODB);
                NewODBText.PopupText.Text = "There is currently no database selected containing a list of your owned cards. \nWould you like to create a new one?";
                NewODB.ControlGrid.Children.Add(NewODBText);
                NewODB.Show();
                NewODBText.CloseWin += new CheckControl.CheckControlDelegate(CloseWindow);
            }
        }

        /// <summary>
        /// When a card is double clicked this displays a window with the cards info and picture.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CardsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            CardWin ViewCard = new CardWin(Cards[CardsList.SelectedIndex]);
            ViewCard.Show();
        }

        /// <summary>
        /// This calls out to the Database with all the cards and stores all of them in the cards list.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Brings up the settings window when the settings option in the menu is selected.
        /// Also creates event handlers for the close of zed window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Settings_Clicked(object sender, RoutedEventArgs e)
        {
            PopupWin SettingsWin = new PopupWin();
            Settings SettingsControl = new Settings(SettingsWin);

            SettingsWin.ControlGrid.Children.Add(SettingsControl);
            SettingsWin.Show();

            SettingsControl.CloseWindow += new Settings.SettingsWinDelegate(CloseWindow);
            //SettingsControl.SaveCloseWindow += new Settings.SettingsWinDelegate(UpdateCloseWindow); Not Needed with new event handler
        }

        /// <summary>
        /// Closes the Popup window that is passed in.
        /// </summary>
        /// <param name="parent"></param>
        private void CloseWindow(object parent)
        {
            PopupWin ParentWin = parent as PopupWin;
            ParentWin.Close();
        }

        /// <summary>
        /// Closes the Popup window that is passed in when the settings are changed.
        /// Also updates the Cards list.
        /// </summary>
        /// <param name="parent"></param>
        //private async void UpdateCloseWindow(object parent) Not needed with new event handler
        //{
        //    PopupWin SettingsWin = parent as PopupWin;
        //    SettingsWin.Close();
        //    GetCards();
        //}

        /// <summary>
        /// Starts the filter statement asyncronusly (Doesn't wait for it to finish)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Search_Click(object sender, RoutedEventArgs e)
        {
            FilterDB();
        }

        /// <summary>
        /// Based on the info in the search boxes this creates a query statement that filter the database. 
        /// The return is then stored in the Cards list and updated to cards view.
        /// </summary>
        /// <returns></returns>
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

            if (CardTypeCheck.IsChecked == true && CardTypeBox.Text != "")
                Filter.Add("%" + CardTypeBox.Text + "%");
            else
                Filter.Add("%");

            //if (CardPowerCheck.IsChecked == true && CardTypeBox.Text != "")
            //    Filter.Add("Power = '" + CardPowerBox.Text + "'");
            //else
            //    Filter.Add("");

            //if (CardToughnessCheck.IsChecked == true && CardToughnessBox.Text != "")
            //    Filter.Add("Toughness = '" + CardToughnessBox.Text + "'");
            //else
            //    Filter.Add("");

            try
            {
                OleDbConnection DBCon = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + Properties.Settings.Default.DatabaseLocation);
                await DBCon.OpenAsync();
                
                OleDbDataAdapter CardDA = new OleDbDataAdapter("SELECT * FROM Cards WHERE Name LIKE '" + Filter[0] + "' and Expansion LIKE '" + Filter[1] + "' and Type LIKE '"
                    + Filter[2] + "'", DBCon);
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
                //PopupWin ErrorWin = new PopupWin();
                //Error ErrorControl = new Error(ErrorWin);
                //ErrorWin.ControlGrid.Children.Add(ErrorControl);
                //ErrorControl.ErrorText.Text = "There was a problem connection to the cards database. Please go to File > Settings and make sure the path is correct.";
                //ErrorWin.Show();
                //ErrorControl.CloseWin += new Error.ErrorControlDelegate(CloseWindow);
            };
        }
    }
}
