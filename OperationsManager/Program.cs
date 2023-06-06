using OperationsManager.BackgroundServices;
using OperationsManager.BackgroundServices.Workers;
using OperationsManager.Configurations;
using OperationsManager.Database;
using OperationsManager.Database.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<KafkaConfigSection>(builder.Configuration.GetSection("KafkaConfigSection"));
builder.Services.Configure<DatabaseApiConfigSection>(builder.Configuration.GetSection("DatabaseApiConfigSection"));

builder.Services.AddSingleton<KafkaQueue>();
builder.Services.AddSingleton<TKafkaConsumer>();
builder.Services.AddSingleton<TMessageHandler>();
builder.Services.AddSingleton<IDatabaseProvider, DatabaseProvider>();

var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

var messageHandler = app.Services.GetService<TMessageHandler>();
messageHandler?.Start();

// Register the factory method for message handler with a Port argument

var kafkaConsumer = app.Services.GetService<TKafkaConsumer>();
kafkaConsumer?.Start();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

