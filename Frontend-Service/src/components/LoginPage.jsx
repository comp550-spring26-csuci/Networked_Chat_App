import { useState } from "react";
import '../login.css'

import { useNavigate } from "react-router-dom";
import { startSignalRConnection } from "../signalr/chatConnection";

const BASE_URL = "https://vg3jzw0g-7081.usw3.devtunnels.ms";
 
const EyeIcon = ({ open }) => (
  <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
    {open ? (
      <>
        <path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"/>
        <circle cx="12" cy="12" r="3"/>
      </>
    ) : (
      <>
        <path d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94"/>
        <path d="M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19"/>
        <line x1="1" y1="1" x2="23" y2="23"/>
      </>
    )}
  </svg>
);
 
function Field({ label, type = "text", value, onChange, focused, onFocus, onBlur, showToggle, onToggle, showValue, error }) {
  return (
    <div className="field-group">
      <div className="password-label-row">
        <label className="label">{label}</label>
        {showToggle && (
          <button onClick={onToggle} className="show-btn">
            <EyeIcon open={showValue} />
            <span>{showValue ? "Hide" : "Show"}</span>
          </button>
        )}
      </div>
      <input
        type={showToggle ? (showValue ? "text" : "password") : type}
        value={value}
        onChange={e => onChange(e.target.value)}
        onFocus={onFocus}
        onBlur={onBlur}
        className={`input${focused ? " focused" : ""}${error ? " error" : ""}`}
      />
      {error && <span className="error-msg">{error}</span>}
    </div>
  );
}
 
const MONTHS = ["January","February","March","April","May","June","July","August","September","October","November","December"];
const currentYear = new Date().getFullYear();
const YEARS = Array.from({ length: 100 }, (_, i) => currentYear - i);
const DAYS = Array.from({ length: 31 }, (_, i) => i + 1);
 
function DateOfBirthField({ month, day, year, onChange, error }) {
  return (
    <div className="field-group">
      <label className="label">Date of Birth</label>
      <div className="dob-row">
        <select
          className={`input dob-select${error ? " error" : ""}`}
          value={month}
          onChange={e => onChange("month", e.target.value)}
        >
          <option value="">Month</option>
          {MONTHS.map((m, i) => <option key={m} value={i + 1}>{m}</option>)}
        </select>
        <select
          className={`input dob-select${error ? " error" : ""}`}
          value={day}
          onChange={e => onChange("day", e.target.value)}
        >
          <option value="">Day</option>
          {DAYS.map(d => <option key={d} value={d}>{d}</option>)}
        </select>
        <select
          className={`input dob-select${error ? " error" : ""}`}
          value={year}
          onChange={e => onChange("year", e.target.value)}
        >
          <option value="">Year</option>
          {YEARS.map(y => <option key={y} value={y}>{y}</option>)}
        </select>
      </div>
      {error && <span className="error-msg">{error}</span>}
    </div>
  );
}
 
function calculateAge(month, day, year) {
  const today = new Date();
  const dob = new Date(year, month - 1, day);
  let age = today.getFullYear() - dob.getFullYear();
  const m = today.getMonth() - dob.getMonth();
  if (m < 0 || (m === 0 && today.getDate() < dob.getDate())) age--;
  return age;
}
 
