import React, { useState, useEffect } from "react";
import { productAPI } from "../api/productAPI";

const Products = () => {
  const [products, setProducts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [searchTerm, setSearchTerm] = useState("");
  const [selectedCategory, setSelectedCategory] = useState("");
  const [categories, setCategories] = useState([]);

  // Fetch products from database
  const fetchProducts = async () => {
    try {
      setLoading(true);
      setError(null);
      console.log("Fetching products from database...");

      const response = await productAPI.getAllProducts();
      console.log("Products fetched successfully:", response.data);

      setProducts(response.data);

      // Extract unique categories
      const uniqueCategories = [
        ...new Set(response.data.map((product) => product.category)),
      ];
      setCategories(uniqueCategories.filter((cat) => cat)); // Remove empty categories
    } catch (err) {
      console.error("Error fetching products:", err);
      setError(
        err.response?.data?.message || err.message || "Failed to fetch products"
      );
    } finally {
      setLoading(false);
    }
  };

  // Filter products based on search and category
  const filteredProducts = products.filter((product) => {
    const matchesSearch =
      product.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
      product.category.toLowerCase().includes(searchTerm.toLowerCase());
    const matchesCategory =
      selectedCategory === "" || product.category === selectedCategory;
    return matchesSearch && matchesCategory;
  });

  // Format currency
  const formatCurrency = (amount) => {
    return new Intl.NumberFormat("en-US", {
      style: "currency",
      currency: "USD",
    }).format(amount);
  };

  // Load products on component mount
  useEffect(() => {
    fetchProducts();
  }, []);

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-50 via-white to-purple-50">
      <div className="max-w-7xl mx-auto py-4 px-3 sm:px-6 lg:px-8">
        {/* Header */}
        <div className="mb-4">
          <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center space-y-4 sm:space-y-0 bg-white rounded-xl shadow-lg p-6">
            <h2 className="text-3xl font-bold text-gray-800 bg-gradient-to-r from-blue-600 to-purple-600 bg-clip-text text-transparent">
              Products
            </h2>
          </div>
        </div>

        {/* Search and Filter */}
        <div className="grid grid-cols-1 md:grid-cols-12 gap-4 mb-4">
          <div className="md:col-span-6">
            <div className="relative">
              <input
                type="text"
                className="w-full px-4 py-3 pl-12 text-gray-700 bg-white border-2 border-gray-200 rounded-lg focus:border-blue-500 focus:ring focus:ring-blue-200 focus:outline-none transition-all duration-200"
                placeholder="Search products..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
              />
            </div>
          </div>
          <div className="md:col-span-3">
            <select
              className="w-full px-4 py-3 text-gray-700 bg-white border-2 border-gray-200 rounded-lg focus:border-blue-500 focus:ring focus:ring-blue-200 focus:outline-none transition-all duration-200"
              value={selectedCategory}
              onChange={(e) => setSelectedCategory(e.target.value)}
            >
              <option value="">All Categories</option>
              {categories.map((category, index) => (
                <option key={index} value={category}>
                  {category}
                </option>
              ))}
            </select>
          </div>
          <div className="md:col-span-3">
            <div className="flex items-center h-full px-4 py-3 text-gray-600 bg-gray-100 rounded-lg">
              <span className="text-sm font-medium">
                Showing {filteredProducts.length} of {products.length} products
              </span>
            </div>
          </div>
        </div>

        {/* Loading State */}
        {loading && (
          <div className="w-full">
            <div className="flex flex-col items-center justify-center py-16 bg-white rounded-xl shadow-lg">
              <div className="animate-spin rounded-full h-16 w-16 border-4 border-blue-500 border-t-transparent mb-4"></div>
              <p className="text-gray-600 text-lg font-medium">
                Loading products from database...
              </p>
            </div>
          </div>
        )}

        {/* Error State */}
        {error && (
          <div className="w-full">
            <div className="bg-red-50 border-l-4 border-red-500 rounded-lg p-6 shadow-lg">
              <div className="flex flex-col space-y-3">
                <h5 className="text-red-800 font-bold text-lg">
                  Error Loading Products
                </h5>
                <p className="text-red-700">{error}</p>
                <button
                  className="self-start px-4 py-2 bg-red-100 hover:bg-red-200 text-red-800 rounded-lg transition-colors duration-200 font-medium"
                  onClick={fetchProducts}
                >
                  Try Again
                </button>
              </div>
            </div>
          </div>
        )}

        {/* Empty State */}
        {!loading && !error && products.length === 0 && (
          <div className="w-full">
            <div className="flex flex-col items-center justify-center py-16 bg-white rounded-xl shadow-lg">
              <h3 className="text-2xl font-bold text-gray-600 mb-2">
                No Products Found
              </h3>
            </div>
          </div>
        )}

        {/* No Results */}
        {!loading &&
          !error &&
          products.length > 0 &&
          filteredProducts.length === 0 && (
            <div className="w-full">
              <div className="flex flex-col items-center justify-center py-16 bg-white rounded-xl shadow-lg">
                <h4 className="text-xl font-bold text-gray-600 mb-2">
                  No products match your search
                </h4>
                <p className="text-gray-500 mb-6 text-center">
                  Try adjusting your search terms or category filter.
                </p>
                <button
                  className="px-6 py-3 bg-gray-500 hover:bg-gray-600 text-white rounded-lg transition-colors duration-200 font-medium"
                  onClick={() => {
                    setSearchTerm("");
                    setSelectedCategory("");
                  }}
                >
                  Clear Filters
                </button>
              </div>
            </div>
          )}

        {/* Products Grid */}
        {!loading && !error && filteredProducts.length > 0 && (
          <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4">
            {filteredProducts.map((product) => (
              <div key={product.id} className="mb-4">
                <div className="bg-white rounded-xl shadow-lg hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1 h-full flex flex-col">
                  <div className="p-6 flex flex-col flex-grow">
                    {/* Product Name */}
                    <h5 className="text-xl font-bold text-gray-800 mb-3 line-clamp-2">
                      {product.name}
                    </h5>

                    {/* Category Badge */}
                    <div className="mb-4">
                      <span className="inline-block px-3 py-1 text-xs font-semibold text-gray-700 bg-gray-200 rounded-full">
                        {product.category || "Uncategorized"}
                      </span>
                    </div>

                    {/* Price */}
                    <div className="mb-4">
                      <h4 className="text-2xl font-bold text-green-600">
                        {formatCurrency(product.price)}
                      </h4>
                    </div>

                    {/* Action Buttons */}
                    <div className="mt-auto">
                      <div className="flex space-x-2">
                        <button
                          type="button"
                          className="flex-1 px-3 py-2 text-sm font-medium text-blue-600 bg-blue-50 border border-blue-200 rounded-lg hover:bg-blue-100 transition-colors duration-200"
                          onClick={() =>
                            console.log("View product:", product.id)
                          }
                          title="View Details"
                        >
                          View
                        </button>
                        <button
                          type="button"
                          className="flex-1 px-3 py-2 text-sm font-medium text-gray-600 bg-gray-50 border border-gray-200 rounded-lg hover:bg-gray-100 transition-colors duration-200"
                          onClick={() =>
                            console.log("Edit product:", product.id)
                          }
                          title="Edit Product"
                        >
                          Edit
                        </button>
                        <button
                          type="button"
                          className="flex-1 px-3 py-2 text-sm font-medium text-red-600 bg-red-50 border border-red-200 rounded-lg hover:bg-red-100 transition-colors duration-200"
                          onClick={() => {
                            if (window.confirm(`Delete "${product.name}"?`)) {
                              console.log("Delete product:", product.id);
                              // Add delete functionality here
                            }
                          }}
                          title="Delete Product"
                        >
                          Delete
                        </button>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
};

export default Products;
