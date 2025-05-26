import { useEffect, useState } from "react"; // <== добавь импорт
//import { Routes, Route } from "react-router-dom"
import { MyCalendar } from "../MyCalendar/MyCalendar"
import { Header } from "../Header/Header"
import { Events } from '../Events/Events'
import { Dialog } from "../Dialog/Dialog";
import classes from './Layout.module.css'

export const Layout = ({ open }) => {

	let buttinStyle = {
		position: 'fixed',
		bottom: '5px',
		right: '5px'
	}
	const [data, setData] = useState([]);
	const [selectedDate, setSelectedDate] = useState(new Date());

	const loadData = async (targetDate) => {
		try {
			const firstDay = new Date(targetDate.getFullYear(), targetDate.getMonth(), 1, 0, 0, 0);
			const lastDay = new Date(targetDate.getFullYear(), targetDate.getMonth() + 1, 0, 23, 59, 59);

			const response = await fetch(`https://localhost:7104/events?from=${firstDay.toISOString()}&to=${lastDay.toISOString()}`, {
				method: 'GET',
				credentials: 'include'
			});

			if (!response.ok) {
				throw new Error(`HTTP error! Status: ${response.status}`);
			}

			const result = await response.json();
			setData(result.sort((a, b) => new Date(a.time) - new Date(b.time)));
			console.log('Загруженные события:', result);
		} catch (error) {
			console.error("Ошибка при загрузке данных:", error);
		}
	};

	useEffect(() => {


		loadData(selectedDate);
	}, []);

	const handleViewDateChange = (viewDate) => {
		loadData(viewDate);
	};

	return (
		<div className={classes.container}>
			<Header onOpen={open} />
			<MyCalendar date={selectedDate} onDateChange={setSelectedDate} events={data} onViewDateChange={handleViewDateChange} />
			<Events selectedDate={selectedDate} setData={setData} data={data.filter(x => new Date(x.time).toDateString() === selectedDate.toDateString())} />


		</div>
	)
}


