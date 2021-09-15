using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace mtc_spb_relay
{
    public class TerminatorService : IHostedService
    {
        public class TerminatorServiceOptions
        {
            public int TerminateInMs
            {
                get;
                set;
            }
        }
        
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly TerminatorServiceOptions _options;
        
        private ChannelWriter<MTConnect.ClientServiceOutboundChannelFrame> _mtcOutChannelWriter;
        private ChannelWriter<MTConnect.ClientServiceInboundChannelFrame> _mtcInChannelWriter;
        private ChannelWriter<SparkplugB.ClientServiceOutboundChannelFrame> _spbOutChannelWriter;
        private ChannelWriter<SparkplugB.ClientServiceInboundChannelFrame> _spbInChannelWriter;
        
        public TerminatorService(
            IHostApplicationLifetime appLifetime,
            TerminatorServiceOptions options,
            ChannelWriter<MTConnect.ClientServiceOutboundChannelFrame> mtcOutChannelWriter,
            ChannelWriter<MTConnect.ClientServiceInboundChannelFrame> mtcInChannelWriter,
            ChannelWriter<SparkplugB.ClientServiceOutboundChannelFrame> spbOutChannelWriter,
            ChannelWriter<SparkplugB.ClientServiceInboundChannelFrame> spbInChannelWriter)
        {
            _appLifetime = appLifetime;
            _options = options;

            _mtcOutChannelWriter = mtcOutChannelWriter;
            _mtcInChannelWriter = mtcInChannelWriter;
            _spbOutChannelWriter = spbOutChannelWriter;
            _spbInChannelWriter = spbInChannelWriter;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _appLifetime.ApplicationStarted.Register(() =>
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await Task.Delay(_options.TerminateInMs);
                        
                        _mtcOutChannelWriter.Complete();
                        _mtcInChannelWriter.Complete();
                        _spbOutChannelWriter.Complete();
                        _spbInChannelWriter.Complete();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("TerminatorService ERROR");
                        Console.WriteLine(ex);
                    }
                    finally
                    {
                        Console.WriteLine("TerminatorService Stopping");
                        _appLifetime.StopApplication();
                    }
                });
            });

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("TerminatorService Stop");
            
            return Task.CompletedTask;
        }
    }
}