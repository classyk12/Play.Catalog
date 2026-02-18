using Play.Catalog.Service.Endpoints;
using Play.Catalog.Service.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMongoServices();

// register the open-generic repository; MongoRepository<T> now computes its own collection name
builder.Services.AddSingleton(typeof(IRepository<>), typeof(MongoRepository<>));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Play.Catalog.Service v1");
        c.RoutePrefix = string.Empty;
    });
}

// Map minimal API endpoints (Item endpoints)
app.MapItemEndpoints();

app.UseHttpsRedirection();

app.Run();