import React, { useState, useEffect } from "react";
import Chart from "chart.js/auto";
import { Line } from "react-chartjs-2";


const LineChart = ({city, date}) => {
  
  const [chartData, setChartData] = useState([]);
   
  useEffect(()=> { 
    const dataForChart = async (e) => {
      console.log(city)
       const next3DaysData = [];
      const chartData = [];

      for (let i = 1; i <= 3; i++) {
        const nextDay = new Date(date);
        nextDay.setDate(nextDay.getDate() + i);
        const nextDayFormatted = nextDay.toISOString().split("T")[0];

        try {
          const response = await fetch(
            `http://localhost:5186/SolarWatch/GetInfoToSolarWatch?currentDate=${nextDayFormatted}&location=${city}`,
            {
              method: "GET",
              headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${localStorage.getItem("accessToken")}`,
              },
            }
          );

          if (!response.ok) {
            throw new Error(`Error fetching data for ${nextDayFormatted}`);
          }

          const dayData = await response.json();
          const sunriseTime = parseTimeToMinutes(dayData.sunrise);
          chartData.push({ date: nextDayFormatted, sunrise: sunriseTime });
          console.log(chartData);
        } catch (error) {
          console.error(error.message);
        }
      }

      setChartData(chartData);
      console.log("Search successful");
      console.log("next3DaysData", next3DaysData);
      };
dataForChart();
    },[date])

  function parseTimeToMinutes(timeString) {
    const parts = timeString.match(/(\d+):(\d+)/);
    if (!parts) return 0;

    const hours = parseInt(parts[1]);
    const minutes = parseInt(parts[2]);
    return hours * 60 + minutes;
  }

  function formatMinutesToTime(minutes) {
    const hours = Math.floor(minutes / 60);
    const mins = minutes % 60;
    return `${hours.toString().padStart(2, "0")}:${mins
      .toString()
      .padStart(2, "0")}`;
  }
  return (
    <div>
      <Line
        data={{
          labels: chartData.map((data) => data.date),
          datasets: [
            {
              label: "sunrise",
              data: chartData.map((data) => data.sunrise),
              fill: false,
              borderColor: "rgb(75, 192, 192)",
              backgroundColor: "rgba(255, 255, 255, 0.3)",
              tension: 0.1,
            },
          ],
        }}
        options={{
          scales: {
            x: {
              ticks: {
                color: "#0b1c2d",
              },
            },
            y: {
              ticks: {
                color: "#0b1c2d",
                callback: function (value, index, values) {
                  return formatMinutesToTime(value);
                },
                stepSize: 1,
              },
              grid: {
                display: false,
              },
            },
          },
          plugins: {
            legend: {
              labels: {
                color: "#0b1c2d",
                font: {
                  size: 16,
                },
              },
            },
          },
        }}
      />
    </div>
  );
};

export default LineChart;
