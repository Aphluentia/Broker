import time
from ast import literal_eval
from datetime import datetime
from typing import List, Optional

import uvicorn
from fastapi import FastAPI, HTTPException, Request


from producer import KafkaException, KafkaProducer
from confluent_kafka.admin import AdminClient, NewTopic
from models import ApiLog, HeartBeat, PairResponse

app = FastAPI(
    title="CommunicationAPI",
    description="Connects to the Kafka Broker, Allows Operations \
        like Notifications and Pairing",
    version="0.0.1",
)
producer = KafkaProducer(
    "192.168.1.211:8005, 192.168.1.211:8006, 192.168.1.211:8007"
)
client = AdminClient(
    {
        "bootstrap.servers": "192.168.1.211:8005, \
                                192.168.1.211:8006, \
                                192.168.1.211:8007"
    }
)


# Logs #####################################################################
LOGS_FILE = "logs.txt"


def add_log(event: str, client=None, level: Optional[str] = "INFO"):
    with open(LOGS_FILE, "a+") as f:
        if client:
            client = f"{client.host}:{client.port}"

        f.write(
            str(
                ApiLog(
                    level=level,
                    datetime=datetime.now().isoformat(),
                    event=event,
                    client=client,
                ).dict()
            )
            + "\n"
        )


############################################################################
# Base #####################################################################


@app.get("/heartbeat", tags=["Base"], response_model=HeartBeat)
async def heartbeat(request: Request):
    add_log(event="GET: Heartbeat", client=request.client)
    topics = client.list_topics().topics
    broker_status = "Alive"
    if not topics:
        broker_status = "Dead"
    return HeartBeat(
        api_status="Alive",
        broker_status=broker_status,
        time=time.strftime("%d-%m-%Y %H:%M:%S", time.gmtime()),
    ).__dict__


@app.get("/cluster", tags=["Base"])
async def cluster_details(request: Request):
    add_log(event="GET: Cluster Details", client=request.client)
    try:
        return client.list_topics()
    except KafkaException as ex:
        raise HTTPException(status_code=500, detail=ex.args[0].str())


@app.get("/logs", tags=["Base"], response_model=List[ApiLog])
async def logs(request: Request):
    add_log(event="GET: Logs", client=request.client)
    with open(LOGS_FILE, "r") as f:
        logs = [ApiLog(**(literal_eval(line))) for line in f.readlines()]
        return logs


############################################################################
# Pair #####################################################################
@app.get(
    "/pair/{aphluentiaUserId}/{appType}",
    tags=["Pair"],
    response_model=PairResponse,
)
async def broker_pair(aphluentiaUserId: str, appType: str, request: Request):
    add_log(
        event=f"GET: New Pairing {aphluentiaUserId} and {appType}",
        client=request.client,
    )

    try:
        client.create_topics(
            new_topics=[NewTopic(f"{aphluentiaUserId}_{appType}", 3, 2)]
        )
        await producer.publish(
            f"{aphluentiaUserId}_{appType}",
            "Pairing",
            "NEW",
        )
        return PairResponse(
            Topic=f"{aphluentiaUserId}_{appType}",
            WebPlatform=aphluentiaUserId,
            Application=appType,
            Action="NEW",
        )
    except KafkaException as ex:
        raise HTTPException(status_code=500, detail=ex.args[0].str())


@app.get(
    "/pair/accept/{aphluentiaUserId}/{appType}",
    tags=["Pair"],
    response_model=PairResponse,
)
async def broker_pair_accept(
    aphluentiaUserId: str, appType: str, request: Request
):
    add_log(
        event=f"GET: Accept Pairing {aphluentiaUserId} and {appType}",
        client=request.client,
    )

    try:
        await producer.publish(
            f"{aphluentiaUserId}_{appType}",
            "Pairing",
            "ACCEPT",
        )
        return PairResponse(
            Topic=f"{aphluentiaUserId}_{appType}",
            WebPlatform=aphluentiaUserId,
            Application=appType,
            Action="ACCEPT",
        )
    except KafkaException as ex:
        raise HTTPException(status_code=500, detail=ex.args[0].str())


############################################################################
# Topics ###################################################################


@app.get("/topics", tags=["Topics"])
async def get_topics(request: Request):
    add_log(
        event="GET: Broker Topics",
        client=request.client,
    )

    try:
        topics = client.list_topics().topics
        return topics
    except KafkaException as ex:
        raise HTTPException(status_code=500, detail=ex.args[0].str())


@app.get("/topics/{aphluentiaUserId}/{appType}", tags=["Topics"])
async def get_topics_by_id(
    aphluentiaUserId: str, appType: str, request: Request
):
    add_log(
        event=f"GET: Broker Topic {aphluentiaUserId}_{appType}",
        client=request.client,
    )

    try:
        topics = client.list_topics().topics[f"{aphluentiaUserId}_{appType}"]
        return topics
    except KafkaException as ex:
        raise HTTPException(status_code=500, detail=ex.args[0].str())
    except KeyError:
        raise HTTPException(
            status_code=404,
            detail=f"Topic {aphluentiaUserId}_{appType} does not exist",
        )


@app.delete(
    "/topics/{aphluentiaUserId}/{appType}", tags=["Topics"], status_code=204
)
async def broker_pair_delete(
    aphluentiaUserId: str, appType: str, request: Request
):
    add_log(
        event=f"DELETE: Delete Topic {aphluentiaUserId}_{appType}",
        client=request.client,
    )

    try:
        client.delete_topics([f"{aphluentiaUserId}_{appType}"])
        return {}
    except KafkaException as ex:
        raise HTTPException(status_code=500, detail=ex.args[0].str())


@app.delete("/topics", tags=["Topics"], status_code=204)
async def broker_topic_clear(request: Request):
    add_log(
        event="DELETE: Clear topics",
        client=request.client,
    )
    all_topics = await producer.get_topics()
    try:
        client.delete_topics([k[0] for k in (all_topics).items()])
        return {}
    except KafkaException as ex:
        raise HTTPException(status_code=500, detail=ex.args[0].str())


############################################################################
# Main #####################################################################
if __name__ == "__main__":
    add_log(event="App Startup")
    uvicorn.run(app, host="0.0.0.0", port=8008)
