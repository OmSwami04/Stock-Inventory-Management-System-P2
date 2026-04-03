# Stock-Inventory-Management-System-P2

Stock-Inventory-Management-System-P2 is a robust, real-time inventory and warehouse management solution designed for modern businesses. It provides a comprehensive suite of tools to track stock levels, manage multi-warehouse distributions, and generate detailed financial reports.

## 🚀 Key Features

### **Real-Time Dashboard**
- **Dynamic Stats**: Track total products, inventory valuation (in ₹), low stock alerts, and warehouse counts at a glance.
- **Stock Distribution**: Visualize inventory levels across all warehouses using interactive bar charts.
- **Live Updates**: Instant UI synchronization via **SignalR** whenever stock movements occur.

### **Inventory & Stock Management**
- **Internal Transfers**: Seamlessly move stock between warehouses with automated transaction logging.
- **Product Catalog**: Manage detailed product information including SKU, categories, and safety stock levels.
- **Stock History**: Track every transaction (Purchase, Sale, Transfer) with a dedicated history view.

### **Advanced Reporting**
- **PDF Generation**: Download professionally formatted reports for Inventory Valuation and Low Stock Alerts using `jspdf`.
- **Valuation Logic**: Automatic calculation of asset value based on purchase costs and current quantities.

### **Performance & Scalability**
- **Redis Caching**: High-performance data retrieval using distributed caching for product details.
- **CQRS Pattern**: Clean architectural separation between data modification (Commands) and data retrieval (Queries).
- **Validation Pipeline**: Robust chain-of-responsibility pattern for validating complex business rules.

---

## 🛠️ Tech Stack

**Backend:**
- .NET 10 (Web API)
- Entity Framework Core (MySQL)
- SignalR (Real-time notifications)
- StackExchange.Redis (Caching)
- Moq & xUnit (Unit Testing)

**Frontend:**
- React 18 + Vite
- Tailwind CSS (Styling)
- Recharts (Data Visualization)
- Lucide React (Icons)
- Axios (API Communication)

---

## ⚙️ Getting Started

### **Prerequisites**
- .NET SDK (v8.0+)
- Node.js (v18+)
- MySQL Server
- Docker (for Redis)

### **Backend Setup**
1. Navigate to the API directory:
   ```bash
   cd src/Api
   ```
2. Update the connection string in `appsettings.json`.
3. Start the Redis container:
   ```bash
   docker-compose up -d redis
   ```
4. Run migrations and start the server:
   ```bash
   dotnet run
   ```

### **Frontend Setup**
1. Navigate to the frontend directory:
   ```bash
   cd frontend
   ```
2. Install dependencies:
   ```bash
   npm install
   ```
3. Start the development server:
   ```bash
   npm run dev
   ```

---

## 🧪 Testing
The project includes a comprehensive suite of **23 unit tests** covering all core business logic.

To run the tests:
```powershell
cd tests/InventoryManagement.Tests
dotnet test
```

---

## 👤 Author
**Om Swami**
- GitHub: [@OmSwami04](https://github.com/OmSwami04)
- Repository: [Stock-Inventory-Management-System-P2](https://github.com/OmSwami04/Stock-Inventory-Management-System-P2)
