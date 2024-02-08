import React, { useState } from "react";
import { useNavigate } from "react-router-dom";

const Profile = () => {
  const [username, setUsername] = useState("");
  const [email, setEmail] = useState("");


  return (
    <div className="profile-container">
      <h2>Profile</h2>
      <p>Username: {localStorage.userName}</p>
      <p>email: {localStorage.userEmail}</p>
      <p></p>
      <p></p>
    </div>
  );
};

export default Profile;
