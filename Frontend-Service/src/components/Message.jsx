export default function Message({ message, hideHeader }) {
	const time = new Date(message.id).toLocaleTimeString([], {
		hour: "numeric",
		minute: "2-digit",
	});

	return (
		<div className={`message ${hideHeader ? "grouped" : ""}`}>
			{!hideHeader && (
				<div className="message-header">
					<strong>{message.sender}</strong>
					<span className="time">{time}</span>
				</div>
			)}
			<p className="message-text">
				{message.text}
			</p>
		</div>
	)
}