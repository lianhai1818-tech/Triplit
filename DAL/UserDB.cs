using MySql.Data.MySqlClient;
using Model;
using System;
using System.Collections.Generic;
using System.Data.Common; // דרוש עבור DbDataReader
using System.Threading.Tasks;

namespace DAL
{
    public class UserDB
    {
        private readonly string _connectionString;

        public UserDB(string connectionString)
        {
            _connectionString = connectionString;
        }

        // פעולת עזרה פרטית מתוקנת שמקבלת DbDataReader ומונעת אזהרות Null
        private User ReadUser(DbDataReader reader)
        {
            return new User(
                Convert.ToInt32(reader["Id"]),
                reader["Username"]?.ToString() ?? string.Empty,
                reader["Email"]?.ToString() ?? string.Empty,
                reader["Password"]?.ToString() ?? string.Empty,
                Convert.ToBoolean(reader["IsActive"]),
                reader["RoleName"]?.ToString() ?? string.Empty
            );
        }

        // הוספת משתמש חדש
        public async Task<int> AddUserAsync(User user)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Users (Username, Email, Password, IsActive) 
                VALUES (@username, @email, @password, @isActive);
                SELECT LAST_INSERT_ID();";

            command.Parameters.AddWithValue("@username", user.Username);
            command.Parameters.AddWithValue("@email", user.Email);
            command.Parameters.AddWithValue("@password", user.Password);
            command.Parameters.AddWithValue("@isActive", user.IsActive);

            var id = await command.ExecuteScalarAsync();
            return Convert.ToInt32(id);
        }

        // בדיקה האם שם משתמש כבר קיים
        public async Task<bool> UsernameExistsAsync(string username)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(1) FROM Users WHERE Username = @username;";
            command.Parameters.AddWithValue("@username", username);

            var count = Convert.ToInt32(await command.ExecuteScalarAsync());
            return count > 0;
        }

        // בדיקה האם האימייל כבר קיים
        public async Task<bool> EmailExistsAsync(string email)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(1) FROM Users WHERE Email = @email;";
            command.Parameters.AddWithValue("@email", email);

            var count = Convert.ToInt32(await command.ExecuteScalarAsync());
            return count > 0;
        }

        // שליפת כל המשתמשים
        public async Task<List<User>> GetAllUsersAsync()
        {
            var users = new List<User>();
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Username, Email, Password, IsActive, 'Student' AS RoleName FROM Users;";

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                users.Add(ReadUser(reader));
            }

            return users;
        }

        // בדיקת התחברות
        public async Task<User?> CheckLoginAsync(string username, string password)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Username, Email, Password, IsActive, 'Student' AS RoleName FROM Users WHERE Username = @username;";
            command.Parameters.AddWithValue("@username", username);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var user = ReadUser(reader);
                if (user.Password == password)
                {
                    return user;
                }
            }
            return null;
        }
    }
}