from confluent_kafka import Producer

conf = {
    'bootstrap.servers': '192.168.1.211:8005, 192.168.1.211:8006, 192.168.1.211:8007',
    'client.id': 'python-producer'
}

producer = Producer(conf)

def delivery_report(err, msg):
    if err is not None:
        print('Message delivery failed: {}'.format(err))
    else:
        print('Message delivered to {} [{}]'.format(msg.topic(), msg.partition()))

producer.produce('my_topic', key='key', value='value', callback=delivery_report)

producer.flush()
