export default function DMList({ dms, selectedDM, onSelect, currentUser }) {
	const currentUsername = currentUser?.username || "";
  
	// Helper to strip the current user's name from direct message headers
	const formatRoomName = (roomName) => {
	  if (!roomName) return "Unknown Chat";
	  if (!currentUsername) return roomName;
  
	  const parts = roomName.split("-");
	  const filtered = parts.filter(p => p.toLowerCase() !== currentUsername.toLowerCase());
  
	  if (filtered.length === 0) return roomName; // fallback if chatting with yourself
	  return filtered.join(", ");
	};
  
	return (
	  <div className="dm-list">
		<div className="dm-list-header">
		  Direct Messages
		</div>
		{dms.map(dm => (
		  <div
			key={dm.id}
			className={`dm-item ${selectedDM?.id === dm.id ? "active" : ""}`}
			onClick={() => {
			  if (typeof onSelect === "function") {
				onSelect(dm);
			  }
			}}
		  >
			{formatRoomName(dm.name)}
		  </div>
		))}
	  </div>
	);
  }