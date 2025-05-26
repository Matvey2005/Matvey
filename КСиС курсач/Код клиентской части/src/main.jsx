import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { BrowserRouter } from 'react-router-dom'
//import { Routes, Route } from "react-router-dom"
import { Main } from './Main/Main'

createRoot(document.getElementById('root')).render(


  <BrowserRouter>
    < Main />

  </BrowserRouter>
)


