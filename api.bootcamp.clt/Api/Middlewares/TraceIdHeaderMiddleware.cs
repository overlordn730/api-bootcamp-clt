namespace api.bootcamp.clt.Api.Middlewares
{
    public sealed class TraceIdHeaderMiddleware : IMiddleware
    {
        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            context.Response.OnStarting(() =>
            {
                context.Response.Headers.TryAdd("X-Trace-Id", context.TraceIdentifier);
                return Task.CompletedTask;
            });

            return next(context);
        }
    }
}
