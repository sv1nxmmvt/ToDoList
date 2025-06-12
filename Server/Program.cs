using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TodoApp.Data;
using TodoApp.Models;

var builder = WebApplication.CreateBuilder(args);

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// ��������� Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ��������� Identity
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// ��������� Identity
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
});

// ��������� MVC �����������
builder.Services.AddControllers();

var app = builder.Build();

// ������� �� � ��������� �������� ��� �������
using (var scope = app.Services.CreateScope())
{
    await CreateDatabaseIfNotExists();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
}

async Task CreateDatabaseIfNotExists()
{
    var masterConnectionString = "Host=localhost;Database=postgres;Username=postgres;Password=zasada1324";
    var dbName = "tododb";

    using var connection = new Npgsql.NpgsqlConnection(masterConnectionString);
    await connection.OpenAsync();

    // ��������� ������������� ��
    var checkDbCommand = new Npgsql.NpgsqlCommand($"SELECT 1 FROM pg_database WHERE datname = '{dbName}'", connection);
    var dbExists = await checkDbCommand.ExecuteScalarAsync();

    if (dbExists == null)
    {
        // ������� �� ���� �� ����������
        var createDbCommand = new Npgsql.NpgsqlCommand($"CREATE DATABASE {dbName} OWNER postgres", connection);
        await createDbCommand.ExecuteNonQueryAsync();
        Console.WriteLine($"Database '{dbName}' created successfully");
    }
    else
    {
        Console.WriteLine($"Database '{dbName}' already exists");
    }
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