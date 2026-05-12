import { Outlet, useNavigate, useLocation } from "react-router-dom";
import { MdEmail } from "react-icons/md";
import { FaUserFriends } from "react-icons/fa";

export default function AppLayout() {
	const navigate = useNavigate();
	const location = useLocation();

	return (
		<div className="app-shell">
			<div className="sidebar">
				<button 
					className={`sidebar-icon ${location.pathname === "/chat" ? "active" : ""}`}
					onClick={() => navigate("/chat")}
				>
					<MdEmail size={30}/>
				</button>

				<button
  					className={`sidebar-icon ${location.pathname === "/friends" ? "active" : ""}`}
  					onClick={() => navigate("/friends")}
				>
					<FaUserFriends size={30}/>
				</button>
			</div>

			<div className="main-view">
				<Outlet />
			</div>
		</div>
	);
}