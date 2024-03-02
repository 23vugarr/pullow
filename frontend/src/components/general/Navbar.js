import { Link } from "react-router-dom";
import {isAuthenticated, removeAccessToken} from "../../common/jwtCommon";
import useAuth from "../../hooks/useAuth";

import pullowLogo from "../../images/pullow_logo.png"

export const Navbar = () => {
  const { setAuth } = useAuth();

  function generateRandomInteger(max) {
    return (Math.floor(Math.random() * max) + 1).toString();
  }

  const logout = async () => {
    await removeAccessToken();
    setAuth({});
    window.location.reload();
  }

  const homeLink = (
      <li className="nav-item" key={`home-${generateRandomInteger(10000)}`}>
        <Link to="/" className="nav-link">
          Home
        </Link>
      </li>
  )
  const registerLink = (
      <li className="nav-item" key={`register-${generateRandomInteger(10000)}`}>
          <Link to="/register" className="nav-link">
              Register
          </Link>
      </li>
  )

  const loginLink = (
      <li className="nav-item" key={`login-${generateRandomInteger(10000)}`}>
        <Link to="/login" className="nav-link">
          Login
        </Link>
      </li>
  )

  const logoutLink = (
      <li className="nav-item" key={`logout-${generateRandomInteger(10000)}`}>
        <a href="#" onClick={logout} className="nav-link">
          Logout
        </a>
      </li>
  )

  const BuildNavbar = () => {
    let links = [];
    if (isAuthenticated()) {
      links.push(homeLink);
      links.push(logoutLink);
    } else {
      links.push(registerLink)
      links.push(loginLink);
    }
    return links;
  }

  return (
      <nav className="navbar navbar-expand-md navbar-dark bg-dark">
        <button
            className="navbar-toggler"
            type="button"
            data-toggle="collapse"
            data-target="#main-navbar"
            aria-controls="main-navbar"
            aria-expanded="false"
            aria-label="Toggle navigation"
        >
          <span className="navbar-toggler-icon"/>
        </button>
        <div
            className="collapse navbar-collapse justify-content-md-center"
            id="main-navbar"
        >
            <a href="/">
                <img src={pullowLogo} alt={"PuLLoW logo"} width="60px" height="60px"/>
            </a>
            <ul className="navbar-nav" id="navbar-list">
            <BuildNavbar/>
          </ul>
        </div>
      </nav>
  )
}