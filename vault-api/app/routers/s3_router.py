from fastapi import APIRouter, UploadFile, File, HTTPException, Depends
from ..auth import check_auth
from ..config import settings
import boto3
from botocore.exceptions import NoCredentialsError
import uuid

router = APIRouter()

# AWS S3 client initialization
s3_client = boto3.client(
    's3',
    region_name=settings.aws_region,
    aws_access_key_id=settings.aws_access_key_id,
    aws_secret_access_key=settings.aws_secret_access_key
)

@router.post("/upload_file/")
async def upload_file(file: UploadFile = File(...)):
    file_name = f"{uuid.uuid4()}-{file.filename}"

    try:
        s3_client.upload_fileobj(file.file, settings.bucket_name, file_name)
        return {"message": "File uploaded successfully", "file_key": file_name}
    except NoCredentialsError:
        raise HTTPException(status_code=500, detail="Credentials not available")

@router.get("/get_file_url/{file_key}")
def get_file_url(file_key: str):
    try:
        presigned_url = s3_client.generate_presigned_url(
            'get_object',
            Params={'Bucket': settings.bucket_name, 'Key': file_key},
            ExpiresIn=3600
        )
        return {"presigned_url": presigned_url}
    except NoCredentialsError:
        raise HTTPException(status_code=500, detail="Credentials not available")
