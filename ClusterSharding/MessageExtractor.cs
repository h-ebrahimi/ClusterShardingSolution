using Akka.Cluster.Sharding;

namespace ClusterShardingApp
{
    public sealed class MessageExtractor : HashCodeMessageExtractor
    {
        public MessageExtractor(int maxNumberOfShards) : base(maxNumberOfShards) { }
        public override string EntityId(object message)
        {
            switch (message)
            {
                case ShardRegion.StartEntity start: return start.EntityId;
                case ShardEnvelope e: return e.EntityId;
            }

            return string.Empty;
        }
        public override object EntityMessage(object message)
        {
            switch (message)
            {
                case ShardEnvelope e: return e.Message;
                default:
                    return message;
            }
        }
    }

}
