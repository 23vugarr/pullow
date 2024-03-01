from fastapi import APIRouter, Depends
from fastapi.security.api_key import APIKey
from app.core.auth import get_api_key 
from app.schemas.response import ResponseBase

router = APIRouter()


@router.get("/")
def test(api_key: APIKey = Depends(get_api_key)):
    return ResponseBase(data=None, detail=None)
