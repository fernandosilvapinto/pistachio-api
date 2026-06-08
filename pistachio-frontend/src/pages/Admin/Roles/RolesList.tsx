import { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import {  PlusIcon, PencilIcon, TrashIcon } from "@heroicons/react/24/solid";

interface Role {
  id: number;
  name: string;
}

export default function RolesList(){
  const [roles, setRoles] = useState<Role[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [search, setSearch] = useState("");

const token = localStorage.getItem("token"); // pega token do localStorage

const fetchRoles = async () => {
    try {
      setLoading(true);
      const res = await fetch("/api/roles", {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });
      if (!res.ok) throw new Error("Erro ao buscar funções");
      const data: Role[] = await res.json();
      setRoles(data);
    } catch (err) {
      setError("Erro ao carregar funções.");
    } finally {
      setLoading(false);
    }
  };

    const handleDelete = async (id: number) => {
    if (!confirm("Tem certeza que deseja excluir esta função?")) return;
    try {
      const res = await fetch(`/api/roles/${id}`, {
        method: "DELETE",
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });
      if (!res.ok) throw new Error("Erro ao deletar função");
      setRoles(roles.filter((r) => r.id !== id));
    } catch {
      setError("Erro ao excluir função.");
    }
  };

    useEffect(() => {
    fetchRoles();
  }, []);

   const filteredRoles = roles.filter((s) =>
    s.name.toLowerCase().includes(search.toLowerCase())
  );

  return (
    <div className="p-6">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold  text-gray-100">Funções</h1>
        <Link
          to="/admin/roles/new"
          className="bg-indigo-600 hover:bg-indigo-700 px-4 py-2 rounded-md font-semibold transition"
          
        >
          <PlusIcon className="w-5 h-5 cursor-pointer" />
          {/* Novo Serviço */}
        </Link>
      </div>

      <input
        type="text"
        placeholder="Buscar função..."
        className="w-full mb-4 px-4 py-2 rounded-md bg-gray-700 text-gray-200 placeholder-gray-400 border border-gray-600 focus:outline-none focus:ring-2 focus:ring-indigo-500"
        value={search}
        onChange={(e) => setSearch(e.target.value)}
      />

      <div className="bg-gray-800 rounded-lg shadow p-4 overflow-x-auto">
        {loading ? (
          <p>Carregando...</p>
        ) : error ? (
          <p className="text-red-500">{error}</p>
        ) : (
          <table className="min-w-full text-left text-gray-200">
            <thead>
              <tr className="border-b border-gray-600">
                <th className="px-4 py-2">Nome</th>
                <th className="px-4 py-2 text-right">Ações</th>
              </tr>
            </thead>
            <tbody>
              {filteredRoles.map((role) => (
                <tr key={role.id} className="border-b border-gray-700">
                  <td className="px-4 py-2">{role.name}</td>
                  <td className="px-4 py-2 text-right space-x-2 flex justify-end">
                    <Link
                      to={`/admin/roles/${role.id}`}
                      className="bg-green-600 hover:bg-green-700 px-3 py-1 rounded-md text-sm"
                    >
                      {/* Editar */}
                      <PencilIcon className="w-5 h-5 cursor-pointer" />
                    </Link>
                    <button
                      onClick={() => handleDelete(role.id)}
                      className="bg-red-600 hover:bg-red-700 px-3 py-1 rounded-md text-sm"
                    >
                      {/* Excluir */}
                      <TrashIcon className="w-5 h-5 cursor-pointer" />
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>


    </div>
  );
}