
using System;
using System.Data;
using System.Data.OleDb;

namespace DAL
{
    public class DBHelper
    {
        /// <summary>
        /// Creates and returns a new OleDbConnection object.
        /// </summary>
        /// <returns>An OleDbConnection object with the connection string set.</returns>
        public static OleDbConnection GetConnection()
        {
            string connString = @"Provider=Microsoft.ACE.OLEDB.12.0;
                                Data Source=C:\Users\Cyber_User\Desktop\Programming\Sudoku_WPF\DAL\Data\Sudoku_DB1.accdb;
                                Persist Security Info=True";
            OleDbConnection conn = new OleDbConnection(connString);
            return conn;
        }

        /// <summary>
        /// Creates and returns a new OleDbCommand object.
        /// </summary>
        /// <param name="conn">The OleDbConnection to use for the command.</param>
        /// <param name="sqlstmt">The SQL statement to execute.</param>
        /// <returns>An OleDbCommand object with the specified SQL statement and connection.</returns>
        public static OleDbCommand GetCommand(OleDbConnection conn, string sqlstmt)
        {
            OleDbCommand cmd = new OleDbCommand(sqlstmt, conn);
            return cmd;
        }

        /// <summary>
        /// Executes the provided SQL statement and returns the results as a DataTable.
        /// </summary>
        /// <param name="sqlstmt">The SQL statement to execute.</param>
        /// <returns>A DataTable containing the results of the SQL query.</returns>
        public static DataTable GetDataTable(string sqlstmt)
        {
            OleDbConnection conn = GetConnection();
            OleDbCommand cmd = GetCommand(conn, sqlstmt);
            OleDbDataAdapter adp = new OleDbDataAdapter();
            adp.SelectCommand = cmd;
            DataTable dt = new DataTable();
            adp.Fill(dt);
            return dt;
        }

        /// <summary>
        /// Executes a non-query SQL statement with the provided parameters.
        /// </summary>
        /// <param name="sqlStmt">The SQL statement to execute.</param>
        /// <param name="parameters">The parameters to add to the SQL command.</param>
        public static void ExecuteCommand(string sqlStmt, params OleDbParameter[] parameters)
        {
            OleDbConnection conn = GetConnection();
            try
            {
                conn.Open();
                OleDbCommand cmd = new OleDbCommand(sqlStmt, conn);
                cmd.Parameters.AddRange(parameters);
                int ret = cmd.ExecuteNonQuery();
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// Retrieves the next available ID for a specified table and column.
        /// </summary>
        /// <param name="tableName">The name of the table to query.</param>
        /// <param name="idColumnName">The name of the ID column to query.</param>
        /// <returns>The next available ID as an integer.</returns>
        public static int GetNextId(string tableName, string idColumnName)
        {
            int nextId = 1; // Default starting point for IDs
            using (OleDbConnection conn = GetConnection())
            {
                string query = $"SELECT MAX([{idColumnName}]) FROM [{tableName}]";
                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    conn.Open();
                    var result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        nextId = Convert.ToInt32(result) + 1;
                    }
                }
            }
            return nextId;
        }
    }
}
