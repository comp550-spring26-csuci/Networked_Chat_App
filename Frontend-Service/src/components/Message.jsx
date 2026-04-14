export default function Message({ message }) {
	return (
		<div className="message">
			<strong>{message.sender}</strong>
			<p>{message.text}</p>
		</div>
	)
}