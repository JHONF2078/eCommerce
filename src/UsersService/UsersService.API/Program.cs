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

//Ad  API explorer services
builder.Services.AddEndpointsApiExplorer();

//Add swagger generation services to create swagger specification
builder.Services.AddSwaggerGen();

//Add cors services
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDevClient",
        builder =>
        {
            builder.WithOrigins("http://localhost:4200", "https://localhost:4200") // Replace with your Angular app URL
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

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
//add endpoints that can serve the swager.sjon
app.UseSwagger();
//add swager UI (interactive pago to explore and test API endpoints)
app.UseSwaggerUI();

//Auth
app.UseAuthentication();
app.UseAuthorization();

//CORS
app.UseCors("AllowAngularDevClient");

//Controller routes
app.MapControllers();
app.Run();
