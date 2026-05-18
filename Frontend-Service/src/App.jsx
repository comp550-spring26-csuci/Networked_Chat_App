import { useState } from "react";
import { Routes, Route, Navigate } from "react-router-dom";
import LoginPage from "./components/LoginPage";
import ChatLayout from "./components/ChatLayout"; 
import AppLayout from "./components/AppLayout"; 

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
      
      {/* Protected Route Group: 
        If the user is logged in, render AppLayout (the sidebar shell). 
        The nested routes below will render inside AppLayout's <Outlet />!
      */}
      <Route element={user ? <AppLayout /> : <Navigate to="/login" replace />}>
        
        {/* Render ChatLayout when the URL is /chat */}
        <Route 
          path="/chat" 
          element={<ChatLayout currentUser={user} />} 
        />

        {/* Temporary placeholder for the friends page to prevent blank screens */}
        <Route 
          path="/friends" 
          element={<div style={{ padding: "20px", color: "white" }}>Friends page coming soon!</div>} 
        />
        
      </Route>
      
      {/* Catch-all route: sends to chat if logged in, otherwise to login */}
      <Route 
        path="*" 
        element={<Navigate to={user ? "/chat" : "/login"} replace />} 
      />
    </Routes>
  );
}