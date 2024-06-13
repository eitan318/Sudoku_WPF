using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;

namespace DAL
{
    public class DBHelper
    {
        public static OleDbConnection GetConnection()
        {
            string connString = @"Provider=Microsoft.ACE.OLEDB.12.0;
                                Data Source=C:\Users\eitan\Desktop\NEW_WPF\Sudoku_WPF\DAL\Data\Sudoku_DB.accdb;
                                Persist Security Info=True";

            OleDbConnection conn = new OleDbConnection(connString);

            return conn;
        }

        public static OleDbCommand GetCommand(OleDbConnection conn, string sqlstmt)
        {
            OleDbCommand cmd = new OleDbCommand(sqlstmt, conn);

            return cmd;
        }

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

        public static void ExecuteCommand(string sqlStmt, params OleDbParameter[] parameters)
        {
            using (OleDbConnection conn = GetConnection())
            {
                using (OleDbCommand cmd = new OleDbCommand(sqlStmt, conn))
                {
                    cmd.Parameters.AddRange(parameters); // Add parameters to the command

                    conn.Open();
                    cmd.ExecuteNonQuery(); // Execute the non-query command (insert, update, delete)
                }
            }
        }
    }
}
