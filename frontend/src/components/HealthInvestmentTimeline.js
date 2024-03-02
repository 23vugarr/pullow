// HealthInvestmentTimeline.jsx
import React from 'react';
import '../style/HealhtInvestmentTimeline.css';

const HealthInvestmentTimeline = () => {
    return (
        <div className="timeline-container">
            <div className="timeline-header">
                <h2>Your health is secured 😊✅</h2>
            </div>
            <div className="timeline">
                <Milestone date="1st of March 2024" />
                <TimelineEvent
                    title="Narimanov"
                    description="1 bedroom"
                    imageSrc="path-to-narimanov-image.jpg"
                />
                <TimelineEvent
                    title="Binagadi"
                    description="128 m²"
                    imageSrc="path-to-binagadi-image.jpg"
                />
                <Milestone date="1st of March 2027" cashout="30,000 AZN cashout" />
                <TimelineEvent
                    title="Narimanov"
                    description="128 m²"
                    imageSrc="path-to-narimanov-2-image.jpg"
                />
                <Milestone date="1st of March 2030" investmentReturn="Stock investment + 6,000 AZN" />
            </div>
        </div>
    );
};

const TimelineEvent = ({ title, description, imageSrc }) => {
    return (
        <div className="timeline-event">
            <img src={imageSrc} alt={`${title} thumbnail`} />
            <div className="event-details">
                <h3>{title}</h3>
                <p>{description}</p>
            </div>
        </div>
    );
};

const Milestone = ({ date, cashout, investmentReturn }) => {
    return (
        <div className="milestone">
            <p className="date">{date}</p>
            {cashout && <p className="cashout">{cashout}</p>}
            {investmentReturn && <p className="investment-return">{investmentReturn}</p>}
        </div>
    );
};

export default HealthInvestmentTimeline;
