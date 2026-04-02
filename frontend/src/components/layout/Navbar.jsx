import React, { useState, useEffect } from 'react';
import { useAuth } from '../../context/AuthContext';
import { useInventory } from '../../context/InventoryContext';
import { User, Bell, LogOut, ChevronDown, Shield } from 'lucide-react';
import apiClient from '../../api/apiClient';
import { useNavigate } from 'react-router-dom';

const Navbar = () => {
  const { user, logout, hasRole } = useAuth();
  const { lastUpdate } = useInventory();
  const [alertCount, setAlertCount] = useState(0);
  const [showDropdown, setShowDropdown] = useState(false);
  const navigate = useNavigate();

  useEffect(() => {
    const handleClickOutside = (event) => {
      if (showDropdown && !event.target.closest('.profile-dropdown')) {
        setShowDropdown(false);
      }
    };
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, [showDropdown]);

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
        
        <div className="relative profile-dropdown">
          <button 
            onClick={() => setShowDropdown(!showDropdown)}
            className="flex items-center gap-3 p-1.5 rounded-xl hover:bg-slate-50 transition-all duration-200 group"
          >
            <div className="text-right hidden sm:block">
              <p className="text-xs font-semibold text-slate-900 leading-tight">{user?.username}</p>
              <p className="text-[10px] text-slate-500 uppercase font-bold tracking-wider">{user?.role}</p>
            </div>
            <div className="h-10 w-10 rounded-full bg-blue-50 flex items-center justify-center text-blue-600 border border-blue-100 shadow-sm group-hover:bg-blue-600 group-hover:text-white group-hover:border-blue-600 transition-all">
              <User size={20} />
            </div>
            <ChevronDown size={16} className={`text-slate-400 transition-transform duration-200 ${showDropdown ? 'rotate-180' : ''}`} />
          </button>

          {showDropdown && (
            <div className="absolute right-0 mt-2 w-48 bg-white rounded-2xl shadow-xl border border-slate-100 py-2 z-50 animate-in fade-in slide-in-from-top-2 duration-200">
              <div className="px-4 py-2 border-b border-slate-50 mb-1">
                <p className="text-xs font-bold text-slate-400 uppercase tracking-widest">Account</p>
              </div>
              <button 
                className="w-full flex items-center gap-3 px-4 py-2.5 text-sm text-slate-600 hover:bg-slate-50 hover:text-blue-600 transition-colors"
                onClick={() => {
                  setShowDropdown(false);
                  navigate('/profile');
                }}
              >
                <User size={16} />
                <span>My Profile</span>
              </button>

              {hasRole('Admin') && (
                <button 
                  className="w-full flex items-center gap-3 px-4 py-2.5 text-sm text-slate-600 hover:bg-slate-50 hover:text-blue-600 transition-colors"
                  onClick={() => {
                    setShowDropdown(false);
                    navigate('/roles');
                  }}
                >
                  <Shield size={16} />
                  <span>Role Management</span>
                </button>
              )}

              <button 
                className="w-full flex items-center gap-3 px-4 py-2.5 text-sm text-red-600 hover:bg-red-50 transition-colors"
                onClick={() => {
                  setShowDropdown(false);
                  logout();
                }}
              >
                <LogOut size={16} />
                <span>Sign Out</span>
              </button>
            </div>
          )}
        </div>
      </div>
    </header>
  );
};

export default Navbar;
