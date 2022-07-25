using CommonLibs.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace CommonLibs.Database
{
    public class DbCommandData : IDbCommandData
    {
        public string SqlText { get; }

        public IList<IDbDataParameter> Parameters { get; }

        public CommandType Type { get; }

        public DbCommandData(string pSqlText, CommandType pType, params IDbDataParameter[] pParams)
        {
            SqlText = pSqlText;
            Type = pType;
            Parameters = pParams;
        }
    }
}
