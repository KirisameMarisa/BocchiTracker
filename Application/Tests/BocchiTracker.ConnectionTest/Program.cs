using DryIoc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Xml.Linq;
using System;
using System.Threading.Tasks;
using DryIoc.Microsoft.DependencyInjection;
using BocchiTracker.ProcessLink.ProcessData;
using BocchiTracker.ProcessLinkQuery.Queries;
using System.Data.Common;
using Prism.Events;

namespace BocchiTracker.ProcessLink.Test
{
    public class ProcessDataConnectionTest : IProcessData
    {
        public void Process(IEventAggregator inEventAggregator, int inClientID, Packet inPacket)
        {
            if (inPacket.QueryIdType != QueryID.PlayerPosition)
                return;

            var data = inPacket.QueryIdAsPlayerPosition();
            var status = new Dictionary<string, string>();
            status["PlayerPosition.x"] = data.X.ToString();
            status["PlayerPosition.y"] = data.Y.ToString();
            status["PlayerPosition.z"] = data.Z.ToString();
            status["PlayerPosition.stage"] = data.Stage;

            System.Console.WriteLine($"Position({status["PlayerPosition.x"]}, {status["PlayerPosition.y"]}, {status["PlayerPosition.z"]}), Stage({status["PlayerPosition.stage"]})");
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();

            int port = 12345;
            var mediator = new EventAggregator();
            var serviceProcessData = BuildServiceProcessData();

            var connection = new Connection(mediator, serviceProcessData);

            var connectionTask = connection.StartAsync(port);

            await WiatForExit(connection);

            await connectionTask;
        }

        private static Task WiatForExit(Connection ioConnection)
        {
            Console.WriteLine("Press 'q' to stop the connection...");

            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.KeyChar == 'q')
                {
                    ioConnection.Stop();
                    Console.WriteLine("Connection stopped.");
                    break;
                }
            }
            return Task.CompletedTask;
        }

        private static IServiceProcessData BuildServiceProcessData()
        {
            var service = new ServiceProcessData();
            service.Register(QueryID.PlayerPosition, new ProcessDataConnectionTest());
            return service;
        }
    }
}