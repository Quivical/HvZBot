import React, { Component, useState } from 'react';
import LoginButton from "./LoginButton";
import {UserInfo} from "./UserInfo";

export function Home() {
    const [isLoggedIn, setIsLoggedIn] = React.useState(
        () => JSON.parse(localStorage.getItem('auth')) || false);
    
    const handleLogIn = (newState) => {
        setIsLoggedIn(newState);
    }
    
    return (
        <div>
            <h1>Welcome to HvZBot.live!</h1>
            <p>Your new portal for tracking and managing your Humans vs. Zombies games on any campus.</p>
            <ul>
                <li><a href='https://get.asp.net/'>ASP.NET Core</a> and <a href='https://msdn.microsoft.com/en-us/library/67ef8sbd.aspx'>C#</a> for cross-platform server-side code</li>
                <li><a href='https://facebook.github.io/react/'>React</a> for client-side code</li>
                <li><a href='http://getbootstrap.com/'>Bootstrap</a> for layout and styling</li>
            </ul>
            
            {
                isLoggedIn ? (
                    <UserInfo/>
                ) : (
                    <div> 
                        <p>To get started, connect via your Discord account:</p>
                        <LoginButton onLogIn={handleLogIn} />
                    </div>
                )
            }
        </div>
    );
}