using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibs.Database.Interfaces
{
    public interface ISchemaManager
    {
        void EnsureLatestSchema(bool pExecuteDetailedCheck);

        Version RetrieveCurrentSchemaVersion();
    }
}
