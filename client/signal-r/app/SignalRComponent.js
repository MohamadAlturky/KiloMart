// // // import React, { useEffect } from "react";
// // // import * as signalR from "@microsoft/signalr";

// // // const SignalRComponent = ({ bearerToken, url }) => {
// // //   useEffect(() => {
// // //     let connection;

// // //     const startConnection = async () => {
// // //       try {
// // //         connection = new signalR.HubConnectionBuilder()
// // //           .withUrl(url, {
// // //             accessTokenFactory: () => bearerToken,
// // //           })
// // //           .withAutomaticReconnect()
// // //           .configureLogging(signalR.LogLevel.Information)
// // //           .build();

// // //         connection.on("ReceiveNotification", (message) => {
// // //           console.log("Notification received:", message);
// // //           // Handle the notification (e.g., update state or show a toast)
// // //         });

// // //         await connection.start();
// // //         console.log("SignalR connection established");
// // //       } catch (err) {
// // //         console.error("SignalR connection failed:", err);
// // //       }
// // //     };

// // //     startConnection();

// // //     return () => {
// // //       if (connection) {
// // //         connection.stop();
// // //         console.log("SignalR connection stopped");
// // //       }
// // //     };
// // //   }, [bearerToken, url]);

// // //   return null; // This component does not render any UI
// // // };

// // // export default SignalRComponent;
// // "use client"
// // import React, { useEffect, useState } from "react";
// // import * as signalR from "@microsoft/signalr";

// // const SignalRComponent = ({ bearerToken, url }) => {
// //   const [notifications, setNotifications] = useState([]);

// //   useEffect(() => {
// //     let connection;

// //     const startConnection = async () => {
// //       try {
// //         connection = new signalR.HubConnectionBuilder()
// //           .withUrl(url, {
// //             accessTokenFactory: () => bearerToken,
// //           })
// //           .withAutomaticReconnect()
// //           .configureLogging(signalR.LogLevel.Information)
// //           .build();

// //         connection.on("ReceiveNotification", (message) => {
// //           console.log("Notification received:", message);
// //           // Update notifications state with the new message
// //           setNotifications((prevNotifications) => [
// //             { id: Date.now(), message:message.message }, // Add a unique id based on timestamp
// //             ...prevNotifications,
// //           ]);
// //         });

// //         await connection.start();
// //         console.log("SignalR connection established");
// //       } catch (err) {
// //         console.error("SignalR connection failed:", err);
// //       }
// //     };

// //     startConnection();

// //     return () => {
// //       if (connection) {
// //         connection.stop();
// //         console.log("SignalR connection stopped");
// //       }
// //     };
// //   }, [bearerToken, url]);

// //   return (
// //     <div className="absolute top-0 right-0 p-4 w-80">
// //       <h2 className="text-lg font-bold mb-2">Notifications</h2>
// //       <div className="bg-white border rounded shadow-md max-h-60 overflow-y-auto">
// //         <ul>
// //           {notifications.map((notification) => (
// //             <li key={notification.id} className="p-2 border-b last:border-b-0">
// //               {notification.message}
// //             </li>
// //           ))}
// //         </ul>
// //       </div>
// //     </div>
// //   );
// // };

// // export default SignalRComponent;

// "use client";
// import React, { useEffect, useState } from "react";
// import * as signalR from "@microsoft/signalr";

// const SignalRComponent = ({ bearerToken, url }) => {
//   const [notifications, setNotifications] = useState([]);

//   useEffect(() => {
//     let connection;

//     const startConnection = async () => {
//       try {
//         connection = new signalR.HubConnectionBuilder()
//           .withUrl(url, {
//             accessTokenFactory: () => bearerToken,
//           })
//           .withAutomaticReconnect()
//           .configureLogging(signalR.LogLevel.Information)
//           .build();

//         connection.on("ReceiveNotification", (message) => {
//           console.log("Notification received:", message);
//           // Update notifications state with the new message
//           setNotifications((prevNotifications) => [
//             { id: Date.now(), message: message.message }, // Add a unique id based on timestamp
//             ...prevNotifications,
//           ]);
//         });

//         await connection.start();
//         console.log("SignalR connection established");
//       } catch (err) {
//         console.error("SignalR connection failed:", err);
//       }
//     };

//     startConnection();

//     return () => {
//       if (connection) {
//         connection.stop();
//         console.log("SignalR connection stopped");
//       }
//     };
//   }, [bearerToken, url]);

//   return (
//     <div className="fixed top-0 right-0 p-4 w-80 z-50">
//       <h2 className="text-lg font-bold mb-2 text-gray-800">Notifications</h2>
//       <div className="bg-white border border-gray-300 rounded-lg shadow-lg max-h-60 overflow-y-auto">
//         <ul>
//           {notifications.length === 0 ? (
//             <li className="p-4 text-gray-500">No notifications yet.</li>
//           ) : (
//             notifications.map((notification) => (
//               <li key={notification.id} className="p-2 border-b border-gray-200 last:border-b-0 hover:bg-gray-100 transition duration-200">
//                 {notification.message}
//               </li>
//             ))
//           )}
//         </ul>
//       </div>
//     </div>
//   );
// };

// export default SignalRComponent;
"use client";
import React, { useEffect, useState } from "react";
import * as signalR from "@microsoft/signalr";

const SignalRComponent = ({ bearerToken, url }) => {
  const [notifications, setNotifications] = useState([]);

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
          // Update notifications state with the new message
          setNotifications((prevNotifications) => [
            { id: Date.now(), message: message.message }, // Add a unique id based on timestamp
            ...prevNotifications,
          ]);
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

  return (
    <div className="h-screen">
      <h2 className="text-lg font-bold mb-2 text-black text-center">Notifications</h2>
      <div className="bg-white border border-gray-300 rounded-lg shadow-lg max-h-60 overflow-y-auto">
        <ul className="text-center"> {/* Centering notifications */}
          {notifications.length === 0 ? (
            <li className="p-4 text-gray-500">No notifications yet.</li>
          ) : (
            notifications.map((notification) => (
              <li key={notification.id} className="p-2 border-b border-gray-200 last:border-b-0 hover:bg-gray-100 transition duration-200 text-black">
                {notification.message}
              </li>
            ))
          )}
        </ul>
      </div>
    </div>
  );
};

export default SignalRComponent;