import React, { useEffect } from "react";
import * as signalR from "@microsoft/signalr";

const SignalRComponent = ({ bearerToken, url }) => {
  useEffect(() => {
    let connection;

    const startConnection = async () => {
      try {
        connection = new signalR.HubConnectionBuilder()
          .withUrl(url, {
            accessTokenFactory: () => bearerToken,
          })
          .withAutomaticReconnect()
          .configureLogging(signalR.LogLevel.Information)
          .build();

        connection.on("ReceiveNotification", (message) => {
          console.log("Notification received:", message);
          // Handle the notification (e.g., update state or show a toast)
        });

        await connection.start();
        console.log("SignalR connection established");
      } catch (err) {
        console.error("SignalR connection failed:", err);
      }
    };

    startConnection();

    return () => {
      if (connection) {
        connection.stop();
        console.log("SignalR connection stopped");
      }
    };
  }, [bearerToken, url]);

  return null; // This component does not render any UI
};

export default SignalRComponent;
