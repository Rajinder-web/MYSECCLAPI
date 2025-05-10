using MYSECCLAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure HttpClient for SecclApiService
builder.Services.AddHttpClient<ISecclApiService, SecclApiService>();
builder.Services.AddMemoryCache(); // For caching access token

// Register custom services
builder.Services.AddScoped<IPortfolioAggregationService, PortfolioAggregationService>();
// CORS configuration for Blazor WASM app (adjust origin if needed)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient",
        policy =>
        {
        policy.WithOrigins("https://localhost:44632")// URL of your Blazor WebAssembly app
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});


builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowBlazorClient"); // Apply CORS policy

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
