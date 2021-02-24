using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary
{
    /// <summary>
    /// Represents the prizes in the tournament
    /// </summary>
    public class PrizeModel
    {
        /// <summary>
        /// Represents the place in competition associated with the prize.
        /// </summary>
        public int PlaceNumber { get; set; }
        /// <summary>
        /// Represents the name of the place associated with the prize.
        /// </summary>
        public string PlaceName { get; set; }
        /// <summary>
        /// Represents the prize amount in dollars
        /// </summary>
        public decimal PrizeAmount { get; set; }
        /// <summary>
        /// Represents the prize amount as a percentage of the total pot
        /// </summary>
        public double PrizePercentage { get; set; }
    }
}
