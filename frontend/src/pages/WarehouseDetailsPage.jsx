import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import apiClient from '../api/apiClient';
import { 
  ArrowLeft, 
  Warehouse as WarehouseIcon, 
  Package, 
  AlertCircle, 
  Loader2, 
  MapPin, 
  Gauge,
  TrendingUp,
  History
} from 'lucide-react';

const WarehouseDetailsPage = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const [warehouse, setWarehouse] = useState(null);
  const [stockLevels, setStockLevels] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchDetails = async () => {
      try {
        setLoading(true);
        const [warehouseRes, stockRes] = await Promise.all([
          apiClient.get(`/Warehouses/${id}`),
          apiClient.get(`/Stock/warehouse/${id}`)
        ]);
        setWarehouse(warehouseRes.data);
        setStockLevels(stockRes.data || []);
      } catch (err) {
        console.error('Error fetching warehouse details:', err);
      } finally {
        setLoading(false);
      }
    };

    fetchDetails();
  }, [id]);

  if (loading) {
    return (
      <div className="flex flex-col items-center justify-center min-h-[400px] gap-4">
        <Loader2 className="animate-spin text-blue-600" size={40} />
        <p className="text-slate-500 font-medium animate-pulse">Loading Warehouse Inventory...</p>
      </div>
    );
  }

  if (!warehouse) return <div>Warehouse not found.</div>;

  const totalItems = stockLevels.reduce((sum, s) => sum + (s.quantityOnHand || 0), 0);
  const capacityUsage = warehouse.capacity > 0 ? Math.min(Math.round((totalItems / warehouse.capacity) * 100), 100) : 0;
  const isHigh = capacityUsage > 85;

  return (
    <div className="space-y-8 animate-in fade-in duration-500">
      {/* Header */}
      <div className="flex flex-col gap-6">
        <button 
          onClick={() => navigate('/warehouses')}
          className="inline-flex items-center gap-2 text-slate-500 hover:text-blue-600 font-bold transition-colors w-fit group"
        >
          <div className="p-2 bg-white rounded-lg border border-slate-200 group-hover:border-blue-200 shadow-sm transition-all">
            <ArrowLeft size={18} />
          </div>
          Back to Warehouses
        </button>

        <div className="flex flex-col md:flex-row md:items-end justify-between gap-6">
          <div className="flex items-center gap-5">
            <div className="p-4 bg-blue-600 text-white rounded-2xl shadow-xl shadow-blue-200">
              <WarehouseIcon size={32} />
            </div>
            <div>
              <h2 className="text-3xl font-extrabold text-slate-900 tracking-tight">{warehouse.warehouseName}</h2>
              <div className="flex items-center gap-2 text-slate-500 mt-1 font-medium">
                <MapPin size={16} className="text-slate-400" />
                {warehouse.location}
              </div>
            </div>
          </div>

          <div className="flex gap-4">
            <div className="bg-white p-4 rounded-2xl border border-slate-100 shadow-sm flex items-center gap-4">
              <div className="p-2.5 bg-emerald-50 text-emerald-600 rounded-xl">
                <Package size={20} />
              </div>
              <div>
                <p className="text-[10px] font-bold text-slate-400 uppercase tracking-wider">Total Inventory</p>
                <p className="text-xl font-black text-slate-900">{totalItems.toLocaleString()}</p>
              </div>
            </div>
            <div className="bg-white p-4 rounded-2xl border border-slate-100 shadow-sm flex items-center gap-4">
              <div className={`p-2.5 rounded-xl ${isHigh ? 'bg-red-50 text-red-600' : 'bg-blue-50 text-blue-600'}`}>
                <Gauge size={20} />
              </div>
              <div>
                <p className="text-[10px] font-bold text-slate-400 uppercase tracking-wider">Capacity Used</p>
                <p className={`text-xl font-black ${isHigh ? 'text-red-600' : 'text-slate-900'}`}>{capacityUsage}%</p>
              </div>
            </div>
          </div>
        </div>
      </div>

      {/* Inventory Table */}
      <div className="bg-white rounded-3xl shadow-sm border border-slate-100 overflow-hidden">
        <div className="p-6 border-b border-slate-100 bg-slate-50/50 flex items-center justify-between">
          <h3 className="text-lg font-bold text-slate-900 flex items-center gap-2">
            <TrendingUp size={20} className="text-blue-600" />
            Current Stock Levels
          </h3>
          <span className="px-4 py-1.5 bg-white border border-slate-200 rounded-full text-xs font-bold text-slate-500 shadow-sm">
            {stockLevels.length} Unique Products
          </span>
        </div>

        <div className="overflow-x-auto">
          <table className="w-full text-left border-collapse">
            <thead>
              <tr className="bg-slate-50/50">
                <th className="px-6 py-4 text-xs font-bold text-slate-400 uppercase tracking-widest border-b border-slate-100">Product</th>
                <th className="px-6 py-4 text-xs font-bold text-slate-400 uppercase tracking-widest border-b border-slate-100 text-center">In Stock</th>
                <th className="px-6 py-4 text-xs font-bold text-slate-400 uppercase tracking-widest border-b border-slate-100 text-center">Safety Stock</th>
                <th className="px-6 py-4 text-xs font-bold text-slate-400 uppercase tracking-widest border-b border-slate-100 text-center">Status</th>
                <th className="px-6 py-4 text-xs font-bold text-slate-400 uppercase tracking-widest border-b border-slate-100 text-right">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-50">
              {stockLevels.length > 0 ? (
                stockLevels.map((stock) => {
                  const isLow = stock.quantityOnHand <= stock.reorderLevel;
                  return (
                    <tr key={stock.productId} className="hover:bg-slate-50/50 transition-colors group">
                      <td className="px-6 py-5">
                        <div className="flex flex-col">
                          <span className="text-sm font-bold text-slate-900">{stock.productName}</span>
                          <span className="text-[10px] font-bold text-slate-400 uppercase mt-0.5">{stock.sku}</span>
                        </div>
                      </td>
                      <td className="px-6 py-5 text-center">
                        <span className={`text-sm font-black ${isLow ? 'text-red-500' : 'text-slate-900'}`}>
                          {stock.quantityOnHand}
                        </span>
                        <span className="text-[10px] font-medium text-slate-400 ml-1">{stock.unitOfMeasure}</span>
                      </td>
                      <td className="px-6 py-5 text-center">
                        <span className="text-sm font-bold text-slate-600">{stock.reorderLevel}</span>
                      </td>
                      <td className="px-6 py-5">
                        <div className="flex justify-center">
                          {isLow ? (
                            <span className="inline-flex items-center gap-1.5 px-3 py-1 bg-red-50 text-red-600 rounded-full text-[10px] font-bold uppercase tracking-wider animate-pulse">
                              <AlertCircle size={12} />
                              Low Stock
                            </span>
                          ) : (
                            <span className="inline-flex items-center gap-1.5 px-3 py-1 bg-emerald-50 text-emerald-600 rounded-full text-[10px] font-bold uppercase tracking-wider">
                              Healthy
                            </span>
                          )}
                        </div>
                      </td>
                      <td className="px-6 py-5 text-right">
                        <button className="p-2 text-slate-400 hover:text-blue-600 hover:bg-blue-50 rounded-lg transition-all" title="View History">
                          <History size={16} />
                        </button>
                      </td>
                    </tr>
                  );
                })
              ) : (
                <tr>
                  <td colSpan="5" className="px-6 py-12 text-center text-slate-400 font-medium italic">
                    No items currently in stock at this warehouse.
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
};

export default WarehouseDetailsPage;
