import { HashRouter, Routes, Route, Navigate } from "react-router-dom";
import './App.css';

import AppLayout from "./components/AppLayout";
import LoginPage from './components/LoginPage';
import ChatLayout from './components/ChatLayout';
import FriendsList from './components/FriendsList';

function App() {
  // App only contains the routes but will not handle routing
  return (
    <HashRouter>
      <Routes>
        <Route path="/login" element={<LoginPage />} />
        <Route path="/" element={<AppLayout />}>
          <Route path="chat" element={<ChatLayout />} />
        </Route>
      </Routes>
    </HashRouter>
  )
}

export default App
