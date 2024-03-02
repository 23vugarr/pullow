import React from "react";

const Milestone = ({ date, cashout, investmentReturn }) => {
    return (
        <div className="milestone">
            <p className="date">{date}</p>
            {cashout && <p className="cashout">{cashout}</p>}
            {investmentReturn && <p className="investment-return">{investmentReturn}</p>}
        </div>
    );
};

export default Milestone;