import { Outlet, useNavigate, useLocation } from "react-router-dom";

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
					💬
				</button>

				<button
  					className={`sidebar-icon ${location.pathname === "/friends" ? "active" : ""}`}
  					onClick={() => navigate("/friends")}
				>
					👥
				</button>
			</div>

			<div className="main-view">
				<Outlet />
			</div>
		</div>
	);
}