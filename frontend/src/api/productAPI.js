import axios from 'axios';

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5219/api';

// Create axios instance with default config
const apiClient = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
  timeout: 10000,
});

// Add request interceptor for logging
apiClient.interceptors.request.use(
  (config) => {
    console.log(`🚀 API Request: ${config.method?.toUpperCase()} ${config.url}`);
    return config;
  },
  (error) => {
    console.error('❌ API Request Error:', error);
    return Promise.reject(error);
  }
);

// Add response interceptor for error handling
apiClient.interceptors.response.use(
  (response) => {
    console.log(`✅ API Response: ${response.status} ${response.config.url}`);
    return response;
  },
  (error) => {
    console.error('❌ API Response Error:', error.response?.data || error.message);
    return Promise.reject(error);
  }
);

// Product API functions
export const productAPI = {
  // Get all products with optional filters
  getAllProducts: (params = {}) => apiClient.get('/products', { params }),
  
  // Get single product by ID
  getProduct: (id) => apiClient.get(`/products/${id}`),
  
  // Create new product
  createProduct: (productData) => apiClient.post('/products', productData),
  
  // Update existing product
  updateProduct: (id, productData) => apiClient.put(`/products/${id}`, productData),
  
  // Delete product
  deleteProduct: (id) => apiClient.delete(`/products/${id}`),
  
  // Legacy methods for backward compatibility
  getAll: (params = {}) => apiClient.get('/products', { params }),
  getById: (id) => apiClient.get(`/products/${id}`),
  create: (productData) => apiClient.post('/products', productData),
  update: (id, productData) => apiClient.put(`/products/${id}`, productData),
  delete: (id) => apiClient.delete(`/products/${id}`),
  
  // Get product categories
  getCategories: () => apiClient.get('/products/categories'),
  
  // Get low stock products
  getLowStock: (threshold = 10) => apiClient.get('/products/low-stock', { params: { threshold } })
};

// Sales API functions
export const salesAPI = {
  // Get all sales with optional date filters
  getAll: (params = {}) => apiClient.get('/sales', { params }),
  
  // Get single sale by ID
  getById: (id) => apiClient.get(`/sales/${id}`),
  
  // Create new sale
  create: (saleData) => apiClient.post('/sales', saleData),
  
  // Get daily summary
  getDailySummary: (date) => apiClient.get('/sales/daily-summary', { params: { date } }),
  
  // Get sales forecast
  getForecast: () => apiClient.get('/sales/forecast'),
  
  // Get category forecast
  getCategoryForecast: () => apiClient.get('/sales/category-forecast'),
  
  // Get monthly report
  getMonthlyReport: (year, month) => apiClient.get('/sales/monthly-report', { params: { year, month } })
};

// AI Model API functions
export const aiModelAPI = {
  // Check AI model health
  getHealth: () => apiClient.get('/aimodel/health'),
  
  // Get extended forecast
  getExtendedForecast: (months = 12) => apiClient.get('/aimodel/forecast/extended', { params: { months } }),
  
  // Trigger model retrain
  triggerRetrain: () => apiClient.post('/aimodel/retrain'),
  
  // Direct AI API calls (fallback)
  getDirectForecast: (months = 6) => {
    const aiApiClient = axios.create({
      baseURL: 'http://localhost:5001',
      timeout: 10000,
    });
    return aiApiClient.get(`/api/forecast/sales?months=${months}`);
  },
  
  getDirectCategoryForecast: () => {
    const aiApiClient = axios.create({
      baseURL: 'http://localhost:5001',
      timeout: 10000,
    });
    return aiApiClient.get('/api/forecast/categories');
  }
};

// Error handling helper
export const handleApiError = (error) => {
  if (error.response) {
    // Server responded with error status
    const message = error.response.data?.message || error.response.data || 'An error occurred';
    return {
      message,
      status: error.response.status,
      data: error.response.data
    };
  } else if (error.request) {
    // Request made but no response received
    return {
      message: 'Network error - please check your connection',
      status: 0,
      data: null
    };
  } else {
    // Something else happened
    return {
      message: error.message || 'An unexpected error occurred',
      status: -1,
      data: null
    };
  }
};

export default apiClient;
