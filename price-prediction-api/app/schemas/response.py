from pydantic import BaseModel
from typing import Optional

class ResponseBase(BaseModel):
    data: dict
    detail: Optional[str]