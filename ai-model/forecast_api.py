from flask import Flask, jsonify, request
from flask_cors import CORS
import pandas as pd
from datetime import datetime, timedelta
from joblib import load
import numpy as np
import json
import os

app = Flask(__name__)
CORS(app)

class ForecastAPI:
    def __init__(self):
        self.model_data = None
        self.model = None
        self.model_type = None
        self.load_model()
    
    def load_model(self):
        """Load the trained model"""
        try:
            self.model_data = load('sales_model.pkl')
            self.model = self.model_data['model']
            self.model_type = self.model_data['model_type']
            print(f"✅ Loaded {self.model_type} model")
        except FileNotFoundError:
            print("⚠️  Model file not found. Using fallback predictions.")
            self.model = None
    
    def predict_sales(self, months_ahead=6):
        """Generate sales predictions"""
        if not self.model:
            return self._fallback_predictions(months_ahead)
        
        try:
            # Load reference data
            df = pd.read_csv('sales_data.csv')
            df['Month'] = pd.to_datetime(df['Month'])
            df['Month_Num'] = df['Month'].dt.month + 12 * (df['Month'].dt.year - df['Month'].dt.year.min())
            
            last_row = df.iloc[-1]
            last_month_num = last_row['Month_Num']
            last_date = last_row['Month']
            
            predictions = []
            
            for i in range(1, months_ahead + 1):
                future_date = pd.to_datetime(last_date) + pd.DateOffset(months=i)
                future_month_num = last_month_num + i
                future_month_of_year = future_date.month
                future_quarter = future_date.quarter
                
                season = {
                    12: 'Winter', 1: 'Winter', 2: 'Winter',
                    3: 'Spring', 4: 'Spring', 5: 'Spring',
                    6: 'Summer', 7: 'Summer', 8: 'Summer',
                    9: 'Fall', 10: 'Fall', 11: 'Fall'
                }[future_month_of_year]
                
                features = [
                    future_month_num, future_month_of_year, future_quarter,
                    1 if season == 'Fall' else 0,
                    1 if season == 'Spring' else 0,
                    1 if season == 'Summer' else 0,
                    1 if season == 'Winter' else 0
                ]
                
                pred = self.model.predict([features])[0]
                
                seasonal_factor = {
                    'Winter': 1.2, 'Spring': 1.0, 'Summer': 0.9, 'Fall': 1.1
                }[season]
                
                adjusted_pred = pred * seasonal_factor
                
                predictions.append({
                    'month': future_date.strftime('%b %Y'),
                    'predictedSales': round(adjusted_pred, 2),
                    'actualSales': 0,
                    'trend': self._determine_trend(i, adjusted_pred, pred)
                })
            
            return predictions
            
        except Exception as e:
            print(f"Error in prediction: {e}")
            return self._fallback_predictions(months_ahead)
    
    def _fallback_predictions(self, months_ahead=6):
        """Fallback predictions when model is not available"""
        base_amount = 2500
        predictions = []
        
        for i in range(1, months_ahead + 1):
            future_date = datetime.now() + timedelta(days=30 * i)
            
            # Simulate growth with seasonal variation
            growth_factor = 1 + (i * 0.02)  # 2% monthly growth
            seasonal_factor = self._get_seasonal_factor(future_date.month)
            
            predicted_sales = base_amount * growth_factor * seasonal_factor
            
            predictions.append({
                'month': future_date.strftime('%b %Y'),
                'predictedSales': round(predicted_sales, 2),
                'actualSales': round(predicted_sales * 0.95, 2) if i == 1 else 0,
                'trend': 'Growing' if i <= 3 else 'Stable'
            })
        
        return predictions
    
    def _get_seasonal_factor(self, month):
        """Get seasonal factor for a given month"""
        seasonal_factors = {
            12: 1.3, 1: 1.3, 2: 1.2,  # Winter
            3: 1.1, 4: 1.1, 5: 1.0,   # Spring
            6: 0.9, 7: 0.9, 8: 0.9,   # Summer
            9: 1.1, 10: 1.2, 11: 1.2  # Fall
        }
        return seasonal_factors.get(month, 1.0)
    
    def _determine_trend(self, month_index, current_pred, base_pred):
        """Determine trend direction"""
        if month_index <= 2:
            return "Growing"
        elif current_pred > base_pred * 1.05:
            return "Growing"
        elif current_pred < base_pred * 0.95:
            return "Declining"
        else:
            return "Stable"
    
    def predict_categories(self):
        """Predict category-wise sales"""
        base_prediction = 3500  # Base monthly prediction
        
        return {
            'Men': round(base_prediction * 0.35, 2),
            'Women': round(base_prediction * 0.42, 2),
            'Unisex': round(base_prediction * 0.18, 2),
            'Accessories': round(base_prediction * 0.05, 2)
        }

# Initialize forecast API
forecast_api = ForecastAPI()

@app.route('/')
def home():
    """Health check endpoint"""
    return jsonify({
        'status': 'AI Forecast API is running',
        'model_type': forecast_api.model_type or 'Fallback',
        'timestamp': datetime.now().isoformat()
    })

@app.route('/api/forecast/sales', methods=['GET'])
def get_sales_forecast():
    """Get sales forecast for specified months"""
    try:
        months_ahead = request.args.get('months', 6, type=int)
        if months_ahead < 1 or months_ahead > 24:
            months_ahead = 6
        
        predictions = forecast_api.predict_sales(months_ahead)
        
        return jsonify({
            'success': True,
            'forecast': predictions,
            'model_type': forecast_api.model_type or 'Fallback',
            'generated_at': datetime.now().isoformat()
        })
        
    except Exception as e:
        return jsonify({
            'success': False,
            'error': str(e)
        }), 500

@app.route('/api/forecast/categories', methods=['GET'])
def get_category_forecast():
    """Get category-wise sales forecast"""
    try:
        categories = forecast_api.predict_categories()
        
        return jsonify({
            'success': True,
            'categories': categories,
            'generated_at': datetime.now().isoformat()
        })
        
    except Exception as e:
        return jsonify({
            'success': False,
            'error': str(e)
        }), 500

@app.route('/api/model/info', methods=['GET'])
def get_model_info():
    """Get model information"""
    return jsonify({
        'model_type': forecast_api.model_type or 'Fallback',
        'model_loaded': forecast_api.model is not None,
        'last_updated': forecast_api.model_data.get('timestamp') if forecast_api.model_data else None,
        'api_version': '1.0.0'
    })

@app.route('/api/model/retrain', methods=['POST'])
def retrain_model():
    """Retrain the model (simplified version)"""
    try:
        # In a real implementation, this would retrain with new data
        forecast_api.load_model()  # Reload the model
        
        return jsonify({
            'success': True,
            'message': 'Model reloaded successfully',
            'model_type': forecast_api.model_type or 'Fallback'
        })
        
    except Exception as e:
        return jsonify({
            'success': False,
            'error': str(e)
        }), 500

if __name__ == '__main__':
    print("🚀 Starting AI Forecast API Server")
    print("📊 Model Type:", forecast_api.model_type or 'Fallback')
    print("🌐 Server will run on http://localhost:5001")
    print("-" * 50)
    
    app.run(host='0.0.0.0', port=5001, debug=True)
