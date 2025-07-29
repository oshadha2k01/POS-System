#!/bin/bash

echo "🚀 Setting up AI Model Environment"
echo "===================================="

# Check if Python is installed
if ! command -v python3 &> /dev/null; then
    echo "❌ Python 3 is not installed"
    echo "Please install Python 3.8+ from https://python.org"
    exit 1
fi

echo "✅ Python is installed"

# Create virtual environment
echo "📦 Creating virtual environment..."
python3 -m venv ai_env

# Activate virtual environment
echo "🔧 Activating virtual environment..."
source ai_env/bin/activate

# Install requirements
echo "📥 Installing dependencies..."
pip install -r requirements.txt

echo ""
echo "✅ Setup completed successfully!"
echo ""
echo "📋 Next steps:"
echo "   1. Run: python forecast_model.py (to train the model)"
echo "   2. Run: python predict.py (to generate predictions)"
echo "   3. Run: python forecast_api.py (to start API server)"
echo ""
echo "💡 To activate environment later: source ai_env/bin/activate"
echo ""
