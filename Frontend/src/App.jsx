import { BrowserRouter, Routes, Route } from 'react-router-dom'
import './App.css'

import Login from './views/IntroductionView/Login.jsx'
import Register from './views/IntroductionView/Register.jsx'
import MainUser from './views/MainView/User/MainUser.jsx'
import MainAdmin from './views/MainView/Admin/MainAdmin.jsx'
import Profile from './views/Profile.jsx'
import {ToastContainer} from 'react-toastify'

function App() {

  return (
    <>
        <BrowserRouter>
          <Routes>
            <Route element={<Login />} path="/" />
            <Route element={<Register />} path="/register" />
            <Route element={<MainUser />} path="/main-user" />
            <Route element={<MainAdmin />} path="/main-admin" />
            <Route element={<Profile />} path="/profile" />
          </Routes>
        </BrowserRouter>
        <ToastContainer position="bottom-left" style={{zIndex: 1000}} />
    </>
  )
}

export default App
