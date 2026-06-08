import React from "react";
import { Link, useNavigate } from "react-router-dom";

const Header: React.FC = () => {
  const navigate = useNavigate();
  const token = localStorage.getItem("token");
  const role = localStorage.getItem("role");

  const handleLogout = () => {
    localStorage.removeItem("token");
    localStorage.removeItem("role");
    navigate("/");
  };

  return (
       <header className="bg-gray-900 text-gray-100 shadow-md">
      <div className="container mx-auto px-6 py-4 flex justify-between items-center">
        <Link
          to="/"
          className="text-2xl font-bold tracking-wide text-indigo-400 hover:text-indigo-600"
        >
          Pistachio
        </Link>

        <nav className="space-x-6 text-gray-300">
          <Link to="/" className="hover:text-indigo-400 transition">Início</Link>
          <Link to="/services" className="hover:text-indigo-400 transition">Serviços</Link>

          {!token && (
            <Link to="/login" className="hover:text-indigo-400 transition">
              Login
            </Link>
          )}

          {token && role === "User" && (
            <>
              <Link to="/my-schedulings" className="hover:text-indigo-400 transition">
                Meus Agendamentos
              </Link>
              <button
                onClick={handleLogout}
                className="hover:text-red-400 transition"
              >
                Sair
              </button>
            </>
          )}
        </nav>
      </div>
    </header>
  );
};

export default Header;
