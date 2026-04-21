import Message from "./Message";

export default function MessageList({ messages }) {
	function shouldGroup(prev, current) {
		if (!prev) {
			return false;
		}
		const sameUser = prev.senderUsername === current.senderUsername;
		// optional: time gap rule (currently set to group messages within 5 minutes of each other)
		const timeDiff =
			new Date(current.message.timestamp) - new Date(prev.message.timestamp) < 5 * 60 * 1000;

		return sameUser && timeDiff;
	}

	return (
		<div className="messages">
			{messages.map((msg, index) => {
				const prev = messages[index - 1];
				const grouped = shouldGroup(prev, msg);
				return (
					<Message 
						key={msg.message.timestamp} 
						message={msg} 
						hideHeader={grouped}
					/>
				);
			})}
		</div>
	)
}