import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import  Chart from "chart.js/auto";
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faHeart } from "@fortawesome/free-solid-svg-icons";

const SolarWatch = () => {
  const [city, setCity] = useState("");
  const [date, setDate] = useState("");
  const [solarWatchData, setSolarWatchData] = useState("");
  const [chartData, setChartData] = useState(null);
  const [chartInstance, setChartInstance] = useState(null);
  const [favourite, setFavourite] = useState("");
  const [profile, setProfile] = useState([]);
  const [location, setLocation] = useState("");

  const email = localStorage.getItem("userEmail");

  const APIkey = "8c4342c0a59c4611957d1347bb011688";
  console.log(email);

  useEffect(() => {
    const getLocation = () => {
      if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(
          async (position) => {
            const latitude = position.coords.latitude;
            const longitude = position.coords.longitude;
            console.log(longitude);
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
              console.error("'Error fetching location data:", error);
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
          console.log("username:", profile[0]);
        } catch (error) {
          console.error("Error during profile data loading:", error);
        }
      }
    };

    fetchProfileData();
  }, []);

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
        console.error("Search failed:", response.statusText);
        return;
      }

      const data = await response.json();
      setSolarWatchData(data);

      const next3DaysData = await Promise.all(
        Array.from({ length: 3 }, (_, i) => {
          const nextDay = new Date(
            new Date(date).getTime() + (i + 1) * 24 * 60 * 60 * 1000
          );
          const nextDayFormatted = nextDay.toISOString().split("T")[0];

          return fetch(
            `http://localhost:5186/SolarWatch/GetInfoToSolarWatch?currentDate=${nextDayFormatted}&location=${city}`,
            {
              method: "GET",
              headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${localStorage.getItem("accessToken")}`,
              },
            }
          )
            .then((response) => {
              if (!response.ok) {
                throw new Error(`Error fetching data for ${nextDayFormatted}`);
              }
              return response.json();
            })
            .catch((error) => {
              console.error(error.message);
              return null;
            });
        })
      );

      const chartLabels = ["Sunrise"];
      const chartData = [parseTimeToMinutes(data.sunrise)];
      console.log(data.sunrise);
      next3DaysData.forEach((dayData) => {
        if (dayData) {
          const timeValue = parseTimeToMinutes(dayData.sunrise);
          chartLabels.push(`${dayData.date}`);
          chartData.push(timeValue);
        }
      });

      setChartData({
        labels: chartLabels,
        datasets: [
          {
            label: "Sunrise",
            data: chartData,
            fill: false,
            backgroundColor: "rgba(75,192,192,0.2)",
            borderColor: "rgba(75,192,192,1)",
          },
        ],
      });

      const currentChartInstance = chartInstance;
      if (currentChartInstance) {
        currentChartInstance.destroy();
      }

      console.log("Search successful");
      console.log("currentDayData", data.SolarWatch);
      console.log("next3DaysData", next3DaysData);
    } catch (error) {
      console.error("Error during search:", error);
    }
  };

  const addFavourite = async () => {
    setFavourite(city);
    console.log(city);
    const response = await fetch(
      "http://localhost:5186/Auth/AddFavouriteCity",
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

  useEffect(() => {
    if (chartData) {
      const newChartInstance = new Chart(
        document.getElementById("solarWatchChart"),
        {
          type: "line",
          data: chartData,
          options: {
            scales: {
              x: {
                display: true,
                title: {
                  display: true,
                  text: "Date",
                  color: "black",
                },
                ticks: {
                  color: "black",
                },
              },
              y: {
                display: true,
                title: {
                  display: true,
                  text: "Sunrise Time",
                  color: "black",
                },
                ticks: {
                  callback: function (value, index, values) {
                    const hours = Math.floor(value / 60);
                    const minutes = value % 60;
                    const formattedTime = `${String(hours).padStart(
                      2,
                      "0"
                    )}:${String(minutes).padStart(2, "0")}`;
                    return formattedTime;
                  },
                  color: "black",
                  stepSize: 1,
                },
              },
            },
          },
        }
      );
      setChartInstance(newChartInstance);
    }
  }, [chartData]);

  function parseTimeToMinutes(timeString) {
    const parts = timeString.match(/(\d+):(\d+):(\d+) (AM|PM)/);
    if (!parts) return 0;

    let hours = parseInt(parts[1]);
    const minutes = parseInt(parts[2]);
    const seconds = parseInt(parts[3]);
    const period = parts[4];

    if (period === "PM" && hours !== 12) {
      hours += 12;
    }
    console.log(`hours:${hours}, min: ${minutes}`);
    return hours * 60 + minutes + seconds / 60;
  }

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
          <button>
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
        </div>
      )}
      {chartData && (
        <div>
          <h3>Solar Watch Chart</h3>
          <canvas id="solarWatchChart"></canvas>
        </div>
      )}
    </div>
  );
};

export default SolarWatch;
