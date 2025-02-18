using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using System.Diagnostics;
using Cocona;

class Program
{
    static void Main(string[] args)
    {
        CoconaLiteApp.Run(RunServer);
    }

    static void RunServer(
        [Option('r', Description = "Optional path to WebGL Build root folder. Should contain index.html.")] string? root,
        [Option('p', Description = "Optional web server port")] int? port,
        [Option(Description = "Use HTTP instead of HTTPS")] bool http)
    {
        root = root ?? Directory.GetCurrentDirectory();
        port = port ?? 5001;

        Console.WriteLine("Serving WebGL build from "+ root);

        var host = Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseKestrel()
                          .UseContentRoot(root)
                          .ConfigureKestrel(serverOptions =>
                          {
                              serverOptions.ListenAnyIP(port.Value, listenOptions =>
                              {
                                  if (!http)
                                  {
                                      listenOptions.UseHttps();
                                  }
                              });
                          })
                          .Configure(app =>
                          {
                              app.Use(async (context, next) =>
                              {
                                  string? path = context.Request.Path.Value;

                                  if (path != null)
                                  {
                                      // Check if the request is for a .bz file
                                      if (path.EndsWith(".br", StringComparison.OrdinalIgnoreCase))
                                      {
                                          // Set the Content-Encoding header to indicate Bzip2 compression
                                          context.Response.Headers["Content-Encoding"] = "br";
                                      }
                                      if (path.EndsWith(".wasm.br", StringComparison.OrdinalIgnoreCase))
                                      {
                                          // Set the Content-Encoding header to indicate Bzip2 compression
                                          context.Response.Headers["Content-Type"] = "application/wasm";
                                      }
                                  }

                                  await next();
                              });

                              app.UseDefaultFiles();

                              app.UseStaticFiles(new StaticFileOptions
                              {
                                  FileProvider = new PhysicalFileProvider(root),
                                  ServeUnknownFileTypes = true // To serve files without known MIME types
                              });
                          });
            })
            .Build();

        var scheme = http ? "http" : "https";
        Process.Start(new ProcessStartInfo
        {
            FileName = $"{scheme}://localhost:{port}/index.html",
            UseShellExecute = true
        });

        host.Run();
    }
}