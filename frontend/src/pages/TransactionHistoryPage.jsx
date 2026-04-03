import React, { useState, useEffect } from 'react';
import apiClient from '../api/apiClient';
import { 
  History, 
  Search, 
  ArrowUpDown, 
  Calendar, 
  Hash, 
  Loader2, 
  ArrowUpRight, 
  ArrowDownLeft, 
  RefreshCw,
  Info
} from 'lucide-react';

const TransactionHistoryPage = () => {
  const [transactions, setTransactions] = useState([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [sortConfig, setSortConfig] = useState({ key: 'transactionDate', direction: 'desc' });

  const formatDate = (dateStr) => {
    const date = new Date(dateStr);
    return new Intl.DateTimeFormat('en-US', { month: 'short', day: '2-digit', year: 'numeric' }).format(date);
  };

  const formatTime = (dateStr) => {
    const date = new Date(dateStr);
    return new Intl.DateTimeFormat('en-US', { hour: '2-digit', minute: '2-digit', hour12: false }).format(date);
  };

  useEffect(() => {
    fetchTransactions();
  }, []);

  const fetchTransactions = async () => {
    try {
      setLoading(true);
      const response = await apiClient.get('/Stock/transactions');
      setTransactions(response.data || []);
    } catch (err) {
      console.error('Error fetching transactions:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleSort = (key) => {
    let direction = 'asc';
    if (sortConfig.key === key && sortConfig.direction === 'asc') {
      direction = 'desc';
    }
    setSortConfig({ key, direction });
  };

  const sortedTransactions = [...transactions].sort((a, b) => {
    if (a[sortConfig.key] < b[sortConfig.key]) {
      return sortConfig.direction === 'asc' ? -1 : 1;
    }
    if (a[sortConfig.key] > b[sortConfig.key]) {
      return sortConfig.direction === 'asc' ? 1 : -1;
    }
    return 0;
  });

  const filteredTransactions = sortedTransactions.filter(tx => 
    tx.transactionId.toLowerCase().includes(searchTerm.toLowerCase()) ||
    tx.reference.toLowerCase().includes(searchTerm.toLowerCase()) ||
    tx.productName.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const getTransactionIcon = (type) => {
    switch (type.toLowerCase()) {
      case 'purchase': return <ArrowDownLeft className="text-emerald-500" size={18} />;
      case 'sale': return <ArrowUpRight className="text-blue-500" size={18} />;
      case 'adjustment': return <RefreshCw className="text-amber-500" size={18} />;
      default: return <Info className="text-slate-400" size={18} />;
    }
  };

  if (loading) {
    return (
      <div className="flex flex-col items-center justify-center min-h-[400px] gap-4">
        <Loader2 className="animate-spin text-blue-600" size={40} />
        <p className="text-slate-500 font-medium animate-pulse">Loading Transaction History...</p>
      </div>
    );
  }

  return (
    <div className="space-y-8 animate-in fade-in duration-500">
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
        <div>
          <h2 className="text-3xl font-extrabold text-slate-900 tracking-tight">Transaction History</h2>
          <p className="text-slate-500 font-medium mt-1">Review all inventory movements and stock adjustments</p>
        </div>
        <button 
          onClick={fetchTransactions}
          className="p-3 bg-white border border-slate-200 rounded-xl hover:bg-slate-50 transition-all shadow-sm"
          title="Refresh"
        >
          <RefreshCw size={20} className="text-slate-600" />
        </button>
      </div>

      <div className="bg-white rounded-3xl shadow-sm border border-slate-100 overflow-hidden">
        <div className="p-6 border-b border-slate-100 bg-slate-50/50 flex flex-col md:flex-row gap-4 justify-between">
          <div className="relative max-w-md w-full">
            <Search className="absolute left-4 top-1/2 -translate-y-1/2 text-slate-400" size={18} />
            <input
              type="text"
              placeholder="Search by ID, Reference, or Product..."
              className="w-full pl-11 pr-4 py-2.5 bg-white border border-slate-200 rounded-xl focus:ring-2 focus:ring-blue-500 outline-none transition-all font-medium text-slate-700 shadow-sm"
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
            />
          </div>

          <div className="flex items-center gap-2">
            <button 
              onClick={() => handleSort('transactionDate')}
              className={`inline-flex items-center gap-2 px-4 py-2 rounded-xl text-sm font-bold transition-all ${
                sortConfig.key === 'transactionDate' ? 'bg-blue-600 text-white shadow-md shadow-blue-100' : 'bg-white border border-slate-200 text-slate-600 hover:bg-slate-50'
              }`}
            >
              <Calendar size={16} />
              Date {sortConfig.key === 'transactionDate' && (sortConfig.direction === 'asc' ? '↑' : '↓')}
            </button>
            <button 
              onClick={() => handleSort('transactionId')}
              className={`inline-flex items-center gap-2 px-4 py-2 rounded-xl text-sm font-bold transition-all ${
                sortConfig.key === 'transactionId' ? 'bg-blue-600 text-white shadow-md shadow-blue-100' : 'bg-white border border-slate-200 text-slate-600 hover:bg-slate-50'
              }`}
            >
              <Hash size={16} />
              ID {sortConfig.key === 'transactionId' && (sortConfig.direction === 'asc' ? '↑' : '↓')}
            </button>
          </div>
        </div>

        <div className="overflow-x-auto">
          <table className="w-full text-left border-collapse">
            <thead>
              <tr className="bg-slate-50/50">
                <th className="px-6 py-4 text-xs font-bold text-slate-400 uppercase tracking-widest border-b border-slate-100">Type</th>
                <th className="px-6 py-4 text-xs font-bold text-slate-400 uppercase tracking-widest border-b border-slate-100">Product</th>
                <th className="px-6 py-4 text-xs font-bold text-slate-400 uppercase tracking-widest border-b border-slate-100">Warehouse</th>
                <th className="px-6 py-4 text-xs font-bold text-slate-400 uppercase tracking-widest border-b border-slate-100 text-center">Quantity</th>
                <th className="px-6 py-4 text-xs font-bold text-slate-400 uppercase tracking-widest border-b border-slate-100">Reference</th>
                <th className="px-6 py-4 text-xs font-bold text-slate-400 uppercase tracking-widest border-b border-slate-100 text-right">Date</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-50">
              {filteredTransactions.length > 0 ? (
                filteredTransactions.map((tx) => (
                  <tr key={tx.transactionId} className="hover:bg-slate-50/50 transition-colors group">
                    <td className="px-6 py-5">
                      <div className="flex items-center gap-3">
                        <div className="p-2 bg-slate-50 rounded-lg group-hover:bg-white transition-colors shadow-sm">
                          {getTransactionIcon(tx.transactionType)}
                        </div>
                        <span className="text-sm font-bold text-slate-700 capitalize">{tx.transactionType}</span>
                      </div>
                    </td>
                    <td className="px-6 py-5">
                      <div className="flex flex-col">
                        <span className="text-sm font-bold text-slate-900">{tx.productName}</span>
                        <span className="text-[10px] font-bold text-slate-400 uppercase tracking-tight truncate max-w-[150px]" title={tx.transactionId}>
                          ID: {tx.transactionId.substring(0, 8)}...
                        </span>
                      </div>
                    </td>
                    <td className="px-6 py-5">
                      <span className="text-sm font-medium text-slate-600">{tx.warehouseName}</span>
                    </td>
                    <td className="px-6 py-5 text-center">
                      <span className={`text-sm font-black ${tx.quantity < 0 ? 'text-red-500' : 'text-emerald-600'}`}>
                        {tx.quantity > 0 ? `+${tx.quantity}` : tx.quantity}
                      </span>
                    </td>
                    <td className="px-6 py-5">
                      <span className="px-2.5 py-1 bg-slate-100 text-slate-600 rounded-md text-[10px] font-black uppercase tracking-wider">
                        {tx.reference || 'N/A'}
                      </span>
                    </td>
                    <td className="px-6 py-5 text-right">
                      <div className="flex flex-col items-end">
                        <span className="text-sm font-bold text-slate-900">{formatDate(tx.transactionDate)}</span>
                        <span className="text-[10px] font-medium text-slate-400">{formatTime(tx.transactionDate)}</span>
                      </div>
                    </td>
                  </tr>
                ))
              ) : (
                <tr>
                  <td colSpan="6" className="px-6 py-12 text-center text-slate-400 font-medium italic">
                    No transactions found matching your search.
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

export default TransactionHistoryPage;
