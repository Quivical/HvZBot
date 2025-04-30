import React, {Component, useEffect, useState} from 'react';
import LoginButton from "./LoginButton";
import {UserInfo} from "./UserInfo";
import {ServerList} from "./ServerList";
import {isLoggedIn} from "../constants";

export function Home() {
    const [isAuthenticated, setIsAuthenticated] = useState(null);

    useEffect(() => {
        const checkAuth = async () => {
            try {
                const loggedIn = await isLoggedIn();
                setIsAuthenticated(loggedIn);
            } catch (error) {
                console.error("Error checking authentication:", error);
                setIsAuthenticated(false);
            }
        };

        checkAuth();
    }, []);

    
    return (
        <div>
            <h1>Welcome to HvZBot.live!</h1>
            <p>Your new portal for tracking and managing your Humans vs. Zombies games on any campus.</p>
            {
                isAuthenticated ? (
                    <div>
                        <UserInfo size={128} withName={true} />
                        <p>Please see the servers you're signed up for HvZ in below:</p>
                        <ServerList/>
                    </div>
                ) : (
                    <div> 
                        <p>To get started, connect via your Discord account:</p>
                        <LoginButton/>
                    </div>
                )
            }
        </div>
    );
}