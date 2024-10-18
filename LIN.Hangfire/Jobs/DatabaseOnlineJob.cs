namespace LIN.Hangfire.Jobs;

public class DatabaseOnlineJob
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
        ConfigureServices();
        foreach (var url in ServicesUrl!)
        {
            try
            {
                var client = new HttpClient();
                var response = await client.PostAsync(url, new StringContent(""));
            }
            catch { }
        }
    }


    /// <summary>
    /// Configurar los servicios.
    /// </summary>
    private void ConfigureServices()
    {
        ServicesUrl ??= [.. File.ReadAllLines("wwwroot/rutasDb.txt")];
    }

}