from fastapi import HTTPException, Header

API_KEY = "your_static_api_key"  # Set your static API key

def check_auth(authorization: str = Header(None)):
    if authorization != f"Bearer {API_KEY}":
        raise HTTPException(status_code=403, detail="Invalid or missing API Key")
