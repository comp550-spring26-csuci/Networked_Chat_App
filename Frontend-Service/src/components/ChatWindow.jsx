import { useEffect, useRef } from "react";

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
      		<div className="messages">
        		{/* messages will be populated here */}
      		</div>
      		<input className="text-box" ref={inputRef} placeholder={`Message ${selectedDM.name}`} />
    	</div>
  	);
}