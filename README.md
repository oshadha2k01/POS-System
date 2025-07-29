# Clothing POS System

A comprehensive Point of Sale (POS) system for clothing stores with AI-powered sales forecasting and inventory management capabilities.

![POS System](https://img.shields.io/badge/Status-Active-brightgreen)
![.NET](https://img.shields.io/badge/.NET-9.0-blue)
![React](https://img.shields.io/badge/React-19.1.0-blue)
![MongoDB](https://img.shields.io/badge/MongoDB-Atlas-green)
![Python](https://img.shields.io/badge/Python-3.9+-yellow)

## 📋 Table of Contents

- [Overview](#-overview)
- [Features](#-features)
- [Technology Stack](#-technology-stack)
- [Project Structure](#-project-structure)
- [Prerequisites](#-prerequisites)
- [Installation](#-installation)
- [Configuration](#-configuration)
- [Running the Application](#-running-the-application)
- [API Documentation](#-api-documentation)
- [Database Schema](#-database-schema)
- [AI Model](#-ai-model)
- [Contributing](#-contributing)
- [License](#-license)

## Overview

This POS system is designed specifically for clothing stores, offering a modern, responsive interface for managing products, processing sales, and leveraging AI for business intelligence. The system features a microservices architecture with separate frontend, backend API, and AI model components.

### Key Highlights
- **Real-time Inventory Management** - Track stock levels, low inventory alerts
- **AI-Powered Forecasting** - Predict sales trends and category performance
- **Modern UI/UX** - Clean, responsive design built with React and Tailwind CSS
- **Cloud Database** - MongoDB Atlas for scalable data storage
- **RESTful API** - Well-documented ASP.NET Core Web API
- **Cross-Platform** - Runs on Windows, macOS, and Linux

## Features

### Core POS Features
- **Product Management**
  - Add, edit, delete products
  - Category-based organization
  - Price and inventory tracking
  - Product search and filtering

- **Sales Processing**
  - Quick checkout process
  - Multiple payment methods support
  - Receipt generation
  - Sales history tracking

- **Inventory Management**
  - Real-time stock updates
  - Low stock alerts
  - Inventory reporting
  - Category-wise analytics

### AI-Powered Features
- **Sales Forecasting**
  - Predict future sales trends
  - Category-specific predictions
  - Machine learning model integration
  - Historical data analysis

- **Business Intelligence**
  - Sales performance dashboards
  - Revenue analytics
  - Customer behavior insights
  - Inventory optimization suggestions

### Dashboard & Reporting
- **Real-time Analytics**
  - Daily sales summary
  - Revenue tracking
  - Product performance metrics
  - Interactive charts and graphs

## Technology Stack

### Frontend
- **Framework**: React 19.1.0
- **Build Tool**: Vite
- **Styling**: Tailwind CSS
- **HTTP Client**: Axios
- **Routing**: React Router DOM
- **Charts**: Recharts
- **Language**: JavaScript (ES6+)

### Backend
- **Framework**: ASP.NET Core 9.0
- **Language**: C#
- **Database**: MongoDB Atlas (Cloud)
- **ORM**: MongoDB.Driver
- **API Documentation**: Swagger/OpenAPI
- **Authentication**: JWT (Ready for implementation)

### AI Model
- **Language**: Python 3.9+
- **Framework**: Scikit-learn
- **API**: Flask/FastAPI
- **Data Processing**: Pandas, NumPy
- **Model Type**: Linear Regression, Time Series Analysis

### Database
- **Primary**: MongoDB Atlas (Cloud NoSQL)
- **Collections**: Products, Sales, Users (Future)
- **Features**: BSON, GridFS support, Auto-scaling

### Development Tools
- **IDE**: Visual Studio Code, Visual Studio
- **Version Control**: Git
- **Package Managers**: npm (Frontend), NuGet (Backend), pip (AI)
- **Containerization**: Docker (Ready)


## Prerequisites

Before running this application, make sure you have the following installed:

### Required Software
- **Node.js** (v18.0.0 or higher) - [Download](https://nodejs.org/)
- **.NET 9.0 SDK** - [Download](https://dotnet.microsoft.com/download)
- **Python 3.9+** - [Download](https://python.org/)
- **Git** - [Download](https://git-scm.com/)

### Optional Tools
- **Visual Studio Code** - [Download](https://code.visualstudio.com/)
- **Visual Studio 2022** - [Download](https://visualstudio.microsoft.com/)
- **MongoDB Compass** - [Download](https://www.mongodb.com/products/compass)

### Database Access
- **MongoDB Atlas Account** - [Sign up](https://www.mongodb.com/cloud/atlas)
- Network access configured for your IP address
- Database connection string (provided in configuration)

## Installation

### 1. Clone the Repository
```bash
git clone https://github.com/oshadha2k01/POS-System.git
cd POS-System
```

### 2. Backend Setup (.NET Core API)
```bash
# Navigate to backend directory
cd backend/ClothingPOS.API

# Restore NuGet packages
dotnet restore

# Build the project
dotnet build

# Update database connection string in appsettings.json (see Configuration section)
```

### 3. Frontend Setup (React)
```bash
# Navigate to frontend directory
cd frontend

# Install npm dependencies
npm install

# Install additional packages if needed
npm install axios react-router-dom recharts
```

### 4. AI Model Setup (Python)
```bash
# Navigate to AI model directory
cd ai-model

# Create virtual environment
python -m venv venv

# Activate virtual environment
# Windows:
venv\Scripts\activate
# macOS/Linux:
source venv/bin/activate

# Install Python dependencies
pip install -r requirements.txt

# Run setup script
# Windows:
setup.bat
# macOS/Linux:
chmod +x setup.sh && ./setup.sh
```

## Configuration

### 1. Backend Configuration

Create or update `appsettings.json` in the backend directory:

```json
{
  "ConnectionStrings": {
    "MongoDB": "mongodb+srv://username:password@cluster.mongodb.net/?retryWrites=true&w=majority"
  },
  "DatabaseSettings": {
    "DatabaseName": "ClothingPOSDB",
    "ProductsCollectionName": "Products",
    "SalesCollectionName": "Sales"
  },
  "AIModel": {
    "BaseUrl": "http://localhost:5001",
    "Timeout": 30,
    "RetryAttempts": 3,
    "FallbackEnabled": true
  }
}
```

### 2. Frontend Configuration

Create `.env` file in the frontend directory:

```env
VITE_API_URL=http://localhost:5112/api
VITE_AI_MODEL_URL=http://localhost:5001
```

### 3. AI Model Configuration

Update configuration in `forecast_api.py`:

```python
# API Configuration
API_HOST = "localhost"
API_PORT = 5001
DEBUG_MODE = True

# Model Configuration
MODEL_PATH = "sales_model.pkl"
DATA_PATH = "sales_data.csv"
```

##  Running the Application

### Development Mode

#### 1. Start the Backend API
```bash
cd backend/ClothingPOS.API
dotnet run
# API will be available at http://localhost:5112
```

#### 2. Start the Frontend
```bash
cd frontend
npm run dev
# Frontend will be available at http://localhost:5173
```

#### 3. Start the AI Model (Optional)
```bash
cd ai-model
# Activate virtual environment first
python forecast_api.py
# AI API will be available at http://localhost:5001
```

### Production Mode

#### Frontend Build
```bash
cd frontend
npm run build
npm run preview
```

#### Backend Build
```bash
cd backend/ClothingPOS.API
dotnet publish -c Release -o ./publish
```

## API Documentation

### Base URL
```
http://localhost:5112/api
```

### Main Endpoints

#### Products
- `GET /api/Product` - Get all products
- `GET /api/Product/{id}` - Get product by ID
- `POST /api/Product` - Create new product
- `PUT /api/Product/{id}` - Update product
- `DELETE /api/Product/{id}` - Delete product

#### Sales
- `GET /api/Sales` - Get all sales
- `GET /api/Sales/{id}` - Get sale by ID
- `POST /api/Sales` - Create new sale
- `GET /api/Sales/daily-summary` - Get daily sales summary

#### AI Model (Optional)
- `GET /api/aimodel/health` - Check AI model status
- `GET /api/sales/forecast` - Get sales forecast
- `GET /api/sales/category-forecast` - Get category forecast

## AI Model

### Overview
The AI model component provides sales forecasting capabilities using machine learning algorithms.

### Features
- **Sales Trend Prediction**: Forecast future sales based on historical data
- **Category Analysis**: Predict performance by product category
- **Seasonal Patterns**: Identify seasonal trends and patterns
- **Inventory Optimization**: Suggest optimal stock levels

### Model Details
- **Algorithm**: Linear Regression with Time Series Analysis
- **Training Data**: Historical sales data from CSV files
- **Features**: Date, category, seasonal factors, historical trends
- **Output**: Predicted sales values with confidence intervals

### API Endpoints
```python
# Health Check
GET /health

# Sales Forecast
GET /forecast?days=30&category=optional

# Category Forecast
GET /category-forecast
```

## Testing

### Backend Testing
```bash
cd backend/ClothingPOS.API
dotnet test
```

### Frontend Testing
```bash
cd frontend
npm run test
```

### API Testing
Use the included `ClothingPOS.API.http` file with tools like:
- REST Client (VS Code extension)
- Postman
- Insomnia

## Development

### Code Style
- **Backend**: Follow C# coding conventions
- **Frontend**: Use ESLint and Prettier
- **AI Model**: Follow PEP 8 Python style guide

### Git Workflow
1. Create feature branch: `git checkout -b feature/your-feature`
2. Make changes and commit: `git commit -m "Add your feature"`
3. Push to branch: `git push origin feature/your-feature`
4. Create Pull Request

### Environment Variables
Never commit sensitive data like:
- Database connection strings
- API keys
- Passwords
- Certificates

## Deployment

### Frontend (Vercel/Netlify)
```bash
npm run build
# Deploy dist/ folder
```

### Backend (Azure/AWS)
```bash
dotnet publish -c Release
# Deploy publish/ folder
```

### AI Model (Docker)
```dockerfile
FROM python:3.9-slim
WORKDIR /app
COPY requirements.txt .
RUN pip install -r requirements.txt
COPY . .
EXPOSE 5001
CMD ["python", "forecast_api.py"]
```


