using System.Threading.Channels;

using Microsoft.Extensions.Logging;

using Akka.Actor;
using Akka.Hosting;

namespace XData.Kernel;

public class LocalActor : ReceiveActor
{
    private Channel<double> _channel;

    public LocalActor(ILogger<LocalActor> logger, IRequiredActor<RemoteActor> remoteActor)
    {
        logger.LogInformation("LocalActor created");

        Receive<string>(msg =>
        {
            logger.LogInformation("Received msg {msg} command from {from}", msg, Sender.Path.ToStringWithAddress());

            _channel = Channel.CreateBounded<double>(10);

            Sender.Tell(_channel);

            remoteActor.ActorRef.Tell(5);

            logger.LogInformation("Channel created and RemoteActor triggered");
        });

        Receive<double>(msg =>
        {
            logger.LogInformation("Received double: {msg} from {from}", msg, Sender.Path.ToStringWithAddress());
            _channel.Writer.TryWrite(msg);

            if (msg == 0)
            {
                logger.LogInformation("Completed channel");
                _channel.Writer.Complete();
            }
        });
    }
}
