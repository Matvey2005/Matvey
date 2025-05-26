import { useState } from "react"
import classes from './AddEdit.module.css'

export const AddEdit = ({ onClose, event, setData, selectedDate }) => {

	function toLocalDateInputValue(date) {
		const local = new Date(date);
		local.setMinutes(date.getMinutes() - date.getTimezoneOffset()); // Сдвигаем на локальный часовой пояс
		return local.toISOString().slice(0, 10); // YYYY-MM-DD
	}


	const eventDate = event ? new Date(event.time) : selectedDate;

	const [description, setDescription] = useState(event?.description || '')
	const [date, setDate] = useState(toLocalDateInputValue(eventDate))
	const [time, setTime] = useState(eventDate.toTimeString().slice(0, 5))

	const handleSubmit = async () => {
		if (!date || !time || !description) {
			alert("Заполните все поля");
			return;
		}

		const combinedDate = new Date(`${date}T${time}`);
		const eventPayload = {
			Description: description,
			Time: combinedDate.toISOString()
		};

		try {
			let response, result;
			if (event?.id) {
				response = await fetch(`https://localhost:7104/events/${event.id}`, {
					method: 'PUT',
					headers: { 'Content-Type': 'application/json' },
					body: JSON.stringify(eventPayload),
					credentials: 'include'
				});
				if (!response.ok) throw new Error('Ошибка при обновлении');
				result = await response.json();
			} else {
				response = await fetch(`https://localhost:7104/events`, {
					method: 'POST',
					headers: { 'Content-Type': 'application/json' },
					body: JSON.stringify(eventPayload),
					credentials: 'include'
				});
				if (!response.ok) throw new Error('Ошибка при добавлении');
				result = await response.json();
			}

			setData(prev => {
				const found = prev.find(ev => ev.id === result.id);
				return found
					? prev.map(ev => ev.id === result.id ? result : ev).sort((a, b) => new Date(a.time) - new Date(b.time))
					: [...prev, result].sort((a, b) => new Date(a.time) - new Date(b.time));
			})
			//closeDialog();
			onClose()
		} catch (err) {
			console.error("Ошибка:", err);
			alert("Не удалось сохранить событие");
		}
	};

	return (
		<div>
			<h1>{event === null ? 'Добавление события' : 'Изменение события'}</h1>
			<div className={classes.containerInput}>
				<input type="date" value={date} onChange={(e) => setDate(e.target.value)} />
				<input type="time" value={time} onChange={(e) => setTime(e.target.value)} />
				<input type="text" placeholder="Событие" value={description} onChange={(e) => setDescription(e.target.value)} />
			</div>
			<div className={classes.containerButton}>
				<button style={{
					padding: '0.3rem', borderRadius: '0.5rem', border: '1px solid #3e9ac9', backgroundColor: 'white'
				}} onClick={onClose} >Закрыть</button>
				<button style={{ padding: '0.3rem', borderRadius: '0.5rem', border: '1px solid #3e9ac9', backgroundColor: 'white' }} onClick={handleSubmit} >Сохранить</button>
			</div>

		</div>
	)
}