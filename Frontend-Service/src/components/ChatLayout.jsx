import { useEffect, useState, useRef } from "react";
import DMList from "./DMList";
import ChatWindow from "./ChatWindow";
import GroupDrawer from "./GroupDrawer";
import { joinChatRoom, leaveChatRoom, TUNNEL_URL } from "../signalr/chatConnection";

export default function ChatLayout() {
	const currentUser = JSON.parse(localStorage.getItem("user"));

	const [isGroupDrawerOpen, setIsGroupDrawerOpen] = useState(false);
	const [drawerMode, setDrawerMode] = useState("create");
	const [availableFriends, setAvailableFriends] = useState([]); 
	const [currentRoomMembers, setCurrentRoomMembers] = useState([]);
    const [roomMembersMap, setRoomMembersMap] = useState({}); 
	const username = localStorage.getItem("username");

	const [dms, setDms] = useState([]);
	// may have to change the start useState to null and then fetch DMs with api
	const [selectedDM, setSelectedDM] = useState(null);
	const idToNameRef = useRef({});

	// --- Persistence: Fetch existing groups on load ---
    useEffect(() => {
        const fetchUserGroups = async () => {
            if (!currentUser?.userId) return;

            try {
                const response = await fetch(`/api/ChatGroups/user/${currentUser.userId}`, {
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

                    // setDms(prev => {
                    //     // Filter out old group items to prevent duplicates
                    //     const nonGroupDms = (prev || []).filter(dm => !dm.id?.toString().includes("group")); 
                    //     return [...nonGroupDms, ...formattedGroups];
                    // });
					setDms(formattedGroups);
                } else {
                    console.error("Failed to fetch groups on login.");
                }
            } catch (error) {
                console.error("Network error fetching groups:", error);
            }
        };

        fetchUserGroups();
    }, [currentUser?.userId, setDms]);

    // const openCreateGroupDrawer = () => {
    //     setDrawerMode("create");
    //     fetchFriends(); 
    //     setIsGroupDrawerOpen(true);
    // };

    // const openManageGroupDrawer = async () => {
	// 	setDrawerMode("manage");

	// 	fetchFriends();

	// 	console.log("BEFORE ID CHECK");

	// 	console.log("selectedDM:", selectedDM);
	// 	console.log("selectedDM.id:", selectedDM?.id);

	// 	if (!selectedDM?.id) return;

	// 	console.log("AFTER ID CHECK");

	// 	const fetchedMembers = await getGroupChatMembers(selectedDM.id);

	// 	console.log("AFTER MEMBER FETCH");

	// 	const safeMembers = fetchedMembers?.length
	// 		? fetchedMembers
	// 		: [];

	// 	setCurrentRoomMembers(safeMembers);

	// 	setRoomMembersMap(prev => ({
	// 		...prev,
	// 		[selectedDM.id]: safeMembers
	// 	}));

	// 	setIsGroupDrawerOpen(true);
	// };

	// THIS DOESNT EXIST ANYMORE
    // --- Get Group ID by Name ---
    // const getGroupIdByName = async (groupName) => {
    //     if (!currentUser?.userId) return null;

    //     try {
    //         const response = await fetch(`/api/ChatGroups/get-id-by-name/${encodeURIComponent(groupName)}`, {
    //             method: 'GET',
    //             headers: {
    //                 "X-Tunnel-Skip-AntiPhishing-Page": "true",
    //                 "Authorization": `Bearer ${currentUser?.token}`
    //             }
    //         });

    //         if (response.ok) {
    //             const result = await response.json();
    //             return result.groupId;
    //         } else {
    //             console.error("Could not find group ID.");
    //             return null;
    //         }
    //     } catch (error) {
    //         console.error("Network Error while fetching group ID:", error);
    //         return null;
    //     }
    // };

    // --- Get Group Chat Members ---
    const getGroupChatMembers = async (groupId) => {
        try {
			console.log("GET GROUP CHAT MEMBERS");
            const response = await fetch(`/api/ChatGroups/${groupId}/get-group-chat-members`, {
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

		console.log("GROUP DATA: ", groupData);

        try {
            // 1. Create the group
            const response = await fetch("/api/ChatGroups/create", {
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
				const data = await response.json();

				console.log("CREATE GROUP RESPONSE DATA:", data);

                const newRoom = { id: data.groupId, name: groupName };
                
                // 3. Update the UI state
                setDms(prev => [...prev, newRoom]);
                setSelectedDM(newRoom);

                setRoomMembersMap(prev => ({
                    ...prev,
                    [newRoom.id]: [{ username: currentUser?.username || "You", userId: currentUser.userId }]
                }));

                // 4. Add selected friends using the fetched roomId
                if (memberUsernames.length > 0) {
                    for (const username of memberUsernames) {
                        const friend = availableFriends.find(f => f.username === username);
                        if (friend && friend.userId) {
                            await handleAddMember(newRoom.id, friend.userId, username);
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
            const response = await fetch("/api/ChatGroups/add-member", {
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
			const response = await fetch(`/api/ChatGroups/remove-member/${roomId}/${currentUser.userId}/${targetUserId}`, {
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
			const response = await fetch(`/api/ChatGroups/delete-group/${roomId}/${currentUser.userId}`, {
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
			const response = await fetch(`/api/ChatGroups/leave-group/${roomId}/${currentUser.userId}`, {
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

	// --- Fetch Friends (Includes userId to prevent 400 errors) ---
    const fetchFriends = async () => {
        if (!currentUser?.userId) return;

        try {
            const res = await fetch(`/api/friends/list/${currentUser.userId}`, {
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

	const openCreateGroupDrawer = async () => {
		await fetchFriends();
		setDrawerMode("create");
		setIsGroupDrawerOpen(true);
	};

	const openManageGroupDrawer = async (dm) => {
		const targetDM = dm || selectedDM;
		if (!targetDM?.id) return;

		setDrawerMode("manage");

		// fetch friends if needed
		await fetchFriends();

		// fetch members
		const res = await fetch(`/api/ChatGroups/${targetDM.id}/get-group-chat-members`);
		const data = await res.json();

		const members = Array.isArray(data)
		? data
		: data.$values || [];

		setCurrentRoomMembers(members);
			setRoomMembersMap(prev => ({
			...prev,
			[selectedDM.id]: members
		}));

		setIsGroupDrawerOpen(true);

		return members; // optional if you want to pass down later
	};

	useEffect(() => {
		async function fetchDMRooms() {
			try {
				const res = await fetch("/api/chathistory/user/mine/rooms", {
					method: 'GET',
					headers: {
						'Authorization': `Bearer ${localStorage.getItem("access_token")}`,
						'Content-Type': 'application/json'
					}
				});

				const data = await res.json();
				const roomsArray = Object.entries(data).map(([id, name]) => ({
					id,
					name
				}));

				//setDms(roomsArray); // try commenting this out to see if it resolves duplicates in the dm list on launch

				const resp = await fetch("/api/users/all-users", {
					method: 'GET'
				});

				const dataUsers = await resp.json();
				idToNameRef.current = Object.fromEntries(
					dataUsers.map(user => [user.id, user.username])
				);
				
				if(roomsArray.length > 0) {
					setSelectedDM(roomsArray[0]);
				}
			} catch(err) {
				console.error("Failed to fetch rooms:", err);
			}
		}
		fetchDMRooms();
	}, []);

	// track previous DM
	const prevDMRef = useRef(null);

	useEffect(() => {
		if(!selectedDM) return;

		async function switchDM() {
			try {
				const newDMId = selectedDM.id;
				const prevDMId = prevDMRef.current;

				if(prevDMId) {
					await leaveChatRoom(prevDMId);
				}

				await joinChatRoom(newDMId);
				prevDMRef.current = newDMId;
				console.log("Switched room:", newDMId);
			} catch (err) {
        		console.error("Room switch failed:", err);
      		}
		}
		switchDM();
		console.log(`SELECTED DM ROOM NAME: ${selectedDM.name}`)
	}, [selectedDM]);

  	return (
		<div className="chat-container">
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

      		<DMList 
        		dms={dms} 
        		selectedDM={selectedDM}
        		onSelect={setSelectedDM}
				onCreateGroup={openCreateGroupDrawer}
      		/>
			<ChatWindow
				idToNameRef={idToNameRef}
				selectedDM={selectedDM} 
				sender={username}
				onManageGroup={openManageGroupDrawer}
			/>
    	</div>
  	);
}