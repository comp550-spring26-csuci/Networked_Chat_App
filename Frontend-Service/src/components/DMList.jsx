export default function DMList({ dms, selectedDM, onSelect }) {
  return (
    <div className="dm-list">
		<div className="dm-list-header">
			Direct Messages
		</div>
    	{dms.map(dm => (
    		<div
        		key={dm.id}
          		className={`dm-item ${selectedDM?.id === dm.id ? "active" : ""}`}
          		onClick={() => onSelect(dm)}
        	>
        		{dm.name}
    		</div>
    	))}
    </div>
  );
}