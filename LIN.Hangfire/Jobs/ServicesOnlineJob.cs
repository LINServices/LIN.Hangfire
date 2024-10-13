namespace LIN.Hangfire.Jobs;

public class ServicesOnlineJob
{

    /// <summary>
    /// Lista de rutas a las que se les hará ping
    /// </summary>
    public List<string>? ServicesUrl { get; set; }


    /// <summary>
    /// Ejecutar el job.
    /// </summary>
    public async Task Run()
    {
        Log();
        ConfigureServices();
        foreach (var url in ServicesUrl!)
        {
            try
            {
                var client = new HttpClient();
                var response = await client.GetAsync(url);
            }
            catch { }
        }
    }


    /// <summary>
    /// Configurar los servicios.
    /// </summary>
    private void ConfigureServices()
    {
        ServicesUrl ??= [.. File.ReadAllLines("wwwroot/rutas.txt")];
    }

    private void Log()
    {
        try
        {
            if (!File.Exists("wwwroot/log.txt"))
                File.Create("wwwroot/log.txt");

            File.AppendAllText("wwwroot/log.txt", $"{DateTime.Now} - ServicesOnlineJob - ConfigureServices\n");

        }
        catch { }

    }

}