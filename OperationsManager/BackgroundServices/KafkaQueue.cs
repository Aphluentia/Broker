using OperationsManager.BackgroundServices.Workers;
using OperationsManager.Models;

namespace OperationsManager.BackgroundServices
{
    public class KafkaQueue : IKafkaConsumer, IKafkaMessageHandler
    {
        private Queue<BrokerMessage> MessagesToProcess { get; set; }

        // Reentrant Lock
        private readonly object _lock = new object();
        public KafkaQueue()
        {
            this.MessagesToProcess = new Queue<BrokerMessage>();
        }

        public bool AddIncomingMessage(BrokerMessage _message)
        {
            lock (_lock)
            {
                try
                {
                    MessagesToProcess.Enqueue(_message);
                    Monitor.PulseAll(_lock);
                }
                catch (Exception e)
                {
                    return false;
                }
            }
            return true;
        }

        public BrokerMessage? FetchIncomingMessage()
        {
            var fetchedMessage = new BrokerMessage();
            lock (_lock)
            {
                try
                {
                    while (MessagesToProcess.Count() == 0)
                    {
                        Monitor.Wait(_lock);
                    }
                    fetchedMessage = MessagesToProcess.Dequeue();
                }
                catch (Exception)
                {
                    return null;
                }

            }
            return fetchedMessage;
        }
    }
}
