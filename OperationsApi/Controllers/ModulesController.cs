using Confluent.Kafka;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OperationsApi.Cofigurations;
using OperationsApi.Database.Entities;
using OperationsApi.Models.BrokerMessageDataField.Users;
using OperationsApi.Models;
using OperationsApis.Models.BrokerMessageDataField.Modules;
using OperationsApi.Models.BrokerMessageDataField.Modules;

namespace OperationsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModulesController : ControllerBase
    {
        private readonly KafkaConfigSection KafkaConfig;
        private readonly IProducer<Null, string> Producer;
        public ModulesController(IOptions<KafkaConfigSection> _config)
        {
            KafkaConfig = _config.Value;
            Producer = new ProducerBuilder<Null, string>(
                 new ProducerConfig
                 {
                     BootstrapServers = KafkaConfig.BootstrapServers,
                 })
                .Build();
        }

        [HttpPost]
        public async Task<IActionResult> CreateModule([FromBody] Module module)
        {
            var result = await Producer.ProduceAsync(KafkaConfig.Topic, new Message<Null, string>
            {
                Value = JsonConvert.SerializeObject(new BrokerMessage<CreateModule>
                {
                    OperationCode = Models.Enums.Operation.CREATE_MODULE,
                    Data = new CreateModule
                    {
                        ModuleType = module.ModuleType,
                        Id = module.Id,
                        Data = module.Data
                    }
                })
            });
            if (result.Status != PersistenceStatus.Persisted)
                return BadRequest();
            return NoContent();
        }

        [HttpPut("{ModuleId}")]
        public async Task<IActionResult> UpdateModule(string ModuleId,[FromBody] Module module)
        {
            var result = await Producer.ProduceAsync(KafkaConfig.Topic, new Message<Null, string>
            {
                Value = JsonConvert.SerializeObject(new BrokerMessage<UpdateModule> { OperationCode = Models.Enums.Operation.UPDATE_MODULE, Data = new UpdateModule { ModuleId = ModuleId, Module = module} })
            });
            if (result.Status != PersistenceStatus.Persisted)
                return BadRequest();
            return Ok();
        }

        [HttpDelete("{ModuleId}")]
        public async Task<IActionResult> DeleteUser(string ModuleId)
        {
            var result = await Producer.ProduceAsync(KafkaConfig.Topic, new Message<Null, string>
            {
                Value = JsonConvert.SerializeObject(new BrokerMessage<DeleteModule> { OperationCode = Models.Enums.Operation.DELETE_MODULE, Data = new DeleteModule { ModuleId = ModuleId } })
            });
            if (result.Status != PersistenceStatus.Persisted)
                return BadRequest();
            return NoContent();
        }
    }
}
