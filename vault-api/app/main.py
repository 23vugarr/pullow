from fastapi import FastAPI, Depends
from fastapi.middleware.cors import CORSMiddleware
from .routers import s3_router
from .auth import check_auth

app = FastAPI(dependencies=[Depends(check_auth)])
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],  # Allows all origins
    allow_credentials=True,
    allow_methods=["*"],  # Allows all methods
    allow_headers=["*"],  # Allows all headers
)
app.include_router(s3_router.router)
