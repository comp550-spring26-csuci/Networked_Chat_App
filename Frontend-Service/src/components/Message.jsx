export default function Message({ message, hideHeader }) {
	// This formats the timestamp similar to Discord
	// Messages from today have a timestamp like: 5:40 PM
	// Messages from yesterday have a timestamp like: Yesterday at 5:40 PM
	// Messages that are older have a timestamp like: 5/18/26 at 5:40 PM

	console.log(message.message.senderUsername);

	const msgDate = new Date(message.message.timestamp);
	const timeNow = new Date();

	const isToday = msgDate.toDateString() === timeNow.toDateString();
	const isYesterday = new Date(timeNow.setDate(timeNow.getDate() - 1)).toDateString() === msgDate.toDateString();

	const timePart = msgDate.toLocaleTimeString([], {
		hour: "numeric",
		minute: "2-digit",
	});

	let formattedTime;

	if (isToday) {
		formattedTime = timePart;
	} else if (isYesterday) {
		formattedTime = `Yesterday at ${timePart}`;
	} else {
		formattedTime = `${msgDate.toLocaleDateString([], {
			month: "numeric",
			day: "numeric",
			year: "2-digit",
		})}, ${timePart}`;
	}

	return (
		<div className={`message ${hideHeader ? "grouped" : ""}`}>
			{!hideHeader && (
				<div className="message-header">
					<strong>{message.message.senderUsername}</strong>
					<span className="time">{formattedTime}</span>
				</div>
			)}
			<p className="message-text">
				{message.message.content}
			</p>
		</div>
	)
}