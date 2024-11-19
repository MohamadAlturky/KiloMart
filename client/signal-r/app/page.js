"use client"
import React from 'react';
import SignalRComponent from './SignalRComponent';

export default function App () 
{
  const bearerToken = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VySWQiOiIyIiwiRW1haWwiOiJtNzk5Njc4QGdtYWlsLmNvbSIsIlJvbGUiOiIxIiwiUGFydHkiOiIyIiwiZXhwIjoxNzM0NDE4OTE2LCJpc3MiOiJLaWxvTWFydCIsImF1ZCI6IktpbG9NYXJ0In0.5FzWn8Vi91oajFZtomYitDXdFpmHMi61yYVJ1QUUwTE';
  const url = 'http://localhost:5017/notificationHub';

  return (
    <SignalRComponent bearerToken={bearerToken} url={url} />
  );
};