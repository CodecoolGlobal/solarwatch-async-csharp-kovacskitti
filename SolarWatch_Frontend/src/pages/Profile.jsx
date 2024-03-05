import React, { useState, useEffect } from "react";

const Profile = () => {
  const [favouriteCities, setFavouriteCities] = useState([]);
  const email = localStorage.getItem("userEmail");

  useEffect(() => {
    const fetchFavouriteCities = async () => {
      if (email) {
        try {
          const response = await fetch(
            `http://localhost:5186/User/GetFavouriteCities/${email}`,
            {
              method: "GET",
              headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${localStorage.getItem("accessToken")}`,
              },
            }
          );

          if (!response.ok) {
             console.error(
               "Get user's favourite cities failed:",response.statusText
             );
            return;
          }

          const data = await response.json();
          setFavouriteCities(data);
          console.log("User's favourite cities ok");
        } catch (error) {
          console.error("Error during user's favourite cities loading:", error);
        }
      }
    };

    fetchFavouriteCities();
  }, [email]);

  return (
    <div className="profile-container">
      <h2>Profile</h2>
      <table className="profile-table">
        <tbody>
          <tr>
            <td>Username:</td>
            <td>{localStorage.userName}</td>
          </tr>
          <tr>
            <td>email:</td>
            <td>{localStorage.userEmail} </td>
          </tr>
          <tr>
            <td> favourite city / cities:</td>
            <td>
              <ul>
                {favouriteCities.map((city) => (
                  <li>{city}</li>
                ))}
              </ul>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  );
};

export default Profile;
