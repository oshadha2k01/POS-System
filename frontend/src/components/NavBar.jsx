import React from "react";
import { NavLink } from "react-router-dom";

const NavBar = () => (
  <header className="bg-blue-900 shadow-lg fixed top-0 left-0 w-full z-50">
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <div className="flex items-center justify-between h-16">
        {/* Logo left-aligned */}
        <div className="text-2xl font-bold text-white ml-1">POS System</div>
        {/* Navigation right-aligned */}
        <nav className="flex space-x-4">
          <NavLink
            to="/products"
            className={({ isActive }) =>
              `px-3 py-2 rounded-md text-sm font-medium transition-colors duration-200 ${
                isActive
                  ? "text-white underline underline-offset-8 decoration-white"
                  : "text-white hover:underline hover:underline-offset-8 hover:decoration-white"
              }`
            }
            style={{ textDecoration: "none" }}
          >
            Products
          </NavLink>
        </nav>
      </div>
    </div>
  </header>
);

export default NavBar;
