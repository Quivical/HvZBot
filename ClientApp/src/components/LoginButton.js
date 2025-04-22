import React from 'react';
import axios from "axios";
import {API_BASE_URL} from "../constants";
import '../custom.css';

function LoginButton({ onLogIn }) {
  const login = () => {
    console.log('Button clicked!');
    window.location.href = API_BASE_URL + "/api/auth/login";
    localStorage.setItem("auth", JSON.stringify(true));
    onLogIn(true)
  };
  
  return (
      <button onClick={login} className="btn-primary">
        Login with Discord
      </button>
  );
}

export default LoginButton;