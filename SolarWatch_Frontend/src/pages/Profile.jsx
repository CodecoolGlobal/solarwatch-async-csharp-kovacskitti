import React, { useState, useEffect } from "react";

const Profile = () => {
  const [profile,setProfile] = useState ([]);
  const email = localStorage.getItem("userEmail");

     useEffect(() => {
       const fetchProfileData = async () => {
         if (email) {
           console.log("vmi");
           try {
             const response = await fetch(
               `http://localhost:5186/Auth/GetProfileData/${email}`,
               {
                 method: "GET",
                 headers: {
                   "Content-Type": "application/json",
                   Authorization: `Bearer ${localStorage.getItem(
                     "accessToken"
                   )}`,
                 },
               }
             );

             if (!response.ok) {
               console.error("Get user data failed:", response.statusText);
               return;
             }

             const data = await response.json();
             setProfile(data);
             console.log("Profile data ok");
             console.log("username:",profile[0]);
           } catch (error) {
             console.error("Error during profile data loading:", error);
           }
         }
       };

       fetchProfileData();
     }, [email]);

  return (
    <div className="profile-container">
      <h2>Profile</h2>
      <p>Username: {localStorage.userName}</p>
      <p>email: {localStorage.userEmail}</p>
      <ul>favourite city / cities:
        {profile.map((city)=>
        <li>
          {city}
        </li>)}
      </ul>
      <p></p>
    </div>
  );
};

export default Profile;
