import React, { useState, useEffect } from 'react';
import apiClient from '../api/apiClient';
import { AlertTriangle, ShieldAlert, Package, Warehouse, ArrowRight } from 'lucide-react';
import { Link } from 'react-router-dom';

const AlertsPage = () => {
  const [alerts, setAlerts] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchAlerts = async () => {
      try {
        const res = await apiClient.get('/Stock');
        const stockData = res.data || [];
        const lowStock = stockData.filter(s => s.quantityOnHand <= s.reorderLevel);
        setAlerts(lowStock);
      } catch (err) {
        console.error(err);
      } finally {
        setLoading(false);
      }
    };
    fetchAlerts();
  }, []);

  if (loading) return <div className="flex items-center justify-center min-h-[400px]">Scanning for alerts...</div>;

  return (
    <div className="space-y-8">
      <div>
        <h2 className="text-3xl font-extrabold text-slate-900 tracking-tight">Active Stock Alerts</h2>
        <p className="text-slate-500 font-medium mt-1">Real-time notification of inventory health issues</p>
      </div>

      <div className="grid grid-cols-1 gap-4">
        {alerts.length === 0 ? (
          <div className="bg-white p-12 rounded-3xl border border-slate-100 text-center space-y-4">
            <div className="mx-auto h-16 w-16 bg-emerald-50 text-emerald-500 rounded-full flex items-center justify-center">
              <ShieldAlert size={32} />
            </div>
            <p className="text-xl font-bold text-slate-900">All systems optimal</p>
            <p className="text-slate-500">No low stock or critical inventory issues detected currently.</p>
          </div>
        ) : (
          alerts.map((alert) => {
            const isCritical = alert.quantityOnHand <= alert.safetyStock;
            return (
              <div key={alert.stockLevelId} className={`bg-white p-6 rounded-3xl border transition-all hover:shadow-md flex items-center gap-6 ${isCritical ? 'border-red-200 bg-red-50/10' : 'border-amber-100'}`}>
                <div className={`p-4 rounded-2xl ${isCritical ? 'bg-red-500 text-white' : 'bg-amber-100 text-amber-600'}`}>
                  {isCritical ? <ShieldAlert size={24} /> : <AlertTriangle size={24} />}
                </div>
                <div className="flex-1 min-w-0">
                  <div className="flex items-center gap-2 mb-1">
                    <span className={`px-2 py-0.5 rounded text-[10px] font-black uppercase tracking-widest ${isCritical ? 'bg-red-600 text-white' : 'bg-amber-500 text-white'}`}>
                      {isCritical ? 'Critical' : 'Low Stock'}
                    </span>
                    <h3 className="text-lg font-extrabold text-slate-900 truncate">{alert.productName}</h3>
                  </div>
                  <div className="flex items-center gap-6 text-sm font-medium text-slate-500">
                    <div className="flex items-center gap-1.5">
                      <Warehouse size={14} />
                      {alert.warehouseName}
                    </div>
                    <div className="flex items-center gap-1.5">
                      <Package size={14} />
                      Qty: <span className={`font-bold ${isCritical ? 'text-red-600' : 'text-slate-900'}`}>{alert.quantityOnHand}</span>
                    </div>
                    <div className="text-[11px] font-bold text-slate-400 uppercase">Threshold: {alert.reorderLevel}</div>
                  </div>
                </div>
                <Link
                  to="/stock-movement"
                  state={{ productId: alert.productId, warehouseId: alert.warehouseId, type: 0 }}
                  className="p-3 bg-slate-50 hover:bg-slate-100 text-slate-400 hover:text-blue-600 rounded-2xl transition-all"
                >
                  <ArrowRight size={20} />
                </Link>
              </div>
            );
          })
        )}
      </div>
    </div>
  );
};

export default AlertsPage;
