using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TodoApp.Data;
using TodoApp.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

Console.Write("¬ведите пароль дл€ суперпользовател€ PostgreSQL: ");
var superUserPassword = ReadPasswordFromConsole();

Console.Write("¬ведите пароль дл€ базы данных tododb: ");
var todoDbPassword = ReadPasswordFromConsole();

var masterConnectionString = $"Host=localhost;Database=postgres;Username=postgres;Password={superUserPassword}";
var defaultConnectionString = $"Host=localhost;Database=tododb;Username=postgres;Password={todoDbPassword}";

builder.Configuration["ConnectionStrings:DefaultConnection"] = defaultConnectionString;
builder.Configuration["ConnectionStrings:MasterConnection"] = masterConnectionString;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
});

builder.Services.AddControllers();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    await CreateDatabaseIfNotExists();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
}

async Task CreateDatabaseIfNotExists()
{
    var masterConnString = builder.Configuration.GetConnectionString("MasterConnection");
    var dbName = "tododb";

    using var connection = new Npgsql.NpgsqlConnection(masterConnString);
    await connection.OpenAsync();

    var checkDbCommand = new Npgsql.NpgsqlCommand($"SELECT 1 FROM pg_database WHERE datname = '{dbName}'", connection);
    var dbExists = await checkDbCommand.ExecuteScalarAsync();

    if (dbExists == null)
    {
        var createDbCommand = new Npgsql.NpgsqlCommand($"CREATE DATABASE {dbName} OWNER postgres", connection);
        await createDbCommand.ExecuteNonQueryAsync();
        Console.WriteLine($"Database '{dbName}' created successfully");
    }
    else
    {
        Console.WriteLine($"Database '{dbName}' already exists");
    }
}

static string ReadPasswordFromConsole()
{
    var password = new StringBuilder();
    ConsoleKeyInfo keyInfo;

    do
    {
        keyInfo = Console.ReadKey(true);

        if (keyInfo.Key == ConsoleKey.Backspace)
        {
            if (password.Length > 0)
            {
                password.Remove(password.Length - 1, 1);
                Console.Write("\b \b");
            }
        }
        else if (keyInfo.Key != ConsoleKey.Enter)
        {
            password.Append(keyInfo.KeyChar);
            Console.Write("*");
        }
    }
    while (keyInfo.Key != ConsoleKey.Enter);

    Console.WriteLine();
    return password.ToString();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();