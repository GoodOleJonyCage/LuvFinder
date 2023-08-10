

export const LoadProfile = async (userID) => {

    let response = await fetch(`profile/profilequestionnaire`, {
        headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/json',
            //'Authorization': "Bearer " + getJwtToken()
        },
        //method: 'POST',
        //body: JSON.stringify({ quizid: quizid }),
    });

    if (response.ok) {
        const data = await response.json();
        return data;
    }

    throw response;

}

export const LoadUserProfile = async (username) => {

    let response = await fetch(`profile/userprofile`, {
        headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/json',
            //'Authorization': "Bearer " + getJwtToken()
        },
        method: 'POST',
        body: JSON.stringify({ username: username }),
    });

    if (response.ok) {
        const data = await response.json();
        return data;
    }

    throw response;

}

export const SaveProfile = async (username, vm) => {

    let response = await fetch(`profile/profilesaved`, {
        headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/json',
            //'Authorization': "Bearer " + getJwtToken()
        },
        method: 'POST',
        body: JSON.stringify({
            username: username,
            vm: vm
        }),
    });

    if (response.ok) {
        const data = await response.json();
        return data;
    }

    return Promise.reject(response);

}

export const RegisterUser = async (username, password) => {

    let response = await fetch(`user/register`, {
        headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/json',
            //'Authorization': "Bearer " + getJwtToken()
        },
        method: 'POST',
        body: JSON.stringify({
            username: username,
            password : password
        }),
    });

    if (response.ok) {
        const data = await response.json();
        return data;
    }

    return Promise.reject(response);

}

export const LoginUser = async (username, password) => {

    let response = await fetch(`user/login`, {
        headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/json',
            //'Authorization': "Bearer " + getJwtToken()
        },
        method: 'POST',
        body: JSON.stringify({
            username: username,
            password: password
        }),
    });

    if (response.ok) {
        const data = await response.json();
        return data;
    }

    return Promise.reject(response);

}