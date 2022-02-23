using WebApiApex.DTO;
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


/// <summary>
/// 1. return departments whose expenses meet or exceed their funding
/// </summary>
app.MapGet("/DepartmentsExpensesOverFunding", (ServiceNYCData s) =>
{
    try
    {
        return Results.Ok(s.DepartmentsExpensesOverFunding());
    }
    catch (Exception e)
    {
        //Maybe log the exception happened.
        return Results.Problem();
    }
})
    .Produces<List<DepartmentsExpensesOverFundingResponseModel>>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status500InternalServerError);

/// <summary>
/// 2. return deparments whose expenses have increased over time by user specified percentage (int) and # of years (int)
/// </summary>
app.MapGet("/DepartmentsExpensesIncreased/{percentIncreaseFilter}/{numberOfYearsFilter}", (int percentIncreaseFilter, int numberOfYearsFilter, ServiceNYCData s) =>
{
    try
    {
        return Results.Ok(s.DepartmentsExpensesIncreased(percentIncreaseFilter, numberOfYearsFilter));
    }
    catch (Exception e)
    {
        //Maybe log the exception happened.
        return Results.Problem();
    }

})
    .Produces<List<DepartmentsExpensesIncreasedResponseModel>>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status500InternalServerError);

/// <summary>
/// 3. return departments whose expenses are a user specified percentage below their funding year over year.
/// </summary>
app.MapGet("/DepartmentsExpensesBelowFunding/{belowFundingPercentageFilter}", (int belowFundingPercentageFilter, ServiceNYCData s) =>
{
    try
    {
        return Results.Ok(s.DepartmentsExpensesBelowFunding(belowFundingPercentageFilter));
    }
    catch (Exception e)
    {
        //Maybe log the exception happened.
        return Results.Problem();
    }
})
    .Produces<List<DepartmentsExpensesBelowFundingResponseModel>>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status500InternalServerError);

//Run the app.
app.Run();

