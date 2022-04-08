using WebApplication1.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var redisDefaultSection = builder.Configuration.GetSection("Redis:Default");
var redisConnectionString = redisDefaultSection.GetValue<string>("Connection");
var redisInstanceName = redisDefaultSection.GetValue<string>("InstanceName");
var redisDefaultDB = redisDefaultSection.GetValue<int>("DefaultDB");



builder.Services.AddSingleton(new RedisHelper(redisConnectionString, redisInstanceName, redisDefaultDB));



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
