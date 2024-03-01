from fastapi import HTTPException, Header, Depends
from .config import settings

def check_auth(authorization: str = Header(None)):
    if authorization != f"Bearer {settings.api_key}":
        raise HTTPException(status_code=403, detail="Invalid or missing API Key")
    return authorization