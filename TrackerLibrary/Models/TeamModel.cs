using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Models
{
    /// <summary>
    /// Represents teams that can compete in the tournaments.
    /// </summary>
    public class TeamModel
    {
        /// <summary>
        /// The id of the team
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The list of team members in the team
        /// </summary>
        public List<PersonModel> TeamMembers { get; set; } = new List<PersonModel>();
        /// <summary>
        /// The name of the team
        /// </summary>
        public string TeamName { get; set; }

    }
}
