from pydantic_settings import BaseSettings


class Settings(BaseSettings):
    apiUsername: str
    apiPassword: str

    class Config:
        env_file = ".env"


settings = Settings()
