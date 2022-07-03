using Akka.Actor;
using Akka.Configuration;
using Akka.Persistence;
using Akka.Persistence.Sql.Common.Journal;
using Akka.Persistence.SqlServer;
using Akka.Persistence.SqlServer.Journal;
using Microsoft.Data.SqlClient;
using System.Data.Common;

namespace ClusterShardingApp
{
    public class MyActor : ReceiveActor
    {
        public MyActor()
        {
            Receive<object>(message =>
            {
                Console.WriteLine($"I have received message : {message}");
                // update state
            });
        }
    }

}
