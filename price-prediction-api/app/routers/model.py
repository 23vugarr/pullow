from fastapi import APIRouter, Depends
from fastapi.security.api_key import APIKey

from app.core.auth import get_api_key
from app.schemas.model import MlBase
from app.schemas.response import ResponseBase
from model.model import ModelUtily

router = APIRouter()


@router.post("/")
async def test(mlbase: MlBase, api_key: APIKey = Depends(get_api_key)):
    sq_price = ModelUtily.get_square(price=mlbase.price, city=mlbase.city)
    future_price = ModelUtily.predict_and_adjust(city=mlbase.city, type_=0, months_to_predict=120)
    future_price = list(future_price)
    for index, value in enumerate(future_price):
        future_price[index] *= sq_price * 1.03
        future_price[index] = int(future_price[index])

    return ResponseBase(data={"result": future_price}, detail=None)

