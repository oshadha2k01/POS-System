@echo off
echo 🚀 Setting up AI Model Environment
echo ====================================

REM Check if Python is installed
python --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ Python is not installed or not in PATH
    echo Please install Python 3.8+ from https://python.org
    pause
    exit /b 1
)

echo ✅ Python is installed

REM Create virtual environment
echo 📦 Creating virtual environment...
python -m venv ai_env

REM Activate virtual environment
echo 🔧 Activating virtual environment...
call ai_env\Scripts\activate.bat

REM Install requirements
echo 📥 Installing dependencies...
pip install -r requirements.txt

echo.
echo ✅ Setup completed successfully!
echo.
echo 📋 Next steps:
echo    1. Run: python forecast_model.py (to train the model)
echo    2. Run: python predict.py (to generate predictions)
echo    3. Run: python forecast_api.py (to start API server)
echo.
echo 💡 To activate environment later: ai_env\Scripts\activate.bat
echo.
pause
