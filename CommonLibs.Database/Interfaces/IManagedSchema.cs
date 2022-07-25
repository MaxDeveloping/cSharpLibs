using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace CommonLibs.Database.Interfaces
{
    public interface IManagedSchema
    {
        IDatabase Database { get; }

        IDatabaseValidator Validator { get; }

        Version CurrentSchemaVersion { get; }

        string SchemaMetaDataTableName { get; }

        void InitializeDatabase();

        IDictionary<string, IList<IDbCommandData>> CreateSchemaTablesDbCommandData();

        IDictionary<string, IList<IDbCommandData>> CreateSchemaProceduresDbCommandData();

        void FinalizeSchema();

        IDbCommandData CreateInsertSchemaMetaDataCommandData(Version pOldSchemaVersion, Version pNewSchemaVersion);

        IDbCommandData CreateSelectLatestSchemaMetaDataCommandData();

        ISchemaMetaData MapToSchemaMetaData(IDataReader pReader);

        IEnumerable<IDbCommandData> CreateUpdateToLatestSchemaCommandData(Version pCurrentSchemaVersion, Version pTargetSchemaVersion);
    }
}
