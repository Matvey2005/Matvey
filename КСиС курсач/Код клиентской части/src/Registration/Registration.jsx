import { useState } from "react"
import styles from './Registration.module.css';
import { UNSAFE_decodeViaTurboStream } from "react-router-dom";
import { useNavigate } from "react-router-dom";

export const Registration = ({ onClose }) => {

	const navigate = useNavigate(); // Создаём объект navigate для навигации
	const [name, setName] = useState('')
	const [email, setEmail] = useState('')
	const [password, setPassword] = useState('')

	const [isSignIn, setSignIn] = useState(false)
	const changeName = (e) => {
		setName(e.target.value)
	}

	const changeEmail = (e) => {
		setEmail(e.target.value)
	}

	const changePassword = (e) => {
		setPassword(e.target.value)
	}

	const handleSignUp = async () => {

		if (!isSignIn) {
			const data = {
				UserName: name,
				Password: password,
				Email: email
			};

			try {
				const response = await fetch('https://localhost:7104/registr', {
					method: 'POST',
					credentials: 'include', // чтобы отправить куки
					headers: {
						'Content-Type': 'application/json' // Указываем, что отправляем JSON
					},
					body: JSON.stringify(data), // Преобразовываем объект в строку
				});

				if (!response.ok) {
					throw new Error(`HTTP error! Status: ${response.status}`);
				}


				setName('')
				setPassword('')
				setSignIn(true);

			} catch (error) {
				//setSignIn(false);
				console.error("Ошибка при загрузке данных:", error);
			}
		}
		else {
			setSignIn(false);
		}
	}

	const handleSignIn = async () => {
		// Устанавливаем состояние входа

		if (isSignIn) {
			const data = {
				UserName: name,
				Password: password
			};

			try {
				const response = await fetch('https://localhost:7104/login', {
					method: 'POST',
					credentials: 'include', // чтобы отправить куки
					headers: {
						'Content-Type': 'application/json' // Указываем, что отправляем JSON
					},
					body: JSON.stringify(data), // Преобразовываем объект в строку
				});

				if (!response.ok) {
					alert('Пользователь с таким именем уже сущетсвует')
					throw new Error(`HTTP error! Status: ${response.status}`);
				}


				onClose()

			} catch (error) {
				console.error("Ошибка при загрузке данных:", error);
			}
		}
		else {
			setSignIn(true);
		}




	};

	return (
		<div className={styles.reg}>
			{!isSignIn ? (
				<>
					<div className={styles.containerReg}>
						<h1 style={{ textAlign: 'center' }} >Регистрация</h1>
						<input onChange={changeName} type="text" placeholder="Введите имя" value={name} />
						<input onChange={changeEmail} type="email" placeholder="Введите email" value={email} />
						<input onChange={changePassword} type="text" placeholder="Введите пароль" value={password} />
					</div>

				</>
			) : (
				<>
					<div className={styles.containerAuth}>
						<h1 style={{ textAlign: 'center' }}>Вход</h1>
						<input onChange={changeName} type="text" placeholder="Введите имя" value={name} />
						<input onChange={changePassword} type="text" placeholder="Введите пароль" value={password} />
					</div>
				</>
			)}

			<button style={{ padding: '0.3rem', borderRadius: '0.5rem', border: '1px solid #3e9ac9', backgroundColor: 'white' }} onClick={handleSignUp}>Зарегистрироваться</button>
			<button style={{ padding: '0.3rem', borderRadius: '0.5rem', border: '1px solid #3e9ac9', backgroundColor: 'white' }} onClick={handleSignIn}>Войти</button>
		</div>
	);
}