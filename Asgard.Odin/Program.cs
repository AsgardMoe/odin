using System.Net;
using NLog;
using NLog.Web;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

logger.Info("Initializing Odin of Asgard.");
logger.Info("Powered by AsgardMoe Project and qyl27.");

try
{
    Run(args);
}
catch (Exception ex)
{
    logger.Error(ex, "Who set up the TNT?");
    throw;
}
finally
{
    LogManager.Shutdown();
}

static void Run(string[] args)
{
    var builder = WebApplication.CreateBuilder(args);

    builder.WebHost.UseKestrel(kestrel =>
    {
        // qyl27: Multi-listen-endpoints support.
        foreach (var hostingConfig in builder.Configuration.GetSection("Hosting").GetChildren())
        {
            if (!bool.TryParse(hostingConfig["Enabled"], out var result) || !result)
            {
                continue;
            }

            var host = IPAddress.TryParse(hostingConfig["Host"], out var ip) ? ip : IPAddress.Any;
            var port = int.TryParse(hostingConfig["Port"], out var p) ? p : 35172;
            
            kestrel.Listen(host, port, options =>
            {
                // Todo: qyl27: SSL support need more test. 
                if (string.IsNullOrWhiteSpace(hostingConfig["Cert"]))
                {
                    return;
                }

                // Todo: qyl27: There will be a support of HTTP/3. 
                // options.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;

                options.UseHttps(hostingConfig["Cert"], hostingConfig["Pass"]);
            });
        }
    });

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(LogLevel.Trace);
    builder.Host.UseNLog();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    
    app.MapControllers();

    app.Run();
}
