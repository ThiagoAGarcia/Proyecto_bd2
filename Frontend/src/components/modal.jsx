export default function Modal({open, onClose, children}) {
  return (
    <div
      onClick={onClose}
      className={`fixed inset-0 z-[200] flex justify-center items-start lg:items-center transition-colors overflow-y-auto py-4 ${
        open ? 'visible bg-black/20' : 'invisible'
      } `}>
      <div
        onClick={(e) => e.stopPropagation()}
        className={`bg-white w-full md:w-[90%] lg:w-auto rounded-xl shadow p-6 transition-all my-auto ${
          open ? 'scale-100 opacity-100' : 'scale-125 opacity-0'
        } `}>
        <button
          className="absolute hidden lg:inline cursor-pointer top-2 right-2 p-1 rounded-lg text-gray-400 bg-white hover:bg-gray-50 hover:text-gray-600"
          onClick={onClose}>
          <i className="fa-solid fa-xmark fa-lg"></i>
        </button>
        {children}
      </div>
    </div>
  )
}