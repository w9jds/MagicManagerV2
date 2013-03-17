using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicApplicationV2.Classes
{
    class OwnedCardListData : CardListData
    {
        /// <summary>
        /// Gets / Sets how many of this Card you Own.
        /// </summary>
        public int OwnedAmount { get; set; }
        /// <summary>
        /// Gets / Sets how many of this Card you wish to own.
        /// </summary>
        public int WishOwnedAmount { get; set; }
        /// <summary>
        /// Gets / Sets how many of this Card you own that is foil.
        /// </summary>
        public int FoilOwnedAmount { get; set; }
    }
}
