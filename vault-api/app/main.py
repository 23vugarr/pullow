from fastapi import FastAPI, Depends
from .routers import s3_router
from .auth import check_auth

app = FastAPI(dependencies=[Depends(check_auth)])

app.include_router(s3_router.router)
