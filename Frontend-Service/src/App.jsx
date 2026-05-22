import { HashRouter, Routes, Route, Navigate } from "react-router-dom";
import './App.css';

import AppLayout from "./components/AppLayout";
import LoginPage from './components/LoginPage';
import ChatLayout from './components/ChatLayout';

//import { startConnection } from './signalr/chatConnection';

function App() {
  return (
    <HashRouter>
      <Routes>
        {/* redirect root */}
        <Route path="/" element={<Navigate to="/login" replace />} />

        {/* login page */}
        <Route path="/login" element={<LoginPage />} />
        
        {/* authenticated app */}
        <Route path="/" element={<AppLayout />}>
          <Route path="chat" element={<ChatLayout />} />
        </Route>
      </Routes>
    </HashRouter>
  )
}

export default App
