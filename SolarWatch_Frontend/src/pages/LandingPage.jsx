import React from "react";
import { Outlet, Link, useNavigate } from "react-router-dom";
import "../index.css";

const LandingPage = ({ children }) => {
 
  return (
    <div className="landing-page">
      <nav className="nav-container">
        <ul className="nav-list">
          <li>
            <Link to="/solarwatch">Solarwatch</Link>
          </li>
          <li>
            <Link to="/login">Login</Link>
          </li>
          <li>
            <Link to="/registration">Registration</Link>
          </li>
          <li>
            <Link to="/profile">Profile</Link>
          </li>
        </ul>
      </nav>
      <Outlet />
    </div>
  );
};

export default LandingPage;
