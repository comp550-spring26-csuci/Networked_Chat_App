import { Outlet, useNavigate, useLocation } from "react-router-dom";
import { useState } from "react";
import { MdEmail } from "react-icons/md";
import { FaUserFriends } from "react-icons/fa";

import FriendsList from "./FriendsList";

export default function AppLayout() {
	const navigate = useNavigate();
	const location = useLocation();

	const [friendsOpen, setFriendsOpen] = useState(false);

	return (
		<div className="app-shell">
			<div className="sidebar">
				{/* currrently the friends route doesnt exist on this branch so this will redirect you to nothing */}
				<button
					className="sidebar-icon"
					onClick={() => setFriendsOpen(true)}
				>
					<FaUserFriends size={30}/>
				</button>

				<FriendsList
					isOpen={friendsOpen}
					setIsOpen={setFriendsOpen}
				/>

				<button 
					className={`sidebar-icon ${location.pathname === "/chat" ? "active" : ""}`}
					onClick={() => navigate("/chat")}
				>
					<MdEmail size={30}/>
					<span className="badge">3</span>
				</button>
			</div>
			{/* all the content to the sight of the sidebar is rendered in Outlet (Possible routes are nested in AppLayout in App.jsx */}
			<div className="main-view">
				<Outlet />
			</div>
		</div>
	);
}