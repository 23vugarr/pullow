import { useLocation, Navigate, Outlet } from "react-router-dom";
import { getDecodedAccessToken } from "../../common/jwtCommon";

const RequireAuth = ({allowedRoles}) => {
  const location = useLocation();
  const tokenData = getDecodedAccessToken();
 return (
     allowedRoles?.includes("Unauthenticated") ?
         ! tokenData
             // user is not authenticated and goes to login/register
             ? <Outlet/>
             // user is authenticated but wants to go login/register
             : <Navigate to="/home" replace={true}/>
           : tokenData
                // user is authenticated
                ? <Outlet/>
                // user is not authenticated
                : <Navigate to={`/login`} state={{from: location}} replace />
     );
}

export default RequireAuth;