import { useEffect, useState, useRef } from "react";
import DMList from "./DMList";
import ChatWindow from "./ChatWindow";
import { joinChatRoom, leaveChatRoom, TUNNEL_URL } from "../signalr/chatConnection";

export default function ChatLayout() {
	const username = localStorage.getItem("username");

	const [dms, setDms] = useState([]);
	// may have to change the start useState to null and then fetch DMs with api
	const [selectedDM, setSelectedDM] = useState(null);
	const idToNameRef = useRef({});

	useEffect(() => {
		async function fetchDMRooms() {
			try {
				const res = await fetch(`${TUNNEL_URL}/api/chathistory/user/mine/rooms`, {
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

				setDms(roomsArray);

				const resp = await fetch(`${TUNNEL_URL}/api/test/all-users`, {
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
      		<DMList 
        		dms={dms} 
        		selectedDM={selectedDM}
        		onSelect={setSelectedDM}
      		/>
			<ChatWindow
				idToNameRef={idToNameRef}
				selectedDM={selectedDM} 
				sender={username} 
			/>
    	</div>
  	);
}