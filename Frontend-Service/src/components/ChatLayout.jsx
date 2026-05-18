import { useEffect, useState, useRef } from "react";
import DMList from "./DMList";
import ChatWindow from "./ChatWindow";
import FriendsList from "./FriendsList"; 
import GroupManager from "./GroupManager";
import { joinChatRoom, leaveChatRoom, TUNNEL_URL } from "../signalr/chatConnection";
import { useOutletContext } from "react-router-dom";

export default function ChatLayout({ currentUser }) {
	const { isFriendsOpen, setIsFriendsOpen } = useOutletContext();
	const [dms, setDms] = useState([]);
	const [selectedDM, setSelectedDM] = useState(null);
	const idToNameRef = useRef({});
	const prevDMRef = useRef(null);

	useEffect(() => {
		async function fetchDMRooms() {
			try {
				const token = localStorage.getItem("access_token");
				const res = await fetch(`${TUNNEL_URL}/api/chathistory/user/mine/rooms`, {
					method: 'GET',
					headers: {
						'Authorization': `Bearer ${token}`,
						'Content-Type': 'application/json',
						'X-Tunnel-Skip-AntiPhishing-Page': 'true'
					}
				});

				if (res.ok) {
					const data = await res.json();
					const safeData = data || {};
					
					let roomsArray = [];
					
					// Hyper-safe data parsing: Handles Arrays, .NET $values, or Dictionaries
					if (Array.isArray(safeData)) {
						roomsArray = safeData;
					} else if (safeData.$values) {
						roomsArray = safeData.$values;
					} else {
						roomsArray = Object.entries(safeData).map(([key, val]) => {
							if (typeof val === 'object' && val !== null) return val;
							return { id: key, name: val };
						});
					}

					// Ensure uniform structure to prevent 'undefined' errors
					const formattedRooms = roomsArray.map(room => ({
						id: room.id || room.roomId || `fallback-${Math.random()}`,
						name: room.name || room.roomName || "Unnamed Room"
					}));

					setDms(formattedRooms);
					if (formattedRooms.length > 0) {
						setSelectedDM(formattedRooms[0]);
					}
				}

				const resp = await fetch(`${TUNNEL_URL}/api/test/all-users`, {
					method: 'GET',
					headers: { 'X-Tunnel-Skip-AntiPhishing-Page': 'true' }
				});

				if (resp.ok) {
					const dataUsers = await resp.json();
					const usersList = Array.isArray(dataUsers) ? dataUsers : (dataUsers?.$values || []);
					idToNameRef.current = Object.fromEntries(
						usersList.map(user => [user.id, user.username])
					);
				}
			} catch(err) {
				console.error("Failed to fetch rooms safely:", err);
			}
		}
		fetchDMRooms();
	}, []);

	useEffect(() => {
		if(!selectedDM) return;

		async function switchDM() {
			try {
				const newDMId = selectedDM.id;
				const prevDMId = prevDMRef.current;

				// BYPASS: If the previous room was a mock group, don't ask SignalR to leave it
				if(prevDMId && !prevDMId.toString().startsWith("group-mock")) {
					await leaveChatRoom(prevDMId);
				}

				// BYPASS: If the new room is a mock group, don't ask SignalR to join it
				if (!newDMId.toString().startsWith("group-mock")) {
					await joinChatRoom(newDMId);
				}
				
				prevDMRef.current = newDMId;
			} catch (err) {
        		console.error("Room switch failed safely:", err);
      		}
		}
		switchDM();
	}, [selectedDM]);

	const handleStartChat = async (friend) => {
		if (!friend?.username) return;
		const existingRoom = dms.find(dm => dm.name?.includes(friend.username));
		
		if (existingRoom) {
			setSelectedDM(existingRoom);
		} else {
			try {
				const token = localStorage.getItem("access_token");
				const res = await fetch(`${TUNNEL_URL}/api/chat/room/create`, { 
					method: 'POST',
					headers: {
						'Authorization': `Bearer ${token}`,
						'Content-Type': 'application/json',
						'X-Tunnel-Skip-AntiPhishing-Page': 'true'
					},
					body: JSON.stringify({ targetUsername: friend.username })
				});

				if (res.ok) {
					const newRoomData = await res.json();
					const newRoom = { id: newRoomData.id, name: newRoomData.name };
					setDms(prev => [...prev, newRoom]);
					setSelectedDM(newRoom);
				}
			} catch (err) {
				console.error("Error creating DM:", err);
			}
		}
	};

	return (
		<div className="chat-container">
			<FriendsList 
				currentUser={currentUser} 
				onStartChat={handleStartChat} 
				isOpen={isFriendsOpen}         
				setIsOpen={setIsFriendsOpen}   
			/>
		  
			<div className="dm-list-wrapper">
				<GroupManager 
					dms={dms}
					setDms={setDms}
					selectedDM={selectedDM}
					setSelectedDM={setSelectedDM}
					currentUser={currentUser}
				/>

				<DMList 
					dms={dms} 
					selectedDM={selectedDM} 
					onSelect={setSelectedDM}    
				/>
			</div>
		  
			<div className="chat-window-wrapper" style={{ flex: 1, display: 'flex', flexDirection: 'column' }}>
				<ChatWindow 
					idToNameRef={idToNameRef}
					selectedDM={selectedDM}
					sender={currentUser?.username}
				/>
			</div>
		</div>
	);
}