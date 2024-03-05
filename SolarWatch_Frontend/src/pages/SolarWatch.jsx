import React, { useState, useEffect } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faHeart } from "@fortawesome/free-solid-svg-icons";
import LineChart from "../Components/LineChart.jsx";

const SolarWatch = () => {
  const [city, setCity] = useState("");
  const [date, setDate] = useState("");
  const [solarWatchData, setSolarWatchData] = useState("");
  const [profile, setProfile] = useState([]);
  const [location, setLocation] = useState("");

  const email = localStorage.getItem("userEmail");
  const APIkey = "8c4342c0a59c4611957d1347bb011688";

  useEffect(() => {
    const getLocation = () => {
      if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(
          async (position) => {
            const latitude = position.coords.latitude;
            const longitude = position.coords.longitude;
            try {
              const response = await fetch(
                `https://api.opencagedata.com/geocode/v1/json?key=${APIkey}&q=${latitude}+${longitude}&language=en&pretty=1`
              );
              const data = await response.json();

              if (data.results && data.results.length > 0) {
                const city = data.results[0].components.city;
                const country = data.results[0].components.country;
                setLocation(city);
              } else {
                console.error("No location information found.");
              }
            } catch (error) {
              console.error("Error fetching location data:", error);
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
            console.error("Get user data failed:", response.statusText);
            return;
          }

          const data = await response.json();
          setProfile(data);
          console.log("Profile data ok");
        } catch (error) {
          console.error("Error during profile data loading:", error);
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
    const response = await fetch(
      "http://localhost:5186/User/AddFavouriteCity",
      {
        method: "PATCH",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${localStorage.getItem("accessToken")}`,
        },
        body: JSON.stringify({
          Location: city,
          UserEmail: localStorage.getItem("userEmail"),
        }),
      }
    );
  };

  return (
    <div className="basic-container">
      <h2>Solar Watch</h2>
      <ul className="city-list">
        <li>
          <button onClick={(e) => setCity(location)}>{location}</button>
        </li>
        {profile.map((city) => (
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
          <button type="button">
            <FontAwesomeIcon icon={faHeart} onClick={addFavourite} />
          </button>
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
