import { useNavigate } from "react-router-dom";
import { useState, useEffect } from 'react'
import { LoadProfiles } from '../Services/Services'
import { LoadingDiv } from './LoadingDiv'
import { ProfileIcons } from './ProfileIcons'

export const ProfileList = () => {

    const [loaded, setloaded] = useState(false);
    const [profiles, setprofiles] = useState([]);
    const navigate = useNavigate();

    const LoadData = async () => {

        try {
            let vm = await LoadProfiles();
            //console.log(vm);
            setprofiles(vm);
            setloaded(true);
        } catch (e) {

        }
    }
    useEffect(() => {
        LoadData();
    }, []);


    const Profile = (props) => {
        return <>
            <div className="col-xl-2 col-lg-3 col-md-4 col-6">
                <div className="profile-item-container lab-item member-item style-1">
                    <div className="lab-inner">
                        <div onClick={(e) => navigate('/viewprofile', { state: { username: props.profile.userName } })}>
                            <div className="lab-thumb">
                                {/*randomly selectying image from 01 to 20 png*/}
                                <img src={`assets/images/member/${(props.index % 21 < 9 ? "0" + ((props.index + 1) % 21) : ((props.index + 1) % 21) === 0 ? "01" : (props.index + 1) % 21)}.jpg`} alt="member-img" />
                            </div>
                            <div className="lab-content profile-item">
                                <h6>{props.profile.firstName + " " + props.profile.lastName} </h6>
                                <div>{props.profile.userName}</div>
                                <div className="profile-item-maritalstatus">
                                    <div >{props.profile.maritalStatus} {props.profile.gender} ({props.profile.age})</div>
                                    <div className="profile-item-maritalstatus-seeking">seeking</div>
                                    <div>{props.profile.seekingGender}</div>
                                </div>
                                <div className="profile-item-address">
                                    <div>{props.profile.cityName},</div>
                                    <div>{props.profile.regionName}</div>
                                    <div>{props.profile.countryName}</div>
                                </div>
                                </div>
                        </div>
                        <ProfileIcons username={props.profile.userName}></ProfileIcons>
                    </div>
                </div>
            </div>
        </>
    }

    return <div className="row justify-content-center g-3 g-md-4">
        {
            profiles.length === 0 ?
                (loaded ? <div className="highlight-error text-center">No profiles to load</div> : <LoadingDiv></LoadingDiv>) :
                profiles.map((profile, index) => {
                    return <Profile profile={profile} key={index} index={index}></Profile>
                })
        }
    </div>;

}