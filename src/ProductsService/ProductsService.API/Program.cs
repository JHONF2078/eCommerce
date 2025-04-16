using FluentValidation.AspNetCore;
using ProductsService.API.Middlewares;
using ProductsService.BusinessLogicLayer;
using ProductsService.DataAccessLayer;

var builder = WebApplication.CreateBuilder(args);

//Add DAL and BLL services
builder.Services.AddDataAccessLayer();
builder.Services.AddBusinessLogicLayer();

builder.Services.AddControllers();

//FluentValidations
builder.Services.AddFluentValidationAutoValidation();


var app = builder.Build();

app.UseExceptionHandlingMiddleware();
app.UseRouting();

//Auth
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
