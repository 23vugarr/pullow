// HealthInvestmentTimeline.jsx
import React from 'react';
import '../style/HealhtInvestmentTimeline.css';
import Milestone from "./Milestone";
import TimelineEvent from "./TimeLineEvent";
import {alternativeTimeLines, availableTimeLines} from "./helper/TimeLineData";

const HealthInvestmentTimeline = (timeLineIndex) => {

    const timeLine = availableTimeLines[1];
    const altTimeLine = alternativeTimeLines[1];

    return (
        <div className="timeline-container">
            <div className="timeline-header">
                <h2>Your health is secured 😊✅</h2>
            </div>
            <div className="timeline">
                {
                    timeLine.map((item) => {
                        if (item.type % 2 === 0) {
                            return (
                                <TimelineEvent
                                    title={item.title}
                                    description={item.description}
                                    imageSrc={item.imageSrc}
                                />
                            );
                        } else {
                            return (
                                <Milestone date={item.date} cashout={item.cashout}
                                           investmentReturn={item.investmentReturn}/>
                            );
                        }
                    })
                }
            </div>
            {
                <div className="timeline timeline-reverse" style={{width: "50%", marginLeft: "50%"}}>
                    {
                        altTimeLine.map((item) => {
                            if (item.type % 2 === 0) {
                                return (
                                    <TimelineEvent
                                        title={item.title}
                                        description={item.description}
                                        imageSrc={item.imageSrc}
                                    />
                                );
                            } else {
                                return (
                                    <Milestone date={item.date} cashout={item.cashout}
                                               investmentReturn={item.investmentReturn}/>
                                );
                            }
                        })
                    }
                </div>
            }
        </div>
    );
};

export default HealthInvestmentTimeline;
