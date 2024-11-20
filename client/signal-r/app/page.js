"use client";
import React, { useState } from 'react';
import SignalRComponent from './SignalRComponent';

export default function App() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [bearerToken, setBearerToken] = useState('');
  const url = 'http://localhost:5017/notificationHub';

  const handleLogin = async (e) => {
    e.preventDefault();

    try {
      const response = await fetch('http://localhost:5017/api/user/login', {
        method: 'POST',
        headers: {
          'Accept': '*/*',
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ email, password }),
      });

      const data = await response.json();

      if (data.success) {
        setBearerToken(data.token);
      } else {
        console.error('Login failed:', data);
      }
    } catch (error) {
      console.error('Error during login:', error);
    }
  };

  return (
    <>

      {bearerToken ? (
        <SignalRComponent bearerToken={bearerToken} url={url} />
      ) : (

        <div className="flex flex-col items-center justify-center min-h-screen bg-gray-100">

          <form onSubmit={handleLogin} className="bg-white p-6 rounded shadow-md w-80">
            <h2 className="text-lg font-bold mb-4">Login</h2>
            <div className="mb-4">
              <label htmlFor="email" className="block text-sm font-medium text-gray-700">Email</label>
              <input
                type="email"
                id="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                required
                className="mt-1 block w-full border border-gray-300 rounded-md p-2"
              />
            </div>
            <div className="mb-4">
              <label htmlFor="password" className="block text-sm font-medium text-gray-700">Password</label>
              <input
                type="password"
                id="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                required
                className="mt-1 block w-full border border-gray-300 rounded-md p-2"
              />
            </div>
            <button
              type="submit"
              className="w-full bg-blue-500 text-black p-2 rounded hover:bg-blue-600 transition duration-200"
            >
              Login
            </button>
          </form>


        </div>

      )}
    </>
  );
}