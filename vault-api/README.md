# Vault FastAPI Application Setup

## Environment Variables

Before running the application, you need to set up the following environment variables. These variables are crucial for the application's interaction with AWS services and for API authentication.

Create a `.env` file in the root directory of the project and add the following lines:

```env
AWS_ACCESS_KEY_ID=xxxxxxxxxxxxxxxxxxxx
AWS_SECRET_ACCESS_KEY=xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
AWS_REGION=us-east-1
BUCKET_NAME=pashalife
API_KEY=xxxxxxxxxxxxxxxxxxxxxxxxxxxx
```

Replace `xxxxxxxxxxxxxxxxxxxx` and `xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx` with your actual AWS credentials. The `API_KEY` is used for securing your API endpoints, so make sure it's a strong, unique value.

**Note:** Never commit your `.env` file or share your AWS credentials and API key publicly.


## Running the Application

### Without Docker

1. **Install Dependencies:** Run `pip install -r requirements.txt` to install the required packages.
2. **Run the Application:** Execute `uvicorn app.main:app --reload` to start the FastAPI server.

### With Docker

1. **Build the Docker Image:** 
   - Ensure the `Dockerfile` is in the root of your project directory.
   - Run `docker build -t my_fastapi_app .` to create a Docker image for the application.
   
2. **Run the Docker Container:** 
   - Execute `docker run -d --name my_fastapi_app_container -p 8000:8000 my_fastapi_app` to start a container from the image.

After starting the application, you can access it at `http://localhost:8000`.

## API Endpoints

- **POST /upload_file/**: Uploads a file to AWS S3 and returns the file key.
- **GET /get_file_url/{file_key}**: Retrieves a presigned URL for the specified file key.