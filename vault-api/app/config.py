from pydantic_settings import BaseSettings

class Settings(BaseSettings):
    aws_access_key_id: str
    aws_secret_access_key: str
    aws_region: str
    bucket_name: str
    api_key: str

    class Config:
        env_file = ".env"

settings = Settings()
