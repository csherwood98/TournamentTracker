using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary
{
    /// <summary>
    /// Represents a tournament to be tracked in the tournament tracker
    /// </summary>
    public class TournamentModel
    {
        /// <summary>
        /// The name of the tournament
        /// </summary>
        public string TournamentName { get; set; }
        /// <summary>
        /// The entry fee for participating in the tournament
        /// </summary>
        public decimal EntryFee { get; set; }
        /// <summary>
        /// The list of teams that have entered the tournament
        /// </summary>
        public List<TeamModel> EnteredTeams { get; set; } = new List<TeamModel>();
        /// <summary>
        /// The list of prizes that can be won in the tournament
        /// </summary>
        public List<PrizeModel> Prizes { get; set; } = new List<PrizeModel>();
        /// <summary>
        /// List of rounds in the tournament between teams
        /// </summary>
        public List<List<MatchupModel>> Rounds { get; set; } = new List<List<MatchupModel>>();
    }
}
