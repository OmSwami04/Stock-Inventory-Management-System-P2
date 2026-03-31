import React from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import Layout from './components/layout/Layout';
import ProtectedRoute from './components/ProtectedRoute';
import LoginPage from './pages/LoginPage';
import Dashboard from './pages/Dashboard';
import ProductList from './pages/ProductList';
import ProductForm from './pages/ProductForm';
import StockMovement from './pages/StockMovement';
import AlertsPage from './pages/AlertsPage';

function App() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      
      <Route element={<ProtectedRoute />}>
        <Route element={<Layout />}>
          <Route path="/" element={<Dashboard />} />
          <Route path="/products" element={<ProductList />} />
          <Route path="/products/new" element={<ProductForm />} />
          <Route path="/products/edit/:id" element={<ProductForm />} />
          <Route path="/stock-movement" element={<StockMovement />} />
          <Route path="/alerts" element={<AlertsPage />} />
        </Route>
      </Route>

      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
}

export default App;
