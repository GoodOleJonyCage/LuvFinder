import { useEffect, useState } from 'react';
import { UserStore } from './UserStore'
import { LikeUser, GetLikeUserStatus } from '../Services/Services'

export const ProfileIcons = (props) => {

    const [like, setlike] = useState(false);
    const [likedby, setlikedby] = useState(false);
    const [error, seterror] = useState('');
    const {getUsername, isLoggedIn} = UserStore();

    const loadData = async () => {
        if (isLoggedIn()) {

            let hasLiked = await GetLikeUserStatus(getUsername(), props.username);
            setlike(hasLiked);
            //liked by
            let hasLikedBy = await GetLikeUserStatus(props.username, getUsername());
            setlikedby(hasLikedBy);
        }
    }

    useEffect(() => {
         loadData();
    }, []);

    return <>
        {!isLoggedIn() ? <></> :
            <div className="profile-item-icons"
                onClick={(e) => { }}
            >
                <i className="fa-solid fa-people-group"></i>
                <i className={likedby ? "fa-solid fa-shield-heart fa-heart-red": "fa-solid fa-shield-heart"} tooltip="They liked you"></i>
                <i className={like ? "fa fa-heart heart-icon fa-heart-red" : "fa fa-heart heart-icon"}
                    onClick={async (e) => {
                        try {
                            var liked = await LikeUser(getUsername(), props.username);  
                            if (liked)
                                setlike(true);
                        } catch (e) {
                            e.json().then(error => {
                                seterror(error);
                            })
                        }
                    }}
                ></i>
                <div className="highlight-error text-center">{error}</div>
            </div>
        }
    </>
}