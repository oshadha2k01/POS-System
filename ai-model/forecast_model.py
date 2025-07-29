import pandas as pd
from sklearn.linear_model import LinearRegression
from sklearn.ensemble import RandomForestRegressor
from sklearn.model_selection import train_test_split
from sklearn.metrics import mean_absolute_error, mean_squared_error
from joblib import dump
import numpy as np
from datetime import datetime
import warnings
warnings.filterwarnings('ignore')

class SalesForecastModel:
    def __init__(self):
        self.linear_model = LinearRegression()
        self.rf_model = RandomForestRegressor(n_estimators=100, random_state=42)
        self.best_model = None
        self.model_type = None
        
    def prepare_data(self, csv_file='sales_data.csv'):
        """Load and prepare sales data for training"""
        print("📊 Loading sales data...")
        df = pd.read_csv(csv_file)
        df['Month'] = pd.to_datetime(df['Month'])
        
        # Create features
        df['Month_Num'] = df['Month'].dt.month + 12 * (df['Month'].dt.year - df['Month'].dt.year.min())
        df['Month_of_Year'] = df['Month'].dt.month
        df['Year'] = df['Month'].dt.year
        df['Quarter'] = df['Month'].dt.quarter
        
        # Add seasonal features
        df['Season'] = df['Month_of_Year'].map({
            12: 'Winter', 1: 'Winter', 2: 'Winter',
            3: 'Spring', 4: 'Spring', 5: 'Spring',
            6: 'Summer', 7: 'Summer', 8: 'Summer',
            9: 'Fall', 10: 'Fall', 11: 'Fall'
        })
        
        # One-hot encode season
        season_dummies = pd.get_dummies(df['Season'], prefix='Season')
        df = pd.concat([df, season_dummies], axis=1)
        
        print(f"✅ Data loaded: {len(df)} records from {df['Month'].min().strftime('%Y-%m')} to {df['Month'].max().strftime('%Y-%m')}")
        return df
    
    def train_models(self, df):
        """Train both Linear and Random Forest models"""
        print("🧠 Training forecasting models...")
        
        # Features for training
        feature_cols = ['Month_Num', 'Month_of_Year', 'Quarter'] + [col for col in df.columns if col.startswith('Season_')]
        X = df[feature_cols]
        y = df['Sales']
        
        # Train/Test split
        X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.3, random_state=42)
        
        # Train Linear Regression
        self.linear_model.fit(X_train, y_train)
        linear_pred = self.linear_model.predict(X_test)
        linear_mae = mean_absolute_error(y_test, linear_pred)
        
        # Train Random Forest
        self.rf_model.fit(X_train, y_train)
        rf_pred = self.rf_model.predict(X_test)
        rf_mae = mean_absolute_error(y_test, rf_pred)
        
        # Select best model
        if linear_mae < rf_mae:
            self.best_model = self.linear_model
            self.model_type = 'Linear Regression'
            best_mae = linear_mae
        else:
            self.best_model = self.rf_model
            self.model_type = 'Random Forest'
            best_mae = rf_mae
        
        print(f"📈 Linear Regression MAE: {linear_mae:.2f}")
        print(f"🌲 Random Forest MAE: {rf_mae:.2f}")
        print(f"🏆 Best Model: {self.model_type} (MAE: {best_mae:.2f})")
        
        return X_train, X_test, y_train, y_test
    
    def save_model(self, filename='sales_model.pkl'):
        """Save the trained model"""
        model_data = {
            'model': self.best_model,
            'model_type': self.model_type,
            'timestamp': datetime.now().isoformat()
        }
        dump(model_data, filename)
        print(f"✅ Model saved as {filename}")
    
    def predict_future(self, df, months_ahead=6):
        """Generate future predictions"""
        print(f"🔮 Generating {months_ahead} months forecast...")
        
        # Get last month data
        last_row = df.iloc[-1]
        last_month_num = last_row['Month_Num']
        last_date = last_row['Month']
        
        predictions = []
        feature_cols = ['Month_Num', 'Month_of_Year', 'Quarter'] + [col for col in df.columns if col.startswith('Season_')]
        
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
            
            # Create feature vector
            features = [future_month_num, future_month_of_year, future_quarter]
            for col in [col for col in df.columns if col.startswith('Season_')]:
                season_name = col.replace('Season_', '')
                features.append(1 if season_name == season else 0)
            
            # Make prediction
            pred = self.best_model.predict([features])[0]
            
            predictions.append({
                'month': future_date.strftime('%Y-%m'),
                'date': future_date,
                'predicted_sales': round(pred, 2),
                'trend': 'Growing' if i <= 3 else 'Stable'
            })
            
            print(f"📅 {future_date.strftime('%Y-%m')} — 💰 Predicted Sales: ${pred:,.2f}")
        
        return predictions

def main():
    """Main training function"""
    print("🚀 Starting Sales Forecast Model Training")
    print("=" * 50)
    
    # Initialize model
    forecast_model = SalesForecastModel()
    
    # Load and prepare data
    df = forecast_model.prepare_data()
    
    # Train models
    X_train, X_test, y_train, y_test = forecast_model.train_models(df)
    
    # Save the best model
    forecast_model.save_model()
    
    # Generate sample predictions
    predictions = forecast_model.predict_future(df, months_ahead=6)
    
    print("\n🎯 Sample 6-Month Forecast:")
    print("-" * 40)
    for pred in predictions:
        print(f"{pred['month']}: ${pred['predicted_sales']:,.2f} ({pred['trend']})")
    
    print("\n✅ Model training completed successfully!")
    print("💡 Use predict.py to generate new forecasts")

if __name__ == "__main__":
    main()
