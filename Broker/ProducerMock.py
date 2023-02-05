from confluent_kafka import Producer

def delivery_report(err, msg):
    """ Called once for each message produced to indicate delivery result.
        Triggered by poll() or flush(). """
    if err is not None:
        print('Message delivery failed: {}'.format(err))
    else:
        print('Message delivered to {} [{}]'.format(msg.topic(), msg.partition()))

# Set up the Kafka producer configuration
conf = {
    'bootstrap.servers': '89.114.83.106:85,89.114.83.106:86,89.114.83.106:87', # IP addresses of the remote Kafka brokers
    'client.id': 'python-producer'
}

# Create the Kafka producer instance
producer = Producer(conf)

# Produce a message to the desired topic
producer.produce('topic', key='key', value='value', callback=delivery_report)

# Wait for any outstanding messages to be delivered and delivery report callbacks to be received
producer.flush()
