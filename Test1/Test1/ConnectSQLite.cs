using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Data.SQLite;
//using SQLiteSamples;

namespace Test1
{
    public class ConnectSQLite
    {
        //数据库连接
        SQLiteConnection m_dbConnection;

        string m_sSqlFile = string.Empty;   // 数据库名称
        string m_sTableName = "highscores";

        public ConnectSQLite(string sSqlFile)
        {
            createNewDatabase(sSqlFile);
            connectToDatabase();
            createTable("CrossLog");//createTable("highscores");
            //fillTable();
            //printHighscores();
        }

        //创建一个空的数据库
        void createNewDatabase(string sSqlFile)
        {
            string sPath = "SqlDataBase";
            if (!Directory.Exists(sPath))
            {
                Directory.CreateDirectory(sPath);
            }

            m_sSqlFile = sPath + "\\" + sSqlFile;
            SQLiteConnection.CreateFile(m_sSqlFile);
            //SQLiteConnection.CreateFile("MyDatabase.sqlite");
        }

        //创建一个连接到指定数据库
        void connectToDatabase()
        {
            //m_dbConnection = new SQLiteConnection("Data Source=MyDatabase.sqlite;Version=3;");
            m_dbConnection = new SQLiteConnection("Data Source="+ m_sSqlFile + ";Version=3;");
            m_dbConnection.Open();
        }

        //在指定数据库中创建一个table
        void createTable(string sTableName)
        {
            m_sTableName = sTableName;
            //string sql = "create table highscores (name varchar(20), score int)";
            string sql = "create table " + sTableName + " (时间 varchar(50), 事件 varchar(50))";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();
        }

        // 报表增加数据
        public void AddTableData(string sDate, string sLog)
        {
            //string sql = "insert into highscores (时间, 事件) values ('sDate', 'sLog')";
            string sql = "insert into " + m_sTableName + " (时间, 事件) values ('sDate', '切换日志')";
            sql = sql.Replace("sDate", sDate);
            sql = sql.Replace("切换日志", sLog);
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();
        }



        //插入一些数据
        void fillTable()
        {
            string sql = "insert into highscores (name, score) values ('Me', 3)";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();

            sql = "insert into highscores (name, score) values ('Myself', 6)";
            command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();

            sql = "insert into highscores (name, score) values ('And I', 10)";
            command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();
        }

        //使用sql查询语句，并显示结果
        void printHighscores()
        {
            string sql = "select * from highscores order by score desc";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
                Console.WriteLine("Name: " + reader["name"] + "\tScore: " + reader["score"]);
            Console.ReadLine();
        }

    }
}
