import { useLocation } from "react-router-dom";
import { useNavigate } from "react-router-dom";

const GetBreadCrumbPath = () => {

    let path = "";
    const location = useLocation();
    switch (location.pathname) {

        case "/home":
            path = "";
            break;

        case "/login":
            path = "/login";
            break;

        case "/register":
            path = "/register";
            break;

        case "/profile":
            path = "/profile";
            break;

        case "/createprofile":
            path = "/createprofile";
            break;

        default:
            path = "";
            break;
    }
    return path;
}

const GetHeaderTitle = () => {

    let path = "";
    const location = useLocation();
    switch (location.pathname) {

        case "/home":
            path = "";
            break;

        case "/login":
            path = "Login";
            break;

        case "/register":
            path = "Register";
            break;

        case "/profile":
            path = "Profile";
            break;

        case "/createprofile":
            path = "CreateProfile";
            break;

        default:
            path = "";
            break;
    }
    return path;
}

const GetBreadCrumbTitle = () => {

    let path = "";
    const location = useLocation();
    switch (location.pathname) {

        case "/home":
            path = "";
            break;

        case "/login":
            path = "Login";
            break;

        case "/register":
            path = "Register";
            break;

        case "/profile":
            path = "Profile";
            break;

        case "/createprofile":
            path = "CreateProfile";
            break;

        default:
            path = "";
            break;
    }
    return path;
}


export const PageHeader = () => {

    const navigate = useNavigate();

    return <section className="page-header-section style-1" >
        <div className="container">
            <div className="page-header-content">
                <div className="page-header-inner">
                    <div className="page-title">
                        <h2>TuruLav {GetHeaderTitle()} Page</h2>
                    </div>
                    <ol className="breadcrumb">
                        <li><a onClick={(e) => { navigate('/home'); }}>Home</a></li>
                        <li>{GetBreadCrumbTitle().length > 0 ? ">" : ""}</li>
                        <li className="active">
                            <a onClick={(e) => { navigate(GetBreadCrumbPath()); }}>{GetBreadCrumbTitle()}</a></li>
                    </ol>
                </div>
            </div>
        </div>
    </section>

}