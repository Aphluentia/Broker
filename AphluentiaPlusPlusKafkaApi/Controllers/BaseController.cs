using AphluentiaPlusPlusKafkaApi.ConfigurationSection;
using Confluent.Kafka;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AphluentiaPlusPlusKafkaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        private readonly ILogger<KafkaController> _logger;
        private readonly KafkaConfigSection KafkaConfig;

        public BaseController(ILogger<KafkaController> logger, IOptions<KafkaConfigSection> _config)
        {
            _logger = logger;
            KafkaConfig = _config.Value;
          
        }
        [HttpGet("KafkaConfiguration")]
        public KafkaConfigSection GetKafkaConfiguration()
        {
            return KafkaConfig;
        }
       
    }
}
