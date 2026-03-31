import React from 'react';
import { useAuth } from '../../context/AuthContext';
import { User, Bell } from 'lucide-react';

const Navbar = () => {
  const { user } = useAuth();

  return (
    <header className="h-16 bg-white border-b border-slate-200 flex items-center justify-between px-8">
      <h1 className="text-xl font-semibold text-slate-800 tracking-tight">
        Inventory Management System
      </h1>
      
      <div className="flex items-center gap-6 text-slate-500">
        <button className="hover:text-blue-600 transition-colors">
          <Bell size={20} />
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
