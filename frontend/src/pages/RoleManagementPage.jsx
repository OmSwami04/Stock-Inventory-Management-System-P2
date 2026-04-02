import React, { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext';
import { Shield, Plus, Loader2, Trash2, AlertCircle, Info, CheckCircle2 } from 'lucide-react';
import apiClient from '../api/apiClient';

const RoleManagementPage = () => {
  const [roles, setRoles] = useState([]);
  const [loading, setLoading] = useState(true);
  const [newRole, setNewRole] = useState('');
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const { hasRole } = useAuth();

  const fetchRoles = async () => {
    try {
      const res = await apiClient.get('/Auth/roles');
      setRoles(res.data || []);
    } catch (err) {
      console.error(err);
      setError('Failed to fetch roles');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchRoles();
  }, []);

  const handleAddRole = async (e) => {
    e.preventDefault();
    if (!newRole.trim()) return;

    setSubmitting(true);
    setError('');
    setSuccess('');

    try {
      await apiClient.post('/Auth/roles', { roleName: newRole });
      setSuccess(`Role "${newRole}" added successfully!`);
      setNewRole('');
      fetchRoles();
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to add role');
    } finally {
      setSubmitting(false);
    }
  };

  if (!hasRole('Admin')) {
    return (
      <div className="flex flex-col items-center justify-center min-h-[400px] space-y-4">
        <div className="h-16 w-16 bg-red-100 rounded-2xl flex items-center justify-center text-red-600">
          <AlertCircle size={32} />
        </div>
        <h1 className="text-2xl font-bold text-slate-900">Access Denied</h1>
        <p className="text-slate-500">Only administrators can manage system roles.</p>
      </div>
    );
  }

  return (
    <div className="max-w-4xl mx-auto space-y-8">
      <div className="flex flex-col md:flex-row md:items-end gap-6 pb-8 border-b border-slate-200">
        <div className="h-16 w-16 bg-blue-600 rounded-2xl flex items-center justify-center text-white shadow-lg shadow-blue-200">
          <Shield size={32} />
        </div>
        <div className="space-y-1">
          <h1 className="text-3xl font-bold text-slate-900 tracking-tight">Role Management</h1>
          <p className="text-slate-500 font-medium">Define and manage system access levels</p>
        </div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
        <div className="md:col-span-2 space-y-6">
          <div className="bg-white rounded-3xl p-8 border border-slate-100 shadow-sm space-y-6">
            <h2 className="text-lg font-bold text-slate-900 flex items-center gap-2">
              Existing Roles
            </h2>
            
            <div className="space-y-4">
              {loading ? (
                <div className="flex items-center justify-center py-12">
                  <Loader2 className="animate-spin text-blue-600" size={32} />
                </div>
              ) : roles.length === 0 ? (
                <div className="text-center py-12 bg-slate-50 rounded-2xl border-2 border-dashed border-slate-200">
                  <p className="text-slate-400 font-medium">No roles found</p>
                </div>
              ) : (
                <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                  {roles.map((role) => (
                    <div key={role} className="flex items-center justify-between p-4 bg-slate-50 rounded-2xl border border-slate-100 hover:border-blue-200 hover:bg-white transition-all group">
                      <div className="flex items-center gap-3">
                        <div className="p-2 rounded-xl bg-white border border-slate-100 group-hover:bg-blue-50 group-hover:border-blue-100">
                          <Shield size={18} className="text-slate-400 group-hover:text-blue-600" />
                        </div>
                        <span className="font-bold text-slate-700">{role}</span>
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </div>
          </div>

          <div className="bg-blue-50 rounded-3xl p-6 flex gap-4 items-start border border-blue-100/50">
            <Info className="text-blue-600 shrink-0 mt-1" size={20} />
            <div className="space-y-1">
              <p className="text-sm font-bold text-blue-900">About Roles</p>
              <p className="text-sm text-blue-700 leading-relaxed">
                Roles define the permissions users have within the system. Default roles like Admin, InventoryManager, and InventoryClerk are seeded automatically.
              </p>
            </div>
          </div>
        </div>

        <div className="space-y-6">
          <div className="bg-white rounded-3xl p-8 border border-slate-100 shadow-sm space-y-6 sticky top-8">
            <h2 className="text-lg font-bold text-slate-900">Add New Role</h2>
            
            <form onSubmit={handleAddRole} className="space-y-6">
              <div className="space-y-2">
                <label className="text-xs font-bold text-slate-400 uppercase tracking-widest px-1">Role Name</label>
                <div className="relative group">
                  <Shield className="absolute left-4 top-1/2 -translate-y-1/2 text-slate-400 group-focus-within:text-blue-600 transition-colors" size={20} />
                  <input
                    type="text"
                    value={newRole}
                    onChange={(e) => setNewRole(e.target.value)}
                    className="w-full pl-12 pr-4 py-3.5 bg-slate-50 border border-slate-200 rounded-2xl focus:outline-none focus:ring-4 focus:ring-blue-50 focus:border-blue-600 transition-all text-slate-900 font-medium"
                    placeholder="e.g. Supervisor"
                    required
                  />
                </div>
              </div>

              {error && (
                <div className="flex gap-2 items-center text-red-600 bg-red-50 p-4 rounded-2xl text-sm font-medium border border-red-100">
                  <AlertCircle size={18} className="shrink-0" />
                  {error}
                </div>
              )}

              {success && (
                <div className="flex gap-2 items-center text-emerald-600 bg-emerald-50 p-4 rounded-2xl text-sm font-medium border border-emerald-100">
                  <CheckCircle2 size={18} className="shrink-0" />
                  {success}
                </div>
              )}

              <button
                type="submit"
                disabled={submitting || !newRole.trim()}
                className="w-full bg-slate-900 hover:bg-slate-800 text-white font-bold py-4 rounded-2xl shadow-lg shadow-slate-200 transition-all flex items-center justify-center gap-2 group disabled:opacity-50 disabled:hover:bg-slate-900"
              >
                {submitting ? (
                  <Loader2 className="animate-spin" size={20} />
                ) : (
                  <>
                    <Plus size={20} />
                    Create Role
                  </>
                )}
              </button>
            </form>
          </div>
        </div>
      </div>
    </div>
  );
};

export default RoleManagementPage;
