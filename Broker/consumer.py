from confluent_kafka import Consumer, KafkaError

# Define a callback function to handle incoming messages
def on_message(message):
    if message is None:
        print("Message is None")
    elif message.error() is not None:
        if message.error().code() == KafkaError._PARTITION_EOF:
            print(f"Reached end of partition {message.partition()}")
        else:
            print(f"Error while consuming message: {message.error()}")
    else:
        print(f"Received message: key={message.key()}, value={message.value()}")

# Set up the Kafka Consumer to connect to the broker and consume from a topic
conf = {
    "bootstrap.servers": "192.168.1.211:8005, 192.168.1.211:8006",
    "group.id": "my-group",
    "auto.offset.reset": "earliest"
}

consumer = Consumer(conf)
consumer.subscribe(["my_topic"])

# Continuously poll for incoming messages
while True:
    msg = consumer.poll(1.0)
    if msg is None:
        continue
    if msg.error():
        if msg.error().code() == KafkaError._PARTITION_EOF:
            print(f"Reached end of partition {msg.partition()}")
        else:
            print(f"Error while consuming message: {msg.error()}")
    else:
        on_message(msg)
