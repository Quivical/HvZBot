import React, {useEffect, useState} from 'react';
import {API_BASE_URL} from "../constants";
import axios from "axios";

export function ServerList(){
    const [servers, setServers] = useState([]);
    
    useEffect(() => {
        axios.get(API_BASE_URL + `/bot/servers`,
            {withCredentials: true})
            .then(res => {
                setServers(res.data);
                console.log(res.data);
            })
    }, []);


    return (
        <div>
            <p>Server(s):</p>
            <ul>
                {servers.map(server => (
                    <li key={server}>
                        <img src={`https://cdn.discordapp.com/icons/${server.id}/${server.icon}.webp?size=100`} alt='Discord Server Icon'/>
                        <p>{server.display_name}</p>
                    </li>
                ))}
            </ul>
        </div>
    );
}