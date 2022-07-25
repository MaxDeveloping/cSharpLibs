using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibs.Database.Exceptions
{
    [Serializable]
    public class DatabaseException : Exception
    {
        public DatabaseException()
        {
        }

        public DatabaseException(string pMsg) : base(pMsg)
        {
        }

        public DatabaseException(string pMsg, Exception pInEx) : base(pMsg, pInEx)
        {
        }
    }
}
