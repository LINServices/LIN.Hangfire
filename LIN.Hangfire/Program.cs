using Hangfire;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddHangfire(config =>
    {
        config.UseSqlServerStorage(builder.Configuration.GetConnectionString("hangfire"), new()
        {
            CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
            SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
            QueuePollInterval = TimeSpan.Zero,
            UseRecommendedIsolationLevel = true,
            DisableGlobalLocks = true,
        });
        config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170);
        config.UseSimpleAssemblyNameTypeSerializer();
        config.UseRecommendedSerializerSettings();
    });

    builder.Services.AddHangfireServer();

    builder.Services.AddSingleton<LIN.Hangfire.Jobs.ServicesOnlineJob, LIN.Hangfire.Jobs.ServicesOnlineJob>();

    var app = builder.Build();

    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseHangfireDashboard("/dash", new DashboardOptions() { });
    app.UseAuthorization();

    app.MapControllers();

    RecurringJob.AddOrUpdate<LIN.Hangfire.Jobs.ServicesOnlineJob>("servicesJob", (v) => v.Run(), Cron.MinuteInterval(2));

    app.Run();
}
catch (Exception ex)
{
    if (!File.Exists("wwwroot/error.txt"))
        File.Create("wwwroot/error.txt");
    File.WriteAllText("wwwroot/error.txt", ex.ToString());
}

