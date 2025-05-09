using MYSECCLAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
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
    options.AddPolicy("AllowBlazorApp",
        policy =>
        {
            policy.WithOrigins(builder.Configuration["AllowedCorsOrigin"] ?? "https://localhost:44632", // Default Blazor WASM port, update if different
                               "http://localhost:44633") // Allow http for Blazor dev server
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowBlazorApp"); // Apply CORS policy

app.UseAuthorization();

app.MapControllers();

app.Run();
