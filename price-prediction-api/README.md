
# FastAPI Backend for Real Estate Price Prediction

Welcome to the FastAPI backend for real estate price prediction! This backend uses TensorFlow for modeling and prediction, Pandas for data processing, and Sklearn for data scaling. It provides an API endpoint to predict real estate prices based on historical data and adjust predictions using the Compound Annual Growth Rate (CAGR). 

## Features
- Predict future real estate prices for a specified city and property type.
- Adjust predictions using CAGR to account for long-term growth trends.
- Calculate the square meter price of property in a specified city.

## Requirements
- Python 3.8 or later
- FastAPI
- TensorFlow
- Pandas
- Sklearn
- Uvicorn (for running the server)

## Installation

1. **Clone the repository**

   Start by cloning this repository to your local machine using Git.

   ```
   git clone <repository-url>
   ```

2. **Install Poetry**

   This project uses Poetry for dependency management. If you don't have Poetry installed, you can install it by following the instructions on the [Poetry documentation](https://python-poetry.org/docs/#installation).

3. **Install dependencies**

   Navigate to the project directory and use Poetry to install the required dependencies.

   ```
   cd <project-directory>
   poetry install
   ```

4. **Activate the virtual environment**

   Once the dependencies are installed, activate the Poetry virtual environment.

   ```
   poetry shell
   ```

5. **Run the FastAPI server**

   Use Uvicorn to run the FastAPI server. This command will start the server on `localhost` with port `8000`.

   ```
   uvicorn app.main:app --reload
   ```

## Usage

After starting the server, you can interact with the API using HTTP requests. The main endpoint is `POST /` which expects a JSON body with details about the city, property type, and the number of months to predict.

Example request body:

```json
{
  "city": "Yasamal",
  "type": 0,
}
```

## Endpoint Details

- `POST /`: Predict future real estate prices and adjust them using CAGR.
  - **Body Parameters**:
    - `city` (str): The city for which to predict prices.
    - `type` (int): The type of property (e.g., 0 for residential).

