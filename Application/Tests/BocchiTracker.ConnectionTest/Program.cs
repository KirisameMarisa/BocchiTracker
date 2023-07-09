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

namespace BocchiTracker.ProcessLink.Test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();

            // テスト用のポート番号とMediatorのインスタンスを準備
            int port = 12345;
            var mediator = BuildMediator();

            // Connectionインスタンスを生成
            var connection = new Connection(port, mediator);

            // Connectionを非同期で開始
            var connectionTask = connection.StartAsync();

            // Connectionの終了を待機
            await connectionTask;
        }

        private static IMediator BuildMediator()
        {
            var container = new Container();
            // Since Mediator has multiple constructors, consider adding rule to allow that
            // var container = new Container(rules => rules.With(FactoryMethod.ConstructorWithResolvableArguments))

            container.Use<TextWriter>(Console.Out);

            //Pipeline works out of the box here

            container.RegisterMany(new[] { typeof(IMediator).GetAssembly(), typeof(Ping).GetAssembly() }, Registrator.Interfaces);
            //Without the container having FactoryMethod.ConstructorWithResolvableArguments commented above
            //You must select the desired constructor
            container.Register<IMediator, Mediator>(made: Made.Of(() => new Mediator(Arg.Of<IServiceProvider>())));

            var services = new ServiceCollection();

            var adapterContainer = container.WithDependencyInjectionAdapter(services);

            return adapterContainer.GetRequiredService<IMediator>();
        }
    }
}