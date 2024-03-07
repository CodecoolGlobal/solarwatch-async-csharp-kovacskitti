
import React from "react";
import { Outlet, Link } from "react-router-dom";


const Layout = () => {
  const getLogout =  () => {
        if (localStorage.getItem("userEmail")) {
           localStorage.removeItem("accessToken");
           localStorage.removeItem("userEmail");
           localStorage.removeItem("userName");
           console.log("removed items from localstorage");
           window.location.href = "/";
        } else {
          console.log("Token is not in the localStorage");
        }
      };
     
  
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
          <li>
            <Link to="/"  onClick={getLogout}>Logout</Link>
          </li>
        </ul>
      </nav>
      <Outlet />
    </div>
  );
};

export default Layout;
