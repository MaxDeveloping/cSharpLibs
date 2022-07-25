using CommonLibs.Database.Exceptions;
using CommonLibs.Database.Interfaces;
using CommonLibs.Database.Settings.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CommonLibs.Database
{
    public abstract class DatabaseBase : IDatabase
    {
        private readonly IDbBasicSettings m_DbBasicSettings;

        protected abstract string ConnectionString { get; }

        protected DatabaseBase(IDbBasicSettings pDbBasicSettings)
        {
            m_DbBasicSettings = pDbBasicSettings;
        }

        public abstract IDbConnection CreateAndOpenConnection();

        protected abstract bool IsPrimaryKeyViolationException(Exception pE);


        public int ExecuteNonQuery(IDbCommandData pCmdData)
        {
            using (var c = CreateAndOpenConnection())
            {
                return ExecuteNonQuery(pCmdData, c);
            }
        }

        public virtual int ExecuteNonQuery(IDbCommandData pCmdData, IDbConnection pOpenConnection)
        {
            return ExecuteNonQuery(pCmdData, pOpenConnection, null);
        }

        public virtual int ExecuteNonQuery(IDbCommandData pCmdData, IDbTransaction pOpenTransaction)
        {
            if (pOpenTransaction?.Connection == null)
                throw new ArgumentException("Transaction must have open connection.", nameof(pOpenTransaction));

            return ExecuteNonQuery(pCmdData, pOpenTransaction.Connection, pOpenTransaction);
        }

        protected virtual int ExecuteNonQuery(IDbCommandData pCmdData, IDbConnection pOpenConnection, IDbTransaction pOpenTransaction)
        {
            try
            {
                using (var cmd = CreateCommand(pCmdData, pOpenConnection, pOpenTransaction))
                {
                    return cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                if (e is DatabaseException)
                    throw;

                if (IsPrimaryKeyViolationException(e))
                    throw new DatabasePrimaryKeyViolationException("Failed to execute NonQuery on database because of primary key violation.", e);

                throw new DatabaseException(string.Format(CultureInfo.CurrentCulture, "Failed to execute NonQuery on database: {0}", e.Message), e);
            }
        }



        public virtual object ExecuteScalar(IDbCommandData pCmdData)
        {
            using (var c = CreateAndOpenConnection())
                return ExecuteScalar(pCmdData, c);
        }

        public object ExecuteScalar(IDbCommandData pCmdData, IDbConnection pOpenConnection)
        {
            return ExecuteScalar(pCmdData, pOpenConnection, null);
        }

        public virtual object ExecuteScalar(IDbCommandData pCmdData, IDbTransaction pOpenTransaction)
        {
            if (pOpenTransaction?.Connection == null)
                throw new ArgumentException("Transaction must have open connection.", nameof(pOpenTransaction));

            return ExecuteScalar(pCmdData, pOpenTransaction.Connection, pOpenTransaction);
        }

        protected virtual object ExecuteScalar(IDbCommandData pCmdData, IDbConnection pOpenConnection, IDbTransaction pOpenTransaction)
        {
            try
            {
                using (var cmd = CreateCommand(pCmdData, pOpenConnection, pOpenTransaction))
                {
                    return cmd.ExecuteScalar();
                }
            }
            catch (Exception e)
            {
                if (e is DatabaseException)
                    throw;

                if (IsPrimaryKeyViolationException(e))
                    throw new DatabasePrimaryKeyViolationException("Failed to execute Scalar on database because of primary key violation.", e);

                throw new DatabaseException(string.Format(CultureInfo.CurrentCulture, "Failed to execute Scalar on database: {0}", e.Message), e);
            }
        }



        public virtual DataSet ExecuteDataset(IDbCommandData pCmdData)
        {
            using (var c = CreateAndOpenConnection())
            {
                return ExecuteDataset(pCmdData, c);
            }
        }

        public DataSet ExecuteDataset(IDbCommandData pCmdData, IDbConnection pOpenConnection)
        {
            return ExecuteDataset(pCmdData, pOpenConnection, null);
        }

        public virtual DataSet ExecuteDataset(IDbCommandData pCmdData, IDbTransaction pOpenTransaction)
        {
            if (pOpenTransaction?.Connection == null)
                throw new ArgumentException("Transaction must have open connection.", nameof(pOpenTransaction));

            return ExecuteDataset(pCmdData, pOpenTransaction.Connection, pOpenTransaction);
        }

        protected abstract DataSet ExecuteDataset(IDbCommandData pCmdData, IDbConnection pOpenConnection, IDbTransaction pOpenTransaction);



        public List<T> ExecuteReader<T>(IDbCommandData pCmdData, Func<IDataReader, T> pReaderFunc)
        {
            using (var c = CreateAndOpenConnection())
            {
                return ExecuteReader(pCmdData, pReaderFunc, c);
            }
        }

        public List<T> ExecuteReader<T>(IDbCommandData pCmdData, Func<IDataReader, T> pReaderFunc, IDbConnection pOpenConnection)
        {
            return ExecuteReader(pCmdData, pReaderFunc, pOpenConnection, null);
        }

        public List<T> ExecuteReader<T>(IDbCommandData pCmdData, Func<IDataReader, T> pReaderFunc, IDbTransaction pOpenTransaction)
        {
            if (pOpenTransaction?.Connection == null)
                throw new ArgumentException("Transaction must have open connection.", nameof(pOpenTransaction));

            return ExecuteReader(pCmdData, pReaderFunc, pOpenTransaction.Connection, pOpenTransaction);
        }

        protected abstract List<T> ExecuteReader<T>(IDbCommandData pCmdData, Func<IDataReader, T> pReaderFunc, IDbConnection pOpenConnection, IDbTransaction pOpenTransaction);



        public int ExecuteProcedureAndReturnAffectedRows(IDbCommandData pCmdData)
        {
            using (var c = CreateAndOpenConnection())
            {
                return ExecuteProcedureAndReturnAffectedRows(pCmdData, c);
            }
        }

        public int ExecuteProcedureAndReturnAffectedRows(IDbCommandData pCmdData, IDbConnection pOpenConnection)
        {
            return ExecuteProcedureAndReturnAffectedRows(pCmdData, pOpenConnection, null);
        }

        public virtual int ExecuteProcedureAndReturnAffectedRows(IDbCommandData pCmdData, IDbTransaction pOpenTransaction)
        {
            return ExecuteProcedureAndReturnAffectedRows(pCmdData, pOpenTransaction.Connection, pOpenTransaction);
        }

        protected virtual int ExecuteProcedureAndReturnAffectedRows(IDbCommandData pCmdData, IDbConnection pOpenConnection, IDbTransaction pOpenTransaction)
        {
            if (pCmdData.Type != CommandType.StoredProcedure)
                throw new ArgumentException("Command is not of StoredProcedure type.", nameof(pCmdData));

            return pOpenTransaction == null ? ExecuteNonQuery(pCmdData, pOpenConnection) : ExecuteNonQuery(pCmdData, pOpenTransaction);
        }


        public virtual IList<string> RemoveInvalidPartsAndSplitSqlStatements(string pSqlText, string pStatementSeparator)
        {
            if (pStatementSeparator == null || !pSqlText.Contains(pStatementSeparator))
                return new List<string> { pSqlText };

            var splittedStatements = Regex.Split(pSqlText, @"^[\t\r\n]*" + Regex.Escape(pStatementSeparator) + @"[\t\r\n]*\d*[\t\r\n]*(?:--.*)?$",
                RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);

            return new List<string>(splittedStatements.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim(' ', '\r', '\n')));
        }


        private IDbCommand CreateCommand(IDbCommandData pCmdData, IDbConnection pOpenConnection, IDbTransaction pOpenTransaction)
        {
            var cmd = pOpenConnection.CreateCommand();
            cmd.CommandTimeout = (int)m_DbBasicSettings.CommandTimeout.TotalSeconds;
            cmd.CommandText = pCmdData.SqlText;
            cmd.CommandType = pCmdData.Type;
            cmd.Connection = pOpenConnection;
            if (pOpenTransaction != null)
                cmd.Transaction = pOpenTransaction;

            AddParamsToCommand(cmd, pCmdData);
            return cmd;
        }

        protected static void AddParamsToCommand(IDbCommand pCmd, IDbCommandData pCmdData)
        {
            foreach (var p in pCmdData.Parameters)
                pCmd.Parameters.Add(p);
        }
    }
}
