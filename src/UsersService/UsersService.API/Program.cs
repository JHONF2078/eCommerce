using UsersService.Infrastructure;
using UsersService.Application;
using UsersService.API.Middlewares;
using System.Text.Json.Serialization;
using UsersService.Application.Mappers;
using FluentValidation.AspNetCore;


var builder = WebApplication.CreateBuilder(args);

var environment = builder.Environment.EnvironmentName;
Console.WriteLine($"Entorno actual: {environment}"); // Te dirá si es Development, Debug o Production


//Add Infrastructure services to the IoC container
builder.Services.AddInfrastructure();
builder.Services.AddApplication();

//Add controllers to tke service collection
builder.Services.AddControllers().AddJsonOptions
    (options =>
    {
        options.JsonSerializerOptions.Converters.Add
        (
            //para serializar enumeraciones cuando se manda la peticion,
            //por ejemplo  "Gender": "Female", para que visual estudio convierta vien la enumeracion
            new JsonStringEnumConverter()
        );
    });

//Add automapper profile to the service collection
builder.Services.AddAutoMapper(typeof(UserMappingProfile).Assembly);
//builder.Services.AddAutoMapper(typeof(RegisterRequestMappingProfile).Assembly);

//FluentValidations
builder.Services.AddFluentValidationAutoValidation();

//Build the web application
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    // puedes cargar más middlewares o configuraciones específicas
}
else
{
    app.UseExceptionHandler("/error");
}


app.UseExceptionHandlingMiddleware();

//Routing
app.UseRouting();

//Auth
app.UseAuthentication();
app.UseAuthorization();

//Controller routes
app.MapControllers();
app.Run();
