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
        <Route path="/" element={<LoginPage />} />
        <Route path="/" element={<AppLayout />}>
          <Route path="chat" element={<ChatLayout />} />
        </Route>
      </Routes>
    </HashRouter>
  )
}

export default App
