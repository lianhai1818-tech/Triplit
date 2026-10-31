using DAL;
using MySql.Data.MySqlClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// --- 1. הוספת השורות האלו כדי שה־API יתמוך ב־Swagger ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// --------------------------------------------------------

// שליפת מחרוזת החיבור והוספת שם בסיס הנתונים TriplitDB עבור UserDB
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (!string.IsNullOrEmpty(connectionString))
{
    var mysqlBuilder = new MySqlConnectionStringBuilder(connectionString)
    {
        Database = "TriplitDB"
    };
    // רישום מחלקת UserDB כ־Singleton כך שתוזרק אוטומטית לקונטרולרים
    builder.Services.AddSingleton<UserDB>(sp => new UserDB(mysqlBuilder.ConnectionString));
}

var app = builder.Build();

// --- 2. הוספת השורות האלו כדי להפעיל את ממשק ה־Swagger בדפדפן ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// -----------------------------------------------------------------

// קריאה לאתחול מסד הנתונים והטבלאות בהרצה הראשונה
if (!string.IsNullOrEmpty(connectionString))
{
    await DatabaseInitializer.InitializeAsync(connectionString);
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();