import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import apiClient from '../api/apiClient';
import { useAuth } from '../context/AuthContext';
import { Warehouse as WarehouseIcon, Search, Plus, Edit2, Trash2, MapPin, Loader2, Gauge, Package } from 'lucide-react';
import WarehouseModal from '../components/modals/WarehouseModal';

const WarehousePage = () => {
  const navigate = useNavigate();
  const [warehouses, setWarehouses] = useState([]);
  const [stocks, setStocks] = useState([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [selectedWarehouse, setSelectedWarehouse] = useState(null);
  const { hasRole } = useAuth();

  const fetchData = async () => {
    try {
      setLoading(true);
      const [warehouseRes, stockRes] = await Promise.all([
        apiClient.get('/Warehouses'),
        apiClient.get('/Stock')
      ]);
      setWarehouses(warehouseRes.data || []);
      setStocks(stockRes.data || []);
    } catch (err) {
      console.error('Error fetching data:', err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, []);

  const handleAdd = () => {
    setSelectedWarehouse(null);
    setIsModalOpen(true);
  };

  const handleEdit = (warehouse) => {
    setSelectedWarehouse(warehouse);
    setIsModalOpen(true);
  };

  const handleDelete = async (id) => {
    if (window.confirm('Are you sure you want to delete this warehouse?')) {
      try {
        await apiClient.delete(`/Warehouses/${id}`);
        fetchData();
      } catch (err) {
        console.error('Error deleting warehouse:', err);
      }
    }
  };

  const handleSave = async (formData) => {
    try {
      if (selectedWarehouse) {
        await apiClient.put(`/Warehouses/${selectedWarehouse.warehouseId}`, {
          ...selectedWarehouse,
          ...formData
        });
      } else {
        await apiClient.post('/Warehouses', formData);
      }
      fetchData();
    } catch (err) {
      console.error('Error saving warehouse:', err);
      throw err;
    }
  };

  const getWarehouseStats = (warehouseId, totalCapacity) => {
    const warehouseStocks = stocks.filter(s => s.warehouseId === warehouseId);
    const totalItems = warehouseStocks.reduce((sum, s) => sum + (s.quantityOnHand || 0), 0);
    const capacityUsage = totalCapacity > 0 ? Math.min(Math.round((totalItems / totalCapacity) * 100), 100) : 0;
    return { totalItems, capacityUsage };
  };

  const filteredWarehouses = warehouses.filter(w =>
    w.warehouseName.toLowerCase().includes(searchTerm.toLowerCase()) ||
    w.location?.toLowerCase().includes(searchTerm.toLowerCase())
  );

  if (loading) {
    return (
      <div className="flex flex-col items-center justify-center min-h-[400px] gap-4">
        <Loader2 className="animate-spin text-blue-600" size={40} />
        <p className="text-slate-500 font-medium animate-pulse">Loading Warehouses...</p>
      </div>
    );
  }

  return (
    <div className="space-y-8 animate-in fade-in duration-500">
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
        <div>
          <h2 className="text-3xl font-extrabold text-slate-900 tracking-tight">Warehouse Network</h2>
          <p className="text-slate-500 font-medium mt-1">Monitor storage facilities and distribution points</p>
        </div>
        
        {hasRole(['Admin', 'InventoryManager']) && (
          <button
            onClick={handleAdd}
            className="inline-flex items-center gap-2 bg-blue-600 hover:bg-blue-700 text-white px-6 py-3 rounded-xl font-bold shadow-lg shadow-blue-200 transition-all active:scale-[0.98]"
          >
            <Plus size={20} />
            Add New Warehouse
          </button>
        )}
      </div>

      <div className="bg-white rounded-3xl shadow-sm border border-slate-100 overflow-hidden">
        <div className="p-6 border-b border-slate-100 bg-slate-50/50">
          <div className="relative max-w-md">
            <Search className="absolute left-4 top-1/2 -translate-y-1/2 text-slate-400" size={18} />
            <input
              type="text"
              placeholder="Search by name or location..."
              className="w-full pl-11 pr-4 py-2.5 bg-white border border-slate-200 rounded-xl focus:ring-2 focus:ring-blue-500 outline-none transition-all font-medium text-slate-700 shadow-sm"
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
            />
          </div>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 p-6 bg-slate-50/30">
          {filteredWarehouses.length > 0 ? (
            filteredWarehouses.map((warehouse) => {
              const { totalItems, capacityUsage } = getWarehouseStats(warehouse.warehouseId, warehouse.capacity);
              const isHigh = capacityUsage > 85;

              return (
                <div key={warehouse.warehouseId} className="bg-white p-6 rounded-2xl border border-slate-100 shadow-sm hover:shadow-md transition-all group relative">
                  <div className="absolute top-0 right-0 p-4 opacity-0 group-hover:opacity-100 transition-opacity flex gap-2">
                    {hasRole(['Admin', 'InventoryManager']) && (
                      <>
                        <button 
                          onClick={() => handleEdit(warehouse)}
                          className="p-2 text-slate-400 hover:text-blue-600 hover:bg-blue-50 rounded-lg transition-all"
                        >
                          <Edit2 size={16} />
                        </button>
                        <button 
                          onClick={() => handleDelete(warehouse.warehouseId)}
                          className="p-2 text-slate-400 hover:text-red-600 hover:bg-red-50 rounded-lg transition-all"
                        >
                          <Trash2 size={16} />
                        </button>
                      </>
                    )}
                  </div>

                  <div className="flex items-center gap-4 mb-6">
                    <div className="p-3 bg-blue-50 text-blue-600 rounded-xl group-hover:bg-blue-600 group-hover:text-white transition-all duration-300">
                      <WarehouseIcon size={24} />
                    </div>
                    <div className="min-w-0 flex-1">
                      <h3 className="text-lg font-bold text-slate-900 truncate">{warehouse.warehouseName}</h3>
                      <div className="flex items-center gap-1.5 text-slate-400">
                        <MapPin size={12} className="shrink-0" />
                        <span className="text-xs font-bold uppercase tracking-wider truncate">{warehouse.location}</span>
                      </div>
                    </div>
                  </div>

                  <div className="space-y-6">
                    <div>
                      <div className="flex items-center justify-between mb-2">
                        <div className="flex items-center gap-2 text-slate-600">
                          <Gauge size={16} className="text-slate-400" />
                          <span className="text-sm font-bold">Storage Capacity</span>
                        </div>
                        <span className={`text-sm font-black ${isHigh ? 'text-red-500' : 'text-emerald-500'}`}>
                          {capacityUsage}%
                        </span>
                      </div>
                      <div className="h-2.5 bg-slate-100 rounded-full overflow-hidden">
                        <div 
                          className={`h-full rounded-full transition-all duration-1000 ${isHigh ? 'bg-red-500' : 'bg-emerald-500'}`}
                          style={{ width: `${capacityUsage}%` }}
                        />
                      </div>
                      <div className="flex justify-between mt-2">
                        <p className="text-[10px] font-bold text-slate-400 uppercase tracking-wide">
                          Total Capacity: {warehouse.capacity.toLocaleString()}
                        </p>
                        <p className="text-[10px] font-bold text-slate-400 uppercase tracking-wide">
                          Items: {totalItems.toLocaleString()}
                        </p>
                      </div>
                    </div>

                    <div className="flex items-center justify-between p-4 bg-slate-50 rounded-xl">
                      <div className="flex items-center gap-3">
                        <div className="p-2 bg-white rounded-lg shadow-sm">
                          <Package size={18} className="text-blue-600" />
                        </div>
                        <div>
                          <p className="text-[10px] font-bold text-slate-400 uppercase tracking-wider">Stock Status</p>
                          <p className="text-sm font-black text-slate-900">
                            {capacityUsage > 90 ? 'Critical' : capacityUsage > 70 ? 'High' : 'Normal'}
                          </p>
                        </div>
                      </div>
                      <button 
                        onClick={() => navigate(`/warehouses/${warehouse.warehouseId}`)}
                        className="text-xs font-bold text-blue-600 hover:underline"
                      >
                        View Details
                      </button>
                    </div>
                  </div>
                </div>
              );
            })
          ) : (
            <div className="col-span-full py-12 flex flex-col items-center justify-center bg-white rounded-3xl border border-dashed border-slate-200">
              <div className="p-4 bg-slate-50 rounded-full text-slate-300 mb-4">
                <WarehouseIcon size={40} />
              </div>
              <p className="text-slate-500 font-bold text-lg">No warehouses found</p>
              <p className="text-slate-400 text-sm font-medium">Try adjusting your search criteria</p>
            </div>
          )}
        </div>
      </div>

      <WarehouseModal 
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        onSave={handleSave}
        warehouse={selectedWarehouse}
      />
    </div>
  );
};

export default WarehousePage;

