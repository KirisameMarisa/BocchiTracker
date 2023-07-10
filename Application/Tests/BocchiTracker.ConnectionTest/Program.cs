using DryIoc;
using MediatR;
using MediatR.Pipeline;
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

namespace BocchiTracker.ProcessLink.Test
{
    public class ProcessDataConnectionTest : IProcessData
    {
        public Task Process(IMediator inMediator, int inClientID, Packet inPacket)
        {
            if (inPacket.QueryIdType != QueryID.PlayerPosition)
                return Task.CompletedTask;

            var data = inPacket.QueryIdAsPlayerPosition();
            var status = new Dictionary<string, string>();
            status["X"] = data.X.ToString();
            status["Y"] = data.Y.ToString();
            status["Z"] = data.Z.ToString();
            status["Stage"] = data.Stage;

            System.Console.WriteLine($"Position({status["X"]}, {status["Y"]}, {status["Z"]}), Stage({status["Stage"]})");
            return Task.CompletedTask;
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();

            int port = 12345;
            var mediator = BuildMediator();
            var serviceProcessData = BuildServiceProcessData();

            var connection = new Connection(port, mediator, serviceProcessData);

            var connectionTask = connection.StartAsync();

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

        private static IMediator BuildMediator()
        {
            var container = new Container();
 
            container.Use<TextWriter>(Console.Out);

            container.RegisterMany(new[] { typeof(IMediator).GetAssembly(), typeof(Ping).GetAssembly() }, Registrator.Interfaces);
            container.Register<IMediator, Mediator>(made: Made.Of(() => new Mediator(Arg.Of<IServiceProvider>())));

            var services = new ServiceCollection();

            var adapterContainer = container.WithDependencyInjectionAdapter(services);

            return adapterContainer.GetRequiredService<IMediator>();
        }
    }
}