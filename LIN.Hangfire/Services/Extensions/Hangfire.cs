using Hangfire;
using Hangfire.MySql;

namespace LIN.Hangfire.Services.Extensions;

public static class Hangfire
{

    /// <summary>
    /// Agregar servicios de Hangfire.
    /// </summary>
    /// <param name="services">Servicios.</param>
    /// <param name="manager">Manager.</param>
    public static IServiceCollection AddSettingsHangfire(this IServiceCollection services, IConfigurationManager manager)
    {

        // Add Hangfire services.
        services.AddHangfire(config =>
        {
            config.UseStorage(
               new MySqlStorage(manager.GetConnectionString("hangfire"), new MySqlStorageOptions
               {
                   TablesPrefix = "Hangfire",
                   QueuePollInterval = TimeSpan.FromSeconds(15),
               }));

            config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170);
            config.UseSimpleAssemblyNameTypeSerializer();
            config.UseRecommendedSerializerSettings();
        });

        services.AddHangfireServer(options =>
        {
            options.Queues = ["default", "mailing"];
        });

        // Jobs.
        services.AddSingleton<Jobs.ServicesOnlineJob, Jobs.ServicesOnlineJob>();
        services.AddSingleton<Jobs.SslOnlineJob, Jobs.SslOnlineJob>();
        services.AddSingleton<Jobs.ClientActiveJob, Jobs.ClientActiveJob>();
        services.AddSingleton<Jobs.DatabaseOnlineJob, Jobs.DatabaseOnlineJob>();
        services.AddSingleton<Jobs.MailSenderJob, Jobs.MailSenderJob>();
        services.AddSingleton<Jobs.DnsJob, Jobs.DnsJob>();
        return services;
    }


    /// <summary>
    /// Usar servicios de Hangfire.
    /// </summary>
    /// <param name="app">App.</param>
    public static IApplicationBuilder UseSettingsHangfire(this IApplicationBuilder app)
    {
        // Configuración del tablero.
        app.UseHangfireDashboard(string.Empty, new DashboardOptions
        {
            AsyncAuthorization = [new Authorization.IdentityAuthorization()],
            DarkModeEnabled = true,
            DashboardTitle = "LIN Hangfire"
        });

        // Agregar job recurrente.
        RecurringJob.AddOrUpdate<Jobs.SslOnlineJob>("sslJob", (v) => v.Run(), "0 */12 * * *"); // Cada 12 horas
        RecurringJob.AddOrUpdate<Jobs.ServicesOnlineJob>("servicesJob", (v) => v.Run(false), $"*/{2} * * * *"); // 2 minutos
        RecurringJob.AddOrUpdate<Jobs.ServicesOnlineJob>("servicesJobMail", (v) => v.Run(true), $"0 */12 * * *"); // 12 horas
        RecurringJob.AddOrUpdate<Jobs.DatabaseOnlineJob>("DataServicesJob", (v) => v.Run(), $"*/{10} * * * *"); // 10 minutos
        RecurringJob.AddOrUpdate<Jobs.ClientActiveJob>("ClientActiveJob", (v) => v.Run(), $"*/{20} * * * *"); // 20 minutos
        RecurringJob.AddOrUpdate<Jobs.DnsJob>("DnsJob", (v) => v.Run(), $"*/{20} * * * *"); // 20 minutos

        return app;
    }

}