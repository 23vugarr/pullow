import { useEffect, useState } from "react";
import axios from "axios";
import {getAccessToken, getDecodedAccessToken} from "../common/jwtCommon";
import { useNavigate } from "react-router-dom";

export const Home = () => {
    const navigate = useNavigate();

    const [load, setLoad] = useState(false);
    const [error, setError] = useState('');

    const [companyName, setCompanyName] = useState('');
    const [latestOutages, setLatestOutages] = useState([]);
    const [latestPayments, setLatestPayments] = useState([]);


    useEffect(() => {
        if(!load) {
            const onPageLoad = async () => {
                await loadCompanyName()
                await loadLatestPayments()
                await loadLatestOutages()
                setLoad(true);
            }
            if (document.readyState === 'complete') {
                onPageLoad();
            } else {
                window.addEventListener('load', onPageLoad);
                return () => window.removeEventListener('load', onPageLoad);
            }
        }
    })

    const loadCompanyName = async () => {
        let token = getDecodedAccessToken()
        setCompanyName(token.CompanyName);
    }

    const loadLatestOutages = async () => {
        // let data = {
        //     "latestOutages" : [
        //         {
        //             "id": 1,
        //             "datacenter": "DC1",
        //             "server": "Server1",
        //             "startDate": "2021-05-01T12:00:00Z",
        //             "isResolved": true,
        //         },
        //         {
        //             "id": 2,
        //             "datacenter": "DC2",
        //             "server": "Server2",
        //             "startDate": "2021-06-01T12:00:00Z",
        //             "isResolved": false,
        //         }
        //     ]
        // }
        // setLatestOutages(data.latestOutages);

        await axios.get(`api/outage`,{
            headers: {
                Authorization: `Bearer ${getAccessToken()}`
            }
        }).then((response) => {
            if(response.status === 200) {
                setLatestOutages(response.data);
            } else {
                setError('Something went wrong');
            }
        }).catch(() => {
            setError('Something went wrong');
        });
    }

    const loadLatestPayments = async () => {
        await axios.get(`api/payment`,{
            headers: {
                Authorization: `Bearer ${getAccessToken()}`
            }
        }).then((response) => {
            if(response.status === 200) {
                setLatestPayments(response.data.slice(0,5));
                console.log(response.data);
            } else {
                setError('Something went wrong');
            }
        }).catch(() => {
            setError('Something went wrong');
        });
    }

    return (
        <div className="container">
            <div className="header">
                <div className="page-name">Home</div>
                <div className="company-name">{companyName}</div>
            </div>
            <div className="content">
                <div className="table-section">
                    <h2>Latest Outages</h2>
                    <table>
                        <thead>
                            <tr>
                                <th>Datacenter</th>
                                <th>Server</th>
                                <th>Start Date</th>
                                <th>Resolved</th>
                            </tr>
                        </thead>
                        <tbody>
                    {latestOutages.map((outage) => (
                        <tr key={outage.id} className="table-item">
                            <th>{outage.dataCenter}</th>
                            <th>{outage.server}</th>
                            <th>{new Date(outage.startDate).toLocaleString()}</th>
                            <th className={outage.isResolved ? 'resolved' : 'not-resolved'}>
                                {outage.isResolved ? '✓' : '❗'}
                            </th>
                        </tr>
                    ))}
                        </tbody>
                    </table>
                </div>
                <div className="table-section">
                    <div className="payments-header">
                        <h2>Latest Payments</h2>
                        <button onClick={() => navigate('/payments')}>All Payments</button>
                    </div>
                    <table>
                        <thead>
                        <tr>
                            <th>Outage Id</th>
                            <th>Customer</th>
                            <th>Service</th>
                            <th>Amount</th>
                        </tr>
                        </thead>
                        <tbody>
                    {latestPayments.map((payment) => (
                        <tr key={payment.id} className="table-item">
                            <td>{payment.outage}</td>
                            <td>{payment.customer}</td>
                            <td>{payment.service}</td>
                            <td>{payment.amount}</td>
                        </tr>
                    ))}
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    )
}