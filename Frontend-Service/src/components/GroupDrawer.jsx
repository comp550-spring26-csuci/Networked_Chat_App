import { useState, useEffect } from "react";
import "./GroupDrawer.css";

export default function GroupDrawer({ 
    isOpen, onClose, mode, selectedDM, availableFriends, currentMembers, 
    onCreateGroup, onAddMember, onRemoveMember, onDeleteRoom, onLeaveGroup, currentUser 
}) {
    const [groupName, setGroupName] = useState("");
    const [selectedFriends, setSelectedFriends] = useState([]);

    // Clear state whenever the modal opens
    useEffect(() => {
        if (isOpen) {
            setGroupName("");
            setSelectedFriends([]);
        }
    }, [isOpen]);

    if (!isOpen) return null;

    const handleToggleFriend = (username) => {
        setSelectedFriends(prev => 
            prev.includes(username) ? prev.filter(u => u !== username) : [...prev, username]
        );
    };

    const handleCreate = () => {
        const nameToUse = groupName.trim() || "New Group Chat";
        onCreateGroup(nameToUse, selectedFriends);
        onClose();
    };

    const isManage = mode === "manage";
    
    // Fallback in case currentUser is missing for some reason
    const myUsername = currentUser?.username || "You";

    return (
        <div className="gd-overlay">
            <div className="gd-modal">
                
                {/* Header */}
                <div className="gd-header">
                    <h2>{isManage ? `Manage Group` : "Create a Group"}</h2>
                    <button className="gd-close-btn" onClick={onClose}>&times;</button>
                </div>

                {/* Body */}
                <div className="gd-body">
                    {!isManage && (
                        <div>
                            <label className="gd-label">Group Name</label>
                            <input 
                                type="text" 
                                className="gd-input"
                                value={groupName} 
                                onChange={(e) => setGroupName(e.target.value)}
                                placeholder="Enter group name"
                            />
                        </div>
                    )}

                    <div>
                        <label className="gd-label">
                            {isManage ? "Current Members" : "Select Friends"}
                        </label>
                        <div className="gd-list-container">
                            {isManage ? (
                                currentMembers.map((member, idx) => (
                                    <div key={idx} className="gd-list-item">
                                        <span>{member.username} {member.username === myUsername ? "(You)" : ""}</span>
                                        
                                        {/* checking against your actual username */}
                                        {member.username === myUsername ? (
                                            <button 
                                                className="gd-btn-remove"
                                                onClick={() => {
                                                    if (onLeaveGroup) onLeaveGroup(selectedDM.id);
                                                    onClose();
                                                }}
                                            >
                                                Leave
                                            </button>
                                        ) : (
                                            <button 
                                                className="gd-btn-remove"
                                                onClick={() => onRemoveMember(selectedDM.id, member.username)}
                                            >
                                                Remove
                                            </button>
                                        )}
                                    </div>
                                ))
                            ) : (
                                availableFriends.length > 0 ? availableFriends.map((friend, idx) => (
                                    <label key={idx} className="gd-checkbox-label">
                                        <input 
                                            type="checkbox" 
                                            checked={selectedFriends.includes(friend.username)}
                                            onChange={() => handleToggleFriend(friend.username)}
                                        />
                                        {friend.username}
                                    </label>
                                )) : <div className="gd-empty">No friends available.</div>
                            )}
                        </div>
                    </div>

                    {isManage && (
                        <div>
                            <label className="gd-label">Add Friends</label>
                            <div className="gd-add-friends-wrapper">
                                {availableFriends.filter(f => !currentMembers.some(m => m.username === f.username)).map((friend, idx) => (
                                    <button 
                                        key={idx} 
                                        className="gd-btn-add"
                                        onClick={() => onAddMember(selectedDM.id, friend.username)}
                                    >
                                        + {friend.username}
                                    </button>
                                ))}
                            </div>
                        </div>
                    )}
                </div>

                {/* Footer */}
                <div className={`gd-footer ${isManage ? "manage" : "create"}`}>
                    {isManage ? (
                        <button 
                            className="gd-btn-danger"
                            onClick={() => { onDeleteRoom(selectedDM.id); onClose(); }} 
                        >
                            Delete Group
                        </button>
                    ) : (
                        <button 
                            className="gd-btn-primary"
                            onClick={handleCreate} 
                        >
                            Create Group
                        </button>
                    )}
                </div>
            </div>
        </div>
    );
}