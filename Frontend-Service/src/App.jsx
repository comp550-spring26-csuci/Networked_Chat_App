import { useState } from "react";
import { Routes, Route, Navigate } from "react-router-dom";
import LoginPage from "./components/LoginPage";
import ChatWindow from "./components/ChatWindow";
import FriendsList from "./components/FriendsList";

export default function App() {
  const [user, setUser] = useState(null);
  // user shape: { username, token, userId }

  return (
    <Routes>
      <Route path="/login" element={<LoginPage onLogin={setUser} />} />
      <Route path="/message" element={user ? <ChatWindow user={user} onLogout={() => setUser(null)} /> : <Navigate to="/login" />} />
      <Route path="/friends" element={user ? <FriendsList currentUser={user} /> : <Navigate to="/login" />} />
      <Route path="*" element={<Navigate to="/login" />} />
    </Routes>
  );
}