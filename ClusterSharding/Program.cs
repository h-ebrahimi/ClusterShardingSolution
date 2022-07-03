using Akka.Actor;
using Akka.Cluster.Hosting;
using Akka.Cluster.Sharding;
using Akka.Hosting;
using Akka.Remote.Hosting;
using Akka.Util;
using Microsoft.Extensions.Hosting;
using Petabridge.Cmd.Cluster;
using Petabridge.Cmd.Host;
using Petabridge.Cmd.Remote;

namespace ClusterShardingApp
{
    public class Program
    {
        static string clusterSystem = "ClusterSystem";
        static int port = 8110;
        static IActorRef region;
        public static void Main(string[] args)
        {
            Console.WriteLine("SeedApp");

            var appBuilder = new HostBuilder();

            appBuilder.ConfigureServices((context, service) =>
            {
                service.AddAkka(clusterSystem, options =>
                {
                    options
                        .WithRemoting("localhost", port)
                        .WithClustering(new ClusterOptions
                        {
                            Roles = new string[] { "Seed" },
                            SeedNodes = new[] {
                                Address.Parse($"akka.tcp://{clusterSystem}@localhost:{port}")
                            }
                        })
                        .WithShardRegion<string>(typeName: "MyActor",
                        entityPropsFactory: ret => Props.Create<MyActor>(),
                        messageExtractor: new MessageExtractor(10), new ShardOptions { StateStoreMode = StateStoreMode.DData })
                        .AddPetabridgeCmd(cmd =>
                        {
                            Console.WriteLine("   PetabridgeCmd Added");
                            cmd.RegisterCommandPalette(new RemoteCommands());
                            cmd.RegisterCommandPalette(ClusterCommands.Instance);
                        })
                        .StartActors((actorSystem, actorRegistery) =>
                        {
                            var userActionsShard = actorRegistery.Get<MyActor>();
                            var sharding = ClusterSharding.Get(actorSystem);
                            region = sharding.ShardRegion("MyActor");
                            ProduceMessages(actorSystem, region);
                            //region.Tell(new ShardEnvelope(entityId: "1", message: "Hi"));
                            //var indexer = actorSystem.ActorOf(Props.Create(() => new Indexer(userActionsShard)), "index");
                            //actorRegistery.TryRegister<Index>(indexer); // register for DI
                        });
                });

            });


            var app = appBuilder.Build();
            app.RunAsync();



            Console.ReadLine();
        }

        private static T PickRandom<T>(T[] items) => items[ThreadLocalRandom.Current.Next(items.Length)];

        private static void ProduceMessages(ActorSystem system, IActorRef shardRegion)
        {
            var customers = new[] { "Yoda", "Obi-Wan", "Darth Vader", "Princess Leia", "Luke Skywalker", "R2D2", "Han Solo", "Chewbacca", "Jabba" };
            var items = new[] { "Yoghurt", "Fruits", "Lightsaber", "Fluffy toy", "Dreamcatcher", "Candies", "Cigars", "Chicken nuggets", "French fries" };

            system.Scheduler.Advanced.ScheduleRepeatedly(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(3), () =>
            {
                var customer = PickRandom(customers);
                var item = PickRandom(items);
                var message = new ShardEnvelope(customer, item);

                shardRegion.Tell(message);
            });
        }
    }
}