import { Counter } from "./components/Counter";
import { FetchData } from "./components/FetchData";
import { HomePage } from "./Pages/HomePage";
import { LoginPage } from "./Pages/LoginPage";
import { RegisterPage } from "./Pages/RegisterPage"
import { ProfilePage } from "./Pages/ProfilePage"
import { CreateProfilePage } from "./Pages/CreateProfilePage"

const AppRoutes = [
    {
        index: true,
        element: <HomePage />
    },
    {
        path: '/counter',
        element: <Counter />
    },
    {
        path: '/fetch-data',
        element: <FetchData />
    },
    {
        path: '/login',
        element: <LoginPage />
    },
    {
        path: '/register',
        element: <RegisterPage />
    },
    {
        path: '/profile',
        element: <ProfilePage />
    },
    {
        path: '/createprofile',
        element: <CreateProfilePage />
    }
];

export default AppRoutes;
