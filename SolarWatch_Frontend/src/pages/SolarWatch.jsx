import React, { useState } from "react";
import { useNavigate } from "react-router-dom";

const SolarWatch = () => {
  const [city, setCity] = useState("");
  const [date, setDate] = useState("");
  const [solarWatchData,setSolarWatchData] = useState("")
  const navigate = useNavigate();

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
        console.error("Login failed:", response.statusText);
        return;
      }

      const data = await response.json();
      setSolarWatchData(data)
      console.log("Login successful");
      console.log("User Email:", data.SolarWatch);
      console.log("User Token:", data);

    } catch (error) {
      console.error("Error during login:", error);
    }
  };

  return (
    <div className="solarwatch-container">
      <h2>Solar Watch</h2>
      <form onSubmit={handleSolarwatch}>
        <label>
          City:
          <input
            type="string"
            value={city}
            onChange={(e) => setCity(e.target.value)}
            required
          />
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
          <h3>{solarWatchData._City.name}</h3>
          <p>Date: {solarWatchData.date}</p>
          <p>Sunrise: {solarWatchData.sunrise}</p>
          <p>Sunset: {solarWatchData.sunset}</p>
        </div>
      )}
    </div>
  );
};

export default SolarWatch;
