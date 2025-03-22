using Hangfire;
using LIN.Hangfire.Services;
using System.Net;
using System.Text;

namespace LIN.Hangfire.Jobs;

public class DnsJob
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
        foreach (var domain in ServicesUrl!)
        {
            try
            {
                IPHostEntry hostEntry = Dns.GetHostEntry(domain.Split('-')[0].Trim());
                foreach (IPAddress ip in hostEntry.AddressList)
                {
                    Console.WriteLine(ip);
                }
            }
            catch (Exception)
            {
                failures.Add(domain.Split('-')[1].Trim());
            }
        }

        // Enviar el mensaje de correo.
        if (failures.Count != 0)
        {
            var template = FileCache.ReadContent("wwwroot/mail/dns.txt");

            template = template.Replace("##DATE##", $"{DateTime.Now}");
            template = template.Replace("##DESCRIPTION##", "Servicios caídos y NO accesibles por DNS");
            template = template.Replace("##TOTAL##", $"{failures.Count}");

            StringBuilder sb = new();
            foreach (var failure in failures)
            {
                sb.AppendLine($"<li>{failure}</li>");
            }
            template = template.Replace("##SERVICES##", sb.ToString());

            // Enviar correo.
            BackgroundJob.Enqueue<MailSenderJob>("mailing", t => t.Run("giraldojhong4@gmail.com", "DNS Importante", template));
        }
    }

    /// <summary>
    /// Configurar los servicios.
    /// </summary>
    private void ConfigureServices()
    {
        ServicesUrl ??= [.. File.ReadAllLines("wwwroot/dns.txt")];
    }
}