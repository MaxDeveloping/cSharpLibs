using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibs.Database.Exceptions
{
    [Serializable]
    public class DatabasePrimaryKeyViolationException : DatabaseException
    {
        public DatabasePrimaryKeyViolationException()
        {
        }

        public DatabasePrimaryKeyViolationException(string pMsg) : base(pMsg)
        {
        }

        public DatabasePrimaryKeyViolationException(string pMsg, Exception pInEx) : base(pMsg, pInEx)
        {
        }
    }
}
