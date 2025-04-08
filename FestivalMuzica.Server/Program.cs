using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using FestivalMuzica.Common.Models;
using FestivalMuzica.Common.Service;
using FestivalMuzica.Server.Config;
using FestivalMuzica.Server.Server;
using FestivalMuzica.Server.Service;
using FestivalMuzica.Server.Repository;
using FestivalMuzica.Server.Repository.Utils;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using FestivalMuzica.Server.Hubs;
using Microsoft.AspNetCore.Builder;
using System.Threading;

namespace FestivalMuzica.Server
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Build configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Configure services
            var services = new ServiceCollection();

            // Add logging
            services.AddLogging(configure =>
            {
                configure.AddConfiguration(configuration.GetSection("Logging"));
                configure.AddConsole();
                configure.AddDebug();
            });

            // Add configuration
            var serverConfig = new ServerConfig();
            configuration.GetSection("ServerConfig").Bind(serverConfig);
            services.AddSingleton(serverConfig);

            // Add database connection
            var dbProps = new Dictionary<string, string>
            {
                { "ConnectionString", serverConfig.DbConnectionString }
            };
            services.AddSingleton<IDictionary<string, string>>(dbProps);

            // Add repositories
            services.AddSingleton<IEmployeeRepo, EmployeeDatabaseRepo>();
            services.AddSingleton<IClientRepo, ClientDatabaseRepo>();
            services.AddSingleton<IShowRepo, ShowDatabaseRepo>();
            services.AddSingleton<ITicketRepo, TicketDatabaseRepo>();

            // Add SignalR hub context accessor
            services.AddSingleton<HubContextAccessor<FestivalHub>>();
            
            // Add SignalR with detailed logging
            services.AddSignalR(options => {
                options.EnableDetailedErrors = true; // Enable detailed errors for debugging
                options.MaximumReceiveMessageSize = 102400; // 100 KB
            }).AddJsonProtocol(options => {
                options.PayloadSerializerOptions.PropertyNamingPolicy = null; // Preserve property names
            });
            
            // Add services - Fixed to avoid circular dependency
            // Register as implementation type first
            services.AddSingleton<FestivalService>();
            // Then register as service interfaces
            services.AddSingleton<IFestivalService>(sp => sp.GetRequiredService<FestivalService>());
            services.AddSingleton<IService<long>>(sp => sp.GetRequiredService<FestivalService>());
            
            // Create a separate service provider for the TCP server
            var tcpServiceProvider = services.BuildServiceProvider();

            // Get logger and hub context accessor
            var logger = tcpServiceProvider.GetRequiredService<ILogger<Program>>();
            var hubContextAccessor = tcpServiceProvider.GetRequiredService<HubContextAccessor<FestivalHub>>();
            
            logger.LogInformation("Starting Festival Muzica Server...");

            try
            {
                // Create and start the ASP.NET Core host for SignalR
                var webHost = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.UseUrls($"http://localhost:{serverConfig.SignalRPort}");
                        webBuilder.ConfigureServices(services =>
                        {
                            services.AddSignalR();
                            
                            // Add logging for the web host
                            services.AddLogging(configure => 
                            {
                                configure.AddConsole();
                                configure.AddDebug();
                                configure.SetMinimumLevel(LogLevel.Debug); // Set to Debug to see all logs
                            });
                            
                            // Share the same service instances with the web host
                            services.AddSingleton(tcpServiceProvider.GetRequiredService<IEmployeeRepo>());
                            services.AddSingleton(tcpServiceProvider.GetRequiredService<IClientRepo>());
                            services.AddSingleton(tcpServiceProvider.GetRequiredService<IShowRepo>());
                            services.AddSingleton(tcpServiceProvider.GetRequiredService<ITicketRepo>());
                            services.AddSingleton(tcpServiceProvider.GetRequiredService<FestivalService>());
                            services.AddSingleton<IFestivalService>(sp => sp.GetRequiredService<FestivalService>());
                            services.AddSingleton<IService<long>>(sp => sp.GetRequiredService<FestivalService>());
                            services.AddSingleton(hubContextAccessor);
                        });
                        webBuilder.Configure(app =>
                        {
                            app.UseRouting();
                            app.UseEndpoints(endpoints =>
                            {
                                endpoints.MapHub<FestivalHub>("/festivalhub", options => {
                                    options.ApplicationMaxBufferSize = 64 * 1024; // 64 KB
                                    options.TransportMaxBufferSize = 64 * 1024; // 64 KB
                                    Console.WriteLine("SignalR hub route '/festivalhub' configured");
                                });
                            });
                        });
                    })
                    .Build();

                // Start the web host for SignalR
                await webHost.StartAsync();
                logger.LogInformation($"SignalR server started on port {serverConfig.SignalRPort}");
                
                // Get the hub context from the SignalR host and update the accessor
                var hubContext = webHost.Services.GetRequiredService<IHubContext<FestivalHub>>();
                hubContextAccessor.HubContext = hubContext;
                logger.LogInformation("SignalR hub context obtained and set in the accessor");

                // Get service and create TCP server
                var festivalService = tcpServiceProvider.GetRequiredService<IService<long>>();
                var server = new TcpServer(festivalService, serverConfig.Port);

                // Handle shutdown for both servers
                Console.CancelKeyPress += async (sender, e) =>
                {
                    e.Cancel = true;
                    logger.LogInformation("Shutting down servers...");
                    server.Stop();
                    await webHost.StopAsync();
                };

                // Start TCP server
                logger.LogInformation($"TCP server starting on port {serverConfig.Port}");
                await server.StartAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error starting server");
                throw;
            }
        }
    }
}
