from confluent_kafka import Producer

# Define the configuration options for the Kafka producer
conf = {
    "bootstrap.servers": "192.168.1.211:8005, 192.168.1.211:8006, 192.168.1.211:8007",  # The address of one or more Kafka brokers
    "client.id": "python-producer",  # An identifier for the client
}

# Create an instance of the Kafka producer
producer = Producer(conf)

# Define the topic and message to be produced
topic = "KAFKA_BROKER"
value = "Hello, Kafka!"

# Send the message to the Kafka broker
producer.produce(topic, value.encode("utf-8"))

# Wait for any outstanding messages to be sent and delivery report results to be received
producer.flush()
