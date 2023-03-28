import { Navigate, Outlet, useLocation } from "react-router-dom";
import { useAppSelector } from "../store/configureStore";

export default function RequiredAuth() {

    // we need to check if we have a user object and this is to tell us if we are actually logged in. On the client side, we cannot check the token is a valid token, so it's not real security. This is more of a user interface element that we're adding here to see if the user should be allowed to go to that particular route.
    const {user} = useAppSelector(state => state.account)
    const location = useLocation() // this will give us the opportunity to redirect the user back to where they came from, because when they encounter our RequredAuth component, which they will do if they try to checkout when they're not logged in, then we're going to redirect them to the login page. After they've logged in, we're going to attempt to redirect them back to where they were trying to get to after they have logged in. 
    if (!user) {
        return <Navigate to='/login' state={{from: location}} />
    }

    return <Outlet />
 }