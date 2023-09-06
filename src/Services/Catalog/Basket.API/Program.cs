using Basket.API.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// adding redis service to the solution
builder.Services.AddStackExchangeRedisCache(options =>
{
    // registering the service connection
    options.Configuration = builder.Configuration.GetValue<string>("CacheSettings:ConnectionString");
});

// basically says that if you see any reference of the interface, its a reference of BasketRepository
builder.Services.AddScoped<IBasketRepository, BasketRepository>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
