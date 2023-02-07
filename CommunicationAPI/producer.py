import asyncio
from typing import Optional

from confluent_kafka import KafkaException, Producer
from confluent_kafka.admin import AdminClient


class KafkaProducer:
    def __init__(
        self, server: str, client_id: Optional[str] = "CommsProducer"
    ) -> None:
        self.bootstrap_servers = server
        self.client_id = client_id
        self._loop = asyncio.get_event_loop()
        self.producer = Producer(
            {
                "bootstrap.servers": self.bootstrap_servers,
                "client.id": self.client_id,
            }
        )

    async def get_topics(self):
        try:
            topics = self.producer.list_topics(timeout=10).topics
            return topics
        except KafkaException as error:
            return f"Failed to fetch topics: {error}"

    async def publish(self, topic: str, key: str, value: str):
        result = self._loop.create_future()

        def ack(err, msg):
            if err:
                self._loop.call_soon_threadsafe(
                    result.set_exception, KafkaException(err)
                )
            else:
                self._loop.call_soon_threadsafe(result.set_result, msg)

        self.producer.produce(
            topic, key=key, value=value.encode("utf-8"), on_delivery=ack
        )
        self.producer.flush()
        return result
