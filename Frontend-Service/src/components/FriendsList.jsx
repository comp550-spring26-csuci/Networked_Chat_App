import { useState, useEffect } from "react";
import "./FriendsList.css";

const BASE_URL = "https://vg3jzw0g-7081.usw3.devtunnels.ms"; 

export default function FriendsList({ currentUser, onStartChat, isOpen, setIsOpen }) {
  const [friends, setFriends] = useState([]); 
  const [addInput, setAddInput] = useState("");
  
  const [errorMsg, setErrorMsg] = useState("");
  const [successMsg, setSuccessMsg] = useState("");

  const [myStatus, setMyStatus] = useState(currentUser?.status ?? 1);
  const [myCustomText, setMyCustomText] = useState(currentUser?.customText || currentUser?.customStatus || "");
  const [statusFeedback, setStatusFeedback] = useState("");

  const [activeStatus, setActiveStatus] = useState(currentUser?.status ?? 1);
  const [activeCustomText, setActiveCustomText] = useState(currentUser?.customText || currentUser?.customStatus || "");

  const handleLogout = () => {
    localStorage.removeItem("access_token");
    localStorage.removeItem("userId");
    window.location.reload();
  };

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
        const rawText = await res.text();
        if (rawText) {
          const data = JSON.parse(rawText);
          if (data.presenceStatus !== undefined) {
            setMyStatus(data.presenceStatus);
            setActiveStatus(data.presenceStatus);
          } else if (data.status !== undefined) {
            setMyStatus(data.status);
            setActiveStatus(data.status);
          }
          
          if (data.customStatusText !== undefined) {
            setMyCustomText(data.customStatusText);
            setActiveCustomText(data.customStatusText);
          } else if (data.customText !== undefined || data.customStatus !== undefined) {
            const text = data.customText || data.customStatus || "";
            setMyCustomText(text);
            setActiveCustomText(text);
          }
        }
      }
    } catch (err) {
      console.error("Network error fetching my profile:", err);
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
      
      const rawText = await response.text();
      
      if (response.ok) {
        let fetchedData = [];
        if (rawText) {
          try {
            fetchedData = JSON.parse(rawText);
          } catch (e) {
            console.error("Failed to parse friends list JSON:", e);
          }
        }
        
        const friendsArray = Array.isArray(fetchedData) ? fetchedData : (fetchedData.$values || []);
        
        const formattedFriends = friendsArray.map(f => ({
          id: f.id,
          username: f.username || "Unknown",
          status: f.presenceStatus !== undefined ? f.presenceStatus : (f.status !== undefined ? f.status : "Offline"),
          customStatus: f.customStatusText || f.customStatus || f.customText || "" 
        }));
        
        setFriends(formattedFriends);
      }
    } catch (error) {
      console.error("Network error fetching friends:", error);
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
      } else {
        setStatusFeedback("Failed to update status.");
      }
    } catch (error) {
      setStatusFeedback("Could not reach the server.");
    }
  };

  const handleAdd = async (e) => {
    e.preventDefault();
    const username = addInput.trim();
    
    setErrorMsg("");
    setSuccessMsg("");

    if (!username) return;
    
    if (username === currentUser?.username) {
      return setErrorMsg("You can't add yourself.");
    }
    if (friends.some(f => f.username?.toLowerCase() === username.toLowerCase())) {
      return setErrorMsg("This user is already your friend.");
    }

    const payload = { 
      userId: currentUser?.userId, 
      friendUsername: username 
    };

    try {
      const res = await fetch(`${BASE_URL}/api/friends/add-by-username`, {
        method: "POST",
        headers: { 
          "Content-Type": "application/json",
          "Authorization": `Bearer ${currentUser?.token}`,
          "X-Tunnel-Skip-AntiPhishing-Page": "true"
        },
        body: JSON.stringify(payload),
      });
      
      const rawText = await res.text();
      
      if (res.ok) {
        let isActuallySuccess = true;
        try {
          const data = JSON.parse(rawText);
          if (data.success === false) {
            isActuallySuccess = false;
            setErrorMsg(data.message || "The server rejected the friend request.");
          }
        } catch(err) { /* Not JSON, assume true success */ }

        if (isActuallySuccess) {
          setSuccessMsg(`${username} added successfully!`);
          setAddInput("");
          await refreshFriendsList(); 
        }
      } else {
        let errorMessage = "User not found or could not be added.";
        try {
          const data = JSON.parse(rawText);
          if (data.message) errorMessage = data.message;
        } catch(err) { /* ignore parse error */ }
        
        setErrorMsg(errorMessage);
      }
    } catch(err) {
      console.error("Add friend network error:", err);
      setErrorMsg("Could not reach the server. Please try again.");
    }
  };

  const handleRemove = async (id, username) => {
    try {
      const res = await fetch(`${BASE_URL}/api/friends/remove-friend-by-username`, {
        method: "DELETE", 
        headers: { 
          "Content-Type": "application/json",
          "Authorization": `Bearer ${currentUser?.token}`,
          "X-Tunnel-Skip-AntiPhishing-Page": "true"
        },
        body: JSON.stringify({ 
          userId: currentUser?.userId, 
          friendUsername: username 
        }),
      });
      
      if (res.ok) {
         await refreshFriendsList();
      } else {
         setFriends(friends.filter(f => f.id !== id));
      }
    } catch {
      setFriends(friends.filter(f => f.id !== id));
    }
  };

  const getStatusClass = (status) => {
    if (status === 2) return "custom";
    if (status === 1 || status === "Online") return "online";
    return "offline";
  };

  const onlineFriends = friends.filter(f => f.status === "Online" || f.status === 1 || f.status === 2);
  const offlineFriends = friends.filter(f => f.status !== "Online" && f.status !== 1 && f.status !== 2);

  const renderSection = (title, list) => {
    if (list.length === 0) return null;
    return (
      <div className="fl-section">
        <div className="fl-section-title">{title} — {list.length}</div>
        {list.map(friend => (
          <div 
            key={friend.id} 
            className="fl-item" 
            onClick={() => {
              if (onStartChat) onStartChat(friend);
              setIsOpen(false);
            }}
          >
            <div className="fl-info">
              <span className={`fl-status-dot ${getStatusClass(friend.status)}`}></span>
              <div className="fl-profile-col">
                <span className="fl-username">{friend.username}</span>
                {friend.customStatus && (
                  <span className="fl-status-text">
                    {friend.customStatus}
                  </span>
                )}
              </div>
            </div>
            
            <button 
              className="fl-remove-btn" 
              onClick={(e) => {
                e.stopPropagation();
                handleRemove(friend.id, friend.username);
              }} 
              title="Remove"
            >
              ✕
            </button>
          </div>
        ))}
      </div>
    );
  };

  return (
    <>
      {/* {!isOpen && (
        <button className="fl-hamburger" onClick={() => setIsOpen(true)}>
          <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
            <line x1="3" y1="12" x2="21" y2="12"></line>
            <line x1="3" y1="6" x2="21" y2="6"></line>
            <line x1="3" y1="18" x2="21" y2="18"></line>
          </svg>
        </button>
      )} */}

      <div className={`fl-overlay ${isOpen ? "open" : ""}`} onClick={() => setIsOpen(false)}></div>

      <div className={`fl-drawer ${isOpen ? "open" : ""}`}>
        <div className="fl-header">
          <h2>Friends</h2>
          <div className="fl-header-actions">
            <button className="fl-logout-btn" onClick={handleLogout}>
              Logout
            </button>
            <button className="fl-close-btn" onClick={() => setIsOpen(false)}>✕</button>
          </div>
        </div>

        <div className="fl-profile-section">
          
          <div className="fl-info fl-active-profile">
            <span className={`fl-status-dot ${getStatusClass(activeStatus)}`}></span>
            <div className="fl-profile-col">
              <span className="fl-username fl-profile-username">
                {currentUser?.username || "My Profile"}
              </span>
              <span className="fl-status-text">
                {activeStatus === 1 ? "Online" : activeStatus === 0 ? "Offline" : activeCustomText || "Custom Status"}
              </span>
            </div>
          </div>

          <label className="fl-status-label">
            Change Status
          </label>
          <div className="fl-status-form">
            <select 
              className="text-box fl-status-select" 
              value={myStatus} 
              onChange={(e) => setMyStatus(Number(e.target.value))}
            >
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
            
            <button className="fl-add-btn fl-update-status-btn" onClick={handleUpdateStatus}>
              Update Status
            </button>
          </div>
          {statusFeedback && (
            <div className={`fl-status-feedback ${statusFeedback.includes("success") ? "success" : "error"}`}>
              {statusFeedback}
            </div>
          )}
        </div>

        <form className="fl-add-form" onSubmit={handleAdd}>
          <input
            type="text"
            placeholder="Enter username..."
            value={addInput}
            onChange={e => { setAddInput(e.target.value); setErrorMsg(""); setSuccessMsg(""); }}
            className="text-box"
          />
          <button type="submit" className="fl-add-btn">Add</button>
        </form>
        
        {errorMsg && <div className="fl-message error">{errorMsg}</div>}
        {successMsg && <div className="fl-message success">{successMsg}</div>}

        <div className="fl-list">
          {friends.length === 0 ? (
            <p className="fl-empty">No friends yet. Add someone above!</p>
          ) : (
            <>
              {renderSection("Online", onlineFriends)}
              {renderSection("Offline", offlineFriends)}
            </>
          )}
        </div>
      </div>
    </>
  );
}