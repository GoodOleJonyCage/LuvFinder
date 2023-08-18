import  { useState } from 'react'

export const UserStore = () => {

    const getToken = () => {

        const tokenStr = localStorage.getItem('token');
        if (tokenStr) {
            const token = JSON.parse(tokenStr);
            return token;
        }
        return null;
    }

    const getUsername = () => {

        let usernamestr = localStorage.getItem('username');
        if (usernamestr) {
            let username = JSON.parse(usernamestr);
            return username;
        }
        return null;
    }

    const [token, setToken] = useState(getToken());
    const [username, setUsername] = useState(getUsername());
    
    const saveToken = (token) => {

        localStorage.setItem("token", JSON.stringify(token));
        setToken(token);
    }

    const saveUsername = (username) => {

        localStorage.setItem("username", JSON.stringify(username));
        setUsername(username);
    }

    const clearToken = () => {
        localStorage.setItem("token", null);
        localStorage.setItem("username", null);
    }

    const isLoggedIn = () => {

        return getUsername() !== null;

    }

    const isAdmin = () => {

        let tokenstr = localStorage.getItem('token');
        if (tokenstr) {

            let jwtData = tokenstr.split('.')[1];
            let decodedJwtJsonData = window.atob(jwtData);
            let decodedJwtData = JSON.parse(decodedJwtJsonData);

            let roles = []
            Object.keys(decodedJwtData)
                .forEach(key => {
                    if (key.match(/.*role$/)) { roles = decodedJwtData[key] }
                })
            //console.log(roles);
            return roles === "Admin";
        }
        return false;
    }
    
    return {
        isLoggedIn: isLoggedIn,
        clearToken: clearToken,
        getUsername: getUsername,
        saveUsername: saveUsername,
        saveToken: saveToken, 
        getToken: getToken,
        token,
        username
    }
}