using WebApiApex.Services;

//Create the Builder
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//DI the HttpClient to get NYC Data from MockBin.
builder.Services.AddHttpClient("MockBinClient", client => client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("MockBinBaseUri")));

//DI the Service for NYC Data 
builder.Services.AddScoped<ServiceNYCData>();

//Build the builder.
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//Middleware that redirects http requests to https.
app.UseHttpsRedirection();


//   1. return departments whose expenses meet or exceed their funding
app.MapGet("/DepartmentsExpensesOverFunding", (ServiceNYCData s) =>
{
    try
    {
        return s.DepartmentsExpensesOverFunding();
    }
    catch (global::System.Exception)
    {

        throw;
    }

});

//   2. return deparments whose expenses have increased over time by user specified percentage (int) and # of years (int)
app.MapGet("/DepartmentsExpensesIncreased/{percentIncreaseFilter}/{numberOfYearsFilter}", (int percentIncreaseFilter, int numberOfYearsFilter, ServiceNYCData s) =>
{
    try
    {
        return s.DepartmentsExpensesIncreased(percentIncreaseFilter, numberOfYearsFilter);
    }
    catch (global::System.Exception)
    {

        throw;
    }

});

//   3. return departments whose expenses are a user specified percentage below their funding year over year.
app.MapGet("/DepartmentsExpensesBelowFunding", (ServiceNYCData s) =>
{
    try
    {
        s.DepartmentsExpensesBelowFunding();
    }
    catch (global::System.Exception)
    {

        throw;
    }


});

//Run the app.
app.Run();



//var summaries = new[]
//{
//    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
//};

//app.MapGet("/weatherforecast", () =>
//{
//    var forecast = Enumerable.Range(1, 5).Select(index =>
//       new WeatherForecast
//       (
//           DateTime.Now.AddDays(index),
//           Random.Shared.Next(-20, 55),
//           summaries[Random.Shared.Next(summaries.Length)]
//       ))
//        .ToArray();
//    return forecast;
//})
//.WithName("GetWeatherForecast");


//internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
//{
//    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
//}