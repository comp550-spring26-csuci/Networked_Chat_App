import { useEffect, useRef, useState } from "react";
import MessageList from "./MessageList";
import ChatInput from "./ChatInput";
import { getConnection, TUNNEL_URL } from "../signalr/chatConnection";

export default function ChatWindow({ idToNameRef, selectedDM, sender }) {
	// Focuses input box when swapping DMs
	const inputRef = useRef(null);
	const [messages, setMessages] = useState({});
	const currentMessages = selectedDM
		? messages[selectedDM.id] || []
		: [];

	useEffect(() => {
		if(!selectedDM) return;
		async function fetchHistory() {
			const token = localStorage.getItem("sr_access_token");
			const res = await fetch(`${TUNNEL_URL}/api/chathistory/room/${selectedDM.id}/messages`, {
				method: 'GET',
				headers : {
					Authorization: `Bearer ${token}`
				}
			});

			const data = await res.json();
			const enriched = data.map(msg => ({
				message: {
					...msg
				},
				senderUsername: idToNameRef.current[msg.senderId],
				chatRoomName: selectedDM.name
			}));
			console.log(`CHAT NAME: ${selectedDM.name}`);

			setMessages((prev) => ({
				...prev,
				[selectedDM.id]: enriched
			}));
		}

		fetchHistory();
		inputRef.current?.focus();
	}, [selectedDM]);

	useEffect(() => {
		function handleReceiveMessage(paylode) {
			const DMId = paylode.message.chatRoomId;

			console.log(`RECEIVE MESSAGE, SENDER USERNAME: ${paylode.message.senderUsername}`);

			setMessages((prev) => ({
				...prev,
				[DMId]: [...(prev[DMId] || []), paylode]
			}));
		}

		const connection = getConnection();

		connection.on("ReceiveMessage", handleReceiveMessage);

		return () => {
			connection.off("ReceiveMessage", handleReceiveMessage);
		};
	}, [selectedDM]);

	const handleSend = async (text) => {
		const DMId = selectedDM.id;
		try {
			const connection = getConnection();
			if(!connection) {
				return;
			}
			await connection.invoke("SendMessageToChatRoom", {
				SendMessageToChatRoom: {
					ChatRoomId: DMId,
					Content: text
				},
				SenderUsername: sender
			});
		} catch(err) {
			console.error("Send failed:", err);
		}
  	};

  	return (
    	<div className="chat-window">
    		<div className="recipient-name">
				{selectedDM?.name || `Welcome ${sender}`}
			</div>
			<MessageList 
				messages={currentMessages} 
			/>
			{selectedDM && (
				<ChatInput 
					ref={inputRef} 
					selectedDM={selectedDM}
					onSend={handleSend}
				/>
			)}
    	</div>
  	);
}