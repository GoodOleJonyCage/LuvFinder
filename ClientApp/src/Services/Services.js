

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
        console.log(data);
        return data;
    }

    throw response;

}