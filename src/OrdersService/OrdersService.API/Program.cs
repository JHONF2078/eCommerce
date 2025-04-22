using OrdersService.DataAccessLayer;
using OrdersService.BusinessLogicLayer;
using FluentValidation.AspNetCore;
using OrdersService.API.Middleware;
using OrdersMicroservice.BusinessLogicLayer.HttpClients;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using OrdersService.BusinessLogicLayer.Policies;

var builder = WebApplication.CreateBuilder(args);

//Add DAL and BLL services
builder.Services.AddDataAccessLayer(builder.Configuration);
builder.Services.AddBusinessLogicLayer(builder.Configuration);


builder.Services.AddControllers();

//FluentValidations
builder.Services.AddFluentValidationAutoValidation();

//Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//cors
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.WithOrigins("http://localhost:4200")
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});


//add policies
builder.Services.AddTransient<IUsersMicroservicePolices, UsersMicroservicePolicies>();


//to comunication with other microservices
builder.Services.AddHttpClient<UsersMicroserviceClient>(client =>
{
    client.BaseAddress = new Uri($"http://" +
        $"{builder.Configuration["USERSMICROSERVICENAME"]}:" +
        $"{builder.Configuration["USERSMICROSERVICEPORT"]}");
}).AddPolicyHandler(
    builder.Services.BuildServiceProvider()
    .GetRequiredService<IUsersMicroservicePolices>()
    .GetRetryPolicy()
)
.AddPolicyHandler(
    builder.Services.BuildServiceProvider()
    .GetRequiredService<IUsersMicroservicePolices>()
    .GetCircuitBreakerPolicy()
 );




builder.Services.AddHttpClient<ProductsMicroserviceClient>(client => {
    client.BaseAddress = new Uri($"http://" +
        $"{builder.Configuration["PRODUCTSMICROSERVICENAME"]}:" +
        $"{builder.Configuration["PRODUCTSMICROSERVICEPORT"]}");
});


//foreach (var kvp in builder.Configuration.AsEnumerable())
//{
//    Console.WriteLine($"Key: {kvp.Key}, Value: {kvp.Value}");
//}

var app = builder.Build();

app.UseExceptionHandlingMiddleware();
app.UseRouting();

//cors
app.UseCors();


//swagger
app.UseSwagger();
app.UseSwaggerUI();

//AUTH
//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

//endpoints
app.MapControllers();

app.Run();
