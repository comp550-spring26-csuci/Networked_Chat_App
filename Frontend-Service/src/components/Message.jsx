export default function Message({ message, hideHeader }) {
	console.log("MSG in message:", message);
	console.log(`MSG SENDER in message: ${message.message.senderUsername}`);

	const time = new Date(message.message.timestamp).toLocaleTimeString([], {
		hour: "numeric",
		minute: "2-digit",
	});

	return (
		<div className={`message ${hideHeader ? "grouped" : ""}`}>
			{!hideHeader && (
				<div className="message-header">
					<strong>{message.message.senderUsername}</strong>
					<span className="time">{time}</span>
				</div>
			)}
			<p className="message-text">
				{message.message.content}
			</p>
		</div>
	)
}