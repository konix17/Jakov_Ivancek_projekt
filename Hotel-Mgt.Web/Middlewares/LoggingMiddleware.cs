using Microsoft.AspNetCore.Http;
using System;
using System.Diagnostics;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HotelMgt.Web.Middlewares;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private static readonly string LogDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "logs");
    private static readonly string LogFilePath = Path.Combine(LogDirectory, "requests.log");

    public LoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        
        // Execute the next middleware in the pipeline
        await _next(context);

        stopwatch.Stop();

        try
        {
            // Ensure directory exists
            if (!Directory.Exists(LogDirectory))
            {
                Directory.CreateDirectory(LogDirectory);
            }

            var request = context.Request;
            var response = context.Response;

            var user = context.User?.Identity?.Name ?? "Anonymous";
            var role = "Guest";
            if (context.User?.IsInRole("Admin") == true) role = "Admin";
            else if (context.User?.Identity?.IsAuthenticated == true) role = "User";

            var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            var logLine = $"[{timestamp}] IP: {ipAddress} | User: {user} ({role}) | Method: {request.Method} | Path: {request.Path}{request.QueryString} | Status: {response.StatusCode} | Time: {stopwatch.ElapsedMilliseconds}ms\n";

            await File.AppendAllTextAsync(LogFilePath, logLine);
        }
        catch (Exception)
        {
            // Fail silently so request process is not disrupted
        }
    }
}
