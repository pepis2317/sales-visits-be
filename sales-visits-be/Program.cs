using entities;
using Microsoft.EntityFrameworkCore;
using sales_visits_be.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        o => o.UseNetTopologySuite()  // ← must be here
    )
);
builder.WebHost.UseUrls("http://0.0.0.0:5075");
builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("InternalOnly", policy =>
    {
        policy
            .WithOrigins("http://localhost:3000", "https://localhost:3000", "https://gray-mud-0a109740f.2.azurestaticapps.net")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddScoped<PriorityScoreService>();
builder.Services.AddSingleton<DistanceMatrixService>();
builder.Services.AddSingleton<RouteSolverService>();
builder.Services.AddTransient<BlobService>();
builder.Services.AddScoped<TerritoryService>();

var app = builder.Build();
app.MapControllers();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("InternalOnly");

app.Run();
