using Bank.Server.Application;
using Bank.Server.Application.Handlers;
using Bank.Server.Infrastructure.DependencyInjection;
using Bank.Server.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddTransient<WithdrawCommandHandler>();
builder.Services.AddHealthChecks();

// Use the traditional Swashbuckle Swagger generation engine
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<BankDbContext>();
        dbContext.Database.Migrate();
    }

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = "swagger";
    });
}

app.MapHealthChecks("/health")
   .WithOpenApi(operation =>
   {
       operation.Summary = "Health Check";
       operation.Description = "Returns the current health status of the application.";
       return operation;
   });

app.MapControllers();

app.Run();
