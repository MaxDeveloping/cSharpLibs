using CommonLibs.Database.Exceptions;
using CommonLibs.Database.Interfaces;
using CommonLibs.Database.Validation;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;

namespace CommonLibs.Database.MySql
{
    public class MySqlDbValidator : DatabaseValidatorBase
    {
        private const int cMinMaxAllowedPacketValue = 50 * 1024 * 1024;

        public MySqlDbValidator(IDatabase pDb) : base(pDb)
        {
        }

        public void CheckMaxAllowedPacketConfigValue()
        {
            var cmdData = new DbCommandData(MySqlValidatorResources.MySqlSelectMaxAllowedPacketSize, CommandType.Text);
            var maxAllowedPacketValue = Database.ExecuteReader(cmdData, r => r.GetInt32(1)).FirstOrDefault();

            if (maxAllowedPacketValue == 0)
                throw new DatabaseException("Failed to retrieve max_allowed_packet value. Query did not return a result.");

            if (maxAllowedPacketValue < cMinMaxAllowedPacketValue)
            {
                throw new DatabaseConfigurationException(string.Format(CultureInfo.CurrentCulture, "The value {0} for MySQL max_allowed_packet is below recommended minimum value of {1} bytes." +
                    "Please adjust it in your MySQL installation.", maxAllowedPacketValue, cMinMaxAllowedPacketValue));
            }
        }

        public override bool TableExists(string pName, IDbTransaction pOpenTransaction = null)
        {
            var cmdData = new DbCommandData(MySqlValidatorResources.MySqlSelectNameOfExistingTable, CommandType.Text,
                new MySqlParameter("@" + cTableNameParamName, MySqlDbType.VarChar) { Value = pName });

            if (pOpenTransaction != null)
                return Database.ExecuteScalar(cmdData, pOpenTransaction) != null;

            return Database.ExecuteScalar(cmdData) != null;
        }

        public override bool ProcedureExists(string pName, IDbTransaction pOpenTransaction = null)
        {
            var cmdData = new DbCommandData(MySqlValidatorResources.MySqlSelectNameOfExistingProcedure, CommandType.Text,
                new MySqlParameter("@" + cProcedureNameParamName, MySqlDbType.VarChar) { Value = pName });

            if (pOpenTransaction != null)
                return Database.ExecuteScalar(cmdData, pOpenTransaction) != null;

            return Database.ExecuteScalar(cmdData) != null;
        }

        protected override void CreateDummyProcedure()
        {
            var cmdData = new DbCommandData(MySqlValidatorResources.MySqlCreateDummyProcedure, CommandType.Text);
            Database.ExecuteNonQuery(cmdData);
        }
    }
}
