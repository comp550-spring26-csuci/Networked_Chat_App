import { useState } from "react";
import { Routes, Route, Navigate } from "react-router-dom";
import LoginPage from "./components/LoginPage";
import FriendsList from "./components/FriendsList";

export default function App() {
  const [user, setUser] = useState(null);
  // user shape: { username, token, userId }

  return (
    <Routes>
      {/* If a user exists, redirect them away from the login page to /friends */}
      <Route 
        path="/login" 
        element={user ? <Navigate to="/friends" replace /> : <LoginPage onLogin={setUser} />} 
      />
      
      {/* if no user exists, redirect back to /login */}
      <Route 
        path="/friends" 
        element={user ? <FriendsList currentUser={user} /> : <Navigate to="/login" replace />} 
      />
      
      {/* sends to friends if logged in, otherwise to login */}
      <Route 
        path="*" 
        element={<Navigate to={user ? "/friends" : "/login"} replace />} 
      />
    </Routes>
  );
}