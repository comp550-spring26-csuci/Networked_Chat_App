import { useState, useEffect } from 'react';
import './App.css'
import LoginPage from './components/LoginPage';
import ChatLayout from './components/ChatLayout'
import { startConnection } from './signalr/chatConnection';

function App() {
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const [username, setUsername] = useState("");
  // will need login persistance so that reloading doesnt log you out
  // will probably have to pass the username or other information as needed from the login to the ChatLayout for things like sender name on the messsages
  
  // useEffect(() => {
  //   if (isLoggedIn) {
  //     startConnection();
  //   }
  // }, [isLoggedIn]);

  const handleLogin = async (name) => {
    try {
      console.log("Name:", name);
      const res = await fetch(
        `https://localhost:7081/api/test/seed-user?UserName=${name}&OverWrite=false`, {
        method: 'POST'
      });

      const data = await res.json();

      console.log("Login response:", data);

      // 💾 store token
      localStorage.setItem("access_token", data.token);

      setUsername(data.username);
      setIsLoggedIn(true);
      startConnection();

    } catch (err) {
      console.error("Login failed:", err);
    }
  };

  return (
    <>
      {isLoggedIn ? (
        <ChatLayout username={username} />
      ) : (
        <LoginPage onLogin={handleLogin} />
      )}
    </>
  )
}

export default App
