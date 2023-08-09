import { UserStore } from './UserStore'

export const Logout = () => {


    const { clearToken } = UserStore(); 

    clearToken();

    window.location.href = "/";

    return <></>;

}