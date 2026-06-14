import { useState, useEffect } from 'react'

export default function Groups() {
  return (
    <>
      <div className="text-xl">
        <div className="sm:flex justify-between sm:items-end items-start w-full sm:pb-4">
          <h1 className="font-sans text-3xl font-bold text-[#0a1628] uppercase tracking-wide leading-none">
            Mis <span className="text-[#c8a84b]">entradas</span>
          </h1>
        </div>

        <div className="flex flex-col gap-2.5">
          <div className="border border-[#d0dcea] rounded-xl p-3.5 transition-all duration-150 hover:border-[#a0b8d8] hover:shadow-[0_2px_10px_rgba(0,107,182,0.08)]">

            <div className="flex md:hidden flex-col gap-3">
              <div className="flex items-center gap-3">
                <div className="p-2.5 rounded-lg bg-[#0a1628] flex items-center justify-center shrink-0">
                  <img className="w-8 h-auto" src="https://upload.wikimedia.org/wikipedia/commons/f/fe/Flag_of_Uruguay.svg" alt="Uruguay" />
                  <span className="px-2 font-extrabold text-[#e0c472] font-['Barlow_Condensed']">-</span>
                  <img className="w-8 h-auto" src="https://upload.wikimedia.org/wikipedia/commons/0/05/Flag_of_Brazil.svg" alt="Brasil" />
                </div>
                <div className="min-w-0">
                  <div className="text-sm font-semibold text-[#0a1628] truncate">Uruguay vs Brasil</div>
                  <div className="text-xs text-[#7a8fa6] mt-0.5">
                    <i className="fa-solid fa-location-dot text-[10px]" /> MetLife Stadium
                  </div>
                  <div className="text-xs text-[#7a8fa6]">
                    <i className="fa-solid fa-calendar text-[10px]" /> 18 jun · 21:00
                  </div>
                </div>
              </div>

              <div className="flex gap-2 border-t border-[#eaf0f8] pt-2.5">
                <button className="flex-1 inline-flex items-center justify-center gap-1.5 px-3 py-2.5 rounded-lg text-sm font-semibold cursor-pointer transition-all font-['Inter'] bg-transparent border border-[#d0dcea] text-[#0a1628] hover:bg-[#f0f4fa]">
                  <i className="fa-solid fa-qrcode" /> Ver entrada
                </button>
                <button className="flex-1 inline-flex items-center justify-center gap-1.5 px-3 py-2.5 rounded-lg text-sm font-semibold cursor-pointer transition-all font-['Inter'] bg-transparent border border-[#d0dcea] text-[#0a1628] hover:bg-[#f0f4fa]">
                  <i className="fa-solid fa-paper-plane" /> Transferir
                </button>
              </div>
            </div>

            <div className="hidden md:flex items-center justify-between gap-3">
              <div className="flex items-center gap-3.5 flex-1 min-w-0">
                <div className="p-3 rounded-lg bg-[#0a1628] flex items-center justify-center shrink-0">
                  <img className="w-10 h-auto" src="https://upload.wikimedia.org/wikipedia/commons/f/fe/Flag_of_Uruguay.svg" alt="Uruguay" />
                  <span className="px-3 font-extrabold text-[#e0c472] font-['Barlow_Condensed']">-</span>
                  <img className="w-10 h-auto" src="https://upload.wikimedia.org/wikipedia/commons/thumb/1/1a/Flag_of_Argentina.svg/960px-Flag_of_Argentina.svg.png" alt="Brasil" />
                </div>
                <div className="min-w-0">
                  <div className="font-semibold text-[#0a1628] truncate">Uruguay vs Brasil</div>
                  <div className="text-sm text-[#7a8fa6] mt-0.5">
                    <i className="fa-solid fa-location-dot text-[11px]" /> MetLife Stadium · 18 jun, 21:00
                  </div>
                </div>
              </div>
              <div className="flex gap-1.5 shrink-0">
                <button className="inline-flex items-center gap-1.5 px-3 py-4 rounded-lg text-xl font-medium cursor-pointer transition-all font-['Inter'] bg-transparent border border-[#d0dcea] text-[#0a1628] hover:bg-[#f0f4fa]">
                  <i className="fa-solid fa-qrcode" /> Ver
                </button>
                <button className="inline-flex items-center gap-1.5 px-3 py-4 rounded-lg text-xl font-medium cursor-pointer transition-all font-['Inter'] bg-transparent border border-[#d0dcea] text-[#0a1628] hover:bg-[#f0f4fa]">
                  <i className="fa-solid fa-paper-plane" /> Transferir
                </button>
              </div>
            </div>

          </div>
        </div>
      </div>
    </>
  )
}