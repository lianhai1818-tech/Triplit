using Microsoft.AspNetCore.Mvc;
using DAL;
using Model;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace EmetGuard.Controllers
{
    [Route("api/[controller]")] // הכתובת תתחיל ב־ api/users[cite: 6, 7]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserDB _userDB;

        // הזרקת ה־UserDB אוטומטית למחלקה
        public UsersController(UserDB userDB)
        {
            _userDB = userDB;
        }

        // שליפת כל המשתמשים (GET: api/users)
        [HttpGet]
        public async Task<ActionResult<List<User>>> GetAllUsers()
        {
            var users = await _userDB.GetAllUsersAsync();
            return Ok(users); // קוד 200[cite: 6, 7]
        }

        // הוספת משתמש חדש (POST: api/users/register)
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            // בדיקה האם שם המשתמש או האימייל כבר תפוסים לפני ההוספה[cite: 6, 7]
            if (await _userDB.UsernameExistsAsync(user.Username) || await _userDB.EmailExistsAsync(user.Email))
            {
                return Conflict("Username or Email is already taken."); // קוד 409 - התנגשות[cite: 6, 7]
            }

            int newId = await _userDB.AddUserAsync(user);
            return StatusCode(201, newId); // קוד 201 - נוצר בהצלחה[cite: 6, 7]
        }

        // בדיקת התחברות (POST: api/users/login)
        [HttpPost("login")]
        public async Task<ActionResult<User>> Login([FromBody] LoginDto loginDto)
        {
            var user = await _userDB.CheckLoginAsync(loginDto.Username, loginDto.Password);
            if (user == null)
            {
                return Unauthorized("Invalid username or password."); // קוד 401 - אין הרשאה[cite: 6, 7]
            }

            return Ok(user); // קוד 200[cite: 6, 7]
        }
    }

    // מחלקי עזר לקבלת נתוני התחברות ב־Body של הבקשה
    public class LoginDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}