using System;
using Akka.Cluster.Hosting;
using Akka.Cluster.Hosting.SBR;
using Akka.DependencyInjection;
using Akka.Hosting;
using Akka.Remote.Hosting;
using Microsoft.Extensions.DependencyInjection;
using XData.Kernel;

namespace XData.API;

public static class DependencyInjector
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddAkka("system", builder =>
        {
            builder.WithActors((system, registry, resolver) =>
            {
                var localActor = system.ActorOf(resolver.Props<LocalActor>(), "local-actor");
                registry.Register<LocalActor>(localActor);
            });

            builder.WithRemoting(Environment.MachineName, 4053);

            builder.WithClustering(new ClusterOptions
            {
                Roles = [ "sharding" ],
                SeedNodes = [ "akka.tcp://system@lighthouse:4053" ],
                SplitBrainResolver = SplitBrainResolverOption.Default
            });

            builder.WithShardRegion<RemoteActor>("default",
                entityPropsFactory: (system, registry, resolver) => (name) => resolver.Props<RemoteActor>(name),
                new MessageExtractor(),
                new ShardOptions()
                {
                    Role = "sharding"
                }
            );
        });
    }
}