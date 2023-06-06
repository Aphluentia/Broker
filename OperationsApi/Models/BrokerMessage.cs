using OperationsApi.Models.Enums;

namespace OperationsApi.Models
{
    public class BrokerMessage<T>
    {
        public Operation OperationCode { get; set; }
        public T Data { get; set; }
    }
}
