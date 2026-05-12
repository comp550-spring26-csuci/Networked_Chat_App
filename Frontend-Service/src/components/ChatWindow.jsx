import { useEffect, useRef, useState } from "react";
import MessageList from "./MessageList";
import ChatInput from "./ChatInput";
import { getConnection } from "../signalr/chatConnection";

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
			const res = await fetch(`https://sslk8rt0-7081.usw3.devtunnels.ms/api/chathistory/room/${selectedDM.id}/messages`, {
				method: 'GET',
			});

			const data = await res.json();
			const enriched = data.map(msg => ({
				message: {
					...msg
				},
				senderUsername: idToNameRef.current[msg.senderId],
				chatRoomName: selectedDM.name
			}));

			setMessages((prev) => ({
				...prev,
				[selectedDM.id]: enriched
			}));
		}

		//fetchHistory();
		inputRef.current?.focus();
	}, [selectedDM]);

	// useEffect(() => {
	// 	function handleReceiveMessage(paylode) {
	// 		const DMId = paylode.message.chatRoomId;

	// 		setMessages((prev) => ({
	// 			...prev,
	// 			[DMId]: [...(prev[DMId] || []), paylode]
	// 		}));
	// 	}

	// 	const connection = getConnection();

	// 	connection.on("ReceiveMessage", handleReceiveMessage);

	// 	return () => {
	// 		connection.off("ReceiveMessage", handleReceiveMessage);
	// 	};
	// }, [selectedDM]);

	const handleSend = async (text) => {
		const DMId = selectedDM.id;
		try {
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