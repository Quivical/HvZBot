import Cookies from 'js-cookie';
import axios from "axios";

export const API_BASE_URL = Object.freeze(
    process.env.NODE_ENV === 'production'
        ? 'https://hvzbot.live/api'
        : 'https://localhost:7235/api'
);

export const isLoggedIn = () => {
    return axios.get(API_BASE_URL + '/auth/check', { withCredentials: true })
        .then((response) => {
            console.log(response.data.loggedIn);
            return response.data.loggedIn;
        })
        .catch((error) => {
            console.error("Error checking login status:", error);
            return false;
        });
};
