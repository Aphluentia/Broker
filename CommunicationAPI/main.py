import time
from ast import literal_eval
from datetime import datetime
from typing import Optional

import uvicorn
from fastapi import FastAPI, Request

from models import ApiLog, HeartBeat

app = FastAPI()

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


if __name__ == "__main__":
    add_log(event="App Startup")
    uvicorn.run(app, host="0.0.0.0", port=8008)
