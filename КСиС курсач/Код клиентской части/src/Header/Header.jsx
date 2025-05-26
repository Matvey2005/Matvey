import classes from './Header.module.css'

export const Header = ({ onOpen }) => {

	async function handlerLogOut() {
		try {
			const response = await fetch('https://localhost:7104/logout', {
				method: 'GET',
				credentials: 'include', // чтобы отправить куки
				// Преобразовываем объект в строку
			});

			if (!response.ok) {
				throw new Error(`HTTP error! Status: ${response.status}`);
			}

			onOpen()

		} catch (error) {
			console.error(error);
		}
	}

	const styleButton = {
		display: 'flex',
		alignItems: 'center'
	}

	return (
		<div className={classes.header}>

			<div style={styleButton} className="buttons">


			</div>
			<img style={{
				height: '2rem',
				width: '2rem',
				position: 'fixed',
				top: '2rem',
				left: 'calc(50% - 1rem)'
			}} onClick={handlerLogOut} src="../../img/2305619_door_exit_logout_open_out_icon.svg" alt="" />

		</div>
	)
}