namespace LIN.Hangfire.Jobs;

public class ServicesOnlineJob
{

    public List<string>? ServicesUrl { get; set; }

    public async Task Run()
    {
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

    private void ConfigureServices()
    {
        ServicesUrl ??= [.. File.ReadAllLines("wwwroot/rutas.txt")];
    }

}