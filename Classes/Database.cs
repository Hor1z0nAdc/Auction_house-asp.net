using System.Data.SQLite;
using System.IO;
using System;

public class Database {
    public SQLiteConnection connection;

    public Database() {
        connection = new SQLiteConnection("Data source=database.sqlite3");
        if(!File.Exists("database.sqlite3")) { 
            SQLiteConnection.CreateFile("database.sqlite3"); 
            Console.WriteLine("Database file has been created");
        }
    }

    public void OpenConnection() {
        if(connection.State != System.Data.ConnectionState.Open) connection.Open();
    }
    
    public void CloseConnection() {
        if(connection.State != System.Data.ConnectionState.Closed) connection.Close();
    }
}