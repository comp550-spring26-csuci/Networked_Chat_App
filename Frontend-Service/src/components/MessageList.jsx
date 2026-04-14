import Message from "./Message";

export default function MessageList({ messages }) {
	function shouldGroup(prev, current) {
		if (!prev) {
			return false;
		} 
		const sameUser = prev.sender === current.sender;
		// optional: time gap rule (set at .1 minutes or 6 seconds for in class demo)
		const timeDiff =
			current.id - prev.id < .1 * 60 * 1000;
		return sameUser && timeDiff;
	}

	return (
		<div className="messages">
			{messages.map((msg, index) => {
				const prev = messages[index - 1];
				const grouped = shouldGroup(prev, msg);
				return (
					<Message 
						key={msg.id} 
						message={msg} 
						hideHeader={grouped}
					/>
				);
			})}
		</div>
	)
}