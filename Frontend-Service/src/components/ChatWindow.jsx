import { useEffect, useRef, useState } from "react";
import MessageList from "./MessageList";
import ChatInput from "./ChatInput";
import { FiMenu } from "react-icons/fi";
import { getConnection, TUNNEL_URL } from "../signalr/chatConnection";

export default function ChatWindow({ idToNameRef, selectedDM, sender, onManageGroup }) {
	const currentUser = JSON.parse(localStorage.getItem("user"));

	// Focuses input box when swapping DMs
	const inputRef = useRef(null);
	const [messages, setMessages] = useState({});
	const currentMessages = selectedDM
		? messages[selectedDM.id] || []
		: [];

	useEffect(() => {
		if(!selectedDM) return;
		async function fetchHistory() {
			const token = localStorage.getItem("access_token");
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
		const connection = getConnection();

		function handleReceiveMessage(payload) {
			const dmId = payload.message.chatRoomId;

			setMessages((prev) => ({
				...prev,
				[dmId]: [...(prev[dmId] || []), payload]
			}));

			connection.invoke(
				"MarkRoomAsRead", { 
					performChatRoomAction: { 
						chatRoomId: dmId
					} 
				}
			).catch(console.error);
		}

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
				{selectedDM?.name || `Welcome ${currentUser.username}`}
				{selectedDM && (
					<button 
						className="manage-group-btn"
						onClick={() => onManageGroup(selectedDM)}
					>
						<FiMenu />
					</button>
				)}
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