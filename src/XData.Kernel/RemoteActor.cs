using Akka.Actor;
using Akka.Cluster.Sharding;
using Akka.Hosting;
using Microsoft.Extensions.Logging;

namespace XData.Kernel;

public class RemoteActor : ReceiveActor
{
    public RemoteActor(string entityId, ILogger<RemoteActor> logger, IRequiredActor<RemoteActor> remoteActor)
    {
        logger.LogInformation("RemoteActor created for entityId: {entityId}", entityId);

        Receive<int>(msg =>
        {
            logger.LogInformation("Received int: {msg} from {from}", msg, Sender.Path.ToStringWithAddress());

            if (msg > 0)
            {
                remoteActor.ActorRef.Forward(msg - 1);
            }

            Sender.Tell((double)msg);
        });
    }
}
