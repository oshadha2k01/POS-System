import React, { useState } from "react";
import {
  BrowserRouter as Router,
  Routes,
  Route,
  NavLink,
} from "react-router-dom";
import Products from "./components/Products";

function App() {
  return (
    <Router>
      <div className="min-h-screen bg-gray-50">
        {/* Header */}
        <header className="bg-white shadow-lg">
          <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
            <div className="flex justify-between items-center h-16">
              {/* Logo */}
              <div className="flex items-center">
                <div className="text-2xl font-bold text-blue-600">
                  POS System
                </div>
              </div>

              {/* Navigation */}
              <nav className="flex space-x-4">
                <NavLink
                  to="/products"
                  className={({ isActive }) =>
                    `px-3 py-2 rounded-md text-sm font-medium transition-colors duration-200 no-underline text-decoration-none ${
                      isActive
                        ? "bg-blue-600 text-white"
                        : "text-gray-700 hover:bg-blue-600 hover:text-white"
                    }`
                  }
                >
                  Products
                </NavLink>
              </nav>
            </div>
          </div>
        </header>

        {/* Main Content */}
        <main>
          <Routes>
            <Route path="/" element={<Products />} />
            <Route path="/products" element={<Products />} />
          </Routes>
        </main>
      </div>
    </Router>
  );
}

export default App;
