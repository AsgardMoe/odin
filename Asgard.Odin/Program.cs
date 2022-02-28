using System.Net;
using NLog;
using NLog.Web;
using OnceMi.AspNetCore.IdGenerator;
using Yitter.IdGenerator;
using ILogger = NLog.ILogger;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

logger.Info("Initializing Odin of Asgard.");
logger.Info("Powered by AsgardMoe Project and qyl27.");

try
{
    Run(args, logger);
}
catch (Exception ex)
{
    logger.Fatal("Who set up the TNT?");
    logger.Fatal(ex);
    throw;
}

static void Run(string[] args, ILogger logger)
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

    builder.Services.AddIdGenerator(option =>
    {
        // option.AppId = 1;
        // option.GeneratorOptions = new IdGeneratorOptions
        // {
        //     
        // };
    });

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(LogLevel.Trace);
    builder.Host.UseNLog();

    var app = builder.Build();

    app.Lifetime.ApplicationStopped.Register(() =>
    {
        logger.Info("Good bye~");
        LogManager.Shutdown();
    });

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.MapControllers();

    app.Run();
}