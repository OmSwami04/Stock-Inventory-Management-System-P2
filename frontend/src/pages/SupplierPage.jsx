import React, { useState, useEffect } from 'react';
import apiClient from '../api/apiClient';
import { useAuth } from '../context/AuthContext';
import { Truck, Search, Plus, Edit2, Trash2, Loader2, Globe, Phone, Mail } from 'lucide-react';
import SupplierModal from '../components/modals/SupplierModal';

const SupplierPage = () => {
  const [suppliers, setSuppliers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [selectedSupplier, setSelectedSupplier] = useState(null);
  const { hasRole } = useAuth();

  const fetchSuppliers = async () => {
    try {
      setLoading(true);
      const response = await apiClient.get('/Suppliers');
      setSuppliers(response.data || []);
    } catch (err) {
      console.error('Error fetching suppliers:', err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchSuppliers();
  }, []);

  const handleAdd = () => {
    setSelectedSupplier(null);
    setIsModalOpen(true);
  };

  const handleEdit = (supplier) => {
    setSelectedSupplier(supplier);
    setIsModalOpen(true);
  };

  const handleDelete = async (id) => {
    if (window.confirm('Are you sure you want to delete this supplier?')) {
      try {
        await apiClient.delete(`/Suppliers/${id}`);
        fetchSuppliers();
      } catch (err) {
        console.error('Error deleting supplier:', err);
      }
    }
  };

  const handleSave = async (formData) => {
    try {
      if (selectedSupplier) {
        await apiClient.put(`/Suppliers/${selectedSupplier.supplierId}`, {
          ...selectedSupplier,
          ...formData
        });
      } else {
        await apiClient.post('/Suppliers', formData);
      }
      fetchSuppliers();
    } catch (err) {
      console.error('Error saving supplier:', err);
      throw err;
    }
  };

  const filteredSuppliers = suppliers.filter(s =>
    s.supplierName.toLowerCase().includes(searchTerm.toLowerCase()) ||
    s.email?.toLowerCase().includes(searchTerm.toLowerCase())
  );

  if (loading) {
    return (
      <div className="flex flex-col items-center justify-center min-h-[400px] gap-4">
        <Loader2 className="animate-spin text-blue-600" size={40} />
        <p className="text-slate-500 font-medium animate-pulse">Loading Suppliers...</p>
      </div>
    );
  }

  return (
    <div className="space-y-8 animate-in fade-in duration-500">
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
        <div>
          <h2 className="text-3xl font-extrabold text-slate-900 tracking-tight">Supplier Management</h2>
          <p className="text-slate-500 font-medium mt-1">Manage your supply chain partners and contact information</p>
        </div>
        
        {hasRole(['Admin', 'InventoryManager']) && (
          <button
            onClick={handleAdd}
            className="inline-flex items-center gap-2 bg-blue-600 hover:bg-blue-700 text-white px-6 py-3 rounded-xl font-bold shadow-lg shadow-blue-200 transition-all active:scale-[0.98]"
          >
            <Plus size={20} />
            Add New Supplier
          </button>
        )}
      </div>

      <div className="bg-white rounded-3xl shadow-sm border border-slate-100 overflow-hidden">
        <div className="p-6 border-b border-slate-100 bg-slate-50/50">
          <div className="relative max-w-md">
            <Search className="absolute left-4 top-1/2 -translate-y-1/2 text-slate-400" size={18} />
            <input
              type="text"
              placeholder="Search by name or contact..."
              className="w-full pl-11 pr-4 py-2.5 bg-white border border-slate-200 rounded-xl focus:ring-2 focus:ring-blue-500 outline-none transition-all font-medium text-slate-700 shadow-sm"
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
            />
          </div>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 p-6 bg-slate-50/30">
          {filteredSuppliers.length > 0 ? (
            filteredSuppliers.map((supplier) => (
              <div key={supplier.supplierId} className="bg-white p-6 rounded-2xl border border-slate-100 shadow-sm hover:shadow-md transition-all group relative overflow-hidden">
                <div className="absolute top-0 right-0 p-4 opacity-0 group-hover:opacity-100 transition-opacity flex gap-2">
                  {hasRole(['Admin', 'InventoryManager']) && (
                    <>
                      <button 
                        onClick={() => handleEdit(supplier)}
                        className="p-2 text-slate-400 hover:text-blue-600 hover:bg-blue-50 rounded-lg transition-all"
                      >
                        <Edit2 size={16} />
                      </button>
                      <button 
                        onClick={() => handleDelete(supplier.supplierId)}
                        className="p-2 text-slate-400 hover:text-red-600 hover:bg-red-50 rounded-lg transition-all"
                      >
                        <Trash2 size={16} />
                      </button>
                    </>
                  )}
                </div>
                
                <div className="flex items-start gap-4">
                  <div className="p-3 bg-blue-50 text-blue-600 rounded-xl group-hover:bg-blue-600 group-hover:text-white transition-colors duration-300">
                    <Truck size={24} />
                  </div>
                  <div className="flex-1 min-w-0">
                    <h3 className="text-lg font-bold text-slate-900 truncate">{supplier.supplierName}</h3>
                    <p className="text-sm font-semibold text-slate-400 uppercase tracking-wider">Supplier ID: {supplier.supplierId.substring(0, 8)}</p>
                  </div>
                </div>

                <div className="mt-6 space-y-3">
                  <div className="flex items-center gap-3 text-slate-600 group/item">
                    <div className="p-1.5 bg-slate-50 rounded-lg group-hover/item:bg-blue-50 group-hover/item:text-blue-600 transition-colors">
                      <Phone size={14} />
                    </div>
                    <span className="text-sm font-medium">{supplier.phone || 'N/A'}</span>
                  </div>
                  <div className="flex items-center gap-3 text-slate-600 group/item">
                    <div className="p-1.5 bg-slate-50 rounded-lg group-hover/item:bg-blue-50 group-hover/item:text-blue-600 transition-colors">
                      <Mail size={14} />
                    </div>
                    <span className="text-sm font-medium truncate">{supplier.email || 'N/A'}</span>
                  </div>
                  <div className="flex items-center gap-3 text-slate-600 group/item">
                    <div className="p-1.5 bg-slate-50 rounded-lg group-hover/item:bg-blue-50 group-hover/item:text-blue-600 transition-colors">
                      <Globe size={14} />
                    </div>
                    <span className="text-sm font-medium line-clamp-1">{supplier.website || 'N/A'}</span>
                  </div>
                </div>
              </div>
            ))
          ) : (
            <div className="col-span-full py-12 flex flex-col items-center justify-center bg-white rounded-3xl border border-dashed border-slate-200">
              <div className="p-4 bg-slate-50 rounded-full text-slate-300 mb-4">
                <Truck size={40} />
              </div>
              <p className="text-slate-500 font-bold text-lg">No suppliers found</p>
              <p className="text-slate-400 text-sm font-medium">Try adjusting your search criteria</p>
            </div>
          )}
        </div>
      </div>

      <SupplierModal 
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        onSave={handleSave}
        supplier={selectedSupplier}
      />
    </div>
  );
};

export default SupplierPage;

