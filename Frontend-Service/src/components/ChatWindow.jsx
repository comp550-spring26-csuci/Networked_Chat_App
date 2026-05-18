import { useEffect, useRef, useState } from "react";
import MessageList from "./MessageList";
import ChatInput from "./ChatInput";
import { getConnection, TUNNEL_URL } from "../signalr/chatConnection";
//had to make some edits in order to render

export default function ChatWindow({ idToNameRef, selectedDM, sender }) {
	const inputRef = useRef(null);
	const [messages, setMessages] = useState({});
	const currentMessages = selectedDM
		? messages[selectedDM.id] || []
		: [];

	useEffect(() => {
		if(!selectedDM) return;
		async function fetchHistory() {
			try {
				const token = localStorage.getItem("access_token");
				const res = await fetch(`${TUNNEL_URL}/api/chathistory/room/${selectedDM.id}/messages`, {
					method: 'GET',
					headers : {
						'Authorization': `Bearer ${token}`,
						'X-Tunnel-Skip-AntiPhishing-Page': 'true' // Added bypass header
					}
				});

				if (res.ok) {
					const data = await res.json();
					// Prevent crashes if data is an unexpected structure or object collection
					const messageList = Array.isArray(data) ? data : (data?.$values || []);
					
					const enriched = messageList.map(msg => ({
						message: {
							...msg
						},
						senderUsername: idToNameRef.current?.[msg.senderId] || "User",
						chatRoomName: selectedDM.name
					}));

					setMessages((prev) => ({
						...prev,
						[selectedDM.id]: enriched
					}));
				}
			} catch (err) {
				console.error("Error fetching message history:", err);
			}
		}

		fetchHistory();
		inputRef.current?.focus();
	}, [selectedDM, idToNameRef]);

	useEffect(() => {
		function handleReceiveMessage(paylode) {
			if (!paylode || !paylode.message) return;
			const DMId = paylode.message.chatRoomId;

			setMessages((prev) => ({
				...prev,
				[DMId]: [...(prev[DMId] || []), paylode]
			}));
		}

		const connection = getConnection();

		// Safe guard: check if connection is active before adding hooks
		if (connection && typeof connection.on === "function") {
			connection.on("ReceiveMessage", handleReceiveMessage);
		}

		return () => {
			if (connection && typeof connection.off === "function") {
				connection.off("ReceiveMessage", handleReceiveMessage);
			}
		};
	}, [selectedDM]);

	const handleSend = async (text) => {
		if (!selectedDM) return;
		const DMId = selectedDM.id;
		try {
			const connection = getConnection();
			if (connection && typeof connection.invoke === "function") {
				await connection.invoke("SendMessageToChatRoom", {
					SendMessageToChatRoom: {
						ChatRoomId: DMId,
						Content: text
					},
					SenderUsername: sender
				});
			}
		} catch(err) {
			console.error("Send failed:", err);
		}
  	};

  	return (
    	<div className="chat-window">
    		<div className="recipient-name">
				{selectedDM?.name || `Welcome ${sender || ""}`}
			</div>
			
			<MessageList 
				messages={currentMessages} 
			/>
			
			{selectedDM ? (
				<ChatInput 
					ref={inputRef} 
					selectedDM={selectedDM}
					onSend={handleSend}
				/>
			) : (
				<div style={{ padding: "20px", color: "#666", textAlign: "center" }}>
					Select a chat room or open friends to talk!
				</div>
			)}
    	</div>
  	);
}