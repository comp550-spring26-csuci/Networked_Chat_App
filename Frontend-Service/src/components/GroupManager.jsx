import { useState } from "react";
import GroupDrawer from "./GroupDrawer";

const BASE_URL = "https://vg3jzw0g-7081.usw3.devtunnels.ms"; 

export default function GroupManager({ 
    dms, 
    setDms, 
    selectedDM, 
    setSelectedDM, 
    currentUser 
}) {
    const [isGroupDrawerOpen, setIsGroupDrawerOpen] = useState(false);
    const [drawerMode, setDrawerMode] = useState("create"); 
    const [currentRoomMembers, setCurrentRoomMembers] = useState([]);
    const [roomMembersMap, setRoomMembersMap] = useState({}); 
    const [availableFriends, setAvailableFriends] = useState([]); 

    const fetchFriends = async () => {
        if (!currentUser?.userId) return;

        try {
            const res = await fetch(`${BASE_URL}/api/friends/list/${currentUser.userId}`, {
                method: "GET",
                headers: {
                    "X-Tunnel-Skip-AntiPhishing-Page": "true",
                    "Authorization": `Bearer ${currentUser?.token}`
                }
            });
            
            if (res.ok) {
                const rawText = await res.text();
                let fetchedData = [];
                if (rawText) {
                    try { fetchedData = JSON.parse(rawText); } catch(e) {}
                }
                
                const friendsArray = Array.isArray(fetchedData) ? fetchedData : (fetchedData.$values || []);
                setAvailableFriends(friendsArray.map(f => ({ username: f.username })));
            }
        } catch (err) {
            console.error("Failed to fetch friends for group drawer:", err);
        }
    };

    const openCreateGroupDrawer = () => {
        setDrawerMode("create");
        fetchFriends(); 
        setIsGroupDrawerOpen(true);
    };

    const openManageGroupDrawer = () => {
        setDrawerMode("manage");
        fetchFriends(); 
        const members = roomMembersMap[selectedDM.id] || [{ username: currentUser?.username || "You" }];
        setCurrentRoomMembers(members);
        setIsGroupDrawerOpen(true);
    };

    const handleCreateGroup = (groupName, memberUsernames) => {
        const roomId = `group-mock-${Date.now()}`;
        const newRoom = { id: roomId, name: groupName };
        
        setDms(prev => [...prev, newRoom]);
        setSelectedDM(newRoom);

        const initialMembers = [
            { username: currentUser?.username || "You" },
            ...memberUsernames.map(u => ({ username: u }))
        ];
        
        setRoomMembersMap(prev => ({
            ...prev,
            [roomId]: initialMembers
        }));
    };

    const handleAddMember = (roomId, username) => {
        const newMember = { username };
        setCurrentRoomMembers(prev => [...prev, newMember]);
        setRoomMembersMap(prev => ({
            ...prev,
            [roomId]: [...(prev[roomId] || []), newMember]
        }));
    };

    const handleRemoveMember = (roomId, username) => {
        setCurrentRoomMembers(prev => prev.filter(m => m.username !== username));
        setRoomMembersMap(prev => ({
            ...prev,
            [roomId]: (prev[roomId] || []).filter(m => m.username !== username)
        }));
    };

    const handleDeleteRoom = (roomId) => {
        setDms(prev => prev.filter(dm => dm.id !== roomId));
        setSelectedDM(prev => prev?.id === roomId ? null : prev);
        
        setRoomMembersMap(prev => {
            const newMap = { ...prev };
            delete newMap[roomId];
            return newMap;
        });
    };

    // --- eaving a group ---
    const handleLeaveGroup = (roomId) => {

        // Remove the room from your frontend DM list
        setDms(prev => prev.filter(dm => dm.id !== roomId));
        
        // Deselect the room if you are currently looking at it
        setSelectedDM(prev => prev?.id === roomId ? null : prev);
        
        // Clean up the memory map so it doesn't take up space
        setRoomMembersMap(prev => {
            const newMap = { ...prev };
            delete newMap[roomId];
            return newMap;
        });
    };

    return (
        <>
            <GroupDrawer 
                isOpen={isGroupDrawerOpen}
                onClose={() => setIsGroupDrawerOpen(false)}
                mode={drawerMode}
                selectedDM={selectedDM}
                availableFriends={availableFriends}
                currentMembers={currentRoomMembers}
                onCreateGroup={handleCreateGroup}
                onAddMember={handleAddMember}
                onRemoveMember={handleRemoveMember}
                onDeleteRoom={handleDeleteRoom}
                onLeaveGroup={handleLeaveGroup} 
                currentUser={currentUser}
            />

            {/* Polished Button Container */}
            <div className="group-controls-container">
                <button className="action-btn btn-new-group" onClick={openCreateGroupDrawer}>
                    + New Group
                </button>
                
                {selectedDM && (
                    <button className="action-btn btn-manage-group" onClick={openManageGroupDrawer}>
                        Manage Group
                    </button>
                )}
            </div>
        </>
    );
}