import React, {useEffect, useState} from 'react';
import axios from "axios";
import {API_BASE_URL} from "../constants";
import '../custom.css';
import {UserAvatar} from "./UserAvatar";
import {User} from "oidc-client";

export function UserInfo({ size, withName }) {
    const [userData, setUserData] = useState({});

    useEffect(() => {
        axios.get(API_BASE_URL + `/bot/info`, 
            {withCredentials: true})
            .then(res => {
                 setUserData(res.data);
            })
    }, []);

    return (
        <div>
            { withName ? (<p>Welcome, {userData.global_name}!</p>) : null }
            <UserAvatar userId={userData.id} avatarHash={userData.avatar} size={size} username={userData.global_name} />
        </div>
    );
}