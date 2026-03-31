import React from 'react';
import { Loader2 } from 'lucide-react';

const Loader = ({ message = 'Loading...' }) => {
  return (
    <div className="flex flex-col items-center justify-center min-h-[400px] gap-4">
      <div className="relative">
        <div className="h-12 w-12 rounded-full border-4 border-slate-100 border-t-blue-600 animate-spin" />
        <Loader2 className="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 text-blue-600 animate-pulse" size={20} />
      </div>
      <p className="text-sm font-black text-slate-400 uppercase tracking-widest">{message}</p>
    </div>
  );
};

export default Loader;
