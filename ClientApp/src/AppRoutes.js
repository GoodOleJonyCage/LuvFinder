import { Counter } from "./components/Counter";
import { FetchData } from "./components/FetchData";
import { HomePage } from "./Pages/HomePage";
import { LoginPage } from "./Pages/LoginPage";
import { Home } from "./components/Home";
import { Login } from "./components/Login";

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
    }
];

export default AppRoutes;
