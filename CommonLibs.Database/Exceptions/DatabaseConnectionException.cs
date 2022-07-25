using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibs.Database.Exceptions
{
    [Serializable]
    public class DatabaseConnectionException : DatabaseException
    {
        public DatabaseConnectionException()
        {
        }

        public DatabaseConnectionException(string pMsg) : base(pMsg)
        {
        }

        public DatabaseConnectionException(string pMsg, Exception pInEx) : base(pMsg, pInEx)
        {
        }
    }
}
