import { useState } from 'react';
import './App.css'
import LoginPage from './components/LoginPage';
import ChatLayout from './components/ChatLayout'

function App() {
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const [username, setUsername] = useState("");
  // will need login persistance so that reloading doesnt log you out
  // will probably have to pass the username or other information as needed from the login to the ChatLayout for things like sender name on the messsages
  const handleLogin = (name) => {
    setUsername(name);
    setIsLoggedIn(true);
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
