import Message from "./Message";

export default function MessageList({ messages }) {
	function shouldGroup(prev, current) {
		if (!prev) {
			return false;
		} 
		const sameUser = prev.sender === current.sender;
		// optional: time gap rule (currently set to group messages within 5 minutes of each other)
		const timeDiff =
			current.id - prev.id < 5 * 60 * 1000;
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