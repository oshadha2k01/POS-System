import pandas as pd
from datetime import datetime, timedelta
from joblib import load
import numpy as np
import sys
import json

class SalesPredictor:
    def __init__(self, model_file='sales_model.pkl'):
        """Initialize the predictor with a trained model"""
        try:
            self.model_data = load(model_file)
            self.model = self.model_data['model']
            self.model_type = self.model_data['model_type']
            print(f"✅ Loaded {self.model_type} model")
        except FileNotFoundError:
            print("❌ Model file not found. Please run forecast_model.py first to train the model.")
            sys.exit(1)
    
    def predict_sales(self, months_ahead=6, output_format='console'):
        """Generate sales predictions for specified months ahead"""
        print(f"🔮 Generating {months_ahead} months forecast using {self.model_type}...")
        
        # Load reference data to get the last date
        try:
            df = pd.read_csv('sales_data.csv')
            df['Month'] = pd.to_datetime(df['Month'])
            df['Month_Num'] = df['Month'].dt.month + 12 * (df['Month'].dt.year - df['Month'].dt.year.min())
        except FileNotFoundError:
            print("❌ sales_data.csv not found.")
            return []
        
        # Get last month data
        last_row = df.iloc[-1]
        last_month_num = last_row['Month_Num']
        last_date = last_row['Month']
        
        predictions = []
        
        for i in range(1, months_ahead + 1):
            future_date = pd.to_datetime(last_date) + pd.DateOffset(months=i)
            future_month_num = last_month_num + i
            future_month_of_year = future_date.month
            future_quarter = future_date.quarter
            
            # Create season features
            season = {
                12: 'Winter', 1: 'Winter', 2: 'Winter',
                3: 'Spring', 4: 'Spring', 5: 'Spring',
                6: 'Summer', 7: 'Summer', 8: 'Summer',
                9: 'Fall', 10: 'Fall', 11: 'Fall'
            }[future_month_of_year]
            
            # Create feature vector (must match training features)
            features = [
                future_month_num, 
                future_month_of_year, 
                future_quarter,
                1 if season == 'Fall' else 0,    # Season_Fall
                1 if season == 'Spring' else 0,  # Season_Spring
                1 if season == 'Summer' else 0,  # Season_Summer
                1 if season == 'Winter' else 0   # Season_Winter
            ]
            
            # Make prediction
            try:
                pred = self.model.predict([features])[0]
                
                # Add some randomness and business logic
                seasonal_factor = {
                    'Winter': 1.2,  # Holiday season boost
                    'Spring': 1.0,
                    'Summer': 0.9,  # Slower season
                    'Fall': 1.1     # Back to school/work
                }[season]
                
                adjusted_pred = pred * seasonal_factor
                
                prediction_data = {
                    'month': future_date.strftime('%b %Y'),
                    'date': future_date.strftime('%Y-%m-%d'),
                    'predicted_sales': round(adjusted_pred, 2),
                    'actual_sales': 0,  # Will be filled in with real data
                    'trend': self._determine_trend(i, adjusted_pred, pred),
                    'confidence': self._calculate_confidence(i),
                    'season': season
                }
                
                predictions.append(prediction_data)
                
                if output_format == 'console':
                    confidence_str = f"({prediction_data['confidence']}% confidence)"
                    print(f"📅 {prediction_data['month']} — 💰 ${adjusted_pred:,.0f} {confidence_str} [{prediction_data['trend']}]")
                
            except Exception as e:
                print(f"❌ Error predicting for {future_date.strftime('%Y-%m')}: {e}")
        
        return predictions
    
    def _determine_trend(self, month_index, current_pred, base_pred):
        """Determine if trend is growing, stable, or declining"""
        if month_index <= 2:
            return "Growing"
        elif current_pred > base_pred * 1.05:
            return "Growing"
        elif current_pred < base_pred * 0.95:
            return "Declining"
        else:
            return "Stable"
    
    def _calculate_confidence(self, month_index):
        """Calculate confidence level (decreases with time)"""
        base_confidence = 85
        decay_rate = 5
        return max(60, base_confidence - (decay_rate * (month_index - 1)))
    
    def predict_category_sales(self):
        """Predict category-wise sales (simulated for demo)"""
        base_predictions = self.predict_sales(months_ahead=1, output_format='silent')
        if not base_predictions:
            return {}
        
        total_prediction = base_predictions[0]['predicted_sales']
        
        # Category distribution (based on typical clothing store data)
        category_distribution = {
            'Men': 0.35,
            'Women': 0.42,
            'Unisex': 0.18,
            'Accessories': 0.05
        }
        
        category_forecast = {}
        for category, percentage in category_distribution.items():
            category_forecast[category] = round(total_prediction * percentage, 2)
        
        return category_forecast
    
    def export_to_json(self, filename='forecast_results.json', months_ahead=6):
        """Export predictions to JSON file"""
        predictions = self.predict_sales(months_ahead=months_ahead, output_format='silent')
        category_forecast = self.predict_category_sales()
        
        export_data = {
            'forecast_date': datetime.now().isoformat(),
            'model_type': self.model_type,
            'monthly_predictions': predictions,
            'category_forecast': category_forecast,
            'summary': {
                'total_months': len(predictions),
                'avg_monthly_sales': round(np.mean([p['predicted_sales'] for p in predictions]), 2),
                'total_forecast': round(sum([p['predicted_sales'] for p in predictions]), 2)
            }
        }
        
        with open(filename, 'w') as f:
            json.dump(export_data, f, indent=2)
        
        print(f"📄 Forecast exported to {filename}")
        return export_data

def main():
    """Main prediction function"""
    print("🔮 Sales Forecasting System")
    print("=" * 40)
    
    # Initialize predictor
    predictor = SalesPredictor()
    
    try:
        # Get user input
        months_ahead = input("\nEnter number of months to forecast (default: 6): ")
        months_ahead = int(months_ahead) if months_ahead.strip() else 6
        
        if months_ahead < 1 or months_ahead > 24:
            print("⚠️  Using default 6 months (valid range: 1-24)")
            months_ahead = 6
        
        print(f"\n📈 Sales Forecast for Next {months_ahead} Months:")
        print("-" * 50)
        
        # Generate predictions
        predictions = predictor.predict_sales(months_ahead=months_ahead)
        
        # Show category forecast
        print(f"\n🏷️  Category Forecast (Next Month):")
        print("-" * 30)
        category_forecast = predictor.predict_category_sales()
        for category, amount in category_forecast.items():
            print(f"{category:12}: ${amount:,.2f}")
        
        # Export option
        export_choice = input(f"\nExport to JSON? (y/N): ").lower().strip()
        if export_choice == 'y':
            predictor.export_to_json(months_ahead=months_ahead)
        
        # Summary
        if predictions:
            total_forecast = sum([p['predicted_sales'] for p in predictions])
            avg_monthly = total_forecast / len(predictions)
            print(f"\n📊 Forecast Summary:")
            print(f"   Total {months_ahead}-month forecast: ${total_forecast:,.2f}")
            print(f"   Average monthly sales: ${avg_monthly:,.2f}")
        
        print("\n✅ Forecasting completed!")
        
    except KeyboardInterrupt:
        print("\n\n👋 Forecasting cancelled by user")
    except ValueError:
        print("❌ Invalid input. Please enter a valid number.")
    except Exception as e:
        print(f"❌ An error occurred: {e}")

if __name__ == "__main__":
    main()
