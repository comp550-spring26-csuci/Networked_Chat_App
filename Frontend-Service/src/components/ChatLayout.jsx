import { useEffect, useState, useRef } from "react";
import DMList from "./DMList";
import ChatWindow from "./ChatWindow";
import { joinChatRoom, leaveChatRoom } from "../signalr/chatConnection";

export default function ChatLayout({ username }) {
	const [dms, setDms] = useState([]);
	// may have to change the start useState to null and then fetch DMs with api
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