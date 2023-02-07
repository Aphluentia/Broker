import time
from ast import literal_eval
from datetime import datetime
from typing import Optional

import uvicorn
from fastapi import FastAPI, HTTPException, Request

from KafkaProducer import KafkaException, KafkaProducer
from models import ApiLog, HeartBeat

app = FastAPI()
producer = KafkaProducer(
    "192.168.1.211:8005, 192.168.1.211:8006, 192.168.1.211:8007"
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


@app.get("/heartbeat")
async def heartbeat(request: Request):
    add_log(event="GET: Heartbeat", client=request.client)
    return HeartBeat(
        status="Active", time=time.strftime("%d-%m-%Y %H:%M:%S", time.gmtime())
    )


@app.get("/logs")
async def logs(request: Request):
    add_log(event="GET: Logs", client=request.client)
    with open(LOGS_FILE, "r") as f:
        logs = [ApiLog(**(literal_eval(line))) for line in f.readlines()]
        return logs


# Pairing ##################################################################


@app.get("/broker/pair/{aphluentiaUserId}/{appType}")
async def broker_pair(aphluentiaUserId: str, appType: str, request: Request):
    add_log(
        event=f"GET: Pairing User {aphluentiaUserId} and {appType}",
        client=request.client,
    )

    try:
        await producer.publish(
            f"{aphluentiaUserId}_{appType}",
            f"{aphluentiaUserId}:{appType}",
            "Pairing",
        )
        return {"timestamp": time.time()}
    except KafkaException as ex:
        raise HTTPException(status_code=500, detail=ex.args[0].str())


@app.get("/broker/topics")
async def get_topics(request: Request):
    add_log(
        event=f"GET: Broker Topics",
        client=request.client,
    )

    try:
        topics = await producer.get_topics()
        return topics
    except KafkaException as ex:
        raise HTTPException(status_code=500, detail=ex.args[0].str())


############################################################################
if __name__ == "__main__":
    add_log(event="App Startup")
    uvicorn.run(app, host="0.0.0.0", port=8008)
