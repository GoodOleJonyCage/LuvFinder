import { useState } from 'react';
import { RegisterUser } from '../Services/Services'
import { useNavigate } from "react-router-dom";

export const Register = () => {

    const [username, setusername] = useState('');
    const [password, setpassword] = useState('');
    const [result, setresult] = useState('');
    const [submitted, setsubmitted] = useState(false);
    const [btnPressed, setbtnPressed] = useState(false);

    const navigate = useNavigate();

    const handleSubmit = async (e) => {

        setresult('');
        setsubmitted(true);
        setbtnPressed(true);

        if (username.length > 0 && password.length > 0) {

            try {

                const isregistered = await RegisterUser(username, password);
                if (isregistered)
                    navigate('/createprofile?username=' + username);

            } catch (response) {
                response.json().then(error => {
                    //console.log(error);
                    setresult(error);
                    setbtnPressed(false);
                })
            }
        }
        else {
            setbtnPressed(false);
        }
    }

    return <div className="login-section padding-tb">
        <div className="container">
            <div className="account-wrapper">
                <h3 className="title">Register Now</h3>
                <div className="account-form">
                    {/*<div className="form-group">*/}
                    {/*    <input type="text" placeholder="User Name" name="username" />*/}
                    {/*</div>*/}
                    <div className="form-group">
                        <input type="text" placeholder="Email" name="email"
                            onChange={(e) => { setusername(e.target.value) }}
                            className={submitted && !username?  "highlight-field": ""}
                        />
                    </div>
                    <div className="form-group">
                        <input type="password" placeholder="Password" name="password"
                            onChange={(e) => { setpassword(e.target.value) }}
                            className={submitted && !password ? "highlight-field" : ""}
                        />
                    </div>
                    {/*<div className="form-group">*/}
                    {/*    <input type="password" placeholder="Confirm Password" name="password" />*/}
                    {/*</div>*/}
                    <div className="form-group highlight-error">
                        {result}
                    </div>
                    <div className="form-group">
                        {!btnPressed &&
                            <button className="d-block lab-btn" type="Submit"
                                onClick={handleSubmit}>Get Started Now</button>
                        }
                    </div>
                </div>
                <div className="account-bottom">
                    <span className="d-block cate pt-10">Are you a member? <a href="login.html">Login</a></span>
                    <span className="or"><span>or</span></span>
                    <h5 className="subtitle">Register With Social Media</h5>
                    <ul className="social-media social-color justify-content-center d-flex lab-ul">
                        <li>
                            <a href="/#" className="facebook"><i className="icofont-facebook"></i></a>
                        </li>
                        <li>
                            <a href="/#" className="twitter"><i className="icofont-twitter"></i></a>
                        </li>
                        <li>
                            <a href="/#" className="linkedin"><i className="icofont-linkedin"></i></a>
                        </li>
                        <li>
                            <a href="/#" className="instagram"><i className="icofont-instagram"></i></a>
                        </li>
                        <li>
                            <a href="/#" className="pinterest"><i className="icofont-pinterest"></i></a>
                        </li>
                    </ul>
                </div>
            </div>
        </div>
    </div>;
}