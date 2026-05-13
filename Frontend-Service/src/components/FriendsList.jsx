import { useState, useEffect, useCallback } from "react";
import "./FriendsList.css";

const BASE_URL = "https://vg3jzw0g-7081.usw3.devtunnels.ms"; 

export default function FriendsList({ currentUser, onStartChat }) {
  const [isOpen, setIsOpen] = useState(false);
  const [friends, setFriends] = useState([]); 
  const [addInput, setAddInput] = useState("");
  
  const [errorMsg, setErrorMsg] = useState("");
  const [successMsg, setSuccessMsg] = useState("");

  const [myStatus, setMyStatus] = useState(currentUser?.status ?? 1);
  const [myCustomText, setMyCustomText] = useState(currentUser?.customText || currentUser?.customStatus || "");
  const [statusFeedback, setStatusFeedback] = useState("");

  const [activeStatus, setActiveStatus] = useState(currentUser?.status ?? 1);
  const [activeCustomText, setActiveCustomText] = useState(currentUser?.customText || currentUser?.customStatus || "");

  const handleLogout = useCallback(() => {
    localStorage.removeItem("access_token");
    localStorage.removeItem("userId");
    window.location.reload();
  }, []);

  const fetchMyProfile = async () => {
    if (!currentUser?.userId) return;
    try {
      const res = await fetch(`${BASE_URL}/api/Status/${currentUser.userId}`, {
        method: "GET",
        headers: {
          "X-Tunnel-Skip-AntiPhishing-Page": "true",
          "Authorization": `Bearer ${currentUser?.token}`
        }
      });

      if (res.ok) {
        const data = await res.json();
        // handle the presenceStatus (1, 2, 0) and custom text from backend
        const statusVal = data.presenceStatus ?? data.status ?? 1;
        const textVal = data.customStatusText ?? data.customText ?? data.customStatus ?? "";
        
        setMyStatus(statusVal);
        setActiveStatus(statusVal);
        setMyCustomText(textVal);
        setActiveCustomText(textVal);
      }
    } catch (err) {
      console.error("Error fetching profile:", err);
    }
  };

  const refreshFriendsList = async () => {
    if (!currentUser?.userId) return;
    try {
      const response = await fetch(`${BASE_URL}/api/friends/list/${currentUser.userId}`, {
        method: "GET",
        headers: {
          "X-Tunnel-Skip-AntiPhishing-Page": "true",
          "Authorization": `Bearer ${currentUser?.token}`
        }
      });
      
      if (response.ok) {
        const fetchedData = await response.json();
        const friendsArray = Array.isArray(fetchedData) ? fetchedData : (fetchedData.$values || []);
        
        const formattedFriends = friendsArray.map(f => ({
          id: f.id,
          username: f.username || "Unknown",
          // Force status to number for consistent comparison
          status: f.presenceStatus !== undefined ? Number(f.presenceStatus) : (f.status !== undefined ? Number(f.status) : 0),
          customStatus: f.customStatusText || f.customStatus || f.customText || "" 
        }));
        
        setFriends(formattedFriends);
      }
    } catch (error) {
      console.error("Error fetching friends:", error);
    }
  };

  useEffect(() => {
    if (currentUser?.userId) {
      fetchMyProfile();
      refreshFriendsList();
    }
  }, [currentUser]);

  const handleUpdateStatus = async () => {
    setStatusFeedback("");
    try {
      const res = await fetch(`${BASE_URL}/api/Status/update-status`, {
        method: "PUT",
        headers: { 
          "Content-Type": "application/json",
          "Authorization": `Bearer ${currentUser?.token}`,
          "X-Tunnel-Skip-AntiPhishing-Page": "true" 
        },
        body: JSON.stringify({ 
          userId: currentUser?.userId,
          newStatus: myStatus, 
          customText: myStatus === 2 ? myCustomText : "" 
        }),
      });
      
      if (res.ok) {
        setStatusFeedback("Status updated successfully!");
        setActiveStatus(myStatus);
        setActiveCustomText(myStatus === 2 ? myCustomText : "");
        setTimeout(() => setStatusFeedback(""), 3000); 
        refreshFriendsList();
      }
    } catch (error) {
      console.error("Status update error:", error);
    }
  };

  const handleAdd = async (e) => {
    e.preventDefault();
    const username = addInput.trim();
    if (!username || username === currentUser?.username) return;

    try {
      const res = await fetch(`${BASE_URL}/api/friends/add-by-username`, {
        method: "POST",
        headers: { 
          "Content-Type": "application/json",
          "X-Tunnel-Skip-AntiPhishing-Page": "true"
        },
        body: JSON.stringify({ userId: currentUser?.userId, friendUsername: username }),
      });
      if (res.ok) {
        setSuccessMsg(`${username} added!`);
        setAddInput("");
        refreshFriendsList();
      }
    } catch(err) {
      setErrorMsg("Failed to add friend.");
    }
  };

  const handleRemove = async (id, username) => {
    try {
      await fetch(`${BASE_URL}/api/friends/remove`, {
        method: "DELETE", 
        headers: { "Content-Type": "application/json", "X-Tunnel-Skip-AntiPhishing-Page": "true" },
        body: JSON.stringify({ userId: currentUser?.userId, friendUsername: username }),
      });
      refreshFriendsList();
    } catch (err) {
      setFriends(friends.filter(f => f.id !== id));
    }
  };

  // --- 1 to green, 2 to purple ---
  const getStatusClass = (status) => {
    if (status === 2) return "custom"; 
    if (status === 1 || status === "Online") return "online";
    return "offline";
  };

  const onlineFriends = friends.filter(f => f.status === 1 || f.status === 2 || f.status === "Online");
  const offlineFriends = friends.filter(f => f.status === 0 || (f.status !== 1 && f.status !== 2 && f.status !== "Online"));

  const renderSection = (title, list) => {
    if (list.length === 0) return null;
    return (
      <div className="fl-section">
        <div className="fl-section-title">{title} — {list.length}</div>
        {list.map(friend => (
          <div key={friend.id} className="fl-item" onClick={() => onStartChat?.(friend)}>
            <div className="fl-info">
              <span className={`fl-status-dot ${getStatusClass(friend.status)}`}></span>
              <div className="fl-profile-col">
                <span className="fl-username">{friend.username}</span>
                {friend.customStatus && (
                  <span className="fl-status-text">{friend.customStatus}</span>
                )}
              </div>
            </div>
            <button className="fl-remove-btn" onClick={(e) => { e.stopPropagation(); handleRemove(friend.id, friend.username); }}>✕</button>
          </div>
        ))}
      </div>
    );
  };

  return (
    <>
      {!isOpen && (
        <button className="fl-hamburger" onClick={() => setIsOpen(true)}>
          <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
            <line x1="3" y1="12" x2="21" y2="12"></line>
            <line x1="3" y1="6" x2="21" y2="6"></line>
            <line x1="3" y1="18" x2="21" y2="18"></line>
          </svg>
        </button>
      )}

      <div className={`fl-overlay ${isOpen ? "open" : ""}`} onClick={() => setIsOpen(false)}></div>

      <div className={`fl-drawer ${isOpen ? "open" : ""}`}>
        <div className="fl-header">
          <h2>Friends</h2>
          <div className="fl-header-actions">
            <button className="fl-logout-btn" onClick={handleLogout}>Logout</button>
            <button className="fl-close-btn" onClick={() => setIsOpen(false)}>✕</button>
          </div>
        </div>

        <div className="fl-profile-section">
          <div className="fl-info fl-active-profile">
            <span className={`fl-status-dot ${getStatusClass(activeStatus)}`}></span>
            <div className="fl-profile-col">
              <span className="fl-username fl-profile-username">{currentUser?.username || "My Profile"}</span>
              <span className="fl-status-text">
                {activeStatus === 1 ? "Online" : activeStatus === 2 ? activeCustomText : "Offline"}
              </span>
            </div>
          </div>

          <div className="fl-status-form">
            <select className="text-box fl-status-select" value={myStatus} onChange={(e) => setMyStatus(Number(e.target.value))}>
              <option value={1}>Online</option>
              <option value={2}>Custom Status</option>
            </select>
            {myStatus === 2 && (
              <input
                type="text"
                placeholder="What's on your mind?"
                value={myCustomText}
                onChange={(e) => setMyCustomText(e.target.value)}
                className="text-box fl-status-input"
              />
            )}
            <button className="fl-add-btn fl-update-status-btn" onClick={handleUpdateStatus}>Update Status</button>
          </div>
        </div>

        <div className="fl-list">
          {renderSection("Online", onlineFriends)}
          {renderSection("Offline", offlineFriends)}
        </div>
      </div>
    </>
  );
}