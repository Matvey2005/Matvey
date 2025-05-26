import classes from './Events.module.css';
import { Dialog } from "../Dialog/Dialog";
import React, { useState } from 'react';
import { AddEdit } from '../AddEdit/AddEdit';

export const Events = ({ data, setData, selectedDate }) => {
	const [isShowAddEdit, setIsShowAddEdit] = useState(false);
	const [editEvent, setEditEvent] = useState(null)

	const formatDate = (date) =>
		date.toLocaleDateString('ru-RU', {
			day: 'numeric',
			month: 'long',
			year: 'numeric',
		});

	const handleDelete = async (eventId) => {
		try {
			const response = await fetch(`https://localhost:7104/events/${eventId}`, {
				method: 'DELETE',
				credentials: 'include',
			});

			if (!response.ok) {
				throw new Error(`Ошибка при удалении: ${response.status}`);
			}

			setData(prev => prev.filter(event => event.id !== eventId));
		} catch (error) {
			console.error("Ошибка при удалении события:", error);
		}
	};

	const handleEdit = (ev) => {
		setEditEvent(ev)
		setIsShowAddEdit(true)
	}

	const handleAdd = () => {
		setEditEvent(null)
		setIsShowAddEdit(true)
	}

	return (
		<div className={classes.catalog} >
			<div style={{ display: 'flex', justifyContent: 'space-between' }}>
				<h1 style={{ margin: '0px 10px 0px 10px', fontSize: '1.5rem' }}>События на {formatDate(selectedDate)}</h1>
				<button className={classes.addEventButton} onClick={handleAdd} ><img style={{ height: '1rem' }} src="../../img/plus.svg" alt="" />Добавить</button>
			</div>

			<ul className={classes.listEvents}>
				{data.map((event) => (
					<li key={event.id}>
						<div style={{ display: 'flex', flexDirection: 'column' }}>
							<div>{event.description}</div>
							<div>
								{new Date(event.time).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
							</div>
						</div>



						<div style={{ display: 'flex', alignItems: 'center' }}>

							<div className={classes.div_svg} onClick={() => handleEdit(event)}>
								<svg width="14" height="14" viewBox="0 0 45.2 45.2" fill="black" cursor="pointer"><path d="M1 44.2l3.7-14.9L27.4 6.6l11.2 11.2-22.7 22.7zm6.3-13.5l-2.4 9.6 9.6-2.4 20.2-20.2-7.2-7.2z"></path><path d="M13.5 39.6c-1-3.9-4-6.9-7.9-7.9l.7-2.8a13.47 13.47 0 0 1 9.9 9.9l-2.7.8"></path><path d="M13.2 34.1l-2-2.1 19.9-19.9 2 2-19.9 20M3 42.2l4.4-1.1a4.6 4.6 0 0 0-3.3-3.3L3 42.2m36.7-25.5L28.5 5.5 33 1l.7.1c5.4.7 9.7 5 10.4 10.4l.1.7zM32.6 5.5l7.2 7.2 1.4-1.4A9.47 9.47 0 0 0 34 4.1z"></path></svg>
							</div>


							<div className={classes.div_svg} onClick={() => handleDelete(event.id)}>
								<svg width="14" height="14" viewBox="0 0 16 16" fill="black" cursor="pointer"><path d="M8 15A7 7 0 1 1 8 1a7 7 0 0 1 0 14zm0 1A8 8 0 1 0 8 0a8 8 0 0 0 0 16z"></path><path d="M4.646 4.646a.5.5 0 0 1 .708 0L8 7.293l2.646-2.647a.5.5 0 0 1 .708.708L8.707 8l2.647 2.646a.5.5 0 0 1-.708.708L8 8.707l-2.646 2.647a.5.5 0 0 1-.708-.708L7.293 8 4.646 5.354a.5.5 0 0 1 0-.708z"></path></svg>
							</div>



						</div>
					</li>
				))}
			</ul>


			<Dialog isOpen={isShowAddEdit} onClose={() => setIsShowAddEdit(false)}>
				<AddEdit event={editEvent} setData={setData} selectedDate={selectedDate} />
			</Dialog>


		</div>
	);
};
