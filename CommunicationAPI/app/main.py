import time
from ast import literal_eval
from datetime import datetime
from typing import List, Optional

from confluent_kafka.admin import AdminClient
from fastapi import FastAPI, HTTPException, Request

from app.models import ApiLog, CommsObject, HeartBeat, PairRequest
from app.producer import KafkaException, KafkaProducer

app = FastAPI(
    title="CommunicationAPI",
    description="Connects to the Kafka Broker, Allows Operations \
        like Notifications and Pairing",
    version="0.0.1",
)
TOPIC = "kafkabroker"
producer = None
client = None

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
# Environment ##############################################################
def env_setup(conn_type: str):
    global producer, client
    conn_string = ""
    LAN_CONN = "192.168.1.211"
    ETH_CONN = "89.114.83.106"
    DOCKER = "host.docker.internal"
    if conn_type.upper() == "DEFAULT":
        conn_string = "127.0.0.1:8095, 127.0.0.1:8096, 127.0.0.1:8097"
    elif conn_type.upper() == "LAN":
        conn_string = f"{LAN_CONN}:29092,\
            {LAN_CONN}:29093,{LAN_CONN}:29094"
    elif conn_type.upper() == "ETH":
        conn_string = f"{ETH_CONN}:85,{ETH_CONN}:86,{ETH_CONN}:87"
    elif conn_type.upper() == "DOCKER":
        conn_string = f"{DOCKER}:9092,{DOCKER}:9093, {DOCKER}:9094"

    producer = KafkaProducer(conn_string)
    client = AdminClient({"bootstrap.servers": conn_string})
    return conn_string


############################################################################
# Base #####################################################################


@app.get("/setup", tags=["Base"])
async def setup(request: Request, conn_type: str = "DEFAULT"):
    add_log(event="GET: Setup", client=request.client)
    return env_setup(conn_type)


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


@app.delete("/logs", tags=["Base"])
async def dev_only_logs(request: Request):
    add_log(event="DELETE: Logs", client=request.client)
    with open(LOGS_FILE, "w") as f:
        f.write("")
        f.close()


############################################################################
# Pair #####################################################################
@app.post("/pair", tags=["Pair"], status_code=201)
async def broker_pair(obj: PairRequest, request: Request):
    add_log(
        event=f"POST: New Pairing \
            {obj.WebPlatformId} and {obj.ApplicationType}",
        client=request.client,
    )
    try:
        message_value = CommsObject(
            WebPlatformId=obj.WebPlatformId,
            ApplicationType=obj.ApplicationType,
            Action="NEW_PAIRING",
        ).__dict__
        await producer.publish(TOPIC, "Pairing", value=str(message_value))
        return message_value
    except KafkaException as ex:
        return HTTPException(status_code=500, detail=ex.args[0].str())


@app.get("/pair/accept", tags=["Pair"], response_model=PairRequest)
async def accept_pairing(webPlatform: str, appType: str, request: Request):
    add_log(
        event=f"GET: Accept Pairing {webPlatform} and {appType}",
        client=request.client,
    )
    if f"{webPlatform}_{appType}" not in client.list_topics().topics:
        raise HTTPException(status_code=404, detail="Pairing Does Not Exist")

    try:
        await producer.publish(
            TOPIC,
            "Pairing",
            CommsObject(
                WebPlatformId=webPlatform,
                ApplicationType=appType,
                Action="ACCEPT_PAIRING",
            ),
        )
        return PairRequest(
            WebPlatformId=webPlatform,
            ApplicationType=appType,
        )
    except KafkaException as ex:
        return HTTPException(status_code=500, detail=ex.args[0].str())


@app.get(
    "/pair/disconnect",
    tags=["Pair"],
    response_model=PairRequest,
)
async def disconnect_pairing(webPlatform: str, appType: str, request: Request):
    add_log(
        event=f"GET: Disconnect Pairing {webPlatform} and {appType}",
        client=request.client,
    )
    try:
        producer.publish(
            CommsObject(
                WebPlatformId=webPlatform,
                ApplicationType=appType,
                Action="DISCONNECT_PAIRING",
            )
        )
        return PairRequest(WebPlatformId=webPlatform, ApplicationType=appType)
    except KafkaException as ex:
        raise HTTPException(status_code=500, detail=ex.args[0].str())


@app.get(
    "/pair/ping",
    tags=["Pair"],
    response_model=PairRequest,
)
async def ping_pairing(webPlatform: str, appType: str, request: Request):
    add_log(
        event=f"GET: Pinging Pairing {webPlatform} and {appType}",
        client=request.client,
    )
    try:
        producer.publish(
            CommsObject(
                WebPlatformId=webPlatform,
                ApplicationType=appType,
                Action="PING_PAIRING",
            )
        )
        return PairRequest(WebPlatformId=webPlatform, ApplicationType=appType)
    except KafkaException as ex:
        raise HTTPException(status_code=500, detail=ex.args[0].str())


@app.get(
    "/pair/update",
    tags=["Pair"],
    response_model=PairRequest,
)
async def update_pairing(webPlatform: str, appType: str, request: Request):
    add_log(
        event=f"GET: Pinging Pairing {webPlatform} and {appType}",
        client=request.client,
    )
    try:
        producer.publish(
            CommsObject(
                WebPlatformId=webPlatform,
                ApplicationType=appType,
                Action="UPDATE_PAIRING",
            )
        )
        return PairRequest(WebPlatformId=webPlatform, ApplicationType=appType)
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


@app.get("/topics/{topic}", tags=["Topics"])
async def get_topics_by_id(topic: str, request: Request):
    add_log(
        event=f"GET: Broker Topic {topic}",
        client=request.client,
    )
    if f"{topic}" not in client.list_topics().topics:
        raise HTTPException(status_code=404, detail="Topic Does Not Exist")

    try:
        topics = client.list_topics().topics[topic]
        return topics
    except KafkaException as ex:
        raise HTTPException(status_code=500, detail=ex.args[0].str())
    except KeyError:
        raise HTTPException(
            status_code=404,
            detail=f"Topic {topic} does not exist",
        )


@app.delete("/topics/{topic}", tags=["Topics"], status_code=204)
async def broker_pair_delete(topic: str, request: Request):
    add_log(
        event=f"DELETE: Delete Topic {topic}",
        client=request.client,
    )
    if f"{topic}" not in client.list_topics().topics:
        raise HTTPException(status_code=404, detail="Topic Does Not Exist")

    try:
        client.delete_topics([topic])
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

add_log(event="App Startup")
