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
using System.Windows.Shapes;

namespace MagicApplicationV2
{
    /// <summary>
    /// Interaction logic for CardWin.xaml
    /// </summary>
    public partial class CardWin : Window
    {
        private CardListData Card = new CardListData();

        public CardWin(CardListData CardIn)
        {
            Card = CardIn;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CardImg.Source = new BitmapImage(new Uri(Card.CardImg.ToString(), UriKind.Absolute));
            CardWindow.DataContext = Card;
        }
    }
}
