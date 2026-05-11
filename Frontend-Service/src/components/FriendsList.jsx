import { useState } from "react";
import "./FriendsList.css";

const BASE_URL = "http://vg3jzw0g-7081.usw3.devtunnels.ms";

const PLACEHOLDER_FRIENDS = [
  { id: "1", username: "kenneth", status: "Online" },
  { id: "2", username: "ivana", status: "Offline" },
  { id: "3", username: "ian", status: "Online" },
];

export default function FriendsList({ currentUser, onBack }) {
  const [friends, setFriends] = useState(PLACEHOLDER_FRIENDS);
  const [addInput, setAddInput] = useState("");
  const [message, setMessage] = useState(""); // Combines error/success

  const handleAdd = async (e) => {
    e.preventDefault();
    const username = addInput.trim();
    if (!username) return;
    
    if (username === currentUser) return setMessage("You can't add yourself.");
    if (friends.some(f => f.username.toLowerCase() === username.toLowerCase())) {
      return setMessage("This user is already your friend.");
    }

    try {
      const res = await fetch(`${BASE_URL}/api/PLACEHOLDER/friends/add`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ username: currentUser, friendUsername: username }),
      });
      const data = await res.json();
      
      if (res.ok) {
        setFriends([...friends, { id: data.friendId || Date.now().toString(), username, status: "Offline" }]);
        setMessage(`${username} added!`);
        setAddInput("");
      } else {
        setMessage(data.message || "User not found.");
      }
    } catch {
      // Local fallback
      setFriends([...friends, { id: Date.now().toString(), username, status: "Offline" }]);
      setMessage(`${username} added! (Offline mode)`);
      setAddInput("");
    }
  };

  const handleRemove = async (id, username) => {
    try {
      await fetch(`${BASE_URL}/api/PLACEHOLDER/friends/remove`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ username: currentUser, friendUsername: username }),
      });
      // Removes locally regardless of success for the fallback
      setFriends(friends.filter(f => f.id !== id));
    } catch {
      setFriends(friends.filter(f => f.id !== id));
    }
  };

  const online = friends.filter(f => f.status === "Online");
  const offline = friends.filter(f => f.status !== "Online");

  const renderSection = (title, list) => {
    if (list.length === 0) return null;
    return (
      <div className="fl-section">
        <div className="fl-section-title">{title} — {list.length}</div>
        {list.map(friend => (
          <div key={friend.id} className="fl-item">
            <div className="fl-info">
              <span className={`fl-status-dot ${friend.status.toLowerCase()}`}></span>
              <span className="fl-username">{friend.username}</span>
            </div>
            <button className="fl-remove-btn" onClick={() => handleRemove(friend.id, friend.username)} title="Remove">
              ✕
            </button>
          </div>
        ))}
      </div>
    );
  };

  return (
    <div className="fl-container">
      <div className="fl-header">
        <button className="fl-back-btn" onClick={onBack}>← Back</button>
        <h2>Friends</h2>
      </div>

      <form className="fl-add-form" onSubmit={handleAdd}>
        <input
          type="text"
          placeholder="Enter username..."
          value={addInput}
          onChange={e => { setAddInput(e.target.value); setMessage(""); }}
          className="text-box"
        />
        <button type="submit" className="fl-add-btn">Add Friend</button>
      </form>
      
      {message && <div className="fl-message">{message}</div>}

      <div className="fl-list">
        {friends.length === 0 ? (
          <p className="fl-empty">No friends yet. Add someone above!</p>
        ) : (
          <>
            {renderSection("Online", online)}
            {renderSection("Offline", offline)}
          </>
        )}
      </div>
    </div>
  );
}