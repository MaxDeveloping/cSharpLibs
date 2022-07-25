using CommonLibs.Database.Exceptions;
using CommonLibs.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;

namespace CommonLibs.Database.Validation
{
    public abstract class DatabaseValidatorBase : IDatabaseValidator
    {
        protected const string cTableNameParamName = "TableName";

        protected const string cProcedureNameParamName = "ProcedureName";

        protected const string cDummyTableName = "DummyTableTest";

        protected const string cDummyProcedureName = "DummyProcedureTest";

        protected IDatabase Database { get; }


        protected DatabaseValidatorBase(IDatabase pDatabase)
        {
            Database = pDatabase;
        }

        public void CheckConnectionAvailable()
        {
            using (var c = Database.CreateAndOpenConnection())
            {
                var state = c.State;
                if (state != ConnectionState.Open)
                    throw new DatabaseConnectionException(string.Format(CultureInfo.CurrentCulture, "Database not opened. State: {0}", state));
            }
        }

        public void CheckTableCreationRights()
        {
            if (TableExists(cDummyTableName))
                DeleteDummyTable();

            CreateDummyTable();

            if (!TableExists(cDummyTableName))
                throw new DatabaseException(string.Format(CultureInfo.CurrentCulture, "Failed to create table {0}.", cDummyTableName));

            DeleteDummyTable();
        }

        protected void CreateDummyTable()
        {
            var createTableCmdData = new DbCommandData(ValidationResources.CreateDummyTable, CommandType.Text);
            Database.ExecuteNonQuery(createTableCmdData);
        }

        protected void DeleteDummyTable()
        {
            var dropTableCmdData = new DbCommandData(ValidationResources.DropDummyTable, CommandType.Text);
            Database.ExecuteNonQuery(dropTableCmdData);

            if (TableExists(cDummyTableName))
                throw new DatabaseException(string.Format(CultureInfo.CurrentCulture, "Failed to drop table {0}.", cDummyTableName));
        }

        public void CheckProcedureCreationRights()
        {
            if (ProcedureExists(cDummyProcedureName))
                DeleteDummyProcedure();

            CreateDummyProcedure();

            if (!ProcedureExists(cDummyProcedureName))
                throw new DatabaseException(string.Format(CultureInfo.CurrentCulture, "Failed to create procedure {0}.", cDummyProcedureName));

            DeleteDummyProcedure();
        }

        protected void DeleteDummyProcedure()
        {
            var dropProcCmdData = new DbCommandData(ValidationResources.DropDummyProcedure, CommandType.Text);
            Database.ExecuteNonQuery(dropProcCmdData);

            if (ProcedureExists(cDummyProcedureName))
                throw new DatabaseException(string.Format(CultureInfo.CurrentCulture, "Failed to drop procedure {0}.", cDummyProcedureName));
        }

        protected abstract void CreateDummyProcedure();

        public abstract bool TableExists(string pName, IDbTransaction pOpenTransaction = null);

        public abstract bool ProcedureExists(string pName, IDbTransaction pOpenTransaction = null);

    }
}
