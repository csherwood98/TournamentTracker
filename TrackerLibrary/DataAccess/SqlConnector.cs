using System;
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
    }
}
