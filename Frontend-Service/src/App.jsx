import { useState } from "react";
import "./login.css";

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
 
export default function LoginPage() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  const [emailFocused, setEmailFocused] = useState(false);
  const [passwordFocused, setPasswordFocused] = useState(false);
  const [mode, setMode] = useState("login");
 
  const handleSubmit = () => {
    alert(`${mode === "login" ? "Logging in" : "Signing up"} with: ${email}`);
  };
 
  return (
    <div className="bg">
      <div className="card">
        <div className="glow-border" />
 
        <h1 className="title">{mode === "login" ? "Login" : "Sign Up"}</h1>
 
        <p className="subtext">
          {mode === "login" ? (
            <>Need an account?{" "}
              <span className="link" onClick={() => setMode("signup")}>Create one!</span>
            </>
          ) : (
            <>Already have an account?{" "}
              <span className="link" onClick={() => setMode("login")}>Log in!</span>
            </>
          )}
        </p>
 
        <div className="field-group">
          <label className="label">Username/Email</label>
          <input
            type="text"
            value={email}
            onChange={e => setEmail(e.target.value)}
            onFocus={() => setEmailFocused(true)}
            onBlur={() => setEmailFocused(false)}
            className={`input${emailFocused ? " focused" : ""}`}
          />
        </div>
 
        <div className="field-group">
          <div className="password-label-row">
            <label className="label">Password</label>
            <button onClick={() => setShowPassword(v => !v)} className="show-btn">
              <EyeIcon open={showPassword} />
              <span>{showPassword ? "Hide" : "Show"}</span>
            </button>
          </div>
          <input
            type={showPassword ? "text" : "password"}
            value={password}
            onChange={e => setPassword(e.target.value)}
            onFocus={() => setPasswordFocused(true)}
            onBlur={() => setPasswordFocused(false)}
            className={`input${passwordFocused ? " focused" : ""}`}
          />
          {mode === "login" && (
            <span className="forgot-link">Forgot username or password?</span>
          )}
        </div>
 
        <button onClick={handleSubmit} className="login-btn">
          {mode === "login" ? "Login" : "Create Account"}
        </button>
      </div>
    </div>
  );
}


