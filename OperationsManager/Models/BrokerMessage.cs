using OperationsManager.Models.Enums;

namespace OperationsManager.Models
{
    public class BrokerMessage
    {
        public Operation OperationCode { get; set; }
        public object Data { get; set; }
    }
}
