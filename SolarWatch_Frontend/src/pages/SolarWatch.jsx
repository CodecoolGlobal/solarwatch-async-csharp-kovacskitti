import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import  Chart from "chart.js/auto";

const SolarWatch = () => {
  const [city, setCity] = useState("");
  const [date, setDate] = useState("");
  const [solarWatchData, setSolarWatchData] = useState("");
  const [chartData, setChartData] = useState(null);
  const [chartInstance, setChartInstance] = useState(null);
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
      console.log(data.sunrise)
      next3DaysData.forEach((dayData) => {
        if (dayData) {
          const timeValue = parseTimeToMinutes(dayData.sunrise);
          chartLabels.push(` ${dayData.date}`);
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
                    )}:${String(minutes).padStart(2, "0")}:00`;
                    return formattedTime;
                  },
                  color: "black",
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
