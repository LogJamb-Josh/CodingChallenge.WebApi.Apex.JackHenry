using WebApiApex.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ServiceNYCData>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
       new WeatherForecast
       (
           DateTime.Now.AddDays(index),
           Random.Shared.Next(-20, 55),
           summaries[Random.Shared.Next(summaries.Length)]
       ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");


//   1. return departments whose expenses meet or exceed their funding
app.MapGet("/DepartmentsExpensesOverFunding", async (ServiceNYCData s) =>
{
    try
    {
        await s.DepartmentsExpensesOverFunding();
    }
    catch (global::System.Exception)
    {

        throw;
    }

});

//   2. return deparments whose expenses have increased over time by user specified percentage (int) and # of years (int)
app.MapGet("/DepartmentsExpensesIncreased", async (ServiceNYCData s) =>
{
    try
    {
        await s.DepartmentsExpensesIncreased();
    }
    catch (global::System.Exception)
    {

        throw;
    }

});

//   3. return departments whose expenses are a user specified percentage below their funding year over year.
app.MapGet("/DepartmentsExpensesBelowFunding", async (ServiceNYCData s) =>
{
    try
    {
        await s.DepartmentsExpensesBelowFunding();
    }
    catch (global::System.Exception)
    {

        throw;
    }


});
app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}