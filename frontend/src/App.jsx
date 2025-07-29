import React, { useState } from "react";
import {
  BrowserRouter as Router,
  Routes,
  Route,
  NavLink,
} from "react-router-dom";

function App() {
  return (
    <div className="min-h-screen bg-background">
      {/* Header */}
      <header className="bg-surface shadow-lg border-b-2 border-primary">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between items-center h-16">
            {/* Logo */}
            <div className="flex items-center">
              <div className="text-2xl font-bold text-primary">ClothingPOS</div>
            </div>
          </div>
        </div>
      </header>

      {/* Footer */}
      <footer className="bg-surface border-t mt-8">
        <div className="max-w-7xl mx-auto px-4 py-6">
          <div className="flex justify-between items-center">
            <div className="text-secondary-text text-sm">
              © 2025 Clothing System.
            </div>
          </div>
        </div>
      </footer>
    </div>
  );
}

export default App;
