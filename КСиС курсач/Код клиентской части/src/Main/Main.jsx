import { useEffect, useState } from 'react';
import { Layout } from '../Layout/Layout';
import { Registration } from '../Registration/Registration';
import { Dialog } from "../Dialog/Dialog";

export const Main = () => {

	const [authorized, setAuthorized] = useState(false)
	const [isShowRegistration, setIsShowRegistration] = useState(true);

	const handleCloseDialog = () => {
		setAuthorized(true);
		setIsShowRegistration(false)
	};

	const handleOpenDialog = () => {
		setAuthorized(false)
		setIsShowRegistration(true)
	}

	useEffect(() => {
		fetch(`https://localhost:7104/refresh`, {
			method: 'POST',
			credentials: 'include',
		})
			.then(async res => {
				if (res.ok) {

					handleCloseDialog()
				} else {

					handleOpenDialog()
				}
			})
			.catch(() => handleOpenDialog());
	}, [])

	return (
		<>
			{authorized ? <Layout open={handleOpenDialog} /> :
				<Dialog isOpen={isShowRegistration} onClose={() => handleCloseDialog()}>
					<Registration onClose={handleCloseDialog} />
				</Dialog>
			}
		</>
	)
}