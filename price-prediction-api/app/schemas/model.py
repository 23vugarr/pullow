from pydantic import BaseModel

class MlBase(BaseModel):
    city: str
    type: int
    time: str