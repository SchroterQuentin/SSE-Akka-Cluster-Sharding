
using System.Text.Json;
using System.Threading;
using System.Threading.Channels;
using System.Collections.Generic;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

using Akka.Hosting;
using Akka.Actor;

using XData.API;
using XData.Kernel;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/", async (HttpContext ctx, IRequiredActor<LocalActor> service, CancellationToken ct) =>
{
    var channel = await service.ActorRef.Ask<Channel<double>>("go", cancellationToken: ct);

    ctx.Response.Headers.TryAdd("Content-Type", "text/event-stream");

    while (!ct.IsCancellationRequested && !channel.Reader.Completion.IsCompleted)
    {
        var item = await channel.Reader.ReadAsync(ct);

        await ctx.Response.WriteAsync($"data: ", cancellationToken: ct);
        await JsonSerializer.SerializeAsync(ctx.Response.Body, item, cancellationToken: ct);
        await ctx.Response.WriteAsync($"\n\n", cancellationToken: ct);
        await ctx.Response.Body.FlushAsync(ct);

        await Task.Delay(500, ct);
    }
});

app.Run();
