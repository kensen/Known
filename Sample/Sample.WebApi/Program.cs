var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.Services.AddKnown(info =>
{
    info.Id = "API";
    info.Type = AppType.WebApi;
});
builder.Services.AddKnownCore(info =>
{
    //���ݿ�����
    info.Connections = [new Known.ConnectionInfo
    {
        Name = "Default",
        DatabaseType = DatabaseType.SQLite,
        ProviderType = typeof(Microsoft.Data.Sqlite.SqliteFactory),
        ConnectionString = builder.Configuration.GetSection("ConnString").Get<string>()
    }];
});

var app = builder.Build();
// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapRazorPages();
app.Run();