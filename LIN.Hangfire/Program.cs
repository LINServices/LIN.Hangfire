using Http.Extensions;
using LIN.Access.Auth;
using LIN.Hangfire.Services;
using LIN.Hangfire.Services.Extensions;

// Builder.
var builder = WebApplication.CreateBuilder(args);

// Agregar servicios a la colección.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthenticationService();
builder.Services.AddLINHttp(useSwagger: false);
builder.Services.AddSettingsHangfire(builder.Configuration);
builder.Services.AddSingleton<EmailService, EmailService>();

// App.
var app = builder.Build();

app.UseRouting();
app.UseSettingsHangfire();

// Usar servicios.
app.UseLINHttp();
app.UseRouting();
app.MapControllers();

app.Run();