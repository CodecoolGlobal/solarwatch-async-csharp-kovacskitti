
import React from "react";
import { Outlet, Link } from "react-router-dom";


const Layout = () => {
  return (
    <div className="Layout">
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

export default Layout;
