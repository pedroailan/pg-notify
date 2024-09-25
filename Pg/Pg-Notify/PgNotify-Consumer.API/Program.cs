using PgNotify_Consumer.API;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<Consumer>();
builder.Services.AddSingleton<Response>();
builder.Services.AddHostedService(provider => provider.GetRequiredService<Consumer>());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// Configurar o middleware para expor métricas no endpoint "/metrics"
app.UseMetricServer();

// Incluir métricas de requisição HTTP
app.UseHttpMetrics();

app.MapControllers();

app.Run();
