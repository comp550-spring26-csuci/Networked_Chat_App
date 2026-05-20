import { useState, useEffect } from "react";
import GroupDrawer from "./GroupDrawer";

let ivanaAddress = 1;
let BASE_URL = " "; 

if (ivanaAddress == 0)
{
  BASE_URL = "https://vg3jzw0g-7081.usw3.devtunnels.ms"; 
}
else 
{
  BASE_URL = "https://vg3jzw0g-5148.usw3.devtunnels.ms";
}

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

    // --- Persistence: Fetch existing groups on load ---
    useEffect(() => {
        const fetchUserGroups = async () => {
            if (!currentUser?.userId) return;

            try {
                const response = await fetch(`${BASE_URL}/api/ChatGroups/user/${currentUser.userId}`, {
                    method: 'GET',
                    headers: {
                        "X-Tunnel-Skip-AntiPhishing-Page": "true",
                        "Authorization": `Bearer ${currentUser?.token}`
                    }
                });

                if (response.ok) {
                    const groupsData = await response.json();
                    
                    const formattedGroups = groupsData.map(group => ({
                        id: group.groupId || group.id,
                        name: group.groupName || group.name 
                    }));

                    setDms(prev => {
                        // Filter out old group items to prevent duplicates
                        const nonGroupDms = (prev || []).filter(dm => !dm.id?.toString().includes("group")); 
                        return [...nonGroupDms, ...formattedGroups];
                    });
                } else {
                    console.error("Failed to fetch groups on login.");
                }
            } catch (error) {
                console.error("Network error fetching groups:", error);
            }
        };

        fetchUserGroups();
    }, [currentUser?.userId, setDms]);

    // --- Fetch Friends (Includes userId to prevent 400 errors) ---
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
                setAvailableFriends(friendsArray.map(f => ({ 
                    username: f.username, 
                    userId: f.id || f.userId 
                })));
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

    const openManageGroupDrawer = async () => {
        setDrawerMode("manage");
        fetchFriends(); // Still fetch friends so the user can add new ones
        
        // Temporarily set it to just the current user while loading
        setCurrentRoomMembers([{ username: currentUser?.username || "You", userId: currentUser?.userId }]);
        setIsGroupDrawerOpen(true);

        if (selectedDM?.id) {
            // Fetch the real, up-to-date members from your C# database
            const fetchedMembers = await getGroupChatMembers(selectedDM.id);

            if (fetchedMembers && fetchedMembers.length > 0) {
                // Update the drawer UI with the real members
                setCurrentRoomMembers(fetchedMembers);
                
                // Update our local cache map so it stays synced
                setRoomMembersMap(prev => ({
                    ...prev,
                    [selectedDM.id]: fetchedMembers
                }));
            }
        }
    };

    // --- Get Group ID by Name ---
    const getGroupIdByName = async (groupName) => {
        if (!currentUser?.userId) return null;

        try {
            const response = await fetch(`${BASE_URL}/api/ChatGroups/get-id-by-name/${encodeURIComponent(groupName)}`, {
                method: 'GET',
                headers: {
                    "X-Tunnel-Skip-AntiPhishing-Page": "true",
                    "Authorization": `Bearer ${currentUser?.token}`
                }
            });

            if (response.ok) {
                const result = await response.json();
                return result.groupId;
            } else {
                console.error("Could not find group ID.");
                return null;
            }
        } catch (error) {
            console.error("Network Error while fetching group ID:", error);
            return null;
        }
    };

    // --- Get Group Chat Members ---
    const getGroupChatMembers = async (groupId) => {
        try {
            const response = await fetch(`${BASE_URL}/api/ChatGroups/${groupId}/get-group-chat-members`, {
                method: 'GET',
                headers: {
                    "X-Tunnel-Skip-AntiPhishing-Page": "true",
                    "Authorization": `Bearer ${currentUser?.token}`
                }
            });

            if (response.ok) {
                const result = await response.json();
                console.log(`Successfully retrieved ${result.length} members!`);
                
                // Map the backend data to match our frontend { username, userId } structure
                // We use || to handle potential C# camelCase or PascalCase differences
                return result.map(m => ({
                    username: m.username || m.userName, 
                    userId: m.userId || m.id
                }));
            } else if (response.status === 404) {
                return []; 
            } else {
                console.error("Failed to retrieve members.");
                return [];
            }
        } catch (error) {
            console.error("Network Error when fetching members:", error);
            return [];
        }
    };

    // --- Create Group ---
    const handleCreateGroup = async (groupName, memberUsernames) => {
        if (!currentUser?.userId) return;

        const groupData = {
            groupName: groupName,
            creatorUserId: currentUser.userId
        };

        try {
            // 1. Create the group
            const response = await fetch(`${BASE_URL}/api/ChatGroups/create-chat-group`, {
                method: 'POST',
                headers: {
                    "Content-Type": "application/json",
                    "X-Tunnel-Skip-AntiPhishing-Page": "true",
                    "Authorization": `Bearer ${currentUser?.token}`
                },
                body: JSON.stringify(groupData)
            });

            if (response.ok) {
                // 2. Fetch the newly created Group ID
                const roomId = await getGroupIdByName(groupName);

                if (!roomId) {
                    alert("Group created, but could not retrieve the new group ID.");
                    return;
                }

                const newRoom = { id: roomId, name: groupName };
                
                // 3. Update the UI state
                setDms(prev => [...prev, newRoom]);
                setSelectedDM(newRoom);

                setRoomMembersMap(prev => ({
                    ...prev,
                    [roomId]: [{ username: currentUser?.username || "You", userId: currentUser.userId }]
                }));

                // 4. Add selected friends using the fetched roomId
                if (memberUsernames.length > 0) {
                    for (const username of memberUsernames) {
                        const friend = availableFriends.find(f => f.username === username);
                        if (friend && friend.userId) {
                            await handleAddMember(roomId, friend.userId, username);
                        }
                    }
                }
            } else {
                const errorResult = await response.json().catch(() => ({}));
                alert("Failed to create group: " + (errorResult.message || "Unknown error"));
            }
        } catch (error) {
            console.error("Network Error:", error);
            alert("A network error occurred while attempting to create the group.");
        }
    };

    // --- Add Member ---
    const handleAddMember = async (roomId, targetUserId, username) => {
        if (!currentUser?.userId) return;

        const memberData = {
            chatGroupId: roomId,
            requesterId: currentUser.userId,
            targetUserId: targetUserId
        };

        try {
            const response = await fetch(`${BASE_URL}/api/ChatGroups/add-member`, {
                method: 'POST',
                headers: {
                    "Content-Type": "application/json",
                    "X-Tunnel-Skip-AntiPhishing-Page": "true",
                    "Authorization": `Bearer ${currentUser?.token}`
                },
                body: JSON.stringify(memberData)
            });

            if (response.ok) {
                const newMember = { username, userId: targetUserId };
                setCurrentRoomMembers(prev => [...prev, newMember]);
                
                setRoomMembersMap(prev => ({
                    ...prev,
                    [roomId]: [...(prev[roomId] || []), newMember]
                }));
            } else {
                const result = await response.json().catch(() => ({}));
                alert("Failed to add member: " + (result.message || result || "Unknown error"));
            }
        } catch (error) {
            console.error("Network Error:", error);
        }
    };

