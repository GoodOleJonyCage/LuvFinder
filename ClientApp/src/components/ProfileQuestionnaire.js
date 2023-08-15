import { useEffect, useState } from 'react'
import { LoadingDiv } from './LoadingDiv'
import { LoadProfile, SaveProfile, LoadInitializedUserInfo, LoadCountries, LoadRegions, LoadCities, LoadMaritalStatuses,LoadGenders } from '../Services/Services'
import { useNavigate, useLocation } from "react-router-dom";
import { Autocomplete, TextField } from '@mui/material';

export const ProfileQuestionnaire = () => {

    const location = useLocation();
    let { username } = location.state;

    const [questions, setquestions] = useState([]);
    const [info, setinfo] = useState({});
    const [errors, seterrors] = useState([]);
    const [error, seterror] = useState('');

    const [countries, setcountries] = useState([]);
    const [regions, setregions] = useState([]);
    const [cities, setcities] = useState([]);

    const [maritalstatuses, setmaritalstatuses] = useState([]);
    const [genders, setgenders] = useState([]);

    const [btnPressed, setbtnPressed] = useState(false);
    const navigate = useNavigate();

    const submitProfile = async () => {

        seterrors([]);
        seterror('');
        setbtnPressed(true);

        try {

            const profileSaved = await SaveProfile(username, questions, info);
            if (profileSaved)
                navigate('/home');

        } catch (e) {

            e.json().then(error => {

                if (error instanceof Array)
                    seterrors(error);
                else
                    seterror(error);

                setbtnPressed(false);
            })
        }
    }

    const LoadCountryList = async () => {
        let countrylist = await LoadCountries();
        setcountries(countrylist);
    }
    const LoadRegionsList = async (countryid) => {
        let regionList = await LoadRegions(countryid);
        setregions(regionList);
    }
    const LoadCitiesList = async (regionid) => {
        let cityList = await LoadCities(regionid);
        setcities(cityList);
    }

    const LoadData = async () => {

        try {

            let vminfo = await LoadInitializedUserInfo();
            setinfo(vminfo);
            console.log(vminfo);

            let statuses = await LoadMaritalStatuses();
            setmaritalstatuses(statuses);

            let genderlist = await LoadGenders();
            setgenders(genderlist);

            /*load countries, city and regions  */
            LoadCountryList();
            /*load countries, city and regions  */

            const vm = await LoadProfile();
            setquestions(vm);
        } catch (e) {

        }
    }

    useEffect(() => {
        LoadData();
    }, []);

    return <div className="profilequestionnaire-container ">
        <div className="col-sm-10 m-auto ">
            <article>
                <div className="info-card mb-20">
                    <div className="info-card-title">
                        <h6>Base Info</h6>
                    </div>
                    <div className="info-card-content profile-form">
                        <ul className="info-list">
                            <li>
                                <p className="info-name">First Name</p>
                                <p className="info-details">
                                    <input
                                        onChange={(e) => { info.firstName = e.target.value }}
                                        type="text" defaultValue={info.firstName}></input>
                                </p>
                            </li>
                            <li>
                                <p className="info-name">Last Name</p>
                                <p className="info-details">
                                    <input
                                        onChange={(e) => { info.lastName = e.target.value }}
                                        type="text" defaultValue={info.lastName}></input>
                                </p>
                            </li>
                            <li>
                                <p className="info-name">I'm a</p>
                                <p className="info-details">
                                    <select id="user-gender" 
                                        onChange={(e) => { info.genderID = e.target.value; }}>
                                        <option value="0">---</option>
                                        {
                                            genders.map((gender, index) => {
                                                return <option key={index} value={gender.id}>{gender.name}</option>
                                            })
                                        }
                                    </select>
                                </p>
                            </li>
                            <li>
                                <p className="info-name">Loking for a</p>
                                <p className="info-details">
                                    <select id="user-gender"  
                                    onChange={(e) => { info.seekingGenderID = e.target.value; }}>
                                    <option value="0">---</option>
                                    {
                                            genders.map((gender, index) => {
                                                return <option key={index} value={gender.id}>{gender.name}</option>
                                            })
                                        }
                                    </select>
                                </p>
                            </li>
                            <li>
                                <p className="info-name">Marital Status</p>
                                <p className="info-details">
                                    <select id="user-maritalstatus"
                                        onChange={(e) => { info.maritalStatusID = e.target.value; }} >
                                        <option value="0">---</option>
                                        {
                                            maritalstatuses.map((status, index) => {
                                                return <option key={index} value={status.id}>{status.name}</option>
                                            })
                                        }
                                    </select>
                                </p>
                            </li>
                            <li>
                                <p className="info-name">Age</p>
                                <p className="info-details">
                                    <input
                                        onChange={(e) => { info.dob = e.target.value }}
                                        type="text" defaultValue={info.dob} />
                                </p>
                            </li>
                            <li>
                                <p className="info-name">Date of Birth</p>
                                <p className="info-details">27-02-1996</p>
                            </li>
                            <li>
                                <p className="info-name">Address</p>
                                <div className="info-details">
                                    <div className="mb-3 region-container">
                                        <div className="first-div">Country</div>
                                        <div>
                                            {countries.length === 0 ? <LoadingDiv></LoadingDiv> :
                                                <Autocomplete
                                                    disablePortal
                                                    id="autocom-countries"
                                                    
                                                    onChange={(event, value) => {
                                                        info.countryID = value.id;
                                                        info.countryName = value.name;
                                                        LoadRegionsList(info.countryID);
                                                    }}

                                                    options={countries}
                                                    getOptionLabel={(option) => option.name || ""}
                                                    sx={{ width: 300 }}
                                                    renderInput={(params) => <TextField  {...params} label="Country" />}
                                                />
                                            }
                                        </div>
                                    </div>
                                    <div className="mb-3 region-container">
                                        <div className="first-div">Region</div>
                                        <div>
                                            {regions.length === 0 ? <div className="highlight-error">Select Country</div> :
                                                <Autocomplete
                                                    disablePortal
                                                    id="autocom-region"
                                                    onChange={(event, value) => {
                                                        if (value) {
                                                            info.regionID = value.id;
                                                            info.regionName = value.name;
                                                            LoadCitiesList(info.regionID);
                                                        }
                                                    }}
                                                    options={regions}
                                                    getOptionLabel={(option) => option.name || ""}
                                                    sx={{ width: 300 }}
                                                    renderInput={(params) => <TextField  {...params} label="Region" />}
                                                />
                                            }
                                        </div>
                                    </div>
                                    <div className="mb-3 region-container">
                                        <div className="first-div">City</div>
                                        <div>
                                            {cities.length === 0 ? <div className="highlight-error">Select Region</div> :
                                                <Autocomplete
                                                    disablePortal
                                                    onChange={(event, value) => {
                                                        info.cityID = value.id;
                                                        info.cityName = value.name;
                                                    }}
                                                    id="autocom-cities"
                                                    options={cities}
                                                    getOptionLabel={(option) => option.name || ""}
                                                    sx={{ width: 300 }}
                                                    renderInput={(params) => <TextField {...params} label="City" />}
                                                />
                                            }
                                        </div>
                                    </div>
                                </div>
                            </li>
                        </ul>
                    </div>
                </div>
                {
                    questions.length === 0 ? <LoadingDiv /> :
                        questions.map((q, index) => {
                            return <div className="info-card mb-20" key={index}>
                                <div className="info-card-title">
                                    <h6>{q.question.shortDesc}</h6>
                                </div>
                                <div className="info-card-content">
                                    {
                                        q.question.answers.length === 0 ?
                                            <>
                                                <div>{q.question.text}</div>
                                                <textarea
                                                    onChange={(e) => { q.answerText = e.target.value }}
                                                    className="profilequestionnaire-textarea" defaultValue="" rows="5" cols="5"></textarea>
                                            </> :
                                            <ul className="info-list">
                                                <li>
                                                    <p className="info-name">{q.question.text}</p>
                                                    <div className="info-details">
                                                        {
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
                                    }
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
                    {!btnPressed &&
                        <button onClick={(e) => submitProfile()}
                        className="smaller lab-btn" type="Submit" >Save Profile</button>
                    }
                </div>
            </div>
        </div>
    </div>

}