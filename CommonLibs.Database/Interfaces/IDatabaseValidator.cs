using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace CommonLibs.Database.Interfaces
{
    public interface IDatabaseValidator
    {
        void CheckConnectionAvailable();

        void CheckTableCreationRights();

        void CheckProcedureCreationRights();

        bool TableExists(string pName, IDbTransaction pOpenTransaction);

        bool ProcedureExists(string pName, IDbTransaction pOpenTransaction);
    }
}
