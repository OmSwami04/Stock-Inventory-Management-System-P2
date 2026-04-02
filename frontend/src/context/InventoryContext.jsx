import React, { createContext, useContext, useEffect, useState } from 'react';
import * as signalR from '@microsoft/signalr';

const InventoryContext = createContext();

export const InventoryProvider = ({ children }) => {
  const [connection, setConnection] = useState(null);
  const [alerts, setAlerts] = useState([]);
  const [lastUpdate, setLastUpdate] = useState(null);

  useEffect(() => {
    const newConnection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5153/inventoryHub') // Replace with your backend URL if different
      .withAutomaticReconnect()
      .build();

    setConnection(newConnection);
  }, []);

  useEffect(() => {
    if (connection) {
      connection.start()
        .then(result => {
          console.log('Connected to SignalR InventoryHub');
          
          connection.on('ReceiveStockUpdate', (update) => {
            console.log('Stock Update Received:', update);
            setLastUpdate(update);
            // We can handle local state updates here if needed
          });
        })
        .catch(e => console.log('Connection failed: ', e));
    }
  }, [connection]);

  return (
    <InventoryContext.Provider value={{ lastUpdate, alerts, setAlerts }}>
      {children}
    </InventoryContext.Provider>
  );
};

export const useInventory = () => useContext(InventoryContext);
