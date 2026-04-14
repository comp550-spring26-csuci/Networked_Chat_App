import { forwardRef } from "react"

const ChatInput = forwardRef (({selectedDM, onSend}, ref) => {
	const handleSend = () => {
 		const value = ref.current.innerText;
		if (!value.trim()) {
			return;
		}
		onSend(value);
		// Clear text box after send
		ref.current.innerText = "";
	};

	// Shift + Enter won't send message but will add a newline
	const handleKeyDown = (e) => {
    	if (e.key === "Enter" && !e.shiftKey) {
      		e.preventDefault();
      		handleSend();
    	}
  	};

	return (
		<div
			className="text-box" 
			contentEditable
			ref={ref}
      		onKeyDown={handleKeyDown}
			data-placeholder={`Message ${selectedDM.name}`}
		/>
	)
})

export default ChatInput;