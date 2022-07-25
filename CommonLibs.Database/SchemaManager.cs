using CommonLibs.Database.Exceptions;
using CommonLibs.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;

namespace CommonLibs.Database
{
    public class SchemaManager : ISchemaManager
    {
        private readonly IManagedSchema m_ManagedSchema;

        public SchemaManager(IManagedSchema pManagedSchema)
        {
            m_ManagedSchema = pManagedSchema;
        }

        public void EnsureLatestSchema(bool pDetailedCheck)
        {
            m_ManagedSchema.InitializeDatabase();

            using (var c = m_ManagedSchema.Database.CreateAndOpenConnection())
            {
                try
                {
                    Version currSchemaVerFoundInDatabase;
                    using (var t = c.BeginTransaction(IsolationLevel.Unspecified))
                    {
                        currSchemaVerFoundInDatabase = RetrieveCurrentSchemaVersion(t);

                        // check procedures & tables exist
                        if (currSchemaVerFoundInDatabase == null || pDetailedCheck)
                        {
                            CreateSchemaObjectsIfNotExist(t, m_ManagedSchema.CreateSchemaTablesDbCommandData(), CreateObjectTypes.Table);
                            CreateSchemaObjectsIfNotExist(t, m_ManagedSchema.CreateSchemaProceduresDbCommandData(), CreateObjectTypes.Procedure);
                        }

                        // migrate if needed
                        if (currSchemaVerFoundInDatabase != null && currSchemaVerFoundInDatabase < m_ManagedSchema.CurrentSchemaVersion)
                        {
                            var updateCmdData = m_ManagedSchema.CreateUpdateToLatestSchemaCommandData(currSchemaVerFoundInDatabase, m_ManagedSchema.CurrentSchemaVersion);
                            foreach (var cmd in updateCmdData)
                                m_ManagedSchema.Database.ExecuteNonQuery(cmd, t);
                        }

                        try
                        {
                            t.Commit();
                        }
                        catch (Exception e)
                        {
                            throw new DatabaseException(string.Format(CultureInfo.CurrentCulture, "Failed to commit schema changes: {0}", e.Message), e);
                        }
                    }

                    m_ManagedSchema.FinalizeSchema();

                    // insert schema update to meta table
                    if (currSchemaVerFoundInDatabase == null || currSchemaVerFoundInDatabase < m_ManagedSchema.CurrentSchemaVersion)
                    {
                        using (var t = c.BeginTransaction(IsolationLevel.ReadCommitted))
                        {
                            var insertCmd = m_ManagedSchema.CreateInsertSchemaMetaDataCommandData(currSchemaVerFoundInDatabase, m_ManagedSchema.CurrentSchemaVersion);
                            m_ManagedSchema.Database.ExecuteNonQuery(insertCmd, t);

                            try
                            {
                                t.Commit();
                            }
                            catch (Exception e)
                            {
                                throw new DatabaseException(string.Format(CultureInfo.CurrentCulture, "Failed to commit schema update insertion: {0}", e.Message), e);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new DatabaseException(e.Message, e);
                }
            }
        }


        private void CreateSchemaObjectsIfNotExist(IDbTransaction pOpenTransaction, IDictionary<string, IList<IDbCommandData>> pCreateObjectsCmds, CreateObjectTypes pObjType)
        {
            foreach (var commands in pCreateObjectsCmds)
            {
                try
                {
                    bool objExists;
                    switch (pObjType)
                    {
                        case CreateObjectTypes.Table:
                            objExists = m_ManagedSchema.Validator.TableExists(commands.Key, pOpenTransaction);
                            break;
                        case CreateObjectTypes.Procedure:
                            objExists = m_ManagedSchema.Validator.ProcedureExists(commands.Key, pOpenTransaction);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(pObjType), pObjType, null);
                    }

                    if (objExists)
                        continue;
                }
                catch (DatabaseException e)
                {
                    throw new DatabaseException(string.Format(CultureInfo.CurrentCulture, "Failed to check existence of {0} {1} on creating schema. {2}", pObjType, commands.Key, e.Message), e);
                }

                try
                {
                    foreach (var cmd in commands.Value)
                    {
                        m_ManagedSchema.Database.ExecuteNonQuery(cmd, pOpenTransaction);
                    }
                }
                catch (DatabaseException e)
                {
                    throw new DatabaseException(string.Format(CultureInfo.CurrentCulture, "Failed to create {0} {1} on creating schema. {2}", pObjType, commands.Key, e.Message), e);
                }
            }
        }

        public Version RetrieveCurrentSchemaVersion()
        {
            try
            {
                using (var c = m_ManagedSchema.Database.CreateAndOpenConnection())
                {
                    using (var t = c.BeginTransaction(IsolationLevel.ReadCommitted))
                    {
                        return RetrieveCurrentSchemaVersion(t);
                    }
                }
            }
            catch (Exception e)
            {
                throw new DatabaseException(e.Message, e);
            }
        }


        private Version RetrieveCurrentSchemaVersion(IDbTransaction pOpenTransaction)
        {
            if (!m_ManagedSchema.Validator.TableExists(m_ManagedSchema.SchemaMetaDataTableName, pOpenTransaction))
                return null;

            var result = m_ManagedSchema.Database.ExecuteReader(m_ManagedSchema.CreateSelectLatestSchemaMetaDataCommandData(), m_ManagedSchema.MapToSchemaMetaData, pOpenTransaction);

            if (!result.Any() || string.IsNullOrEmpty(result[0].TargetVersion))
                return null;

            return new Version(result[0].TargetVersion);
        }

        private enum CreateObjectTypes
        {
            Table, Procedure
        }
    }
}
