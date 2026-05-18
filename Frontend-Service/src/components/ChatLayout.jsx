import { useEffect, useState, useRef } from "react";
import DMList from "./DMList";
import ChatWindow from "./ChatWindow";
import FriendsList from "./FriendsList"; 
import { joinChatRoom, leaveChatRoom, TUNNEL_URL } from "../signalr/chatConnection";
import { useOutletContext } from "react-router-dom";

export default function ChatLayout({ currentUser }) {
	const { isFriendsOpen, setIsFriendsOpen } = useOutletContext();
	const [dms, setDms] = useState([]);
	const [selectedDM, setSelectedDM] = useState(null);
	const idToNameRef = useRef({});

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
					const roomsArray = Object.entries(safeData).map(([id, name]) => ({
						id,
						name
					}));

					setDms(roomsArray);
					if (roomsArray.length > 0) {
						setSelectedDM(roomsArray[0]);
					}
				}

				const resp = await fetch(`${TUNNEL_URL}/api/test/all-users`, {
					method: 'GET',
					headers: {
						'X-Tunnel-Skip-AntiPhishing-Page': 'true'
					}
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

return ( //changed some things here
	<div className="chat-container">
	  <FriendsList 
		currentUser={currentUser} 
		onStartChat={handleStartChat} 
		isOpen={isFriendsOpen}         
		setIsOpen={setIsFriendsOpen}   
	  />
	  
	  <DMList 
		dms={dms} 
		selectedDM={selectedDM} 
		onSelect={setSelectedDM}    
		currentUser={currentUser}   
	  />
	  
	  <ChatWindow 
		idToNameRef={idToNameRef}
		selectedDM={selectedDM}
		sender={currentUser?.username}
	  />
	</div>
  );
}