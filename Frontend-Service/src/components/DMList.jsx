export default function DMList({ dms, selectedDM, onSelect }) {
    return (
        // removed className="dm-list" and are just using flex styling to fill the space
        <div style={{ flex: 1, padding: "8px", overflowY: "auto", display: "flex", flexDirection: "column", gap: "2px" }}>
            {dms.map(dm => (
                <div 
                    key={dm.id} 
                    className={`dm-item ${selectedDM?.id === dm.id ? "active" : ""}`}
                    onClick={() => onSelect(dm)}
                >
                    <span style={{ color: "#80848e", marginRight: "6px" }}>#</span>
                    {typeof dm.name === 'string' && dm.name.trim() !== '' ? dm.name : "Unnamed Chat"}
                </div>
            ))}
        </div>
    );
}