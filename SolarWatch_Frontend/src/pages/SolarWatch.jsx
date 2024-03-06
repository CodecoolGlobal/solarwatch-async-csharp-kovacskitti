import React, { useState, useEffect } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faHeart } from "@fortawesome/free-solid-svg-icons";
import LineChart from "../Components/LineChart.jsx";
import { jwtDecode } from "jwt-decode";
import Popup from "reactjs-popup";
import "reactjs-popup/dist/index.css";

const SolarWatch = () => {
  const [city, setCity] = useState("");
  const [date, setDate] = useState("");
  const [solarWatchData, setSolarWatchData] = useState("");
  const [favouriteCities, setFavouriteCities] = useState([]);
  const [location, setLocation] = useState("");
  const [popupMessage,setPopupMessage] = useState(false)

  const email = localStorage.getItem("userEmail");
  const token = localStorage.getItem("accessToken");
  
  useEffect(() => {
    const checkTokenValidity = async () => {;
      if (email) {
        const decodedToken = jwtDecode(token);
        const currentTime = Date.now() / 1000;
                
          if (decodedToken.exp < currentTime) {
            console.log("The token is invalid");
            localStorage.removeItem("accessToken");
            localStorage.removeItem("email");
          } else {
            console.log("Token is valid");
          }
        } else {
          console.log("Token is not in the localStorage");
        }
      };
    checkTokenValidity();
  }, [email]);

  useEffect(() => {
    const getLocation = () => {
      if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(
          async (position) => {
            const latitude = position.coords.latitude;
            const longitude = position.coords.longitude;
            try {
              const response = await fetch(
                `http://localhost:5186/User/GetCurrentCity/${latitude}&${longitude}`,
              );
              const data = await response.json();
              setLocation(data.cityName);
            } catch (error) {
              console.error("Error fetching geolocation data:", error);
            }
          },
          (error) => {
            console.error("Error during geolocation:", error);
          }
        );
      } else {
        console.error("The browser doesn't support the geolocation.");
      }
    };

    getLocation();
  }, []);

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
            console.error("Get user's favourite cities failed:", response.statusText);
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
  }, []);

  const handleSolarwatch = async (e) => {
    e.preventDefault();
    try {
      const response = await fetch(
        `http://localhost:5186/SolarWatch/GetInfoToSolarWatch?currentDate=${date}&location=${city}`,
        {
          method: "GET",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${localStorage.getItem("accessToken")}`,
          },
        }
      );

      if (!response.ok) {
        console.error("Search failed:", response.statusText);
        return;
      }

      const data = await response.json();
      setSolarWatchData(data);
    } catch (error) {
      console.error(error.message);
    }
  };

  const addFavourite = async () => {
    if (token) {
      const response = await fetch(
        "http://localhost:5186/User/AddFavouriteCity",
        {
          method: "PATCH",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
          },
          body: JSON.stringify({
            Location: city,
            UserEmail: localStorage.getItem("userEmail"),
          }),
        }
      );
      const responseData = await response.json();
      if (response.status == 400) {
        console.error("Search failed:", responseData.Message);
        setPopupMessage(responseData.message);
        return;
      }
      setPopupMessage(`${city} has been added to your list of favorite cities`);
    } else {
      setPopupMessage("Please log in to use this function!");
    }
  };

  return (
    <div className="basic-container">
      <h2>Solar Watch</h2>
      <ul className="city-list">
        <li>
          <button onClick={(e) => setCity(location)}>{location}</button>
        </li>
        {favouriteCities.map((city) => (
          <li>
            <button onClick={(e) => setCity(city)}>{city}</button>
          </li>
        ))}
      </ul>
      <form onSubmit={handleSolarwatch}>
        <label>
          City:
          <input
            type="string"
            value={city}
            onChange={(e) => setCity(e.target.value)}
            required
          />
         <Popup trigger= 
          {<button type="button">
            <FontAwesomeIcon icon={faHeart} onClick={addFavourite} />
          </button>}
          position="right center">
          <div>{popupMessage}</div>
          </Popup>
        </label>
        <label>
          Date:
          <input
            type="date"
            value={date}
            onChange={(e) => setDate(e.target.value)}
            required
          />
        </label>
        <button type="submit">Search</button>
      </form>
      {solarWatchData && (
        <div>
          <h3>Search Results</h3>
          <p>Date: {solarWatchData.date}</p>
          <p>Sunrise: {solarWatchData.sunrise}</p>
          <p>Sunset: {solarWatchData.sunset}</p>
          <p>City: {solarWatchData._City.name}</p>
          <h2>The time of sunrise after {date}</h2>
          <LineChart date={date} city={city} />
        </div>
      )}
    </div>
  );
};

export default SolarWatch;