export default function LoginPage() {
  const navigate = useNavigate();

  const [mode, setMode] = useState("login");
 
  // Login fields
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  const [usernameFocused, setUsernameFocused] = useState(false);
  const [passwordFocused, setPasswordFocused] = useState(false);
  const [loginError, setLoginError] = useState("");
 
  // Sign up fields
  const [suUsername, setSuUsername] = useState("");
  const [suEmail, setSuEmail] = useState("");
  const [suPassword, setSuPassword] = useState("");
  const [suConfirm, setSuConfirm] = useState("");
  const [showSuPassword, setShowSuPassword] = useState(false);
  const [showSuConfirm, setShowSuConfirm] = useState(false);
  const [suUsernameFocused, setSuUsernameFocused] = useState(false);
  const [suEmailFocused, setSuEmailFocused] = useState(false);
  const [suPasswordFocused, setSuPasswordFocused] = useState(false);
  const [suConfirmFocused, setSuConfirmFocused] = useState(false);
  const [dob, setDob] = useState({ month: "", day: "", year: "" });
  const [dobError, setDobError] = useState("");
  const [confirmError, setConfirmError] = useState("");
  const [signupError, setSignupError] = useState("");
  const [loading, setLoading] = useState(false);
 
  const handleDobChange = (field, value) => {
    setDob(prev => ({ ...prev, [field]: value }));
    setDobError("");
  };
 
  const switchMode = (next) => {
    setMode(next);
    setConfirmError("");
    setLoginError("");
    setSignupError("");
    setDobError("");
  };

  const onLoginSuccess = async () => {
    try {
      console.log("Name:", username);
      const seedRes = await fetch(
        `https://sslk8rt0-7081.usw3.devtunnels.ms/api/testdm/seed-user?UserName=${username}&OverWrite=false`, {
        method: 'POST'
      });

      const data = await seedRes.json();
      console.log("SignalR login response:", data);

      // Store SignalR token
      localStorage.setItem("access_token", data.token);

      if(!seedRes.ok) {
        setLoginError("Failed to initialize chat user for SignalR");
        return;
      }

      await startSignalRConnection();

      // Redirect to chat
      navigate("/chat");

    } catch(err) {
      setLoginError("Could not reach the SignalR server");
    }
  };
 
  const handleSubmit = async () => {
    if (mode === "login") {
      if (!username.trim()) { setLoginError("Username is required."); return; }
	    else { onLoginSuccess(); console.log(`TESTING USER: ${username}`); localStorage.setItem("username", username); } // FOR FRONTEND TESTING ONLY
      if (!password) { setLoginError("Password is required."); return; }
      
      setLoading(true);
      setLoginError("");

      try {
        const res = await fetch(`${BASE_URL}/api/users/login`, {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({ username, password }),
        });
        const data = await res.json();
        
        if (res.ok) {
          alert(`Welcome back, ${username}!`);

          // 2. Store the username
          localStorage.setItem("username", data.username);
          // Can store tokens here or status information
          // 3. Start SignalR process after login works
          onLoginSuccess();
		      
          console.log("User ID:", data.userId);
          localStorage.setItem("access_token", data.token); 
          localStorage.setItem("userId", data.userId);
          
          // --- pass status and customText up to the main app layout ---
          onLogin({ 
            username, 
            token: data.token, 
            userId: data.userId,
            status: data.status,
            customText: data.customText || data.customStatus 
          }); 
        } else {
          setLoginError(data.message || "Invalid username or password.");
        }
      } catch (err) {
        setLoginError("Could not reach the server. Please try again.");
      } finally {
        setLoading(false);
      }
      
    } else {
      // Validate all fields filled
      if (!suUsername.trim()) { setSignupError("Username is required."); return; }
      if (!suEmail.trim()) { setSignupError("Email is required."); return; }
      if (!suPassword) { setSignupError("Password is required."); return; }
      if (!suConfirm) { setSignupError("Please confirm your password."); return; }
      if (!dob.month || !dob.day || !dob.year) { setDobError("Please select your full date of birth."); return; }
      if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(suEmail)) { setSignupError("Please enter a valid email address."); return; }
 
      if (suPassword !== suConfirm) {
        setConfirmError("Passwords do not match.");
        return;
      }
      const age = calculateAge(dob.month, dob.day, dob.year);
      if (age < 18) {
        setDobError("You must be at least 18 years old to sign up.");
        return;
      }
 
      setLoading(true);
      setSignupError("");
      try {
        const res = await fetch(`${BASE_URL}/api/users/create-account`, {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({
            username: suUsername,
            email: suEmail,
            password: suPassword,
            age,
          }),
        });
        const data = await res.json();
        if (res.ok) {
          alert(`Account created! Welcome, ${suUsername}. You can now log in.`);
          switchMode("login");
        } else {
          setSignupError(data.message || "Something went wrong. Please try again.");
        }
      } catch (err) {
        setSignupError("Could not reach the server. Please try again.");
      } finally {
        setLoading(false);
      }
    }
  };
 
  return (
    <div className="bg">
      <div className="card">
        <div className="glow-border" />
 
        <h1 className="title">{mode === "login" ? "Login" : "Sign Up"}</h1>
 
        <p className="subtext">
          {mode === "login" ? (
            <>Need an account?{" "}
              <span className="link" onClick={() => switchMode("signup")}>Create one!</span>
            </>
          ) : (
            <>Already have an account?{" "}
              <span className="link" onClick={() => switchMode("login")}>Log in!</span>
            </>
          )}
        </p>
 
        {mode === "login" ? (
          <>
            <Field
              label="Username"
              value={username}
              onChange={setUsername}
              focused={usernameFocused}
              onFocus={() => setUsernameFocused(true)}
              onBlur={() => setUsernameFocused(false)}
            />
            <Field
              label="Password"
              value={password}
              onChange={setPassword}
              focused={passwordFocused}
              onFocus={() => setPasswordFocused(true)}
              onBlur={() => setPasswordFocused(false)}
              showToggle
              showValue={showPassword}
              onToggle={() => setShowPassword(v => !v)}
            />
            <span className="forgot-link">Forgot username or password?</span>
            {loginError && <span className="error-msg">{loginError}</span>}
          </>
        ) : (
          <>
            <Field
              label="Username"
              value={suUsername}
              onChange={setSuUsername}
              focused={suUsernameFocused}
              onFocus={() => setSuUsernameFocused(true)}
              onBlur={() => setSuUsernameFocused(false)}
            />
            <Field
              label="Email"
              type="email"
              value={suEmail}
              onChange={setSuEmail}
              focused={suEmailFocused}
              onFocus={() => setSuEmailFocused(true)}
              onBlur={() => setSuEmailFocused(false)}
            />
            <Field
              label="Password"
              value={suPassword}
              onChange={setSuPassword}
              focused={suPasswordFocused}
              onFocus={() => setSuPasswordFocused(true)}
              onBlur={() => setSuPasswordFocused(false)}
              showToggle
              showValue={showSuPassword}
              onToggle={() => setShowSuPassword(v => !v)}
            />
            <Field
              label="Confirm Password"
              value={suConfirm}
              onChange={(v) => { setSuConfirm(v); setConfirmError(""); }}
              focused={suConfirmFocused}
              onFocus={() => setSuConfirmFocused(true)}
              onBlur={() => setSuConfirmFocused(false)}
              showToggle
              showValue={showSuConfirm}
              onToggle={() => setShowSuConfirm(v => !v)}
              error={confirmError}
            />
            <DateOfBirthField
              month={dob.month}
              day={dob.day}
              year={dob.year}
              onChange={handleDobChange}
              error={dobError}
            />
            {signupError && <span className="error-msg">{signupError}</span>}
          </>
        )}
 
        <button onClick={handleSubmit} className="login-btn" disabled={loading}>
          {loading ? "Please wait..." : mode === "login" ? "Login" : "Create Account"}
        </button>
      </div>
    </div>
  );
}