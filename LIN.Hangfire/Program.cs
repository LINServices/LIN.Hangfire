using Http.Extensions;
using LIN.Access.Auth;
using LIN.Hangfire.Services;
using LIN.Hangfire.Services.Extensions;

try
{
    // Builder.
    var builder = WebApplication.CreateBuilder(args);

    // Agregar servicios a la colección.
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddAuthenticationService();
    builder.Services.AddLINHttp(useSwagger: false);
    builder.Services.AddSettingsHangfire(builder.Configuration);
    builder.Services.AddSingleton<EmailService, EmailService>();

    // App.
    var app = builder.Build();

    app.UseRouting();
    // Usar servicios.
    app.UseLINHttp();

    app.MapControllers();
    app.UseSettingsHangfire();

    app.UseHttpsRedirection();

    app.Run();
}
catch (Exception ex)
{
    if (!File.Exists("wwwroot/i.txt"))
    {
        File.Create("wwwroot/i.txt");
    }

    File.WriteAllText("wwwroot/i.txt", ex.Message);
}