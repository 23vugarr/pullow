import { useEffect, useState } from "react";
import axios from "axios";
import {getAccessToken, getDecodedAccessToken} from "../common/jwtCommon";
import {useNavigate, useParams} from "react-router-dom";
import {GoalModal} from "./modals/AddGoalModal";
import HealthInvestmentTimeline from "./HealthInvestmentTimeline";
import {MemberModal} from "./modals/AddMemberModal";

export const TimeLine = () => {

    const navigate = useNavigate();

    const [load, setLoad] = useState(false);
    const [error, setError] = useState('');

    const [goals, setGoals] = useState([]);
    const [members, setMembers] = useState([]);
    const [showGoalModal, setShowGoalModal] = useState(false);
    const [showMemberModal, setShowMemberModal] = useState(false);

    useEffect(() => {
        if(!load) {
            const onPageLoad = async () => {
                await loadGoals()
                await loadMembers()
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

    const loadGoals = async () => {
        await axios.get(`api/goal`,{
            headers: {
                Authorization: `Bearer ${getAccessToken()}`
            }
        }).then((response) => {
            if(response.status === 200) {
                setGoals(response.data)
            } else {
                setError('Something went wrong');
            }
        }).catch((error) => {
            setError('Something went wrong');
        });
    }

    const loadMembers = async () => {
        await axios.get(`api/member`,{
            headers: {
                Authorization: `Bearer ${getAccessToken()}`
            }
        }).then((response) => {
            if(response.status === 200) {
                setMembers(response.data);
                console.log(response.data);
            } else {
                setError('Something went wrong');
            }
        }).catch((error) => {
            setError('Something went wrong');
        });
    }

    const handleAddGoal = () => {
        setShowGoalModal(true);
    };

    const handleCloseGoalModal = () => {
        setShowGoalModal(false);
    };

    const handleAddMember = () => {
        setShowMemberModal(true);
    };

    const handleCloseMemberModal = () => {
        setShowMemberModal(false);
    };

    const handleSubmitGoal = (goalData) => {
        axios.post("/api/goal", goalData, {
            headers: {
                Authorization: `Bearer ${getAccessToken()}`
            }
        });
        setTimeout(() => window.location.reload(), 1000);
        setShowGoalModal(false);
    };

    const handleSubmitMember = (goalData) => {
        axios.post("/api/member", goalData, {
            headers: {
                Authorization: `Bearer ${getAccessToken()}`
            }
        });
        setTimeout(() => window.location.reload(), 1000);
        setShowMemberModal(false);
    };

    return (
        <div className="container">
            <div className="header">
                <div className="h1" style={{marginLeft: "50%", marginTop: "1%"}}>TimeLine</div>
            </div>
            <div className="content">
                <div className="header">
                    <div className="page-name">{customerName}</div>
                    <div className="company-name">{companyName}</div>
                </div>
                <div className="table-section">
                    <h2>Members</h2>
                    <table>
                        <thead>
                        <tr>
                            <th>Name</th>
                            <th>Current Saving</th>
                            <th>Monthly Payment</th>
                        </tr>
                        </thead>
                        <tbody>
                        {goals.map((goal, index) => (
                            <tr key={index} className="table-item clickable"
                                // onClick={ () => navigate(`/customers/${id}/services/${service.id}`)}
                            >
                                <td> {goal.title}</td>
                            </tr>
                        ))}
                        </tbody>
                    </table>
                    <button className="add-obj-btn" onClick={handleAddMember}>Add Member</button>
                    <MemberModal
                        servers={members}
                        show={showMemberModal}
                        onClose={handleCloseMemberModal}
                        onSubmit={handleSubmitMember}
                    />
                </div>
                <div className="table-section">
                    <h2>Goals</h2>
                    <table>
                        <thead>
                        <tr>
                            <th>Title</th>
                            <th>Status</th>
                        </tr>
                        </thead>
                        <tbody>
                        {goals.map((goal, index) => (
                            <tr key={index} className="table-item clickable"
                                // onClick={ () => navigate(`/customers/${id}/services/${service.id}`)}
                            >
                                <td> {goal.title}</td>
                            </tr>
                        ))}
                        </tbody>
                    </table>
                    <button className="add-obj-btn" onClick={handleAddGoal}>Add Goal</button>
                    <GoalModal
                        servers={members}
                        show={showGoalModal}
                        onClose={handleCloseGoalModal}
                        onSubmit={handleSubmitGoal}
                    />
                </div>

            </div>
                <HealthInvestmentTimeline/>
        </div>
    )
}