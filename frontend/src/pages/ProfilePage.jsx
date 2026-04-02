import React from 'react';
import { useAuth } from '../context/AuthContext';
import { User, Shield, Fingerprint, Mail, Calendar } from 'lucide-react';

const ProfilePage = () => {
  const { user } = useAuth();

  const profileFields = [
    { label: 'Username', value: user?.username, icon: <User size={20} className="text-blue-500" /> },
    { label: 'Role', value: user?.role, icon: <Shield size={20} className="text-emerald-500" /> },
    { label: 'User ID', value: user?.id, icon: <Fingerprint size={20} className="text-slate-500" /> },
  ];

  return (
    <div className="max-w-4xl mx-auto space-y-8">
      <div className="flex flex-col md:flex-row md:items-end gap-6 pb-8 border-b border-slate-200">
        <div className="h-24 w-24 rounded-3xl bg-blue-600 flex items-center justify-center text-white text-4xl font-bold shadow-lg shadow-blue-200 border-4 border-white">
          {user?.username?.charAt(0).toUpperCase()}
        </div>
        <div className="space-y-1">
          <h1 className="text-3xl font-bold text-slate-900 tracking-tight">{user?.username}</h1>
          <p className="text-slate-500 font-medium flex items-center gap-2">
            <span className="px-2.5 py-0.5 rounded-full bg-slate-100 text-slate-600 text-xs font-bold uppercase tracking-wider">
              {user?.role}
            </span>
          </p>
        </div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        <div className="bg-white rounded-3xl p-8 border border-slate-100 shadow-sm space-y-6">
          <h2 className="text-lg font-bold text-slate-900 flex items-center gap-2">
            Account Details
          </h2>
          
          <div className="space-y-6">
            {profileFields.map((field, idx) => (
              <div key={idx} className="flex items-start gap-4">
                <div className="p-2.5 rounded-xl bg-slate-50 border border-slate-100">
                  {field.icon}
                </div>
                <div className="space-y-1">
                  <p className="text-xs font-bold text-slate-400 uppercase tracking-widest">{field.label}</p>
                  <p className="text-slate-900 font-semibold break-all">{field.value}</p>
                </div>
              </div>
            ))}
          </div>
        </div>

        <div className="bg-gradient-to-br from-blue-600 to-blue-700 rounded-3xl p-8 text-white shadow-xl shadow-blue-100 space-y-6 relative overflow-hidden">
          <div className="relative z-10 space-y-6">
            <h2 className="text-lg font-bold flex items-center gap-2">
              Access Permissions
            </h2>
            <p className="text-blue-100 leading-relaxed font-medium">
              You are currently logged in as an <span className="text-white font-bold underline underline-offset-4 decoration-blue-400">{user?.role}</span>. 
              This gives you full access to manage products, stock movements, and system alerts.
            </p>
            <div className="pt-4 flex flex-wrap gap-2">
              {['Inventory Management', 'Reporting', 'User Admin'].map((perm, i) => (
                <span key={i} className="px-3 py-1.5 rounded-xl bg-white/10 border border-white/10 text-xs font-bold backdrop-blur-sm">
                  {perm}
                </span>
              ))}
            </div>
          </div>
          
          {/* Decorative background circle */}
          <div className="absolute -bottom-12 -right-12 w-48 h-48 bg-white/10 rounded-full blur-3xl" />
        </div>
      </div>
    </div>
  );
};

export default ProfilePage;
