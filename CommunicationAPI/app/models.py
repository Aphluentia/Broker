import time
from typing import Optional

from pydantic import BaseModel


class HeartBeat(BaseModel):
    api_status: str
    broker_status: str
    time: str


class ApiLog(BaseModel):
    datetime: str
    level: str
    event: Optional[str] = None
    client: Optional[str] = None


class CommsObject(BaseModel):
    WebPlatformId: str
    ApplicationType: str
    Action: str
    Timestamp: Optional[float] = time.time()
    Message: Optional[str] = None


class PairRequest(BaseModel):
    WebPlatformId: str
    ApplicationType: str
