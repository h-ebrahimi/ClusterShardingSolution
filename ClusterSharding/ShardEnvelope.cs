using Akka.Actor;
using Akka.Event;
using System.Collections.Immutable;

namespace ClusterShardingApp
{
    public sealed class ShardEnvelope
    {
        public readonly string EntityId;
        public readonly object Message;

        public ShardEnvelope(string entityId, object message)
        {
            EntityId = entityId;
            Message = message;
        }
    }

   
}
