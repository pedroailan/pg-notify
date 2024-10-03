using PgNotify_Consumer.API;
using Prometheus;
using Commons;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddLogs();

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

//app.UseSwagger();
//app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();

// Configurar o middleware para expor métricas no endpoint "/metrics"
app.UseMetricServer();

// Incluir métricas de requisição HTTP
app.UseHttpMetrics();

app.MapControllers();

app.Run();
