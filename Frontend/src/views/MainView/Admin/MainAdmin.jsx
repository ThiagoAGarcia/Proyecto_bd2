import NavBar from '../../../components/navBar'
import Footer from '../../../components/footer'
import MatchManagement from './Tabs/MatchManagement'
import StadiumManagement from './Tabs/StadiumManagement'
import { useState, useRef } from 'react'
import { useNavigate } from 'react-router-dom'

export default function MainUser() {
    const [activeTab, setActiveTab] = useState("Gestion partidos");

    return (
        <div className="flex flex-col min-h-screen bg-gray-50">

            <NavBar />

            <section className="grow flex flex-col items-start justify-center sm:px-10 py-4">
                <div className="w-full flex justify-start overflow-x-auto overflow-y-hidden mx-0">
                    {[
                        { id: "Gestion partidos", label: "Gestión partidos", icon: "fa-futbol" },
                        { id: "Gestion estadios", label: "Gestión estadios", icon: "fa-earth-americas" },
                        { id: "Gestion funcionarios", label: "Gestión funcionarios", icon: "fa-user-gear" },
                        { id: "Gestion dispositivo", label: "Gestión dispositivos", icon: "fa-camera" },
                        { id: "Estadisticas", label: "Estadísticas", icon: "fa-chart-simple" }
                    ].map((tab) => (
                        <button key={tab.id} onClick={() => setActiveTab(tab.id)} className={`cursor-pointer relative border-b-white -mb-1 px-6 py-3 text-lg font-medium transition-all duration-200 rounded-t-2xl border bg-[#f0f4fa] text-[#7a8fa6] flex items-center gap-2  ${activeTab === tab.id ? "bg-white border-gray-300 border-b-white text-[#052e66] " : "hover:bg-[#e0e8f4] hover:text-[#0a1628] border-transparent  bg-[#f0f4fa] text-[#7a8fa6]"}`}>
                            <i className={`fa-solid ${tab.icon} mr-2`}></i>
                            <span className="hidden lg:inline">{tab.label}</span>
                        </button>
                    ))}
                </div>

                <div className="w-full sm:max-w-9xl bg-white border border-gray-300 rounded-b-2xl rounded-tr-2xl shadow-md flex flex-col h-[68vh] relative z-10">
                    <div className="sm:p-8 p-4 text-gray-700 text-lg overflow-y-auto scrollbar">
                        {activeTab === "Gestion partidos" && <MatchManagement />}
                        {activeTab === "Gestion estadios" && <StadiumManagement />}
                    </div>
                </div>
            </section>
            <Footer />
        </div>
    )
}