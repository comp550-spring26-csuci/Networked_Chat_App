import { useEffect, useRef, useState } from "react";
import MessageList from "./MessageList";
import ChatInput from "./ChatInput";

export default function ChatWindow({ selectedDM, sender }) {
	// Focuses input box when swapping DMs
	const inputRef = useRef(null);
	const [messages, setMessages] = useState({});

	// Will need to fetch the messages for the chat window from the backend

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
      		sender: sender
    	};
    	// Later: update state / backend / socket when sending a message
		// When exactly would we ask the backend for updated messages when another user sends a message?

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