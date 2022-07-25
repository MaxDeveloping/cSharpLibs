using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace CommonLibs.Database.Interfaces
{
    public interface IDatabase
    {
        IDbConnection CreateAndOpenConnection();

        int ExecuteNonQuery(IDbCommandData pCmdData);

        int ExecuteNonQuery(IDbCommandData pCmdData, IDbConnection pOpenConnection);

        int ExecuteNonQuery(IDbCommandData pCmdData, IDbTransaction pOpenTransaction);

        object ExecuteScalar(IDbCommandData pCmdData);

        object ExecuteScalar(IDbCommandData pCmdData, IDbConnection pOpenConnection);

        object ExecuteScalar(IDbCommandData pCmdData, IDbTransaction pOpenTransaction);

        DataSet ExecuteDataset(IDbCommandData pCmdData);

        DataSet ExecuteDataset(IDbCommandData pCmdData, IDbConnection pOpenConnection);

        DataSet ExecuteDataset(IDbCommandData pCmdData, IDbTransaction pOpenTransaction);

        List<T> ExecuteReader<T>(IDbCommandData pCmdData, Func<IDataReader, T> pReaderFunc);

        List<T> ExecuteReader<T>(IDbCommandData pCmdData, Func<IDataReader, T> pReaderFunc, IDbConnection pOpenConnection);

        List<T> ExecuteReader<T>(IDbCommandData pCmdData, Func<IDataReader, T> pReaderFunc, IDbTransaction pOpenTransaction);

        int ExecuteProcedureAndReturnAffectedRows(IDbCommandData pCmdData);

        int ExecuteProcedureAndReturnAffectedRows(IDbCommandData pCmdData, IDbConnection pOpenConnection);

        int ExecuteProcedureAndReturnAffectedRows(IDbCommandData pCmdData, IDbTransaction pOpenTransaction);

        IList<string> RemoveInvalidPartsAndSplitSqlStatements(string pSqlText, string pStatementSeparator);
    }
}
