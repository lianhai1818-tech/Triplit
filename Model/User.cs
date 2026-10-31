using System;

namespace Model
{
    public class User : BaseEntity
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string RoleName { get; set; } = string.Empty;

        // 1. בנאי ריק - חובה עבור המרת JSON (Deserialization) בקונטרולר
        public User() { }

        // 2. בנאי מלא - משמש את ה־DAL כששולפים נתונים ממסד הנתונים
        public User(int id, string username, string email, string password, bool isActive, string roleName)
            : base(id)
        {
            Username = username;
            Email = email;
            Password = password;
            IsActive = isActive;
            RoleName = roleName;
        }
    }
}