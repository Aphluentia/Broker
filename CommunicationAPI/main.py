from http.client import NO_CONTENT, OK
import logging
from fastapi import FastAPI, Request
import structlog
import uvicorn
import time

from models import HeartBeat

app = FastAPI()

###################################################################### Logs
LOGS_FILE="logs.txt"
structlog.configure(
    processors=[
        structlog.stdlib.PositionalArgumentsFormatter(),
        structlog.processors.StackInfoRenderer(),
        structlog.processors.format_exc_info,
        structlog.dev.ConsoleRenderer(colors=False),
    ],
    wrapper_class=structlog.stdlib.BoundLogger,
    logger_factory=structlog.stdlib.LoggerFactory(),
    cache_logger_on_first_use=True,
)
logging.basicConfig(
    filename=LOGS_FILE,
    format="%(asctime)-15s %(levelname)-8s %(message)s",
    datefmt="%d-%b-%Y %H:%M:%S",
    level=logging.NOTSET,
)
logger = structlog.get_logger()
############################################################################


@app.get("/heartbeat")
async def heartbeat(request: Request):
    logger.info(event="GET: Heartbeat", client=request.client)
    return HeartBeat(status="Active", time=time.strftime("%d-%m-%Y %H:%M:%S", time.gmtime()))



@app.get("/logs")
async def logs(request: Request):
    logger.info(event="GET: Logs", client=request.client)
    return open(LOGS_FILE, "r").read().split("\n")


if __name__ == "__main__":
    logger.info(event="App Startup")
    uvicorn.run(app, host="0.0.0.0", port=8008)