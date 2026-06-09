import {BrowserRouter, Routes, Route} from 'react-router-dom'
import './App.css'

import Login from './views/IntroductionView/Login.jsx'

function App() {

  return (
    <>
      <BrowserRouter>
        <Routes>
          <Route element={<Login />} path="/" />
        </Routes>
      </BrowserRouter>
    </>
  )
}

export default App
