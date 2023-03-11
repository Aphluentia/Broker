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


class Message(BaseModel):
    Source: str
    Target: str
    Action: str
    SourceApplicationType: str
    TargetApplicationType : str
    Timestamp: Optional[str] = None
    Message: Optional[str] = None


class PairRequest(BaseModel):
    WebPlatformId: str
    ModuleId: str
    ApplicationType: str
