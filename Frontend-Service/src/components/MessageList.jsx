import Message from "./Message";

export default function MessageList({ messages }) {
	return (
		<div className="messages">
			{messages.map((msg) => (
        		<Message key={msg.id} message={msg} />
      		))}
		</div>
	)
}