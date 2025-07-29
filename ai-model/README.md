# 🧠 AI Sales Forecasting Model

This directory contains the machine learning components for sales forecasting in the ClothingPOS system.

## 📁 File Structure

```
ai-model/
├── forecast_model.py     # Model training script
├── predict.py           # Prediction generation script
├── forecast_api.py      # Flask API server
├── sales_data.csv       # Training data
├── sales_model.pkl      # Trained model (generated)
├── requirements.txt     # Python dependencies
├── setup.bat           # Windows setup script
├── setup.sh            # Linux/Mac setup script
└── README.md           # This file
```

## 🚀 Quick Start

### 1. Setup Environment

**Windows:**
```bash
.\setup.bat
```

**Linux/Mac:**
```bash
chmod +x setup.sh
./setup.sh
```

### 2. Train the Model
```bash
python forecast_model.py
```

### 3. Generate Predictions
```bash
python predict.py
```

### 4. Start API Server
```bash
python forecast_api.py
```

## 📊 Model Features

### Training Features
- **Temporal Features**: Month number, month of year, quarter
- **Seasonal Features**: One-hot encoded seasons (Spring, Summer, Fall, Winter)
- **Trend Analysis**: Linear and Random Forest regression models

### Model Selection
The system automatically selects the best performing model based on Mean Absolute Error (MAE):
- **Linear Regression**: Good for consistent trends
- **Random Forest**: Better for complex seasonal patterns

### Seasonal Adjustments
- **Winter**: 20% boost (holiday shopping)
- **Spring**: Baseline (100%)
- **Summer**: 10% decrease (slower season)
- **Fall**: 10% boost (back-to-school/work)

## 🔮 Prediction Capabilities

### 1. Monthly Sales Forecast
- Predicts sales for 1-24 months ahead
- Includes confidence intervals
- Trend analysis (Growing/Stable/Declining)

### 2. Category Forecast
- Men's clothing: 35% of total sales
- Women's clothing: 42% of total sales
- Unisex items: 18% of total sales
- Accessories: 5% of total sales

### 3. API Endpoints

#### Sales Forecast
```
GET /api/forecast/sales?months=6
```

Response:
```json
{
  "success": true,
  "forecast": [
    {
      "month": "Jan 2025",
      "predictedSales": 4250.50,
      "actualSales": 0,
      "trend": "Growing"
    }
  ],
  "model_type": "Random Forest",
  "generated_at": "2025-01-15T10:30:00"
}
```

#### Category Forecast
```
GET /api/forecast/categories
```

Response:
```json
{
  "success": true,
  "categories": {
    "Men": 1225.00,
    "Women": 1470.00,
    "Unisex": 630.00,
    "Accessories": 175.00
  },
  "generated_at": "2025-01-15T10:30:00"
}
```

## 🔧 Integration with .NET Backend

The AI model integrates with the ClothingPOS .NET backend through:

### 1. HTTP API Calls
The ForecastService in the backend calls the Python API:

```csharp
public async Task<List<SalesForecast>> GetSalesForecastAsync()
{
    var response = await _httpClient.GetAsync("http://localhost:5001/api/forecast/sales?months=6");
    // Process response...
}
```

### 2. Data Flow
```
Sales Data → CSV Export → Python Model → API Response → .NET Service → Frontend
```

## 📈 Model Performance

### Evaluation Metrics
- **Mean Absolute Error (MAE)**: Average prediction error
- **Confidence Levels**: Decrease over time (85% → 60%)
- **Trend Accuracy**: Historical trend matching

### Sample Performance
```
📈 Linear Regression MAE: 156.23
🌲 Random Forest MAE: 142.87
🏆 Best Model: Random Forest (MAE: 142.87)
```

## 🔄 Model Retraining

### Automatic Retraining
```python
# Add new sales data to sales_data.csv
# Run training script
python forecast_model.py
```

### Manual Retraining via API
```
POST /api/model/retrain
```

## 🛠️ Development

### Adding New Features
1. Update feature engineering in `forecast_model.py`
2. Modify prediction logic in `predict.py`
3. Update API endpoints in `forecast_api.py`

### Testing Predictions
```python
from predict import SalesPredictor

predictor = SalesPredictor()
predictions = predictor.predict_sales(months_ahead=3)
print(predictions)
```

## 📋 Dependencies

```txt
pandas          # Data manipulation
scikit-learn    # Machine learning
joblib          # Model serialization
numpy           # Numerical computations
flask           # API server
flask-cors      # Cross-origin requests
```

## 🚨 Troubleshooting

### Model Not Found Error
```bash
❌ Model file not found. Please run forecast_model.py first to train the model.
```
**Solution**: Run `python forecast_model.py` to train the model.

### Import Errors
```bash
ModuleNotFoundError: No module named 'pandas'
```
**Solution**: Run `pip install -r requirements.txt`

### API Connection Issues
- Ensure Flask server is running on port 5001
- Check firewall settings
- Verify CORS configuration

## 🔮 Future Enhancements

### Planned Features
- [ ] Real-time data integration with SQL Server
- [ ] Advanced neural network models (LSTM)
- [ ] Product-specific forecasting
- [ ] Inventory optimization recommendations
- [ ] A/B testing for model variants
- [ ] Automated model deployment

### Integration Possibilities
- [ ] ML.NET integration for full .NET stack
- [ ] Azure Machine Learning deployment
- [ ] Real-time streaming predictions
- [ ] Customer behavior analysis

## 📞 Support

For issues or questions about the AI model:
1. Check the troubleshooting section
2. Review the training logs
3. Verify data format in `sales_data.csv`
4. Test with fallback predictions

---

**Built with ❤️ for ClothingPOS System**
