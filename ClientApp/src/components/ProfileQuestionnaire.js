import { useEffect, useState } from 'react'
import { LoadingDiv } from './LoadingDiv'
import { LoadProfile, SaveProfile } from '../Services/Services'
import { useNavigate } from "react-router-dom";

export const ProfileQuestionnaire = () => {

    const params = new URLSearchParams(window.location.search); 
    let username = params.get('username');

    const [questions, setquestions] = useState([]);
    const [errors, seterrors] = useState([]);
    const [error, seterror] = useState('');

    const navigate = useNavigate();

    const submitProfile = async () => {

        seterrors([]);
        seterror('');

        try {

            const profileSaved = await SaveProfile(username, questions);
            if (profileSaved)
                navigate('/home');

        } catch (e) {

            e.json().then(error => {

                if (error instanceof Array)
                    seterrors(error);
                 else
                    seterror(error);
                
            })
        }
    }

    const LoadData = async () => {

        try {
            const vm = await LoadProfile();
            //console.log(vm);
            setquestions(vm);
        } catch (e) {

        }
    }

    useEffect(() => {
        LoadData();
    }, []);

    return <div className="profilequestionnaire-container ">
        <div className="col-sm-10 m-auto ">
            <article className="">
                {
                    questions.length === 0 ? <LoadingDiv /> :
                        questions.map((q, index) => {
                            return <div className="info-card mb-20" key={index}>
                                <div className="info-card-title">
                                    <h6>{q.question.shortDesc}</h6>
                                </div>
                                <div className="info-card-content">
                                    <ul className="info-list">
                                        <li>
                                            <p className="info-name">{q.question.text}</p>
                                            <div className="info-details">
                                                {
                                                    q.question.answers.length === 0 ?
                                                        <textarea
                                                            onChange={(e) => { q.answerText = e.target.value }}
                                                            className="profilequestionnaire-textarea" defaultValue="" rows="5" cols="5"></textarea> :
                                                        q.question.answers.map((a, aindex) => {
                                                            return <div key={aindex} className="questionnaire_Control_container">
                                                                {
                                                                    q.question.questionType === 1 ?
                                                                        <label>
                                                                            <input
                                                                                onChange={(e) => { a.selected = e.target.checked }}
                                                                                type="checkbox" value={a.text} />{a.text}
                                                                        </label> :
                                                                        q.question.questionType === 2 ?
                                                                            <label>
                                                                                <input
                                                                                    onChange={(e) => { a.selected = e.target.checked }}
                                                                                    name={q.question.text} type="radio" value={a.text} />{a.text}
                                                                            </label>
                                                                            : <></>
                                                                }
                                                            </div>
                                                        })
                                                }
                                            </div>
                                        </li>
                                    </ul>
                                </div>
                            </div>
                        })
                }
            </article>
        </div>
        <div className="container">
            <div className="m-auto">
                <div className=" text-center  ">
                    <ul>
                        {
                            errors.length > 0 ?
                            errors.map((error, i) => {

                                return <li key={i} className="highlight-error">{error}</li>
                            }) : <></>
                        }
                        <li className="highlight-error">{error}</li>
                    </ul>
                </div>
                <div className="col-sm-3 m-auto banner-form">
                    <button onClick={(e) => submitProfile()}
                        className="smaller lab-btn" type="Submit" >Save Profile</button>
                </div>
            </div>
        </div>
    </div>

}