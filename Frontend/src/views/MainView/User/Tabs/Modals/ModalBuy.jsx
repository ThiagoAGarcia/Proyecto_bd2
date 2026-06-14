import { useState, useEffect } from 'react';
import Modal from './../../../../../components/modal';
import { toast } from 'react-toastify';
import getEstadio from './../../../../../services/EstadioService/getEstadio';
import getAllHabilita from './../../../../../services/HabilitaService/getAllHabilita';

const formatCardNumber = (value) =>
    value
        .replace(/\D/g, '')
        .slice(0, 16)
        .replace(/(.{4})/g, '$1 ')
        .trim();

const formatExpiry = (value) => {
    const digits = value.replace(/\D/g, '').slice(0, 4);
    if (digits.length <= 2) return digits;
    return `${digits.slice(0, 2)}/${digits.slice(2)}`;
};

export default function ProfileModal({ open, onClose, identificadorEstadio, identificadorPartido }) {
    const [estadios, setEstadios] = useState(null);
    const [selectedSector, setSelectedSector] = useState(null);
    const [sectores, setSectores] = useState([]);

    useEffect(() => {
        if (!open) return;

        async function loadData() {
            try {
                setEstadios(null);
                setSectores([]);
                setSelectedSector(null);

                const [estadio, habilita] = await Promise.all([
                    getEstadio(identificadorEstadio),
                    getAllHabilita(
                        identificadorEstadio,
                        identificadorPartido
                    )
                ]);

                setEstadios(estadio || null);
                setSectores(habilita || []);
            } catch (error) {
                console.error(error);
                setEstadios(null);
                setSectores([]);
            }
        }

        loadData();
    }, [open, identificadorEstadio, identificadorPartido]);

    useEffect(() => {
        if (!open) {
            setEstadios(null);
            setSectores([]);
            setSelectedSector(null);
        }
    }, [open]);

    

    return (
        <Modal open={open} onClose={onClose}>
            <form className="w-full flex lg:flex-row flex-col max-w-7xl">
                <div className="lg:p-10 p-0">
                    <div className="relative h-72 w-full overflow-hidden rounded-2xl sm:h-120">
                        <img
                            src={
                                estadios?.imagen ||
                                'https://www.shutterstock.com/shutterstock/videos/3422382539/thumb/1.jpg?ip=x480'
                            }
                            alt={estadios?.nombre || 'Estadio'}
                            className="h-full w-full object-cover"
                            onError={(e) => {
                                e.currentTarget.src = '/images/default-stadium.jpg';
                            }}
                        />

                        <div className="absolute inset-0 bg-linear-to-t from-[#0B1D3A]/90 via-[#0B1D3A]/20 to-transparent" />

                        <div className="absolute left-0 top-0 h-1.5 w-full bg-linear-to-r from-[#D4AF37] via-[#F4D784] to-[#D4AF37]" />

                        <div className="absolute bottom-0 left-0 right-0 p-6">
                            <h3 className="text-3xl font-bold text-white leading-tight">
                                {estadios?.nombre || 'Cargando estadio...'}
                            </h3>

                            <span className="text-white/90">
                                {estadios
                                    ? `${estadios.direccionCalle} ${estadios.direccionNumero}, ${estadios.direccionCodigoPostal} ${estadios.direccionLocalidad}`
                                    : 'Cargando ubicación...'}
                            </span>
                        </div>
                    </div>
                </div>
                <div>
                    <div className="mt-8">
                        <label htmlFor="sector" className="mb-2 block text-base font-semibold text-[#14315C]">
                            Elegí tu sector
                        </label>

                        <div className="relative">
                            <select
                                value={selectedSector?.identificador || ''}
                                disabled={sectores.length === 0}
                                onChange={(e) => { const sector = sectores.find( s => s.identificador === Number(e.target.value) ); setSelectedSector(sector || null); }}
                                className="w-full appearance-none rounded-xl border-2 border-[#14315C]/15 bg-white px-5 py-4 pr-12 text-lg text-[#14315C] font-medium shadow-sm transition focus:border-[#D4AF37] focus:outline-none focus:ring-2 focus:ring-[#D4AF37]/40"
                            >
                                {sectores.length === 0 ? (
                                    <option value="">
                                        No hay sectores disponibles
                                    </option>
                                ) : (
                                    <>
                                        <option value="" disabled>
                                            Seleccionar sector
                                        </option>

                                        {sectores.map((sector) => (
                                            <option
                                                key={sector.identificador}
                                                value={sector.identificador}
                                            >
                                                {sector.nombre} — {sector.tarifaExtra} USD
                                            </option>
                                        ))}
                                    </>
                                )}
                            </select>
                            <i className="fa-solid fa-angle-down pointer-events-none absolute right-4 top-1/2 h-6 w-6 -translate-y-1/2 text-[#14315C]/60"></i>
                        </div>

                        {selectedSector && (
                            <div className="mt-4 flex items-center justify-between rounded-xl border border-[#14315C]/10 bg-linear-to-r from-[#14315C]/3 to-[#D4AF37]/6 px-5 py-4">
                                <div>
                                    <p className="text-base font-semibold text-[#14315C]">
                                        {selectedSector.nombre}
                                    </p>
                                </div>

                                <p className="text-xl font-bold text-[#D4AF37]">
                                    + {selectedSector.tarifaExtra} USD
                                </p>
                            </div>
                        )}
                    </div>

                    <div className="mt-8">
                        <div className="mb-4 flex items-center gap-2">
                            <i className="fa-solid fa-credit-card h-5 w-5 text-[#D4AF37]" />
                            <h4 className="text-base font-semibold text-[#14315C]">
                                Datos de la tarjeta
                            </h4>
                        </div>

                        <div className="space-y-4 rounded-2xl border border-[#14315C]/10 bg-[#F8F9FB] p-5">
                            <div>
                                <label htmlFor="cardName" className="mb-1.5 block text-sm font-medium text-[#14315C]/80">
                                    Nombre del titular
                                </label>
                                <input
                                    id="cardName"
                                    type="text"
                                    placeholder="Como figura en la tarjeta"
                                    className="w-full rounded-xl border-2 border-[#14315C]/15 bg-white px-4 py-3 text-[#14315C] placeholder:text-[#14315C]/35 shadow-sm transition focus:border-[#D4AF37] focus:outline-none focus:ring-2 focus:ring-[#D4AF37]/40"
                                />
                            </div>

                            <div>
                                <label htmlFor="cardNumber" className="mb-1.5 block text-sm font-medium text-[#14315C]/80">
                                    Número de tarjeta
                                </label>
                                <div className="relative">
                                    <input
                                        id="cardNumber"
                                        type="text"
                                        inputMode="numeric"
                                        placeholder="0000 0000 0000 0000"
                                        className="w-full rounded-xl border-2 border-[#14315C]/15 bg-white px-4 py-3 pr-12 text-[#14315C] placeholder:text-[#14315C]/35 shadow-sm transition focus:border-[#D4AF37] focus:outline-none focus:ring-2 focus:ring-[#D4AF37]/40"
                                    />
                                    <i className="fa-solid fa-credit-card pointer-events-none absolute right-4 top-1/2 h-5 w-5 -translate-y-1/2 text-[#14315C]/40" />
                                </div>
                            </div>

                            <div className="grid grid-cols-2 gap-4">
                                <div>
                                    <label htmlFor="cardExpiry" className="mb-1.5 block text-sm font-medium text-[#14315C]/80">
                                        Vencimiento
                                    </label>
                                    <input
                                        id="cardExpiry"
                                        type="text"
                                        inputMode="numeric"
                                        placeholder="MM/AA"
                                        className="w-full rounded-xl border-2 border-[#14315C]/15 bg-white px-4 py-3 text-[#14315C] placeholder:text-[#14315C]/35 shadow-sm transition focus:border-[#D4AF37] focus:outline-none focus:ring-2 focus:ring-[#D4AF37]/40"
                                    />
                                </div>

                                <div>
                                    <label htmlFor="cardCvv" className="mb-1.5 block text-sm font-medium text-[#14315C]/80">
                                        CVV
                                    </label>
                                    <input
                                        id="cardCvv"
                                        type="text"
                                        inputMode="numeric"
                                        placeholder="123"
                                        className="w-full rounded-xl border-2 border-[#14315C]/15 bg-white px-4 py-3 text-[#14315C] placeholder:text-[#14315C]/35 shadow-sm transition focus:border-[#D4AF37] focus:outline-none focus:ring-2 focus:ring-[#D4AF37]/40"
                                    />
                                </div>
                            </div>
                        </div>
                    </div>
                    <div className="flex gap-4">
                        <button type="button" onClick={onClose} className="mt-8 w-[50%] cursor-pointer lg:w-full inline lg:hidden rounded-xl bg-[#be2a2a] px-6 py-4 text-lg font-semibold text-white shadow-md transition hover:bg-[#ff1d1d] focus:outline-none focus:ring-2 focus:ring-[#D4AF37]/50 focus:ring-offset-2">
                            Cancelar
                        </button>
                        <button type="submit" className="mt-8 w-[50%] lg:w-full rounded-xl bg-[#14315C] px-6 py-4 text-lg font-semibold cursor-pointer text-white shadow-md transition hover:bg-[#1c4378] focus:outline-none focus:ring-2 focus:ring-[#D4AF37]/50 focus:ring-offset-2">
                            {selectedSector ? `Confirmar compra — ${ selectedSector.precioBase + selectedSector.tarifaExtra } USD` : 'Confirmar compra'}
                        </button>
                    </div>
                </div>
            </form>
        </Modal>
    )
}