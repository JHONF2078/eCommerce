using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;


var builder = WebApplication.CreateBuilder(args);

//el archivo ocelot.json no puede ser opcional, y si se hace algun cambio en el archivo
//ocelot.json, el contenedor se reinicia automaticamente
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

//agregar ocelop a la coleccion de servicios
builder.Services
    .AddOcelot(builder.Configuration)
    .AddPolly();



var app = builder.Build();

//se agrega el middleware de ocelot
//el middleware de ocelot se encarga de enrutar las peticiones a los microservicios
await app.UseOcelot();

app.Run();
