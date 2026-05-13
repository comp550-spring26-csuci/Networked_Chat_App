import { useEffect, useState, useRef } from "react";
import DMList from "./DMList";
import ChatWindow from "./ChatWindow";
import FriendsList from "./FriendsList"; 
import { joinChatRoom, leaveChatRoom } from "../signalr/chatConnection";

// --- CHANGED: Accept currentUser instead of just username ---
export default function ChatLayout({ currentUser }) {
	const [dms, setDms] = useState([]);
	const [selectedDM, setSelectedDM] = useState(null);
	const idToNameRef = useRef({});

	useEffect(() => {
		async function fetchDMRooms() {
			try {
				const token = localStorage.getItem("access_token");
				const res = await fetch("https://localhost:7081/api/chathistory/user/mine/rooms", {
					method: 'GET',
					headers: {
						'Authorization': `Bearer ${token}`,
						'Content-Type': 'application/json'
					}
				});

				const data = await res.json();
				const roomsArray = Object.entries(data).map(([id, name]) => ({
					id,
					name
				}));

				setDms(roomsArray);
				if(data.length > 0) {
					setSelectedDM(roomsArray[0]);
				}

				const resp = await fetch("https://localhost:7081/api/test/all-users", {
					method: 'GET'
				});

				const dataUsers = await resp.json();
				idToNameRef.current = Object.fromEntries(
					dataUsers.map(user => [user.id, user.username])
				);
			} catch(err) {
				console.error("Failed to fetch rooms:", err);
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
				console.log("Switched room:", newDMId);
			} catch (err) {
        		console.error("Room switch failed:", err);
      		}
		}
		switchDM();
	}, [selectedDM]);

	const handleStartChat = async (friend) => {
		const existingRoom = dms.find(dm => dm.name.includes(friend.username));
		
		if (existingRoom) {
			setSelectedDM(existingRoom);
		} else {
			try {
				const token = localStorage.getItem("access_token");
				const res = await fetch("https://localhost:7081/api/chat/room/create", { 
					method: 'POST',
					headers: {
						'Authorization': `Bearer ${token}`,
						'Content-Type': 'application/json'
					},
					body: JSON.stringify({ targetUsername: friend.username })
				});

				if (res.ok) {
					const newRoomData = await res.json();
					const newRoom = { id: newRoomData.id, name: newRoomData.name };
					
					setDms(prev => [...prev, newRoom]);
					setSelectedDM(newRoom);
				} else {
					console.error("Failed to create new DM room");
				}
			} catch (err) {
				console.error("Error creating DM:", err);
			}
		}
	};

  	return (
		<div className="chat-container">
			{/* --- CHANGED: Pass the entire currentUser object --- */}
			<FriendsList 
				currentUser={currentUser} 
				onStartChat={handleStartChat} 
			/>
			
      		<DMList 
        		dms={dms} 
        		selectedDM={selectedDM}
        		onSelect={setSelectedDM}
      		/>
      		<ChatWindow
				idToNameRef={idToNameRef}
				selectedDM={selectedDM} 
				/* --- CHANGED: Use currentUser.username for the sender --- */
				sender={currentUser?.username} 
			/>
    	</div>
  	);
}