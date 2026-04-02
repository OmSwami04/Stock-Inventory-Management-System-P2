import React, { useState, useEffect } from 'react';
import { useAuth } from '../../context/AuthContext';
import { useInventory } from '../../context/InventoryContext';
import { User, Bell } from 'lucide-react';
import apiClient from '../../api/apiClient';
import { useNavigate } from 'react-router-dom';

const Navbar = () => {
  const { user } = useAuth();
  const { lastUpdate } = useInventory();
  const [alertCount, setAlertCount] = useState(0);
  const navigate = useNavigate();

  const fetchAlertCount = async () => {
    try {
      const res = await apiClient.get('/Inventory/alerts/low-stock');
      // res.data is { count: N }
      setAlertCount(res.data.count || 0);
    } catch (err) {
      console.error('Error fetching alert count:', err);
    }
  };

  useEffect(() => {
    fetchAlertCount();
  }, []);

  useEffect(() => {
    if (lastUpdate) {
      fetchAlertCount();
    }
  }, [lastUpdate]);

  return (
    <header className="h-16 bg-white border-b border-slate-200 flex items-center justify-between px-8">
      <h1 className="text-xl font-semibold text-slate-800 tracking-tight">
        Inventory Management System
      </h1>
      
      <div className="flex items-center gap-6 text-slate-500">
        <button 
          onClick={() => navigate('/alerts')}
          className="hover:text-blue-600 transition-colors relative p-2 rounded-full hover:bg-slate-50"
        >
          <Bell size={20} />
          {alertCount > 0 && (
            <span className="absolute -top-1 -right-1 flex h-4 w-4 items-center justify-center rounded-full bg-red-500 text-[10px] font-bold text-white shadow-sm animate-pulse">
              {alertCount}
            </span>
          )}
        </button>
        <div className="h-8 w-[1px] bg-slate-200 ml-2" />
        <div className="flex items-center gap-3 cursor-default">
          <div className="text-right hidden sm:block">
            <p className="text-xs font-semibold text-slate-900 leading-tight">{user?.username}</p>
            <p className="text-[10px] text-slate-500 uppercase font-bold tracking-wider">{user?.role}</p>
          </div>
          <div className="h-10 w-10 rounded-full bg-slate-100 flex items-center justify-center text-slate-600 border border-slate-200 shadow-sm">
            <User size={20} />
          </div>
        </div>
      </div>
    </header>
  );
};

export default Navbar;
