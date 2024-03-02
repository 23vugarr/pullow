import tensorflow as tf
import pandas as pd
from sklearn.preprocessing import MinMaxScaler
import numpy as np

def calculate_cagr(start_value, end_value, periods):
    """Calculate the Compound Annual Growth Rate."""
    return (end_value / start_value) ** (1/periods) - 1

def predict_and_adjust(city, type_, months_to_predict, model_path='./model/model.h5', dataset_path='./model/dataset.csv'):
    # Load the model and dataset
    model = tf.keras.models.load_model(model_path, compile=False)
    df = pd.read_csv(dataset_path)
    
    # Filter the dataset
    filtered_df = df[(df['City'] == city) & (df['Type'] == type_)].select_dtypes(include=[np.number])
    
    # Prepare data for prediction
    data = filtered_df.values.reshape(-1, 1)
    scaler = MinMaxScaler(feature_range=(0, 1))
    data_scaled = scaler.fit_transform(data)
    
    # We calculated CAGR based on the last 120 months (10 years)
    if len(data) > 120:
        cagr = calculate_cagr(data[-121], data[-1], 10) 
    else:
        cagr = calculate_cagr(data[0], data[-1], len(data)/12)  

    # Predict future values
    # We predict future values based on past 12 months
    n_input = 12
    n_features = 1
    last_sequence = data_scaled[-n_input:].reshape((1, n_input, n_features))
    predictions = []
    
    for _ in range(months_to_predict):
        predicted_scaled = model.predict(last_sequence)
        predicted = scaler.inverse_transform(predicted_scaled).flatten()[0]
        predictions.append(predicted)
        
        # Prepare the next sequence with the predicted value
        new_value_scaled = scaler.transform([[predicted]])
        last_sequence = np.roll(last_sequence, -1, axis=1)
        last_sequence[0, -1, 0] = new_value_scaled[0]
    
    # Adjust predictions using the calculated CAGR
    monthly_growth_rate = (1 + cagr) ** (1/12) - 1
    adjusted_predictions = [predictions[0]] 
    for i in range(1, len(predictions)):
        adjusted_value = adjusted_predictions[i-1] * (1 + monthly_growth_rate)
        adjusted_predictions.append(adjusted_value)
    
    result = [item.item() if isinstance(item, np.ndarray) else item for item in adjusted_predictions]

    return result

city = 'Bakı şəhəri üzrə'
type_ = 0
months_to_predict = 36
predicted_values = predict_and_adjust(city, type_, months_to_predict)
print(f"Predicted values for the next {months_to_predict} months:", predicted_values)
