
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OperationsManager.Configurations;
using OperationsManager.Models;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OperationsManager.BackgroundServices.Workers
{
    public class TKafkaConsumer
    {
        private BackgroundWorker _worker;
        private readonly KafkaConfigSection _kafkaConfig;
        private bool StopFlag;
        private IKafkaConsumer KafkaQueue;
        public TKafkaConsumer(IOptions<KafkaConfigSection> kafkaConfig, KafkaQueue kafkaQueue)
        {
            _kafkaConfig = kafkaConfig.Value;
            StopFlag = false;
            KafkaQueue = kafkaQueue;

            _worker = new BackgroundWorker();
            _worker.DoWork += RunAsync;
        }

        public void Start()
        {
            _worker.RunWorkerAsync();
        }

        private void RunAsync(object sender, DoWorkEventArgs e)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _kafkaConfig.BootstrapServers,
                GroupId = _kafkaConfig.GroupId,
                AutoOffsetReset = AutoOffsetReset.Latest
            };
            using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
            {
                consumer.Subscribe(_kafkaConfig.Topic);

                while (!StopFlag)
                {
                    BrokerMessage newMessage = null;
                    try
                    {
                        var consumed = consumer.Consume(CancellationToken.None).Message.Value;
                        newMessage = JsonConvert.DeserializeObject<BrokerMessage>(consumed);
                    }
                    catch (Exception ex) {
                        Debug.WriteLine(ex);
                    }
                    
                   
                    if (newMessage != null) 
                        KafkaQueue.AddIncomingMessage(newMessage);

                }

                consumer.Close();
            }
           
        }

       
    }
}
