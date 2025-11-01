using Akka.Cluster.Sharding;

namespace XData.Kernel;

public class MessageExtractor : IMessageExtractor
{
    public string EntityId(object message)
    {
        if (message is int id)
        {
            return id.ToString();
        }
        throw new System.NotImplementedException();
    }

    public object EntityMessage(object message)
    {
        if (message is int id)
        {
            return id;
        }
        throw new System.NotImplementedException();
    }

    public string ShardId(object message)
    {
        if (message is int id)
        {
            return (id % 10).ToString();
        }
        throw new System.NotImplementedException();
    }

    public string ShardId(string entityId, object messageHint = null)
    {
        int id = int.Parse(entityId);
        return (id % 10).ToString();
    }
}
