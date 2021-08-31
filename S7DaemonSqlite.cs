using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Data.Sqlite;

namespace S7Console
{
    class S7DaemonSqlite
    {
        // S7Daemon logger
        // Coding by kjurlina. Have a lot of fun
        // Application database master

        private string DboFilePath;
        private string FullDboFilePath;
        private string DboFileName;

        public S7DaemonSqlite(string path, string name)
        {
            // Compose Sqlite file path, name and exitension
            DboFilePath = path;
            DboFileName = name;
            FullDboFilePath = DboFilePath + DboFileName;
        }

        public void CreateDatabase()
        {
	        // This should be revisited	
            // SqliteConnection.CreateFile(FullDboFilePath);
        }

        public bool CheckDatabaseExists()
        {
            // Check if Sqlite database file exists         
            return File.Exists(FullDboFilePath);
        }

        public void CreateTable(string name, string type)
        {
            // First determine requested data type
            string TagDataType;
            if (type == "1" | type == "2")
            {
                TagDataType = "INTEGER";
            }
            else if (type == "3")
            {
                TagDataType = "REAL";
            }
            else
            {
                TagDataType = "varchar(32)";
            }

            // Create database table
            string CmdString = "CREATE TABLE " + name + " (Timestamp varchar(32), Value " + TagDataType + ")";
            SqliteConnection Conn = new SqliteConnection("Data Source=" + FullDboFilePath);
            SqliteCommand Cmd = new SqliteCommand(CmdString, Conn);

            Conn.Open();
            Cmd.ExecuteNonQuery();
            Conn.Close();
        }

        public bool CheckTableExists(string name)
        {
            // Check if database table exists
            bool TableExists = false;
            string QueryString = "SELECT name FROM Sqlite_master WHERE type = 'table'";
            SqliteConnection Conn = new SqliteConnection("Data Source=" + FullDboFilePath);
            SqliteCommand Cmd = new SqliteCommand(QueryString, Conn);

            Conn.Open();
            SqliteDataReader Reader = Cmd.ExecuteReader();
            while (Reader.Read())
                if (Reader.GetString(0) == name)
                {
                    TableExists = true;
                    break;
                }
            Reader.Close();
            Conn.Close();

            return TableExists;
        }

        public void InsertIntoTable(string tag, object value, DateTime jiffy)
        {
            string QueryString = "INSERT INTO " + tag + "(Timestamp, Value) VALUES('" + jiffy.ToString() + "','" + value.ToString() + "')";

            SqliteConnection Conn = new SqliteConnection("Data Source =" + FullDboFilePath + ";");
            SqliteCommand Cmd = new SqliteCommand(QueryString, Conn);

            Conn.Open();
            Cmd.ExecuteNonQuery();
            Conn.Close();
        }
    }
}
