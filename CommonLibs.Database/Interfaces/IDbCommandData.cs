using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace CommonLibs.Database.Interfaces
{
    public interface IDbCommandData
    {
        string SqlText { get; }

        IList<IDbDataParameter> Parameters { get; }

        CommandType Type { get; }
    }
}
