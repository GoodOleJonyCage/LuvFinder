import { useState, useEffect } from 'react'
import { GetActivityFriends, StartFriendShip } from '../Services/Services'
import { LoadingDiv } from './LoadingDiv'
import { useNavigate } from "react-router-dom"
import { UserStore } from './UserStore'
import Moment from 'react-moment';

const ActivityFriend = (props) => {

    const navigate = useNavigate();
    const { getUsername } = UserStore();
    const [activity, setactivity] = useState(props.activity);
    const [error, seterror] = useState('');

    const BecomeFriends = async () => {
        try {
            let newactivity = await StartFriendShip(activity.fromUserInfo.userName, getUsername());
            if (newactivity) {
                setactivity(newactivity);
            }
        } catch (e) {

            e.json().then(error => { seterror(error) });
        }
    }

    return <>
        {activity.fromUserInfo === null ?  <></> :
            <div className="post-item mb-20">
                <div className="post-content">
                    <div className="post-author">
                        <div className="post-author-inner">
                            <div className="author-thumb">
                                <img src="assets/images/profile/dp.png"
                                    alt="img" />
                            </div>
                            <div className="author-details friendship-request-sender">
                                <label><a onClick={(e) => { navigate('/viewprofile', { state: { username: activity.fromUserInfo.userName } }) }}>{activity.userName } {activity.fromUserInfo.firstName} {activity.fromUserInfo.lastName} ({activity.fromUserInfo.age}) from {activity.fromUserInfo.cityName},{activity.fromUserInfo.regionName} {activity.fromUserInfo.countryName}</a></label>
                                
                            </div>
                            
                        </div>
                        <div className="friendship-request-sender-time">
                            <label><i className="icofont-world"></i> Liked you at </label>
                            <label className="friendship-request-sender-date"><i><Moment format="DD MMM YYYY hh:mm:ss:A">{activity.date}</Moment></i></label>
                        </div>
                    </div>
                    {
                        activity.likeAccepted ?
                            <div className="post-description friendship-request-container">
                                <div>You became friends on </div>
                                <div>
                                    <label className="small"><i><Moment format="DD MMM YYYY hh:mm:ss:A">{activity.likeAcceptedDate}</Moment></i></label> 
                                </div>
                            </div>
                            :
                            <div className="post-description friendship-request-container">
                                <div>Liked your profile.
                                </div>
                                <div>
                                    <a className="ml-4" onClick={BecomeFriends} className="ml-4">Start friendship ! <i className="icofont-like"></i></a>
                                </div>
                            </div>
                    }
                    <div className="highlight-error">{error}</div>
                </div>
            </div>
        }
    </>
}

export const ActivityFriends = (props) => {


    const [activities, setactivities] = useState([]);

    const loadData = async () => {

        try {
            let vm = await GetActivityFriends(props.username);
            //console.log(vm);
            setactivities(vm);
        } catch (e) {

        }
    }

    useEffect(() => {
        loadData();
    }, []);

    return <>
        {
            activities.length === 0 ? <LoadingDiv></LoadingDiv> :
                activities.map((activity, index) => {
                    return <ActivityFriend key={index} activity={activity} ></ActivityFriend>
                })
        }
    </>;

}