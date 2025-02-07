﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;
using MySql.Data.MySqlClient;
using Dapper;

namespace TrackerLibrary.DataAccess
{
    public class SqlConnector : IDataConnection
    {
        //TODO - Make the CreatePrize method actually save to the database; change model.Id
        /// <summary>
        /// Saves a new prize to the database
        /// </summary>
        /// <param name="model"> The prize information</param>
        /// <returns>The prize information, including the unique identifier</returns>

        private const string db = "tournaments";
        public PrizeModel CreatePrize(PrizeModel model)
        {
            using (IDbConnection connection = new MySql.Data.MySqlClient.MySqlConnection(GlobalConfig.ConnString(db)))
            {
                var p = new DynamicParameters();
                p.Add("inp_PlaceNumber", model.PlaceNumber);
                p.Add("inp_PlaceName", model.PlaceName);
                p.Add("inp_PrizeAmount", model.PrizeAmount);
                p.Add("inp_PrizePercentage", model.PrizePercentage);
                p.Add("out_id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("sp_Prizes_Insert", p, commandType: CommandType.StoredProcedure);

                model.Id = p.Get<int>("out_id");

                return model;
            }
        }

        public PersonModel CreatePerson(PersonModel model)
        {
            using (IDbConnection connection = new MySql.Data.MySqlClient.MySqlConnection(GlobalConfig.ConnString(db)))
            {
                var p = new DynamicParameters();
                p.Add("inp_FirstName", model.FirstName);
                p.Add("inp_LastName", model.LastName);
                p.Add("inp_EmailAddress", model.EmailAddress);
                p.Add("inp_CellphoneNumber", model.CellphoneNumber);
                p.Add("out_id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("sp_Person_Insert", p, commandType: CommandType.StoredProcedure);
                model.Id = p.Get<int>("out_id");

                return model;
            }
        }

        public List<PersonModel> GetPerson_All()
        {
            List<PersonModel> output;
            using (IDbConnection connection = new MySql.Data.MySqlClient.MySqlConnection(GlobalConfig.ConnString(db)))
            {
                output = connection.Query<PersonModel>("sp_Person_GetAll").ToList();
            }

            return output;
        }

        public TeamModel CreateTeam(TeamModel model)
        {
            using (IDbConnection connection = new MySql.Data.MySqlClient.MySqlConnection(GlobalConfig.ConnString(db)))
            {
                var p = new DynamicParameters();
                p.Add("inp_TeamName", model.TeamName);
                p.Add("out_id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("sp_Teams_Insert", p, commandType: CommandType.StoredProcedure);
                model.Id = p.Get<int>("out_id");


                foreach(PersonModel tm in model.TeamMembers)
                {
                    p = new DynamicParameters();
                    p.Add("inp_TeamId", model.Id);
                    p.Add("inp_PersonId", tm.Id);
                    p.Add("out_id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                    connection.Execute("sp_TeamMembers_Insert", p, commandType: CommandType.StoredProcedure);
                }

                return model;
            }
        }

        public List<TeamModel> GetTeam_All()
        {
            List<TeamModel> output;
            using (IDbConnection connection = new MySql.Data.MySqlClient.MySqlConnection(GlobalConfig.ConnString(db)))
            {
                output = connection.Query<TeamModel>("sp_Team_GetAll").ToList();

                foreach (TeamModel team in output)
                {
                    var p = new DynamicParameters();
                    p.Add("inp_TeamId", team.Id);
                    team.TeamMembers = connection.Query<PersonModel>("sp_TeamMembers_GetByTeam", p, commandType: CommandType.StoredProcedure).ToList();
                }

            }

            return output;
        }

        public void CreateTournament(TournamentModel model)
        {
            using (IDbConnection connection = new MySql.Data.MySqlClient.MySqlConnection(GlobalConfig.ConnString(db)))
            {
                SaveTournament(connection, model);

                SaveTournamentPrizes(connection, model);

                SaveTournamentEntries(connection, model);

                SaveTournamentRounds(connection, model);
            }
        }
        
        private void SaveTournament(IDbConnection connection, TournamentModel model)
        {
            var p = new DynamicParameters();
            p.Add("inp_TournamentName", model.TournamentName);
            p.Add("inp_EntryFee", model.EntryFee);
            p.Add("out_id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

            connection.Execute("sp_Tournaments_Insert", p, commandType: CommandType.StoredProcedure);

            model.Id = p.Get<int>("out_id");
        }
        private void SaveTournamentPrizes(IDbConnection connection, TournamentModel model)
        {
            foreach (PrizeModel pm in model.Prizes)
            {
                var p = new DynamicParameters();
                p.Add("inp_TournamentId", model.Id);
                p.Add("inp_PrizeId", pm.Id);
                p.Add("out_id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("sp_TournamentPrizes_Insert", p, commandType: CommandType.StoredProcedure);
            }
        }
        private void SaveTournamentEntries(IDbConnection connection, TournamentModel model)
        {
            foreach (TeamModel tm in model.EnteredTeams)
            {
                var p = new DynamicParameters();
                p.Add("inp_TournamentId", model.Id);
                p.Add("inp_TeamId", tm.Id);
                p.Add("out_id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("sp_TournamentEntries_Insert", p, commandType: CommandType.StoredProcedure);
            }
        }
        private void SaveTournamentRounds(IDbConnection connection, TournamentModel model)
        {
            //List<List<MatchupModel>> Rounds
            //List<MatchupEntryModel> Entries

            //Loop through the rounds
            //Loop through the matchups
            //Save the matchup
            //Loop through the entries and save them

            foreach (List<MatchupModel> round in model.Rounds)
            {
                foreach (MatchupModel matchup in round)
                {
                    var p = new DynamicParameters();
                    p.Add("inp_TournamentId", model.Id);
                    p.Add("inp_MatchupRound", matchup.MatchupRound);
                    p.Add("out_id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                    connection.Execute("sp_Matchups_Insert", p, commandType: CommandType.StoredProcedure);

                    matchup.Id = p.Get<int>("out_id");

                    foreach (MatchupEntryModel entry in matchup.Entries)
                    {
                        p = new DynamicParameters();
                        p.Add("inp_MatchupId", matchup.Id);

                        if (entry.ParentMatchup == null)
                        {
                            p.Add("inp_ParentMatchupId", null);
                        }
                        else
                        {
                            p.Add("inp_ParentMatchupId", entry.ParentMatchup.Id);
                        }

                        if (entry.TeamCompeting == null)
                        {
                            p.Add("inp_TeamCompetingId", null);
                        }
                        else
                        {
                            p.Add("inp_TeamCompetingId", entry.TeamCompeting.Id);
                        }
                        p.Add("out_id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                        connection.Execute("sp_MatchupEntries_Insert", p, commandType: CommandType.StoredProcedure);
                    }
                }
            }
        }

        public List<TournamentModel> GetTournament_All()
        {
            List<TournamentModel> output;
            var p = new DynamicParameters();

            using (IDbConnection connection = new MySql.Data.MySqlClient.MySqlConnection(GlobalConfig.ConnString(db)))
            {
                output = connection.Query<TournamentModel>("sp_Tournaments_GetAll").ToList();


                foreach (TournamentModel t in output)
                {
                    //Populate Prizes

                    p = new DynamicParameters();
                    p.Add("inp_TournamentId", t.Id);

                    t.Prizes = connection.Query<PrizeModel>("sp_Prizes_GetByTournament", p, commandType: CommandType.StoredProcedure).ToList();

                    p = new DynamicParameters();
                    p.Add("inp_TournamentId", t.Id);
                    //Populate Teams
                    t.EnteredTeams = connection.Query<TeamModel>("sp_Team_GetByTournament", p, commandType: CommandType.StoredProcedure).ToList();

                    foreach (TeamModel team in t.EnteredTeams)
                    {
                        p = new DynamicParameters();
                        p.Add("inp_TeamId", team.Id);

                        team.TeamMembers = connection.Query<PersonModel>("sp_TeamMembers_GetByTeam", p, commandType: CommandType.StoredProcedure).ToList();
                    }

                    p = new DynamicParameters();
                    p.Add("inp_TournamentId", t.Id);

                    //Populate rounds
                    List<MatchupModel> matchups = connection.Query<MatchupModel>("sp_Matchups_GetByTournament", p, commandType: CommandType.StoredProcedure).ToList();

                    foreach (MatchupModel m in matchups)
                    {
                        p = new DynamicParameters();
                        p.Add("inp_MatchupId", m.Id);

                        m.Entries = connection.Query<MatchupEntryModel>("sp_MatchupEntries_GetByMatchup", p, commandType: CommandType.StoredProcedure).ToList();

                        List<TeamModel> allTeams = GetTeam_All();

                        if (m.WinnerId > 0)
                        {
                            m.Winner = allTeams.Where(x => x.Id == m.WinnerId).First();
                        }

                        foreach (var me in m.Entries)
                        {
                            if (me.TeamCompetingId > 0)
                            {
                                me.TeamCompeting = allTeams.Where(x => x.Id == me.TeamCompetingId).First();
                            }

                            if (me.ParentMatchupId > 0)
                            {
                                me.ParentMatchup = matchups.Where(x => x.Id == me.ParentMatchupId).First();
                            }
                        }
                    }
                    //Populating List<List<MatchupModel>> Rounds
                    List<MatchupModel> currRow = new List<MatchupModel>();
                    int currRound = 1;

                    foreach (MatchupModel m in matchups)
                    {
                        if (m.MatchupRound > currRound)
                        {
                            t.Rounds.Add(currRow);
                            currRow = new List<MatchupModel>();
                            currRound += 1;
                        }

                        currRow.Add(m);
                    }

                    t.Rounds.Add(currRow);
                }
            }

            return output;
        }

        public void UpdateMatchup(MatchupModel model)
        {
            using (IDbConnection connection = new MySql.Data.MySqlClient.MySqlConnection(GlobalConfig.ConnString(db)))
            {
                var p = new DynamicParameters();
                if (model.Winner != null)
                {
                    p.Add("inp_Id", model.Id);
                    p.Add("inp_WinnerId", model.Winner.Id);

                    connection.Execute("sp_Matchups_Update", p, commandType: CommandType.StoredProcedure);
                }




                foreach(MatchupEntryModel me in model.Entries)
                {
                    if (me.TeamCompeting != null)
                    {
                        p = new DynamicParameters();
                        p.Add("inp_Id", me.Id);
                        p.Add("inp_TeamCompetingId", me.TeamCompeting.Id);
                        p.Add("inp_Score", me.Score);

                        connection.Execute("sp_MatchupEntries_Update", p, commandType: CommandType.StoredProcedure);
                    }

                }
            }
        }
    }
}
