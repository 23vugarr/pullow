from fastapi import APIRouter, Depends, HTTPException
from fastapi.security.api_key import APIKey

from app.core.auth import get_api_key
from app.schemas.model import MlBase
from app.schemas.response import ResponseBase
from model.model import ModelUtily

router = APIRouter()


@router.post("/")
async def predict_result(mlbase: MlBase, api_key: APIKey = Depends(get_api_key)):
    try:
        sq_price = ModelUtily.get_square(price=mlbase.price, city=mlbase.city)
        future_price = ModelUtily.predict_and_adjust(city=mlbase.city, type_=0, months_to_predict=120)
        result_map = {}
        month = 1

        for index, _ in enumerate(future_price):
            if(month > 35):
                result_map[str(month)] = int(future_price[index] * sq_price * 1.03)
            month+=1

        return ResponseBase(data={"result": result_map}, detail=None)
    except Exception as e:
        return HTTPException(status_code=500, detail=str(e))
