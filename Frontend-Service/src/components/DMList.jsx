import { GoPlus } from "react-icons/go";

export default function DMList({ dms, selectedDM, onSelect, onCreateGroup }) {
  return (
    <div className="dm-list">
		<div className="dm-list-header">
			Direct Messages
			<button 
				className="create-dm-btn" 
				data-tooltip="Create DM"
				onClick={onCreateGroup}
			>
				<GoPlus size={20}/>
			</button>
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