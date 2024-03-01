import React, { useState, useEffect } from "react";

const Profile = () => {
  const [favouriteCities, setFavouriteCities] = useState([]);
  const email = localStorage.getItem("userEmail");

     useEffect(() => {
       const fetchFavouriteCities = async () => {
         if (email) {
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
             setFavouriteCities(data);
             console.log("Profile data ok");
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
      <ul>
        favourite city / cities:
        {favouriteCities.map((city) => (
          <li>{city}</li>
        ))}
      </ul>
      <p></p>
    </div>
  );
};

export default Profile;
