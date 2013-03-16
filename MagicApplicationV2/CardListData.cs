using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicApplicationV2
{
    public class CardListData
    {
        /// <summary>
        /// Get / Set the Multiverse ID number for the card.
        /// </summary>
        public string MultiverseID { get; set; }
        /// <summary>
        /// Get / Set the URL for the image of the card.
        /// </summary>
        public string CardImg { get; set; }
        /// <summary>
        /// Get / Set the Name of the Card.
        /// </summary>
        public string CardName { get; set; }
        /// <summary>
        /// Get / Set the Card's Expansion
        /// </summary>
        public string CardExpansion { get; set; }
        /// <summary>
        /// Get / Set the Rarity of the Card
        /// </summary>
        public string Rarity { get; set; }
        /// <summary>
        /// Get / Set the Converted Mana Cost of the Card
        /// </summary>
        public string ConvMana { get; set; }
        /// <summary>
        /// Get / Set the Type of Card
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Get / Set the Power of the Card
        /// </summary>
        public string Power { get; set; }
        /// <summary>
        /// Get / Set the Toughness of the Card
        /// </summary>
        public string Toughness { get; set; }
    }
}
