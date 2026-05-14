import { useState } from "react";
import { Routes, Route, Navigate } from "react-router-dom";
import LoginPage from "./components/LoginPage";
import ChatLayout from "./components/ChatLayout"; 

export default function App() {
  const [user, setUser] = useState(null);
  // user shape: { username, token, userId }

  return (
    <Routes>
      {/* If a user exists, redirect them away from the login page to /chat */}
      <Route 
        path="/login" 
        element={user ? <Navigate to="/chat" replace /> : <LoginPage onLogin={setUser} />} 
      />
      
      {/* Protect the chat route: loads the main layout which contains the friends drawer */}
      {/* FIX: Pass the entire 'user' object as 'currentUser' instead of just the username */}
      <Route 
        path="/chat" 
        element={user ? <ChatLayout currentUser={user} /> : <Navigate to="/login" replace />} 
      />
      
      {/* Catch-all route: sends to chat if logged in, otherwise to login */}
      <Route 
        path="*" 
        element={<Navigate to={user ? "/chat" : "/login"} replace />} 
      />
    </Routes>
  );
}