using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace api.bootcamp.clt.Api.Middlewares
{
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
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Recurso no encontrado. TraceId: {TraceId}", context.TraceIdentifier);

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Recurso no encontrado",
                    Detail = ex.Message,
                    Instance = context.Request.Path
                };

                problem.Extensions["traceId"] = context.TraceIdentifier;

                await WriteProblemDetailsAsync(context, StatusCodes.Status404NotFound, problem);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Solicitud inválida. TraceId: {TraceId}", context.TraceIdentifier);

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Solicitud inválida",
                    Detail = ex.Message,
                    Instance = context.Request.Path
                };

                problem.Extensions["traceId"] = context.TraceIdentifier;

                await WriteProblemDetailsAsync(context, StatusCodes.Status400BadRequest, problem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error no controlado. TraceId: {TraceId}", context.TraceIdentifier);

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Error interno del servidor",
                    Detail = "Ocurrió un error al procesar la solicitud. Intente nuevamente más tarde.",
                    Instance = context.Request.Path
                };

                problem.Extensions["traceId"] = context.TraceIdentifier;

                await WriteProblemDetailsAsync(context, StatusCodes.Status500InternalServerError, problem);
            }
        }

        private static async Task WriteProblemDetailsAsync(HttpContext context, int statusCode, ProblemDetails problem)
        {
            if (context.Response.HasStarted)
                return;

            context.Response.Clear();
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/problem+json";

            var json = JsonSerializer.Serialize(problem);
            await context.Response.WriteAsync(json);
        }
    }
}
