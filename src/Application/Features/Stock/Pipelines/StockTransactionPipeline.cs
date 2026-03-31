// Re-export from Interfaces.Pipelines for use within Application layer
// TransactionRequestContext and IStockTransactionValidationPipeline live in Interfaces.Pipelines
// to allow both Application and Infrastructure to use them without circular deps.
global using InventoryManagement.Interfaces.Pipelines;