// --- Remove Member  ---
const handleRemoveMember = async (roomId, targetUserId, username) => {
    if (!currentUser?.userId) {
        console.error("Cannot remove member: User ID is missing.");
        return;
    }

    try {
        const response = await fetch(`${BASE_URL}/api/ChatGroups/remove-member/${roomId}/${currentUser.userId}/${targetUserId}`, {
            method: 'DELETE', 
            headers: {
                "X-Tunnel-Skip-AntiPhishing-Page": "true",
                "Authorization": `Bearer ${currentUser?.token}`
            }
        });

        if (response.ok) {
            console.log(`Successfully removed ${username} from the group.`);

            setCurrentRoomMembers(prev => prev.filter(m => m.username !== username));
            setRoomMembersMap(prev => ({
                ...prev,
                [roomId]: (prev[roomId] || []).filter(m => m.username !== username)
            }));
        } else {
            const errorText = await response.text();
            console.error("Failed to remove member:", errorText);
            alert("Failed to remove member: " + (errorText || "Unknown error"));
        }
    } catch (error) {
        console.error("Network Error when removing member:", error);
        alert("A network error occurred while attempting to remove the member.");
    }
};

// --- Delete Group  ---
const handleDeleteRoom = async (roomId) => {
    // Guard clause to ensure we have a user
    if (!currentUser?.userId) {
        console.error("Cannot delete group: User ID is missing.");
        return;
    }

    try {
        const response = await fetch(`${BASE_URL}/api/ChatGroups/delete-group/${roomId}/${currentUser.userId}`, {
            method: 'DELETE',
            headers: {
                "X-Tunnel-Skip-AntiPhishing-Page": "true",
                "Authorization": `Bearer ${currentUser?.token}`
            }
        });

        if (response.ok) {
            const result = await response.json();
            console.log("Group Deleted:", result.message);

            setDms(prev => prev.filter(dm => dm.id !== roomId));
            setSelectedDM(prev => prev?.id === roomId ? null : prev);
            
            setRoomMembersMap(prev => {
                const newMap = { ...prev };
                delete newMap[roomId];
                return newMap;
            });
        } else if (response.status === 401) {
            const errorText = await response.text();
            console.warn("Unauthorized:", errorText);
            alert("Cannot delete group: " + (errorText || "You must be the creator to delete this group."));
        } else {
            const errorText = await response.text();
            console.error("Failed to delete group:", errorText);
            alert("Failed to delete group: " + (errorText || "Unknown error"));
        }
    } catch (error) {
        console.error("Network Error when deleting group:", error);
        alert("A network error occurred while attempting to delete the group.");
    }
};

// --- Leave a Group ---
const handleLeaveGroup = async (roomId) => {
    if (!currentUser?.userId) {
        console.error("Cannot leave group: User ID is missing.");
        return;
    }

    try {
        const response = await fetch(`${BASE_URL}/api/ChatGroups/leave-group/${roomId}/${currentUser.userId}`, {
            method: 'DELETE',
            headers: {
                "X-Tunnel-Skip-AntiPhishing-Page": "true",
                "Authorization": `Bearer ${currentUser?.token}`
            }
        });

        if (response.ok) {
            const result = await response.json();
            console.log("Left Group:", result.message);

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
        } else {
            // If it fails, read the raw text response as your example expects
            const errorText = await response.text();
            console.error("Failed to leave group:", errorText);
            alert("Failed to leave group: " + (errorText || "Unknown error"));
        }
    } catch (error) {
        console.error("Network Error when leaving group:", error);
        alert("A network error occurred while attempting to leave the group.");
    }
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