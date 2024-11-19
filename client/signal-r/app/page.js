"use client"
import React from 'react';
import SignalRComponent from './SignalRComponent';

export default function App () 
{
  const bearerToken = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VySWQiOiIyIiwiRW1haWwiOiJtNzk5Njc4QGdtYWlsLmNvbSIsIlJvbGUiOiIxIiwiUGFydHkiOiIyIiwiZXhwIjoxNzM0NDY1MTc2LCJpc3MiOiJLaWxvTWFydCIsImF1ZCI6IktpbG9NYXJ0In0.durzP3fMcGpNTJkLyE9KLnyMrz0tR0WCYsB3261pDeU';
  const url = 'http://localhost:5017/notificationHub';

  return (
    <SignalRComponent bearerToken={bearerToken} url={url} />
  );
};