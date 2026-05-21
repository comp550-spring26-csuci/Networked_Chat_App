import { GoPlus } from "react-icons/go";

export default function DMList({ dms, selectedDM, onSelect }) {
  return (
    <div className="dm-list">
		<div className="dm-list-header">
			Direct Messages
			<button className="create-dm-btn" data-tooltip="Create DM">
				<GoPlus size={20}/>
			</button>
		</div>
    	{dms.map(dm => (
    		<div
				// TODO: Remove the user's name from the dm name so the name of the dm is the names of the other people in the chatroom
				// Example: For ian, make ian-kenneth -> kenneth, and ian-brielle-kenneth -> brielle, kenneth
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