import React, { type ReactNode } from "react";
import { Link, Outlet, useNavigate } from "react-router-dom";


type Props = {
  children?: ReactNode;
};


const AdminLayout: React.FC<Props> = () => {
  const navigate = useNavigate();

  const handleLogout = () => {
    localStorage.removeItem("token");
    localStorage.removeItem("role");
    navigate("/admin");
  };


  return (
    <div className="flex min-h-screen bg-gray-900 text-gray-100">
      {/* Sidebar */}
      <aside className="w-64 bg-gray-800 p-6 flex flex-col">
        <h2 className="text-2xl font-bold mb-8 text-blue-400">Pistachio Admin</h2>
        <nav className="flex flex-col gap-4">
          <Link to="/admin/users" className="hover:text-blue-400">Usuários</Link>
          <Link to="/admin/roles" className="hover:text-blue-400">Roles</Link>
          <Link to="/admin/services" className="hover:text-blue-400">Serviços</Link>
          <Link to="/admin/schedulings" className="hover:text-blue-400">Agendamentos</Link>
          <Link to="/admin/payments" className="hover:text-blue-400">Pagamentos</Link>
          <button
            onClick={handleLogout}
            className="hover:text-red-400 transition"
          >
            Sair
          </button>
                  
        </nav>
      </aside>

      {/* Conteúdo */}
      <main className="flex-1 p-8">
        <Outlet />
      </main>
    </div>
  );
};

export default AdminLayout;
