import { BrowserRouter, Routes, Route } from 'react-router-dom'
import './App.css'
import Protected from './components/protected.jsx'
import Login from './views/IntroductionView/Login.jsx'
import Register from './views/IntroductionView/Register.jsx'
import MainUser from './views/MainView/User/MainUser.jsx'
import MainAdmin from './views/MainView/Admin/MainAdmin.jsx'
import Profile from './views/Profile.jsx'
import SinToken from './components/sinToken.jsx'
import GruposView from './views/IntroductionView/GrupoView.jsx'
import PartidosView from './views/IntroductionView/partidoView.jsx'
import { ToastContainer } from 'react-toastify'

function App() {
  return (
    <>
      <BrowserRouter>
        <Routes>
          <Route element={<Login />} path="/" />
          <Route element={<Register />} path="/register" />
          <Route path="/grupos" element={<GruposView />} />
          <Route path="/partidos/:grupo" element={<PartidosView />} />
          <Route element={<Protected allowedRoles={'Administrador'} />}>
            <Route element={<MainAdmin />} path="/main-admin" />
          </Route>
          <Route element={<Protected allowedRoles={'Usuario'} />}>
            <Route element={<MainUser />} path="/main-user" />
          </Route>
          <Route element={<Protected allowedRoles={['Administrador', 'Usuario']} />}>
            <Route element={<Profile />} path="/profile" />
          </Route>
          <Route element={<SinToken />} path="/sin-token" />
        </Routes>
      </BrowserRouter>
      <ToastContainer position="bottom-left" style={{ zIndex: 1000 }} />
    </>
  )
}

export default App
