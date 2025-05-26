import { useState } from "react";
import Calendar from 'react-calendar';
import 'react-calendar/dist/Calendar.css';
import classes from './MyCalendar.module.css'

export const MyCalendar = ({ date, onDateChange, onViewDateChange, events }) => {

	const eventDates = new Set(
		events.map(e => new Date(e.time).toDateString())
	);

	const renderTileContent = ({ date, view }) => {
		if (view === 'month' && eventDates.has(date.toDateString())) {
			return (
				<div className={classes.dotWrapper}>
					<div className={classes.dot} />
				</div>
			)
		}
		return null;
	};

	return (
		<div className={classes.wrap_calendar}>

			<Calendar
				onChange={onDateChange}
				value={date}
				tileContent={renderTileContent}
				onActiveStartDateChange={({ activeStartDate }) => onViewDateChange(activeStartDate)}

			/>


		</div>
	);

}
