using AphluentiaPlusPlusKafkaApi.ConfigurationSection;
using AphluentiaPlusPlusKafkaApi.Dtos.Entities;
using AphluentiaPlusPlusKafkaApi.Dtos.Enum;
using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Reflection;
using static Confluent.Kafka.ConfigPropertyNames;

namespace AphluentiaPlusPlusKafkaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KafkaController : ControllerBase
    {

        private readonly ILogger<KafkaController> _logger;
        private readonly KafkaConfigSection KafkaConfig;
        private readonly IProducer<Null, string> Producer;

        public KafkaController(ILogger<KafkaController> logger, IOptions<KafkaConfigSection> _config)
        {
            _logger = logger;
            KafkaConfig = _config.Value;
            Producer = new ProducerBuilder<Null, string>(
                 new ProducerConfig
                 {
                     BootstrapServers = KafkaConfig.BootstrapServers,
                 }).Build();
        }

        [HttpGet("Create")]
        public async Task<PersistenceStatus> CreatePairing(string WebPlatformId, ModuleType ModuleType, string ModuleId)
        {
            var result = await Producer.ProduceAsync(KafkaConfig.Topic, new Message<Null, string>
            {
                Value = JsonConvert.SerializeObject(CreateMessage(Dtos.Enum.Action.Create_Connection, WebPlatformId, ModuleType, ModuleId))
            });
            
            return result.Status;
        }
        [HttpGet("Close")]
        public async Task<PersistenceStatus> ClosePairing(string WebPlatformId, ModuleType ModuleType, string ModuleId)
        {
            var result = await Producer.ProduceAsync(KafkaConfig.Topic, new Message<Null, string>
            {
                Value = JsonConvert.SerializeObject(CreateMessage(Dtos.Enum.Action.Close_Connection, WebPlatformId, ModuleType, ModuleId))
            });

            return result.Status;
        }

        [HttpGet("Ping")]
        public async Task<PersistenceStatus> PingPairing(string WebPlatformId, ModuleType ModuleType, string ModuleId)
        {
            var result = await Producer.ProduceAsync(KafkaConfig.Topic, new Message<Null, string>
            {
                Value = JsonConvert.SerializeObject(CreateMessage(Dtos.Enum.Action.Ping_Connection, WebPlatformId, ModuleType, ModuleId))
            });

            return result.Status;
        }
        [HttpGet("Update")]
        public async Task<PersistenceStatus> UpdatePairing(string WebPlatformId, ModuleType ModuleType, string ModuleId, string Section)
        {
            var message = CreateMessage(Dtos.Enum.Action.Update_Section, WebPlatformId, ModuleType, ModuleId);
            message.Section = Section;
            var result = await Producer.ProduceAsync(KafkaConfig.Topic, new Message<Null, string>
            {
                Value = JsonConvert.SerializeObject(message)
            });

            return result.Status;
        }




        private Message CreateMessage(Dtos.Enum.Action action, string WebPlatformId, ModuleType ModuleType, string ModuleId)
        {
            return new Message()
            {
                Action = action,
                SourceId = ModuleId,
                TargetId = WebPlatformId,
                SourceModuleType = ModuleType,
                TargetModuleType = ModuleType.AphluentiaPlusPlus_Web,
                Timestamp = DateTime.UtcNow.ToString()
            };
        }

    }
}