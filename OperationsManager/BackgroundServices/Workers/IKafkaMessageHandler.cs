
using OperationsManager.Models;

namespace OperationsManager.BackgroundServices.Workers
{
    public interface IKafkaMessageHandler
    {
        public BrokerMessage FetchIncomingMessage();
    }
}
