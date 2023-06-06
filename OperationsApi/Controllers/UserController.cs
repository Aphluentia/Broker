using Confluent.Kafka;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OperationsApi.Cofigurations;
using OperationsApi.Database.Entities;
using OperationsApi.Models;
using OperationsApi.Models.BrokerMessageDataField.Users;
using System.Reflection;

namespace OperationsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly KafkaConfigSection KafkaConfig;
        private readonly IProducer<Null, string> Producer;
        public UserController(IOptions<KafkaConfigSection> _config)
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
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            var result = await Producer.ProduceAsync(KafkaConfig.Topic, new Message<Null, string>
            {
                Value = JsonConvert.SerializeObject(new BrokerMessage<CreateUser> 
                { 
                        OperationCode = Models.Enums.Operation.CREATE_USER, 
                        Data = new CreateUser
                        {
                            Email = user.Email,
                            WebPlatformId = user.WebPlatformId,
                            Password = user.Password,
                            Modules = user.Modules,
                            ActiveScenarios = user.ActiveScenarios,
                            Name = user.Name,
                            PermissionLevel = user.PermissionLevel,
                        }
                })
            });
            if (result.Status != PersistenceStatus.Persisted)
                return BadRequest();
            return NoContent();
        }

        [HttpPut("{Email}")]
        public async Task<IActionResult> UpdateUser(string Email,[FromBody] User user)
        {
            var result = await Producer.ProduceAsync(KafkaConfig.Topic, new Message<Null, string>
            {
                Value = JsonConvert.SerializeObject(new BrokerMessage<UpdateUser> { OperationCode = Models.Enums.Operation.UPDATE_USER, Data = new UpdateUser {Email = Email, User = user } })
            });
            if (result.Status != PersistenceStatus.Persisted)
                return BadRequest();
            return Ok();
        }
        [HttpDelete("{Email}")]
        public async Task<IActionResult> DeleteUser(string Email)
        {
            var result = await Producer.ProduceAsync(KafkaConfig.Topic, new Message<Null, string>
            {
                Value = JsonConvert.SerializeObject(new BrokerMessage<DeleteUser> { OperationCode = Models.Enums.Operation.DELETE_USER, Data = new DeleteUser { Email = Email } })
            });
            if (result.Status != PersistenceStatus.Persisted)
                return BadRequest();
            return NoContent();
        }

        

        [HttpPost("Connection")]
        public async Task<IActionResult> AddModuleConnection([FromBody] ModuleConnection moduleConnection)
        {
            var result = await Producer.ProduceAsync(KafkaConfig.Topic, new Message<Null, string>
            {
                Value = JsonConvert.SerializeObject(new BrokerMessage<ModuleConnection> { OperationCode = Models.Enums.Operation.CREATE_CONNECTION, Data = moduleConnection })
            });
            if (result.Status != PersistenceStatus.Persisted)
                return BadRequest();
            return NoContent();
        }

        [HttpDelete("Connection/{WebPlatformId}/{ModuleId}")]
        public async Task<IActionResult> DeleteModuleConnection(string WebPlatformId, string ModuleId)
        {
            var result = await Producer.ProduceAsync(KafkaConfig.Topic, new Message<Null, string>
            {
                Value = JsonConvert.SerializeObject(new BrokerMessage<ModuleConnection> { OperationCode = Models.Enums.Operation.DELETE_CONNECTION, Data = new ModuleConnection { ModuleId = ModuleId, WebPlatformId = WebPlatformId } })
            });
            if (result.Status != PersistenceStatus.Persisted)
                return BadRequest();
            return NoContent();
        }
    }
}
