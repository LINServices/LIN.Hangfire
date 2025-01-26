using Hangfire;
using System.Text;

namespace LIN.Hangfire.Jobs;

public class SslOnlineJob
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

        StringBuilder message = new StringBuilder();

        foreach (var url in ServicesUrl!)
        {
            try
            {
                var client = new HttpClient();
                var response = await client.GetAsync(url);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("SSL"))
                {
                    message.AppendLine($"Se venció el SSL de {url}");
                }
            }
        }

        string final = message.ToString();

        if (!string.IsNullOrWhiteSpace(final))
        {
            BackgroundJob.Enqueue<MailSenderJob>("mailing", t => t.Run("giraldojhong4@gmail.com", "SSL Vencido", final));
        }
    }


    /// <summary>
    /// Configurar los servicios.
    /// </summary>
    private void ConfigureServices()
    {
        ServicesUrl ??= [.. File.ReadAllLines("wwwroot/rutas.txt")];
    }

}