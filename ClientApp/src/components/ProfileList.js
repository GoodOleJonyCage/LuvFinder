import { useNavigate } from "react-router-dom";
import { useState, useEffect } from 'react'
import {LoadProfiles } from '../Services/Services'
import { LoadingDiv } from './LoadingDiv'

export const ProfileList = () => {

    const [profiles, setprofiles] = useState([]);
    const navigate = useNavigate();

    const LoadData = async () => {

        try {
            let vm = await LoadProfiles();
            console.log(vm);
            setprofiles(vm);
        } catch (e) {

        }
    }
    useEffect(() => {
        LoadData();
    }, []);


    return <div className="row justify-content-center g-3 g-md-4">
            {
            profiles.length === 0 ? <LoadingDiv></LoadingDiv> :
            profiles.map((profile, index) => {
                return <div key={index} className="col-xl-2 col-lg-3 col-md-4 col-6"
                        onClick={(e) =>
                            navigate('/viewprofile', { state: { username: profile.userName } })}>
                    <div className="lab-item member-item style-1">
                        <div className="lab-inner">
                            <div className="lab-thumb">
                                {/*randomly selectying image from 01 to 20 png*/}
                                <img src={`assets/images/member/${(index % 21 < 9 ? "0" + ((index + 1) % 21) : ((index + 1) % 21) === 0 ? "01" : (index + 1) % 21)}.jpg`} alt="member-img" />
                            </div>
                            <div className="lab-content">
                                <h6>{profile.firstName + " " + profile.lastName}</h6>
                                <div>{profile.maritalStatus} {profile.gender} looking for a {profile.seekingGender}</div>
                                <p>{profile.age} days old</p>
                                <p>{profile.cityName},{profile.regionName } {profile.countryName }</p>
                            </div>
                        </div>
                    </div>
                </div>
            })
            }
        </div>;

}