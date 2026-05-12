import { HashRouter, Routes, Route, Navigate } from "react-router-dom";
import './App.css';

import AppLayout from "./components/AppLayout";
import LoginPage from './components/LoginPage';
import ChatLayout from './components/ChatLayout';

//import { startConnection } from './signalr/chatConnection';

function App() {
  // const handleLogin = async (name) => {
  //   try {
  //     console.log("Name:", name);
  //     const res = await fetch(
  //       `https://sslk8rt0-7081.usw3.devtunnels.ms/api/testdm/seed-user?UserName=${name}&OverWrite=false`, {
  //       method: 'POST'
  //     });

  //     const data = await res.json();

  //     console.log("Login response:", data);

  //     // store token
  //     localStorage.setItem("access_token", data.token);

  //     setUsername(data.username);
  //     setIsLoggedIn(true);
  //     startConnection();

  //   } catch (err) {
  //     console.error("Login failed:", err);
  //   }
  // };

  return (
    <HashRouter>
      <Routes>
        <Route path="/" element={<LoginPage />} />
        <Route path="/" element={<AppLayout />}>
          <Route path="chat" element={<ChatLayout />} />
        </Route>
      </Routes>
    </HashRouter>
  )
}

export default App
