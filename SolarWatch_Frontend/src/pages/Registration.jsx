import React, { useState } from "react";

const Registration = () => {
  const [username, setUsername] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  const handleRegistration = async (e) => {
    e.preventDefault();

    const registrationData = {
      UserName: username,
      Email: email,
      Password: password,
      };

    try {
      const response = await fetch("http://localhost:5186/Auth/Register", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(registrationData),
      });

      if (!response.ok) {
        console.error("Registration failed:", response.statusText);
        return;
      }

      const data = await response.json();
      console.log("Registration successful");
    } catch (error) {
      console.error("Error during registration:", error);
    }
  };

  return (
    <div className="basic-container">
      <h2>Registration</h2>
      <form onSubmit={handleRegistration}>
        <label>
          Username:
          <input
            type="text"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            required
          />
        </label>
        <label>
          Email:
          <input
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
          />
        </label>
        <label>
          Password:
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
          />
        </label>
        
        <button type="submit">Register</button>
      </form>
    </div>
  );
};

export default Registration;
