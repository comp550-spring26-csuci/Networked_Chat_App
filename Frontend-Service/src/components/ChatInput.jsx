import { forwardRef } from "react"

const ChatInput = forwardRef (({selectedDM}, ref) => {
	return (
		<input 
			className="text-box" 
			ref={ref} 
			placeholder={`Message ${selectedDM.name}`} 
		/>
	)
})

export default ChatInput;