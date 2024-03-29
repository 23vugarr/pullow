from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware

from app.routers.model import router as ModelRouter


def create_application() -> FastAPI:
    application = FastAPI(
        title="Pullow",
        description="Price Prediction API",
        version="b0.1.0",
        docs_url=None,
    )

    application.add_middleware(
        CORSMiddleware,
        allow_origins=["*"],
        allow_methods=["*"],
        allow_headers=["*"],
        allow_credentials=True,
    )

    application.include_router(
        ModelRouter, prefix="/model", tags=["Model prediction endpoint"]
    )

    return application
