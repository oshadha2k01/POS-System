import React, { useState } from "react";
import { FiX, FiSave, FiPackage, FiTag, FiDollarSign } from "react-icons/fi";
import { productAPI } from "../api/productAPI";
import Swal from "sweetalert2";
import { motion, AnimatePresence } from "framer-motion";

const ProductForm = ({ isOpen, onClose, onProductAdded }) => {
  const [formData, setFormData] = useState({
    name: "",
    category: "",
    price: "",
  });
  const [isSubmitting, setIsSubmitting] = useState(false);

  // Handle form input changes
  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  // Reset form data
  const resetForm = () => {
    setFormData({
      name: "",
      category: "",
      price: "",
    });
  };

  // Handle form submission
  const handleSubmit = async (e) => {
    e.preventDefault();
    
    // Validate form data
    if (!formData.name.trim() || !formData.category.trim() || !formData.price) {
      Swal.fire({
        title: "Validation Error",
        text: "Please fill in all fields",
        icon: "error",
        confirmButtonColor: "#1e40af",
      });
      return;
    }

    if (isNaN(formData.price) || parseFloat(formData.price) <= 0) {
      Swal.fire({
        title: "Invalid Price",
        text: "Please enter a valid price greater than 0",
        icon: "error",
        confirmButtonColor: "#1e40af",
      });
      return;
    }

    setIsSubmitting(true);

    try {
      // Prepare product data
      const productData = {
        name: formData.name.trim(),
        category: formData.category.trim(),
        price: parseFloat(formData.price),
      };

      // Submit to API
      await productAPI.createProduct(productData);

      // Show success message
      Swal.fire({
        title: "Success!",
        text: "Product added successfully",
        icon: "success",
        confirmButtonColor: "#1e40af",
        timer: 2000,
        timerProgressBar: true,
      });

      // Reset form and close modal
      resetForm();
      onClose();
      
      // Refresh products list
      if (onProductAdded) {
        onProductAdded();
      }

    } catch (error) {
      console.error("Error adding product:", error);
      
      Swal.fire({
        title: "Error!",
        text: error.response?.data?.message || "Failed to add product. Please try again.",
        icon: "error",
        confirmButtonColor: "#1e40af",
      });
    } finally {
      setIsSubmitting(false);
    }
  };

  // Handle modal close
  const handleClose = () => {
    if (!isSubmitting) {
      resetForm();
      onClose();
    }
  };

  // Don't render if modal is not open
  if (!isOpen) return null;

  return (
    <AnimatePresence>
      {isOpen && (
        <motion.div 
          className="fixed inset-0 z-50 overflow-y-auto"
          initial={{ opacity: 0 }}
          animate={{ opacity: 1 }}
          exit={{ opacity: 0 }}
          transition={{ duration: 0.2 }}
        >
          {/* Enhanced Backdrop */}
          <motion.div 
            className="fixed inset-0 bg-black bg-opacity-60 transition-opacity"
            onClick={handleClose}
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            exit={{ opacity: 0 }}
          ></motion.div>

          {/* Modal */}
          <div className="flex min-h-full items-center justify-center p-4">
            <motion.div 
              className="relative bg-white rounded-2xl shadow-2xl w-full max-w-3xl transform transition-all"
              initial={{ scale: 0.9, opacity: 0, y: 20 }}
              animate={{ scale: 1, opacity: 1, y: 0 }}
              exit={{ scale: 0.9, opacity: 0, y: 20 }}
              transition={{ duration: 0.3, ease: "easeOut" }}
            >
              {/* Enhanced Header */}
              <div className="flex items-center justify-between p-6 l">
                <div className="flex-1 flex justify-center">
                  <div>
                    <h3 className="text-2xl font-bold text-gray-800">Add New Product</h3>
                  </div>
                </div>
                <motion.button
                  type="button"
                  onClick={handleClose}
                  disabled={isSubmitting}
                  className="p-2 text-gray-400 hover:text-gray-600 rounded-lg transition-all duration-200 border-none focus:outline-none"
                  title="Close"
                  whileHover={{ scale: 1.1 }}
                  whileTap={{ scale: 0.9 }} 
                >
                  <FiX className="w-6 h-6" />
                </motion.button>
              </div>

              {/* Enhanced Form */}
              <form onSubmit={handleSubmit} className="px-6 pb-6">
                <div className="grid grid-cols-1 gap-5">
                  {/* Product Name */}
                  <motion.div 
                    initial={{ opacity: 0, x: -20 }}
                    animate={{ opacity: 1, x: 0 }}
                    transition={{ delay: 0.1 }}
                  >
                    <label htmlFor="name" className="block text-sm font-semibold text-gray-700 mb-2">
                      <div className="flex items-center space-x-2">
                        <FiPackage className="w-4 h-4 text-blue-600" />
                        <span>Product Name </span>
                      </div>
                    </label>
                    <input
                      type="text"
                      id="name"
                      name="name"
                      value={formData.name}
                      onChange={handleInputChange}
                      placeholder="Enter product name"
                      disabled={isSubmitting}
                      className="w-full sm:w-4/5 md:w-3/4 lg:w-2/3 xl:w-1/2 px-4 py-3 text-gray-700 bg-gray-50 border-2 border-gray-200 rounded-lg focus:border-blue-500 focus:ring-2 focus:ring-blue-100 focus:outline-none focus:bg-white transition-all duration-200 disabled:bg-gray-100 mx-auto"
                     style={{ width: '700px' }}
                    />
                  </motion.div>

                  {/* Category */}
                  <motion.div
                    initial={{ opacity: 0, x: -20 }}
                    animate={{ opacity: 1, x: 0 }}
                    transition={{ delay: 0.2 }}
                  >
                    <label htmlFor="category" className="block text-sm font-semibold text-gray-700 mb-2">
                      <div className="flex items-center space-x-2">
                        <FiTag className="w-4 h-4 text-purple-600" />
                        <span>Category </span>
                      </div>
                    </label>
                    <input
                      type="text"
                      id="category"
                      name="category"
                      value={formData.category}
                      onChange={handleInputChange}
                      placeholder="e.g., Electronics, Clothing"
                      disabled={isSubmitting}
                      className="w-full sm:w-4/5 md:w-3/4 lg:w-2/3 xl:w-1/2 px-4 py-3 text-gray-700 bg-gray-50 border-2 border-gray-200 rounded-lg focus:border-purple-500 focus:ring-2 focus:ring-purple-100 focus:outline-none focus:bg-white transition-all duration-200 disabled:bg-gray-100 mx-auto"
                     style={{ width: '700px' }}
                    />
                  </motion.div>

                  {/* Price */}
                  <motion.div
                    initial={{ opacity: 0, x: 20 }}
                    animate={{ opacity: 1, x: 0 }}
                    transition={{ delay: 0.3 }}
                  >
                    <label htmlFor="price" className="block text-sm font-semibold text-gray-700 mb-2">
                      <div className="flex items-center space-x-2">
                        <FiDollarSign className="w-4 h-4 text-green-600" />
                        <span>Price (USD) *</span>
                      </div>
                    </label>
                    <input
                      type="number"
                      id="price"
                      name="price"
                      value={formData.price}
                      onChange={handleInputChange}
                      placeholder="0.00"
                      step="0.01"
                      min="0"
                      disabled={isSubmitting}
                      className="w-full sm:w-4/5 md:w-3/4 lg:w-2/3 xl:w-1/2 px-4 py-3 text-gray-700 bg-gray-50 border-2 border-gray-200 rounded-lg focus:border-green-500 focus:ring-2 focus:ring-green-100 focus:outline-none focus:bg-white transition-all duration-200 disabled:bg-gray-100 mx-auto"
                     style={{ width: '700px' }}
                    />
                  </motion.div>
                </div>

                {/* Enhanced Form Actions */}
                <motion.div 
                  className="flex space-x-4 mt-6 pt-5 border-t border-gray-200"
                  initial={{ opacity: 0, y: 20 }}
                  animate={{ opacity: 1, y: 0 }}
                  transition={{ delay: 0.4 }}
                >
                  <motion.button
                    type="button"
                    onClick={handleClose}
                    disabled={isSubmitting}
                    className="flex-1 px-6 py-3 text-gray-700 bg-gray-100 border-2 border-gray-200 rounded-lg hover:bg-gray-200 hover:border-gray-300 focus:outline-none focus:ring-2 focus:ring-gray-300 transition-all duration-200 font-semibold disabled:opacity-50"
                    whileHover={{ scale: 1.02 }}
                    whileTap={{ scale: 0.98 }}
                  >
                    Cancel
                  </motion.button>
                  <motion.button
                    type="submit"
                    disabled={isSubmitting}
                    className="flex-1 flex items-center justify-center gap-3 px-6 py-3 text-white bg-gradient-to-r from-blue-600 to-blue-800 border-2 border-blue-800 rounded-lg hover:from-blue-700 hover:to-blue-900 focus:outline-none focus:ring-2 focus:ring-blue-300 transition-all duration-200 font-semibold disabled:opacity-50 shadow-lg"
                    whileHover={{ scale: 1.02 }}
                    whileTap={{ scale: 0.98 }}
                  >
                    {isSubmitting ? (
                      <>
                        <motion.div 
                          className="w-5 h-5 border-2 border-white border-t-transparent rounded-full"
                          animate={{ rotate: 360 }}
                          transition={{ duration: 1, repeat: Infinity, ease: "linear" }}
                        />
                        Adding Product...
                      </>
                    ) : (
                      <>
                        <FiSave className="w-5 h-5" />
                        Add Product
                      </>
                    )}
                  </motion.button>
                </motion.div>
              </form>
            </motion.div>
          </div>
        </motion.div>
      )}
    </AnimatePresence>
  );
};

export default ProductForm;
