from fastapi import Depends, HTTPException
from fastapi.openapi.docs import get_swagger_ui_html
from fastapi.responses import RedirectResponse
from fastapi.security import HTTPBasic, HTTPBasicCredentials
from starlette.status import HTTP_401_UNAUTHORIZED

from app.core.config import settings
from app.core.main import create_application

app = create_application()

security = HTTPBasic()


@app.get("/docs", include_in_schema=False)
async def get_documentation(credentials: HTTPBasicCredentials = Depends(security)):
    if (
        credentials.username != settings.apiUsername
        or credentials.password != settings.apiPassword
    ):
        raise HTTPException(
            status_code=HTTP_401_UNAUTHORIZED,
            detail="Incorrect email or password",
            headers={"WWW-Authenticate": "Basic"},
        )
    else:
        return get_swagger_ui_html(
            openapi_url="/openapi.json", title="Price Prediction docs"
        )


@app.get("/", status_code=200)
def read_root():
    return RedirectResponse(url="/docs", status_code=307)
