import { useState, useEffect } from 'react'
import { FriendRequestCount } from '../Services/Services'
import { UserStore } from './UserStore'

export const PendingFriendRequests = () => {

    const { getUsername } = UserStore();
    const [count, setcount] = useState(0);
    const loadData = async () => {
        let cnt = await FriendRequestCount(getUsername());    
        setcount(cnt);
    }

    useEffect(() => {
        loadData();
    }, []);
 

    return <>
        <button className="nav-link" id="pills-friends-tab"
            data-bs-toggle="pill" data-bs-target="#pills-friends"
            type="button" role="tab" aria-controls="pills-friends"
            aria-selected="false"><i className="icofont-favourite"></i>
            Friends<span className="item-number friend-request-count">({count})</span></button>
        </>

}
