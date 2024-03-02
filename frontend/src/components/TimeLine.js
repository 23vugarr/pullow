import {useEffect, useState} from "react";
import axios from "axios";
import {getAccessToken} from "../common/jwtCommon";
import {useNavigate} from "react-router-dom";
import {GoalModal} from "./modals/AddGoalModal";
import HealthInvestmentTimeline from "./HealthInvestmentTimeline";
import {MemberModal} from "./modals/AddMemberModal";
import {availableTimeLines} from "./helper/TimeLineData";

export const TimeLine = () => {

    const navigate = useNavigate();

    const [load, setLoad] = useState(false);
    const [error, setError] = useState('');

    const [goal, setGoal] = useState({})
    const [goals, setGoals] = useState([]);
    const [members, setMembers] = useState([]);
    const [timeLineIndex, setTimeLineIndex] = useState([]);

    const [showGoalModal, setShowGoalModal] = useState(false);
    const [showMemberModal, setShowMemberModal] = useState(false);

    useEffect(() => {
        if (!load) {
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
        await axios.get(`api/goal`, {
            headers: {
                Authorization: `Bearer ${getAccessToken()}`
            }
        }).then((response) => {
            if (response.status === 200) {
                setGoals(response.data)
                if (goal === undefined || goal === null || goal.id === undefined || goal.id === null) {
                    if (response.data.length > 0) {
                        loadGoal(response.data[0].id)
                    }
                }
            } else {
                setError('Something went wrong');
            }
        }).catch((error) => {
            setError('Something went wrong');
        });
    }

    const loadGoal = async (id) => {
        await axios.get(`api/goal/${id}`, {
            headers: {
                Authorization: `Bearer ${getAccessToken()}`
            }
        }).then((response) => {
            if (response.status === 200) {
                setGoal(response.data)
                handleTimeLineChange(response.data)
            } else {
                setError('Something went wrong');
            }
        })
          .catch((error) => {
            setError('Something went wrong');
          });
    }

    const loadMembers = async () => {
        // await axios.get(`api/member`, {
        //     headers: {
        //         Authorization: `Bearer ${getAccessToken()}`
        //     }
        // }).then((response) => {
        //     if (response.status === 200) {
        //         setMembers(response.data);
        //         console.log(response.data);
        //     } else {
        //         setError('Something went wrong');
        //     }
        // }).catch((error) => {
        //     setError('Something went wrong');
        // });
        setMembers([{
            title: "Ali Maharramli",
            currentSaving: 1000,
            monthlyPayment: 100
        }])
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

    const handleGoalChange = async (goalSelection) => {
        await loadGoal(goalSelection.target.value);
    }

    const handleMemberChange = async (memberSelection) => {
        await loadGoal(memberSelection.target.value);
    }

    const handleTimeLineChange = async (goal) => {
        let ind = 0
        console.log(goal.title)
        if (goal.title.toUpperCase().indexOf("NARIMANOV") !== -1) {
            ind = 0
        } else if (goal.title.toUpperCase().indexOf("BMW") !== -1) {
            ind = 1
        } else {
            ind =  Math.floor(Math.random()*100) % availableTimeLines.length
        }
        setTimeLineIndex(ind)
    }

    return (
        <div className="container">
            <div className="header">
                <div className="h1" style={{marginLeft: "50%", marginTop: "1%"}}>TimeLine</div>
            </div>
            <div className="content">
                {/*<SearchableDropdown/>*/}
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
                        {members.map((member, index) => (
                            <tr key={index} className="table-item clickable"
                                // onClick={ () => navigate()}
                            >
                                <td> {member.title}</td>
                                <td> {member.currentSaving}</td>
                                <td> {member.monthlyPayment}</td>
                            </tr>
                        ))}
                        </tbody>
                    </table>
                    {/*<select name="memberId"*/}
                    {/*        className="form-select form-select-lg"*/}
                    {/*        style={{width: "70%"}}*/}
                    {/*        onChange={(e) => handleMemberChange(e)}>*/}
                    {/*    {goals.map((member) => (*/}
                    {/*        <option key={member.id} value={member.id}>*/}
                    {/*            {member.title}*/}
                    {/*        </option>*/}
                    {/*    ))}*/}
                    {/*</select>*/}
                    <button className="add-obj-btn" onClick={handleAddMember}>Add Member</button>
                    <MemberModal
                        show={showMemberModal}
                        onClose={handleCloseMemberModal}
                        onSubmit={handleSubmitMember}
                    />
                </div>
                <div className="table-section">
                    <h2>Goals</h2>
                    <select name="goalId"
                            className="form-select form-select-lg"
                            style={{width: "70%"}}
                            onChange={(e) => handleGoalChange(e)}>
                        {goals.map((goal) => (
                            <option key={goal.id} value={goal.id}>
                                {goal.title}
                            </option>
                        ))}
                    </select>
                    <div>
                        <button className="add-obj-btn" onClick={handleAddGoal}>Add Goal</button>
                    </div>
                    <GoalModal
                        show={showGoalModal}
                        onClose={handleCloseGoalModal}
                        onSubmit={handleSubmitGoal}
                    />
                </div>

            </div>
            <HealthInvestmentTimeline timeLine={timeLineIndex}/>
        </div>
    )
}