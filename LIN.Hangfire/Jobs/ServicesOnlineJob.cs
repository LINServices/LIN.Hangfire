using Hangfire;
using LIN.Hangfire.Services;
using System.Text;

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

        List<string> failures = [];
        ConfigureServices();
        foreach (var url in ServicesUrl!)
        {
            try
            {
                var client = new HttpClient();
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    failures.Add($"{url} - {response.StatusCode}");
                }
            }
            catch (Exception)
            {
            }
        }

        // Enviar el mensaje de correo.
        if (failures.Count != 0)
        {
            var template = FileCache.ReadContent("wwwroot/mail/services.html");

            template = template.Replace("##DATE##", $"{DateTime.Now}");
            template = template.Replace("##DESCRIPTION##", "Servicios caídos");
            template = template.Replace("##TOTAL##", $"{failures.Count}");

            StringBuilder sb = new();
            foreach (var failure in failures)
            {
                sb.AppendLine($"<li>{failure}</li>");
            }
            template = template.Replace("##SERVICES##", sb.ToString());

            // Enviar correo.
            BackgroundJob.Enqueue<MailSenderJob>("mailing", t => t.Run("giraldojhong4@gmail.com", "Servicios abajo", template));
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