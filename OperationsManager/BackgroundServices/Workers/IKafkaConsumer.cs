using OperationsManager.Models;

namespace OperationsManager.BackgroundServices.Workers
{
    public interface IKafkaConsumer
    {
        public bool AddIncomingMessage(BrokerMessage _message);

    }
}
