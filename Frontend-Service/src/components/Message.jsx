export default function Message({ message }) {
	const time = new Date(message.id).toLocaleTimeString([], {
		hour: "numeric",
		minute: "2-digit",
	});

	return (
		<div className="message">
			<div className="message-header">
				<strong>{message.sender}</strong>
				<span className="time">{time}</span>
			</div>
			<p className="message-text">{message.text}</p>
		</div>
	)
}