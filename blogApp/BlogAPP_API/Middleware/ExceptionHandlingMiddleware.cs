
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace BlogAPP_API.Middleware;

public sealed class ExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleAsync(context, ex);
        }
    }

    private async Task HandleAsync(HttpContext context, Exception ex)
    {
        var traceId = context.TraceIdentifier;

        _logger.LogError(ex, "Unhandled exception. TraceId={TraceId}", traceId);

        var (status, title, code, detail) = ex switch
        {

            AppException app => (
                app.StatusCode,
                app.Message,      
                app.Code,
                null
            ),

            _ => (
                StatusCodes.Status500InternalServerError,
                "Ошибка сервера",
                "internal_error",
                "Что-то пошло не так"
            )
        };

        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        problem.Extensions["traceId"] = traceId;
        problem.Extensions["code"] = code;

        // 4) Возвращаем ответ
        context.Response.StatusCode = status;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsync(JsonSerializer.Serialize(problem, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }
}
