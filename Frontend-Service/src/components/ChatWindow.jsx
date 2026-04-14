import { useEffect, useRef } from "react";
import MessageList from "./MessageList";
import ChatInput from "./ChatInput";

export default function ChatWindow({ selectedDM }) {
	/* Focuses input box when swapping DMs */
	const inputRef = useRef(null);
	useEffect(() => {
		if(selectedDM) {
			inputRef.current?.focus();
		}
	}, [selectedDM]);

  	return (
    	<div className="chat-window">
    		<div className="recipient-name">
				{selectedDM.name}
			</div>
      		<MessageList selectedDM={selectedDM} />
			<ChatInput 
				ref={inputRef} 
				selectedDM={selectedDM} 
			/>
    	</div>
  	);
}