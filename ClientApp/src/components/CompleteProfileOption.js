import { useNavigate, useLocation } from "react-router-dom";

export const CompleteProfileOption = (props) => {


    const navigate = useNavigate();
    //get username from path
    const location = useLocation();
    let { username } = location.state;
    //get username from path

    return <div class="container">
            <div className="highlight-error text-center">{props.infoerror}</div>
            <p className="text-center"><button className="lab-btn" onClick={(e) => { navigate('/createprofile', { state: { username: username } }); }}>Complete your Registration</button></p>
          </div>
}