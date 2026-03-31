// Transaction types based on backend enum
export const TRANSACTION_TYPES = [
  { value: 0, label: 'Purchase' },
  { value: 1, label: 'Sale' },
  { value: 2, label: 'Adjustment' },
  { value: 3, label: 'Return' },
  { value: 4, label: 'Transfer' },
];

// Formatting helpers
export const formatCurrency = (amount) => {
  return new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD',
  }).format(amount || 0);
};
