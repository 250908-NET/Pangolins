// src/contexts/SignalRContext.tsx

import { createContext, useState, useContext } from 'react';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';

interface SignalRContextType {
  connection: HubConnection | null;
  connectToHub: (hubPath: string) => Promise<void>;
  disconnect: () => Promise<void>;
}

const SignalRContext = createContext<SignalRContextType | undefined>(undefined);

export function SignalRProvider({ children }: { children: React.ReactNode }) {
  const [connection, setConnection] = useState<HubConnection | null>(null);

  const connectToHub = async (hubPath: string) => {
    // Avoid reconnecting if already connected
    if (connection?.state === 'Connected') {
      return;
    }

    try {
      const hubUrl = `${import.meta.env.VITE_HUB_URL}${hubPath}`;

      const newConnection = new HubConnectionBuilder()
        .withUrl(hubUrl, {
          // Provide the JWT token for authorization
          accessTokenFactory: () => localStorage.getItem('token') || '',
        })
        .withAutomaticReconnect()
        .configureLogging(LogLevel.Information)
        .build();

      await newConnection.start();
      console.log('SignalR Connected.');
      setConnection(newConnection);
    } catch (e) {
      console.error('SignalR Connection Error: ', e);
    }
  };

  const disconnect = async () => {
    if (connection) {
      await connection.stop();
      setConnection(null);
      console.log('SignalR Disconnected.');
    }
  };

  const value = { connection, connectToHub, disconnect };

  return (
    <SignalRContext.Provider value={value}>{children}</SignalRContext.Provider>
  );
}

export function useSignalR() {
  const context = useContext(SignalRContext);
  if (context === undefined) {
    throw new Error('useSignalR must be used within a SignalRProvider');
  }
  return context;
}