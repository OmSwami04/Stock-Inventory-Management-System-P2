import React, { useState, useEffect } from 'react';
import apiClient from '../api/apiClient';
import { TRANSACTION_TYPES } from '../utils/constants';
import { ArrowUpDown, Box, Warehouse, Clipboard, Loader2, Info } from 'lucide-react';

const StockMovement = () => {
  const [products, setProducts] = useState([]);
  const [warehouses, setWarehouses] = useState([]);
  const [formData, setFormData] = useState({
    productId: '',
    warehouseId: '',
    transactionType: 0,
    quantity: 1,
    referenceNumber: '',
  });
  const [currentStock, setCurrentStock] = useState(null);
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [message, setMessage] = useState({ type: '', text: '' });

  useEffect(() => {
    const fetchData = async () => {
      setLoading(true);
      try {
        const [prodRes, warRes] = await Promise.all([
          apiClient.get('/Products?pageSize=1000'),
          apiClient.get('/Warehouses')
        ]);
        
        const prodItems = prodRes.data.items || [];
        const warItems = warRes.data || [];
        
        setProducts(prodItems);
        setWarehouses(warItems);
        
        setFormData(prev => ({
          ...prev,
          productId: prodItems.length > 0 ? prodItems[0].productId : '',
          warehouseId: warItems.length > 0 ? warItems[0].warehouseId : ''
        }));
      } catch (err) {
        console.error(err);
      } finally {
        setLoading(false);
      }
    };
    fetchData();
  }, []);

  useEffect(() => {
    if (formData.productId && formData.warehouseId) {
      const fetchStock = async () => {
        try {
          const res = await apiClient.get(`/Stock/${formData.productId}`);
          const stock = res.data.find(s => s.warehouseId === formData.warehouseId);
          setCurrentStock(stock ? stock.quantityOnHand : 0);
        } catch (err) {
          setCurrentStock(0);
        }
      };
      fetchStock();
    }
  }, [formData.productId, formData.warehouseId]);

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!formData.productId || !formData.warehouseId) {
      setMessage({ type: 'error', text: 'Product and Warehouse are required.' });
      return;
    }
    setSubmitting(true);
    setMessage({ type: '', text: '' });

    if (formData.transactionType === 1 && formData.quantity > currentStock) {
      setMessage({ type: 'error', text: 'Insufficient stock for this transaction.' });
      setSubmitting(false);
      return;
    }

    try {
      await apiClient.post('/Stock/transaction', {
        ...formData,
        transactionDate: new Date().toISOString()
      });
      setMessage({ type: 'success', text: 'Stock transaction recorded successfully!' });
      setFormData(prev => ({ ...prev, quantity: 1, referenceNumber: '' }));
      
      const res = await apiClient.get(`/Stock/${formData.productId}`);
      const stock = res.data.find(s => s.warehouseId === formData.warehouseId);
      setCurrentStock(stock ? stock.quantityOnHand : 0);
    } catch (err) {
      setMessage({ type: 'error', text: err.response?.data?.message || 'Transaction failed.' });
    } finally {
      setSubmitting(false);
    }
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    const isNumeric = name === 'quantity' || name === 'transactionType';
    setFormData(prev => ({
      ...prev,
      [name]: isNumeric ? (value === '' ? '' : parseInt(value)) : value
    }));
  };

  if (loading) return <div className="flex items-center justify-center min-h-[400px]">Loading metadata...</div>;

  const isOut = formData.transactionType === 1;
  const canSubmit = !isOut || (formData.quantity <= currentStock);

  return (
    <div className="max-w-4xl mx-auto space-y-8">
      <div>
        <h2 className="text-3xl font-extrabold text-slate-900 tracking-tight">Stock Movements</h2>
        <p className="text-slate-500 font-medium mt-1">Record purchases, sales, and internal adjustments</p>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
        <div className="lg:col-span-2">
          <div className="bg-white rounded-3xl shadow-sm border border-slate-100 overflow-hidden">
            <form onSubmit={handleSubmit} className="p-8 space-y-8">
              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                <div className="md:col-span-2">
                  <label className="block text-sm font-bold text-slate-700 mb-2">Target Product</label>
                  <div className="relative">
                    <Box className="absolute left-4 top-1/2 -translate-y-1/2 text-slate-400 font-medium" size={18} />
                    <select
                      name="productId"
                      className="w-full pl-11 pr-4 py-3 bg-slate-50 border border-slate-200 rounded-xl focus:ring-2 focus:ring-blue-500 transition-all font-medium text-slate-700 appearance-none"
                      value={formData.productId}
                      onChange={handleChange}
                    >
                      <option value="" disabled>Select a product</option>
                      {products.map(p => (
                        <option key={p.productId} value={p.productId}>{p.productName} ({p.sku})</option>
                      ))}
                    </select>
                  </div>
                </div>

                <div>
                  <label className="block text-sm font-bold text-slate-700 mb-2">Location / Warehouse</label>
                  <div className="relative">
                    <Warehouse className="absolute left-4 top-1/2 -translate-y-1/2 text-slate-400" size={18} />
                    <select
                      name="warehouseId"
                      className="w-full pl-11 pr-4 py-3 bg-slate-50 border border-slate-200 rounded-xl focus:ring-2 focus:ring-blue-500 transition-all font-medium text-slate-700 appearance-none"
                      value={formData.warehouseId}
                      onChange={handleChange}
                    >
                      <option value="" disabled>Select a warehouse</option>
                      {warehouses.map(w => (
                        <option key={w.warehouseId} value={w.warehouseId}>{w.warehouseName}</option>
                      ))}
                    </select>
                  </div>
                </div>

                <div>
                  <label className="block text-sm font-bold text-slate-700 mb-2">Movement Type</label>
                  <div className="relative">
                    <ArrowUpDown className="absolute left-4 top-1/2 -translate-y-1/2 text-slate-400" size={18} />
                    <select
                      name="transactionType"
                      className="w-full pl-11 pr-4 py-3 bg-slate-50 border border-slate-200 rounded-xl focus:ring-2 focus:ring-blue-500 transition-all font-medium text-slate-700 appearance-none"
                      value={formData.transactionType}
                      onChange={handleChange}
                    >
                      {TRANSACTION_TYPES.map(t => (
                        <option key={t.value} value={t.value}>{t.label}</option>
                      ))}
                    </select>
                  </div>
                </div>

                <div>
                  <label className="block text-sm font-bold text-slate-700 mb-2">Transaction Quantity</label>
                  <input
                    type="number"
                    name="quantity"
                    min="1"
                    className="w-full px-4 py-3 bg-slate-50 border border-slate-200 rounded-xl focus:ring-2 focus:ring-blue-500 font-black text-slate-900"
                    value={formData.quantity}
                    onChange={handleChange}
                  />
                </div>

                <div>
                  <label className="block text-sm font-bold text-slate-700 mb-2">Reference Number</label>
                  <div className="relative">
                    <Clipboard className="absolute left-4 top-1/2 -translate-y-1/2 text-slate-400" size={18} />
                    <input
                      name="referenceNumber"
                      className="w-full pl-11 pr-4 py-3 bg-slate-50 border border-slate-200 rounded-xl focus:ring-2 focus:ring-blue-500 font-medium text-slate-700"
                      value={formData.referenceNumber}
                      onChange={handleChange}
                      placeholder="PO-123, SALE-456, etc."
                    />
                  </div>
                </div>
              </div>

              {message.text && (
                <div className={`px-6 py-4 rounded-2xl text-sm font-bold border flex items-center gap-3 ${
                  message.type === 'success' ? 'bg-emerald-50 text-emerald-600 border-emerald-100' : 'bg-red-50 text-red-600 border-red-100'
                }`}>
                  <div className={`h-2 w-2 rounded-full animate-pulse ${message.type === 'success' ? 'bg-emerald-600' : 'bg-red-600'}`} />
                  {message.text}
                </div>
              )}

              <button
                type="submit"
                disabled={submitting || !canSubmit}
                className="w-full py-4 bg-blue-600 hover:bg-blue-700 disabled:bg-slate-200 disabled:text-slate-400 text-white font-black rounded-xl shadow-lg shadow-blue-100 transition-all flex items-center justify-center gap-2"
              >
                {submitting ? <Loader2 className="animate-spin" size={20} /> : 'Post Transaction'}
              </button>
            </form>
          </div>
        </div>

        <div className="space-y-6">
           <div className="bg-white p-6 rounded-3xl shadow-sm border border-slate-100">
             <h3 className="text-sm font-black text-slate-400 uppercase tracking-widest mb-4">Stock Status</h3>
             <div className="text-center py-6">
                <p className="text-slate-500 text-sm font-medium">Quantity on Hand</p>
                <div className={`text-5xl font-black mt-2 ${currentStock <= 10 ? 'text-red-500' : 'text-slate-900'}`}>
                  {currentStock !== null ? currentStock : '--'}
                </div>
             </div>
             {isOut && currentStock < formData.quantity && (
               <div className="mt-4 p-4 bg-red-50 rounded-2xl border border-red-100 flex gap-3">
                 <Info size={20} className="text-red-500 flex-shrink-0" />
                 <p className="text-xs font-bold text-red-600 leading-relaxed">
                   CRITICAL: You are trying to sell/move more quantity than available in this warehouse.
                 </p>
               </div>
             )}
           </div>

           <div className="bg-slate-900 p-8 rounded-3xl shadow-xl text-white relative overflow-hidden">
             <div className="relative z-10">
               <h3 className="text-sm font-bold text-blue-400 uppercase tracking-widest mb-2">Quick Tip</h3>
               <p className="text-sm font-medium text-slate-300 leading-relaxed">
                 Use **Adjustments** for correcting inventory errors, and **Returns** for processing customer returns back into stock.
               </p>
             </div>
             <Box className="absolute -bottom-4 -right-4 text-white/5" size={120} />
           </div>
        </div>
      </div>
    </div>
  );
};

export default StockMovement;
