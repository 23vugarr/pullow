import React from "react";

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

export default TimelineEvent;