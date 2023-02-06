
from pydantic import BaseModel
class HeartBeat(BaseModel):
    status: str
    time: str