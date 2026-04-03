import React, { useState, useEffect } from 'react';
import apiClient from '../api/apiClient';
import { FileText, Download, BarChart3, TrendingUp, AlertTriangle, Warehouse } from 'lucide-react';
import jsPDF from 'jspdf';
import autoTable from 'jspdf-autotable';
import Loader from '../components/Loader';

const ReportsPage = () => {
  const [loading, setLoading] = useState(true);
  const [summaryData, setSummaryData] = useState({
    inventoryValuation: [],
    lowStockItems: [],
    recentTransactions: [],
    warehouseDistribution: []
  });

  const fetchData = async () => {
    try {
      setLoading(true);
      const [valuationRes, stockRes, transactionsRes, warehousesRes, productsRes] = await Promise.all([
        apiClient.get('/Inventory/valuation'),
        apiClient.get('/Stock'),
        apiClient.get('/Stock/transactions'),
        apiClient.get('/Warehouses'),
        apiClient.get('/Products?pageSize=1000')
      ]);

      const stockData = stockRes.data || [];
      const products = productsRes.data.items || [];
      const lowStock = stockData.filter(s => s.quantityOnHand <= s.reorderLevel);

      // Create a map of products for easy lookup
      const productMap = products.reduce((acc, p) => {
        acc[p.productId] = p;
        return acc;
      }, {});

      // Enrich stock data with SKU and Unit Cost for the report
      const enrichedStockData = stockData.map(s => {
        const product = productMap[s.productId];
        return {
          ...s,
          sku: product?.sku || 'N/A',
          unitCost: product?.cost || 0,
          totalValue: (product?.cost || 0) * s.quantityOnHand
        };
      });

      // Warehouse distribution
      const distribution = stockData.reduce((acc, curr) => {
        const warehouse = curr.warehouseName || 'Unknown';
        if (!acc[warehouse]) acc[warehouse] = 0;
        acc[warehouse] += curr.quantityOnHand;
        return acc;
      }, {});

      setSummaryData({
        inventoryValuation: enrichedStockData,
        totalValue: valuationRes.data.totalInventoryValue || 0,
        lowStockItems: lowStock,
        recentTransactions: (transactionsRes.data || []).slice(0, 10),
        warehouseDistribution: Object.entries(distribution).map(([name, count]) => ({ name, count }))
      });
    } catch (err) {
      console.error('Error fetching report data:', err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, []);

  const downloadInventoryReport = () => {
    const doc = new jsPDF();
    const tableColumn = ["Product Name", "SKU", "Warehouse", "Quantity", "Unit Cost", "Total Value"];
    const tableRows = [];

    summaryData.inventoryValuation.forEach(item => {
      const rowData = [
        item.productName,
        item.sku,
        item.warehouseName,
        item.quantityOnHand,
        `₹${item.unitCost.toLocaleString()}`,
        `₹${item.totalValue.toLocaleString()}`
      ];
      tableRows.push(rowData);
    });

    doc.setFontSize(18);
    doc.text("Inventory Valuation Report", 14, 22);
    doc.setFontSize(11);
    doc.setTextColor(100);
    doc.text(`Generated on: ${new Date().toLocaleString()}`, 14, 30);
    doc.text(`Total Inventory Value: ₹${summaryData.totalValue.toLocaleString()}`, 14, 38);

    autoTable(doc, {
      head: [tableColumn],
      body: tableRows,
      startY: 45,
      theme: 'striped',
      headStyles: { fillColor: [59, 130, 246] } // Corrected: Ensured replacement is effective
    });

    doc.save(`inventory_report_${new Date().toISOString().split('T')[0]}.pdf`);
  };

  const downloadLowStockReport = () => {
    const doc = new jsPDF();
    const tableColumn = ["Product Name", "Warehouse", "On Hand", "Reorder Level"];
    const tableRows = [];

    summaryData.lowStockItems.forEach(item => {
      const rowData = [
        item.productName,
        item.warehouseName,
        item.quantityOnHand,
        item.reorderLevel
      ];
      tableRows.push(rowData);
    });

    doc.setFontSize(18);
    doc.text("Low Stock Alert Report", 14, 22);
    doc.setFontSize(11);
    doc.text(`Generated on: ${new Date().toLocaleString()}`, 14, 30);

    autoTable(doc, {
      head: [tableColumn],
      body: tableRows,
      startY: 40,
      theme: 'grid',
      headStyles: { fillColor: [239, 68, 68] } // Red color for low stock alerts
    });

    doc.save(`low_stock_report_${new Date().toISOString().split('T')[0]}.pdf`);
  };

  if (loading) return <Loader />;

  return (
    <div className="max-w-6xl mx-auto space-y-8 animate-in fade-in duration-500">
      <div>
        <h2 className="text-3xl font-extrabold text-slate-900 tracking-tight">Application Reports</h2>
        <p className="text-slate-500 font-medium mt-1">Generate and download detailed inventory insights</p>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        {/* Inventory Summary Card */}
        <div className="bg-white p-8 rounded-3xl shadow-sm border border-slate-100 flex flex-col justify-between">
          <div>
            <div className="flex items-center gap-3 mb-4">
              <div className="p-3 rounded-2xl bg-blue-50 text-blue-600">
                <TrendingUp size={24} />
              </div>
              <h3 className="text-xl font-bold text-slate-900">Inventory Valuation</h3>
            </div>
            <p className="text-slate-500 text-sm leading-relaxed mb-6">
              A comprehensive breakdown of all products across warehouses, including unit costs and total valuation.
            </p>
            <div className="text-2xl font-black text-slate-900 mb-2">
              ₹{summaryData.totalValue.toLocaleString()}
            </div>
            <p className="text-xs font-bold text-slate-400 uppercase tracking-widest">Total Asset Value</p>
          </div>
          <button
            onClick={downloadInventoryReport}
            className="mt-8 flex items-center justify-center gap-2 w-full py-3 bg-blue-600 hover:bg-blue-700 text-white font-bold rounded-xl shadow-lg shadow-blue-100 transition-all"
          >
            <Download size={18} />
            Download PDF Report
          </button>
        </div>

        {/* Low Stock Alerts Card */}
        <div className="bg-white p-8 rounded-3xl shadow-sm border border-slate-100 flex flex-col justify-between">
          <div>
            <div className="flex items-center gap-3 mb-4">
              <div className="p-3 rounded-2xl bg-red-50 text-red-600">
                <AlertTriangle size={24} />
              </div>
              <h3 className="text-xl font-bold text-slate-900">Low Stock Alerts</h3>
            </div>
            <p className="text-slate-500 text-sm leading-relaxed mb-6">
              Identify items that are at or below their defined reorder levels and require immediate attention.
            </p>
            <div className="text-2xl font-black text-red-600 mb-2">
              {summaryData.lowStockItems.length}
            </div>
            <p className="text-xs font-bold text-slate-400 uppercase tracking-widest">Items Requiring Reorder</p>
          </div>
          <button
            onClick={downloadLowStockReport}
            className="mt-8 flex items-center justify-center gap-2 w-full py-3 bg-red-600 hover:bg-red-700 text-white font-bold rounded-xl shadow-lg shadow-red-100 transition-all"
          >
            <Download size={18} />
            Download Alert List (PDF)
          </button>
        </div>
      </div>

      {/* Distribution Summary */}
      <div className="bg-white p-8 rounded-3xl shadow-sm border border-slate-100">
        <div className="flex items-center gap-3 mb-8">
          <div className="p-3 rounded-2xl bg-amber-50 text-amber-600">
            <Warehouse size={24} />
          </div>
          <h3 className="text-xl font-bold text-slate-900">Stock Distribution by Warehouse</h3>
        </div>
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
          {summaryData.warehouseDistribution.map((w, idx) => (
            <div key={idx} className="p-4 bg-slate-50 rounded-2xl border border-slate-100">
              <p className="text-xs font-bold text-slate-400 uppercase tracking-widest mb-1">{w.name}</p>
              <p className="text-xl font-black text-slate-900">{w.count.toLocaleString()} Units</p>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
};

export default ReportsPage;
