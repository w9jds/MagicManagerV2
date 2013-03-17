using MagicApplicationV2.Classes;
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
        private ObservableCollection<OwnedCardListData> MyCards = new ObservableCollection<OwnedCardListData>();

        public MainWindow()
        {
            InitializeComponent();
            
            Cards.CollectionChanged += new NotifyCollectionChangedEventHandler(UpdateCardsListView_Event);
            MyCards.CollectionChanged += new NotifyCollectionChangedEventHandler(UpdateMyCardsListView_Event);
            Properties.Settings.Default.PropertyChanged += new PropertyChangedEventHandler(UpdateEverything_Event);
        }

        /// <summary>
        /// When one of the settings is changed it calls here. This updates the ListViews for the setting that threw this event.
        /// </summary>
        private async void UpdateEverything_Event(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "OwnedDatabase")
                GetOwnedCards();
            if (e.PropertyName == "DatabaseLocation")
                GetCards();
        }

        /// <summary>
        /// When the Cards List is changed it updates the Cards ListView
        /// </summary>
        private void UpdateCardsListView_Event(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.CardsList.ItemsSource = Cards;
        }

        /// <summary>
        /// When the MyCards List is changed it updates the MyCards ListView
        /// </summary>
        private void UpdateMyCardsListView_Event(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.MyCardsList.ItemsSource = MyCards;
        }

        /// <summary>
        /// When the window is loaded it check to see if you have a Owned Database path and if not asks if you want to set one up. 
        /// Also Gets all the cards from the database and adds to the Cards list.
        /// </summary>
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        { 
            GetCards();

            if (Properties.Settings.Default.OwnedDatabase == "" || System.IO.File.Exists(Properties.Settings.Default.OwnedDatabase) == false)
            {
                PopupWin NewODB = new PopupWin();
                CheckControl NewODBText = new CheckControl(NewODB);
                NewODBText.PopupText.Text = "There is currently no database selected containing a list of your owned cards. \nWould you like to create a new one?";
                NewODB.ControlGrid.Children.Add(NewODBText);
                NewODB.ShowDialog();
            }
            else
                GetOwnedCards();
        }

        /// <summary>
        /// Calls the AddCard function to run async (Doesn't wait)
        /// </summary>
        private async void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            AddCard(CardsList.SelectedIndex);
        }

        /// <summary>
        /// Calls the RemoveCard function to run async (Doesn't wait)
        /// </summary>
        private async void RemoveBtn_Click(object sender, RoutedEventArgs e)
        {
            RemoveCard(MyCardsList.SelectedIndex);
        }

        /// <summary>
        /// Adds the currently selected card in the CardsListView to the MyCards Database.
        /// </summary>
        /// <returns></returns>
        private async Task AddCard(int Index)
        {
            bool Duplicate = false;
            int DuplicateIndex = 0;

            for (int i = 0; i < MyCards.Count; i++)
            {
                if (MyCards[i].MultiverseID.Equals(Cards[Index].MultiverseID.ToString()) == true)
                {
                    DuplicateIndex = i;
                    Duplicate = true;
                    break;
                }
            }
            
            OleDbConnection DBCon = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + Properties.Settings.Default.OwnedDatabase);
            
            if (Duplicate != true)
            {
                OleDbCommand DBcmd = new OleDbCommand("INSERT INTO MyCards([MultiverseID], [OwnedAmount]) VALUES ('" + Cards[Index].MultiverseID + "', '1')", DBCon);
                DBCon.Open();
                DBcmd.ExecuteNonQuery();
                DBCon.Close();
            }
            else
            {
                MyCards[DuplicateIndex].OwnedAmount += 1;

                OleDbCommand cmd = new OleDbCommand("Update MyCards set OwnedAmount = '" + MyCards[DuplicateIndex].OwnedAmount + "' where MultiverseID = '" + MyCards[DuplicateIndex].MultiverseID + "'", DBCon);
                DBCon.Open();
                cmd.ExecuteNonQuery();
                DBCon.Close();
            }
            MyCards.Clear();
            GetOwnedCards();
        }

        /// <summary>
        /// This function removes one card from the selected index at a time. Unless it is one then it remove it from the DB
        /// </summary>
        private async Task RemoveCard(int Index)
        {
            OleDbConnection DBCon = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + Properties.Settings.Default.OwnedDatabase);

            if (MyCards[Index].OwnedAmount != 1)
            {
                MyCards[Index].OwnedAmount -= 1;
                OleDbCommand DBCmd = new OleDbCommand("UPDATE MyCards Set OwnedAmount = '" + MyCards[Index].OwnedAmount + "' where MultiverseID = '" + MyCards[Index].MultiverseID + "'", DBCon);
                await DBCon.OpenAsync();
                DBCmd.ExecuteNonQuery();
                DBCon.Close();
            }
            else
            {
                OleDbCommand DBCmd = new OleDbCommand("Delete From MyCards Where MultiverseID = '" + MyCards[Index].MultiverseID + "'", DBCon);
                await DBCon.OpenAsync();
                DBCmd.ExecuteNonQuery();
                DBCon.Close();
            }
            MyCards.Clear();
            await GetOwnedCards();
            try
            {
                this.MyCardsList.SelectedIndex = Index;
            }
            catch (Exception) { }
        }

        /// <summary>
        /// When a card is double clicked this displays a window with the cards info and picture.
        /// </summary>
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
                ErrorWin.ShowDialog();
            };
        }

        /// <summary>
        /// This calls out to the Database with all of the owned cards and stores all of them in the mycards list.
        /// </summary>
        /// <returns></returns>
        private async Task GetOwnedCards()
        {
            try
            {
                OleDbConnection DBCon = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + Properties.Settings.Default.OwnedDatabase);
                await DBCon.OpenAsync();

                OleDbDataAdapter MyCardDA = new OleDbDataAdapter("SELECT * FROM MyCards", DBCon);
                DataSet MyCardDS = new DataSet();
                MyCardDA.Fill(MyCardDS);
                DBCon.Close();

                for (int i = 0; i < MyCardDS.Tables[0].Rows.Count; i++)
                {
                    DBCon = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + Properties.Settings.Default.DatabaseLocation);
                    await DBCon.OpenAsync();

                    OleDbDataAdapter CardDA = new OleDbDataAdapter("SELECT * FROM Cards Where MultiverseID = '" + MyCardDS.Tables[0].Rows[i]["MultiverseID"].ToString() + "'", DBCon);
                    DataSet CardDS = new DataSet();
                    CardDA.Fill(CardDS);
                    DBCon.Close();

                    MyCards.Add(new OwnedCardListData
                    {
                        MultiverseID = MyCardDS.Tables[0].Rows[i]["MultiverseID"].ToString(),
                        CardName = CardDS.Tables[0].Rows[0]["Name"].ToString(),
                        CardExpansion = CardDS.Tables[0].Rows[0]["Expansion"].ToString(),
                        CardImg = CardDS.Tables[0].Rows[0]["ImgURL"].ToString(),
                        Rarity = CardDS.Tables[0].Rows[0]["Rarity"].ToString(),
                        ConvMana = CardDS.Tables[0].Rows[0]["ConvManaCost"].ToString(),
                        Type = CardDS.Tables[0].Rows[0]["Type"].ToString(),
                        Power = CardDS.Tables[0].Rows[0]["Power"].ToString(),
                        Toughness = CardDS.Tables[0].Rows[0]["Toughness"].ToString(),
                        OwnedAmount = Convert.ToInt32(MyCardDS.Tables[0].Rows[i]["OwnedAmount"].ToString())
                        //WishOwnedAmount = MyCardDS.Tables[0].Rows[i]["WishOwnedAmount"],
                        //FoilOwnedAmount = (MyCardDS.Tables[0].Rows[i]["FoilOwnedAmount"]
                    });
                }
            }
            catch (Exception)
            {
                PopupWin ErrorWin = new PopupWin();
                Error ErrorControl = new Error(ErrorWin);
                ErrorWin.ControlGrid.Children.Add(ErrorControl);
                ErrorControl.ErrorText.Text = "There was a problem connection to owned cards database. \nPlease go to File > Settings and make sure the path is correct.";
                ErrorWin.ShowDialog();
            };
        }

        /// <summary>
        /// This updates the binding to the image on the quick view on the side of the ListViews
        /// </summary>
        private void CardsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CardTab.DataContext = Cards[CardsList.SelectedIndex];
            CardImg.Source = new BitmapImage(new Uri(Cards[CardsList.SelectedIndex].CardImg.ToString(), UriKind.Absolute));
        }

        /// <summary>
        /// Brings up the settings window when the settings option in the menu is selected.
        /// Also creates event handlers for the close of zed window.
        /// </summary>
        private void Settings_Clicked(object sender, RoutedEventArgs e)
        {
            PopupWin SettingsWin = new PopupWin();
            Settings SettingsControl = new Settings(SettingsWin);

            SettingsWin.ControlGrid.Children.Add(SettingsControl);
            SettingsWin.ShowDialog();
        }

        /// <summary>
        /// Closes the Popup window that is passed in.
        /// </summary>
        private void CloseWindow(object parent)
        {
            PopupWin ParentWin = parent as PopupWin;
            ParentWin.Close();
        }

        /// <summary>
        /// Starts the filter statement asyncronusly (Doesn't wait for it to finish)
        /// </summary>
        private async void Search_Click(object sender, RoutedEventArgs e)
        {
            FilterDB();
        }

        /// <summary>
        /// Based on the info in the search boxes this creates a query statement that filter the database. 
        /// The return is then stored in the Cards list and updated to cards view.
        /// </summary>
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
            catch (Exception) { };
        }
    }
}
