import './App.css';
import RequireAuth from "./components/auth/RequireAuth";

import { Component } from "react";
import { BrowserRouter, Route, Routes } from "react-router-dom";
import { AuthProvider } from "./context/AuthProvider";
import {
  Login,
  Layout,
  Unauthorized,
  Missing, Home, Payments, Customers, Customer, Server,
} from "./components";
import {Service} from "./components/Service";
import {Outage} from "./components/Outage";
import {Register} from "./components/auth/Register";

class App extends Component {
  render() {
    return (
        <BrowserRouter>
          <AuthProvider>
            <Routes>
              <Route path="/" element={<Layout/>}>
                {/* public routes */}
                <Route path="/unauthorized" element={<Unauthorized/>}/>

                <Route element={<RequireAuth allowedRoles={["Unauthenticated"]}/>}>
                  <Route path="/" element={<Login/>}/>
                  <Route path="/login" element={<Login/>}/>
                  <Route path="/register" element={<Register/>}/>
                </Route>

                {/*protected routes */}
                <Route element={<RequireAuth/>}>
                  <Route path="/home" element={<Home/>}/>
                  <Route path="/customers" element={<Customers/>}/>
                  <Route path="/customers/:id/services/:serviceId" element={<Service/>}/>
                  <Route path="/customers/:id" element={<Customer/>}/>
                  <Route path="/payments" element={<Payments/>}/>
                  <Route path="/servers/:id" element={<Server/>}/>
                  <Route path="/outages/:id" element={<Outage/>}/>
                </Route>

                {/* catch all */}
                <Route path="*" element={<Missing/>}/>
              </Route>
            </Routes>
          </AuthProvider>
        </BrowserRouter>
    );
  }
}

export default App;
