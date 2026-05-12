import { Outlet, useNavigate } from "react-router-dom";

export default function AppLayout() {
	const navigate = useNavigate();

	return (
		<div className="app-shell">
			<div className="sidebar">
				<button onClick={() => navigate("/chat")}>
					Chats
				</button>

				<button /*onClick={() => navigate("/friends")}*/>
					Friends
				</button>
			</div>

			<div className="main-view">
				<Outlet />
			</div>
		</div>
	);
}