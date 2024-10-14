using Http.Extensions;
using LIN.Access.Auth;
using LIN.Hangfire.Services.Extensions;

// Builder.
var builder = WebApplication.CreateBuilder(args);

// Agregar servicios a la colección.
builder.Services.AddAuthenticationService();
builder.Services.AddLINHttp(useSwagger: false);
builder.Services.AddSettingsHangfire(builder.Configuration);

// App.
var app = builder.Build();

// Usar servicios.
app.UseLINHttp();
app.UseSettingsHangfire();
app.UseAuthorization();

app.Run();