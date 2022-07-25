using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace CommonLibs.GeneralLibrary.Exceptions
{
    public class InvalidArgumentException : Exception
    {
        public InvalidArgumentException()
        {
        }

        public InvalidArgumentException(string pMessage) : base(pMessage)
        {
        }

        public InvalidArgumentException(string pMessage, Exception pInnerException) : base(pMessage, pInnerException)
        {
        }

        protected InvalidArgumentException(SerializationInfo pInfo, StreamingContext pContext) : base(pInfo, pContext)
        {
        }
    }
}
