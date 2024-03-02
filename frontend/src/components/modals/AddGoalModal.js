import React from 'react';
import styled from 'styled-components';

const ModalBackdrop = styled.div`
  position: fixed;
  z-index: 1040;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
`;

const ModalBox = styled.div`
  background: white;
  border-radius: 5px;
  padding: 20px;
  min-width: 300px;
  z-index: 1050;
`;

const CloseButton = styled.button`
  float: right;
  color: black;
  background: none;
  font-size: 20px;
  cursor: pointer;
`;

export const GoalModal = ({show, onClose, onSubmit }) => {
  if (!show) return null;

  const handleSubmit = (event) => {
    event.preventDefault();
    const form = event.target;
    const goalData = {
        title: form.title.value,
        url: form.url.value
    };
    onSubmit(goalData);
  };

  return (
    <ModalBackdrop>
      <ModalBox className={"modal-box"}>
        <CloseButton onClick={onClose}>&times; </CloseButton>
        <h2>Add Goal</h2>
        <form onSubmit={handleSubmit}>
          <input type="text" name="title" placeholder="Goal Title" required />
          <input type="text" name="url" placeholder="Filtered Url Of The Goal" required />

          <button type="submit">Add Goal</button>
        </form>
      </ModalBox>
    </ModalBackdrop>
  );
};
