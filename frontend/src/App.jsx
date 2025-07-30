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
        {/* Header removed, NavBar will be used in Products.jsx */}

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
