import React, {useEffect, useState} from 'react';
import axios from "axios";
import {API_BASE_URL} from "../constants";
import '../custom.css';
import {UserAvatar} from "./UserAvatar";

export function UserInfo() {
    const [userData, setUserData] = useState({});

    useEffect(() => {
        axios.get(API_BASE_URL + `/api/discord/info`, 
            {withCredentials: true})
            .then(res => {
                 setUserData(res.data);
            })
    }, []);

    return (
        <div>
            <p>Welcome, {userData.global_name}!</p>
            <UserAvatar avatarUrl={`https://cdn.discordapp.com/avatars/${userData.id}/${userData.avatar}.webp`} username={userData.global_name} />
        </div>
    );
}