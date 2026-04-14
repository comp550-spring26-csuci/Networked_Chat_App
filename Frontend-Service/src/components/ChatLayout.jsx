import { useState } from "react";
import DMList from "./DMList";
import ChatWindow from "./ChatWindow";

export default function ChatLayout() {
	// hard coded names, will later be done with api
  	const dms = [
    	{ id: 1, name: "Kenneth" },
    	{ id: 2, name: "Brielle" },
    	{ id: 3, name: "Ian" }
  	];
	// may have to change the start useState to null and then fetch DMs with api
	const [selectedDM, setSelectedDM] = useState(dms[0]);

  	return (
		<div className="chat-container">
      		<DMList 
        		dms={dms} 
        		selectedDM={selectedDM}
        		onSelect={setSelectedDM}
      		/>
      		<ChatWindow selectedDM={selectedDM} />
    	</div>
  	);
}