using CommonLibs.Database.Exceptions;
using CommonLibs.Database.Interfaces;
using CommonLibs.Database.Settings.Interfaces;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace CommonLibs.Database.MySql
{
    public class MySqlDatabase : DatabaseBase
    {
        private const string cCreateDbSqlTemplate = "CREATE DATABASE {0} CHARACTER SET {1} COLLATE {2};";

        private const string cSelectDatabasesQuery = "SELECT SCHEMA_NAME FROM information_schema.SCHEMATA";

        private const string cSelectDatabaseWhereConstraints = " WHERE SCHEMA_NAME NOT IN ({0})";

        private readonly IList<string> m_SystemTableNames = new List<string> { "information_schema", "mysql", "performance_schema", "sys" };


        private readonly IDbServerPortSettings m_DbSettings;

        private MySqlConnectionStringBuilder m_ConnectionStringBuilder;

        public MySqlDatabase(IDbServerPortSettings pDbSettings) : base(pDbSettings)
        {
            m_DbSettings = pDbSettings;
        }

        protected override string ConnectionString
        {
            get
            {
                if (m_ConnectionStringBuilder == null)
                {
                    m_ConnectionStringBuilder = new MySqlConnectionStringBuilder
                    {
                        CharacterSet = "utf8mb4",
                        UserID = m_DbSettings.Username,
                        Password = m_DbSettings.Password,
                        ConnectionProtocol = MySqlConnectionProtocol.Tcp,
                        Database = m_DbSettings.DefaultDbName,
                        ConnectionTimeout = (uint)m_DbSettings.ConnectionTimeout.TotalSeconds,
                        DefaultCommandTimeout = (uint)m_DbSettings.CommandTimeout.TotalSeconds,
                        Server = m_DbSettings.Host,
                        Port = (uint)m_DbSettings.Port,
                        Pooling = true
                    };
                }

                return m_ConnectionStringBuilder.ToString();
            }
        }

        public override IDbConnection CreateAndOpenConnection()
        {
            MySqlConnection connection = null;
            try
            {
                try
                {
                    connection = new MySqlConnection(ConnectionString);
                    connection.Open();
                }
                catch (MySqlException)
                {
                    connection?.Dispose();
                    try
                    {
                        connection = new MySqlConnection(ConnectionString);
                        connection.Open();
                    }
                    catch (MySqlException e)
                    {
                        throw new DatabaseConnectionException(string.Format(CultureInfo.CurrentCulture, "Failed to create connection to database: ", e.Message), e);
                    }
                }
                catch (ArgumentException e)
                {
                    throw new DatabaseConnectionException(string.Format(CultureInfo.CurrentCulture, "Failed to create connection to database: ", e.Message), e);
                }
            }
            catch
            {
                connection?.Dispose();
                throw;
            }

            return connection;
        }


        protected override DataSet ExecuteDataset(IDbCommandData pCmdData, IDbConnection pOpenConnection, IDbTransaction pOpenTransaction)
        {
            if (pOpenTransaction != null && !(pOpenTransaction is MySqlTransaction))
                throw new ArgumentException("Invalid transaction type provided.", nameof(pOpenTransaction));

            if (!(pOpenConnection is MySqlConnection connection))
                throw new ArgumentException("Invalid connection type provided.", nameof(pOpenConnection));

            try
            {
                using (var cmd = new MySqlCommand(pCmdData.SqlText, connection))
                {
                    try
                    {
                        cmd.CommandTimeout = (int)m_DbSettings.CommandTimeout.TotalSeconds;
                        cmd.CommandType = pCmdData.Type;
                        if (pOpenTransaction != null)
                            cmd.Transaction = (MySqlTransaction)pOpenTransaction;

                        AddParamsToCommand(cmd, pCmdData);

                        DataSet result = null;

                        using (var adapter = new MySqlDataAdapter(cmd))
                        {
                            try
                            {
                                result = new DataSet { Locale = CultureInfo.InvariantCulture };
                                adapter.Fill(result);
                                return result;
                            }
                            catch
                            {
                                result?.Dispose();
                                throw;
                            }
                        }
                    }
                    finally
                    {
                        cmd.Parameters.Clear(); // using same transaction may reuse command internally
                    }
                }
            }
            catch (MySqlException e)
            {
                if (IsPrimaryKeyViolationException(e))
                    throw new DatabasePrimaryKeyViolationException("Failed to execute DataSet query on database because of primary key violation.", e);

                throw new DatabaseException(string.Format(CultureInfo.CurrentCulture, "Failed to execute DataSet query on database: {0}", e.Message), e);
            }
        }

        protected override List<T> ExecuteReader<T>(IDbCommandData pCmdData, Func<IDataReader, T> pReaderFunc, IDbConnection pOpenConnection, IDbTransaction pOpenTransaction)
        {
            if (pOpenTransaction != null && !(pOpenTransaction is MySqlTransaction))
                throw new ArgumentException("Invalid transaction type provided.", nameof(pOpenTransaction));

            if (!(pOpenConnection is MySqlConnection connection))
                throw new ArgumentException("Invalid connection type provided.", nameof(pOpenConnection));

            try
            {
                using (var cmd = new MySqlCommand(pCmdData.SqlText, connection))
                {
                    try
                    {
                        cmd.CommandTimeout = (int)m_DbSettings.CommandTimeout.TotalSeconds;
                        cmd.CommandType = pCmdData.Type;
                        if (pOpenTransaction != null)
                            cmd.Transaction = (MySqlTransaction)pOpenTransaction;

                        AddParamsToCommand(cmd, pCmdData);

                        using (var reader = cmd.ExecuteReader())
                        {
                            var resultList = new List<T>();
                            while (reader.Read())
                            {
                                var mappedObj = pReaderFunc(reader);
                                if (mappedObj != null)
                                    resultList.Add(mappedObj);
                            }
                            return resultList;
                        }
                    }
                    finally
                    {
                        cmd.Parameters.Clear(); // using same transaction may reuse command internally
                    }
                }
            }
            catch (MySqlException e)
            {
                if (IsPrimaryKeyViolationException(e))
                    throw new DatabasePrimaryKeyViolationException("Failed to execute DataReader query on database because of primary key violation.", e);

                throw new DatabaseException(string.Format(CultureInfo.CurrentCulture, "Failed to execute DataReader query on database: {0}", e.Message), e);
            }
        }

        public void CreateDatabase(string pName, string pCharSet, string pCollate)
        {
            try
            {
                var sqlText = string.Format(CultureInfo.InvariantCulture, cCreateDbSqlTemplate, "`" + pName + "`", "`" + pCharSet + "`", "`" + pCollate + "`");
                ExecuteNonQuery(new DbCommandData(sqlText, CommandType.Text));
            }
            catch (MySqlException e)
            {
                throw new DatabaseException(string.Format(CultureInfo.CurrentCulture, "Failed to create database {0} with charset {1} and collate {2}: {3}", pName, pCharSet, pCollate, e.Message), e);
            }
        }

        public override IList<string> RemoveInvalidPartsAndSplitSqlStatements(string pSqlText, string pStatementSeparator)
        {
            var result = pSqlText;
            
            // occurences are invalid in ADO.Net
            var removePatternList = new List<string>
            {
                @"(DELIMITER\s?" + Regex.Escape(@"$$") + ")",
                @"(DELIMITER\s?;)"
            };

            foreach (var pattern in removePatternList)
                result = Regex.Replace(result, pattern, string.Empty, RegexOptions.IgnoreCase);

            result = Regex.Replace(result, Regex.Escape(@"$$"), ";", RegexOptions.IgnoreCase);

            return base.RemoveInvalidPartsAndSplitSqlStatements(result, pStatementSeparator);
        }

        protected override bool IsPrimaryKeyViolationException(Exception pE)
        {
            if (pE is MySqlException e)
                return e.ErrorCode == 1022;

            return false;
        }

    }
}
