import React, { useRef, useState, useEffect } from 'react';

export const Dialog = ({ isOpen, onClose, children }) => {
	if (!isOpen) return null;


	return (
		<div style={{
			position: 'fixed',
			top: 0,
			left: 0,
			right: 0,
			bottom: 0,
			backgroundColor: 'rgba(0,0,0,0.5)',
			display: 'flex',
			justifyContent: 'center',
			alignItems: 'center',
			zIndex: 1000
		}}>
			<div style={{
				backgroundColor: 'white',
				padding: '20px',
				borderRadius: '8px',
				minWidth: '300px',
				border: '3px solid #3e9ac9'
			}}>
				{React.cloneElement(children, { onClose })}
			</div>
		</div>
	);
};
