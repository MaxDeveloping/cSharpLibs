using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibs.Database.Exceptions
{
    [Serializable]
    public class DatabaseConfigurationException : DatabaseException
    {
        public DatabaseConfigurationException()
        {
        }

        public DatabaseConfigurationException(string pMsg) : base(pMsg)
        {
        }

        public DatabaseConfigurationException(string pMsg, Exception pInEx) : base(pMsg, pInEx)
        {
        }
    }
}
