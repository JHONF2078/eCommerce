using FluentValidation.AspNetCore;
using ProductsService.API.Middlewares;
using ProductsService.BusinessLogicLayer;
using ProductsService.DataAccessLayer;
using ProductsService.API.APIEndpoints;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

//Add DAL and BLL services
builder.Services.AddDataAccessLayer(builder.Configuration);
builder.Services.AddBusinessLogicLayer();

builder.Services.AddControllers();

//FluentValidations
builder.Services.AddFluentValidationAutoValidation();

//Add model binder to read values from JSON to enum
builder.Services.ConfigureHttpJsonOptions(options =>{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var app = builder.Build();

app.UseExceptionHandlingMiddleware();
app.UseRouting();

//Auth
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
//Map endpoints
app.MapProductAPIEndpoints();

app.Run();
