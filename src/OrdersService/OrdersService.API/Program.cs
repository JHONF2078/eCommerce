using OrdersService.DataAccessLayer;
using OrdersService.BusinessLogicLayer;
using FluentValidation.AspNetCore;
using OrdersService.API.Middleware;

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

var app = builder.Build();

app.UseExceptionHandlingMiddleware();
app.UseRouting();

//cors
app.UseCors();


//swagger
app.UseSwagger();
app.UseSwaggerUI();

//AUTH
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

//endpoints
app.MapControllers();

app.Run();
