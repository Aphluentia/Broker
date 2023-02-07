from confluent_kafka import Producer

# Define the configuration options for the Kafka producer
conf = {
    'bootstrap.servers': "89.114.83.106:85, 89.114.83.106:86, 89.114.83.106:87", # The address of one or more Kafka brokers
    'client.id': 'python-producer' # An identifier for the client
}

# Create an instance of the Kafka producer
producer = Producer(conf)

# Define the topic and message to be produced
topic = 'my_topic'
value = 'Hello, Kafka!'

# Send the message to the Kafka broker
producer.produce(topic, value.encode('utf-8'))

# Wait for any outstanding messages to be sent and delivery report results to be received
producer.flush()
