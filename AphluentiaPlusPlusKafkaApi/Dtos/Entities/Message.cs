using AphluentiaPlusPlusKafkaApi.Dtos.Enum;

namespace AphluentiaPlusPlusKafkaApi.Dtos.Entities
{
    public class Message
    {

        public Enum.Action Action { get; set; }
        public string SourceId { get; set; }
        public string TargetId { get; set; }
        public ModuleType SourceModuleType { get; set; }
        public ModuleType TargetModuleType { get; set; }
        public string? Section { get; set; }
        public string Timestamp { get; set; }
    }
}
