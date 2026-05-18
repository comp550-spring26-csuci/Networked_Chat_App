import { useState } from "react";
import { Outlet, useNavigate, useLocation } from "react-router-dom";
import { MdEmail } from "react-icons/md";
import { FaUserFriends } from "react-icons/fa";

export default function AppLayout() {
	const navigate = useNavigate();
	const location = useLocation();
	
	// 1. Create the state here in the parent shell
	const [isFriendsOpen, setIsFriendsOpen] = useState(false);

	return (
		<div className="app-shell">
			<div className="sidebar">
				<button
					// Highlight the icon if the drawer is open
					className={`sidebar-icon ${isFriendsOpen ? "active" : ""}`}
					onClick={() => {
						navigate("/chat"); // Ensure we are on the chat route
						setIsFriendsOpen(!isFriendsOpen); // Toggle the drawer
					}}
				>
					<FaUserFriends size={30}/>
				</button>

				<button 
					className={`sidebar-icon ${location.pathname === "/chat" && !isFriendsOpen ? "active" : ""}`}
					onClick={() => {
						setIsFriendsOpen(false); // Close drawer if they click messages
						navigate("/chat");
					}}
				>
					<MdEmail size={30}/>
					<span className="badge">3</span>
				</button>
			</div>
			<div className="main-view">
				{/* 2. Pass the state down to any nested routes! */}
				<Outlet context={{ isFriendsOpen, setIsFriendsOpen }} />
			</div>
		</div>
	);
}