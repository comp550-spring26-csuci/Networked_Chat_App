import { Outlet, useNavigate, useLocation } from "react-router-dom";
import { MdEmail } from "react-icons/md";
import { FaUserFriends } from "react-icons/fa";
import { useEffect } from "react";
import FriendsList from "./FriendsList";
import { useState } from "react";
import { ensureSignalRConnection } from "../signalr/chatConnection";

export default function AppLayout() {
	const navigate = useNavigate();
	const location = useLocation();

	const [isFriendsListOpen, setIsFriendsListOpen] = useState(false);
    const [signalRReady, setSignalRReady] = useState(false);
	const [notificationCount, setNotificationCount] = useState(0);


	// Gates the app from trying to load before the reconnection is made
    useEffect(() => {
        let mounted = true;

        ensureSignalRConnection()
            .then(() => {
                if (mounted) setSignalRReady(true);
            })
            .catch(console.error);

        return () => {
            mounted = false;
        };
    }, []);

    if (!signalRReady) {
        return <div style={{ color: "white" }}>Connecting...</div>;
    }

	return (
		<div className="app-shell">
			<div className="sidebar">
				{/* currrently the friends route doesnt exist on this branch so this will redirect you to nothing */}
				<button
  					className={`sidebar-icon ${location.pathname === "/friends" ? "active" : ""}`}
  					onClick={() => setIsFriendsListOpen(true)}
				>
					<FaUserFriends size={30}/>
				</button>

				<FriendsList 
					isOpen={isFriendsListOpen}
					setIsOpen={setIsFriendsListOpen}
				/>

				<button 
					className={`sidebar-icon ${location.pathname === "/chat" ? "active" : ""}`}
					onClick={() => navigate("/chat")}
				>
					<MdEmail size={30}/>
					{notificationCount > 0 && (
						<span className="badge">{notificationCount}</span>
					)}
				</button>
			</div>
			{/* all the content to the sight of the sidebar is rendered in Outlet (Possible routes are nested in AppLayout in App.jsx */}
			<div className="main-view">
				<Outlet context={{ setNotificationCount }}/>
			</div>
		</div>
	);
}