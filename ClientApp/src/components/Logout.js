import { UserStore } from './UserStore'
import { useNavigate } from "react-router-dom";
export const Logout = () => {

    const navigate = useNavigate();
    const { clearToken } = UserStore(); 

    clearToken();

   navigate("/home");

    return <></>;

}