from pydantic import BaseModel

class MlBase(BaseModel):
    city: str
    price: int