import { useEffect, useRef, useState } from "react";
import MessageList from "./MessageList";
import ChatInput from "./ChatInput";

export default function ChatWindow({ selectedDM }) {
	// Focuses input box when swapping DMs
	const inputRef = useRef(null);
	const [messages, setMessages] = useState({});

	useEffect(() => {
		if(selectedDM) {
			inputRef.current?.focus();
		}
	}, [selectedDM]);

	const handleSend = (text) => {
    	console.log(`Send message:\n${text}`);
		const newMessage = {
      		id: Date.now(),
      		text,
      		sender: "eidolon" // Placeholder for now
    	};
    	// Later: update state / backend / socket

		setMessages((prev) => {
      		const dmId = selectedDM.id;

      		return {
        		...prev,
        		[dmId]: [...(prev[dmId] || []), newMessage]
      		};
    	});
  	};

	const currentMessages = messages[selectedDM.id] || [];

  	return (
    	<div className="chat-window">
    		<div className="recipient-name">
				{selectedDM.name}
			</div>
      		<MessageList messages={currentMessages} />
			<ChatInput 
				ref={inputRef} 
				selectedDM={selectedDM}
				onSend={handleSend}
			/>
    	</div>
  	);
}