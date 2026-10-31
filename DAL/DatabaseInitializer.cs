using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;

namespace DAL
{
    public static class DatabaseInitializer
    {
        public static async Task InitializeAsync(string connectionString)
        {
            await CreateDatabaseAsync(connectionString);
            await CreateTablesAsync(connectionString);
        }

        private static async Task CreateDatabaseAsync(string baseConnectionString)
        {
            // מחלצים את השרת והפורט בלבד כדי להתחבר לשרת ה-MySQL ללא ציון שם בסיס נתונים
            var builder = new MySqlConnectionStringBuilder(baseConnectionString)
            {
                Database = ""
            };

            using var connection = new MySqlConnection(builder.ConnectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "CREATE DATABASE IF NOT EXISTS TriplitDB;";
            await command.ExecuteNonQueryAsync();
        }

        private static async Task CreateTablesAsync(string baseConnectionString)
        {
            // מוסיפים את שם בסיס הנתונים למחרוזת החיבור
            var builder = new MySqlConnectionStringBuilder(baseConnectionString)
            {
                Database = "TriplitDB"
            };

            using var connection = new MySqlConnection(builder.ConnectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();

            // יצירת טבלת משתמשים (Users)
            // 1. יצירת טבלת המשתמשים (Users) כולל שדה IsActive
            command.CommandText = @"
    CREATE TABLE IF NOT EXISTS Users (
        Id INT AUTO_INCREMENT PRIMARY KEY,
        Username VARCHAR(100) NOT NULL,
        Email VARCHAR(150) NOT NULL,
        Password VARCHAR(255) NOT NULL,
        IsActive BOOLEAN NOT NULL DEFAULT TRUE
    );";
            await command.ExecuteNonQueryAsync();

            // יצירת טבלת טיולים (Trips) עם קשר לטבלת המשתמשים
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Trips (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    Title VARCHAR(150) NOT NULL,
                    Destination VARCHAR(100) NOT NULL,
                    StartDate DATETIME NOT NULL,
                    EndDate DATETIME NOT NULL,
                    UserId INT NOT NULL,
                    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
                );";
            await command.ExecuteNonQueryAsync();
        }
    }
}