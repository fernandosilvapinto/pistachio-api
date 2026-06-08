import { useState } from "react";
import { Link } from "react-router-dom";
import { useFetch } from "../../../hooks/useFetch";
import { useDelete } from "../../../hooks/useDelete";
import {  PlusIcon, PencilIcon, TrashIcon } from "@heroicons/react/24/solid";

type Scheduling = {
  id: number;
  scheduledDate: string;
  serviceName: string;
  userId: number;
  user: User;
  serviceId: number;
  service?: Service;
};

export default function SchedulingsList() {
  const [search, setSearch] = useState("");
  const {
    data: schedulings,
    setData: setSchedulings,
    loading,
    error,
  } = useFetch<Scheduling[]>("/api/schedulings");

  const { handleDelete, loading: deleting } = useDelete<Scheduling>(
    "/api/schedulings",
    setSchedulings
  );

  const filteredSchedulings = (schedulings || []).filter(
    (s) =>
      s.serviceName.toLowerCase().includes(search.toLowerCase()) ||
      s.user.name.toLowerCase().includes(search.toLowerCase())
  );

  return (
    <div className="p-6">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold text-gray-100">Agendamentos</h1>
        <Link
          to="/admin/schedulings/new"
          className="bg-indigo-600 hover:bg-indigo-700 text-white px-4 py-2 rounded-md"
        >
          <PlusIcon className="w-5 h-5 cursor-pointer" />
        </Link>
      </div>

      {error && (
        <p className="text-red-500 bg-red-900/20 p-2 rounded-md mb-4">
          {error}
        </p>
      )}

      <input
        type="text"
        placeholder="Buscar por usuário ou serviço..."
        value={search}
        onChange={(e) => setSearch(e.target.value)}
        className="mb-4 w-full px-4 py-2 rounded-md bg-gray-800 text-gray-200 border border-gray-700 focus:ring-2 focus:ring-indigo-500"
      />

      {loading ? (
        <p className="text-gray-400">Carregando...</p>
      ) : (
        <div className="overflow-x-auto bg-gray-800 rounded-lg shadow">
          <table className="min-w-full text-sm text-gray-300">
            {/* <thead className="bg-gray-700 text-gray-200"> */}
            <thead>
              <tr>
                <th className="px-4 py-2 text-left">ID</th>
                <th className="px-4 py-2 text-left">Data</th>
                <th className="px-4 py-2 text-left">Serviço</th>
                <th className="px-4 py-2 text-left">Usuário</th>
                <th className="px-4 py-2 text-right">Ações</th>
              </tr>
            </thead>
            <tbody>
              {filteredSchedulings.map((s) => (
                <tr
                  key={s.id}
                  className="border-t border-gray-700 hover:bg-gray-700/50"
                >
                  <td className="px-4 py-2">{s.id}</td>
                  <td className="px-4 py-2">
                    {new Date(s.scheduledDate).toLocaleString("pt-BR")}
                  </td>
                  <td className="px-4 py-2">{s.serviceName}</td>
                  <td className="px-4 py-2">{s.user?.name}</td>
                  <td className="px-4 py-2 text-right space-x-2 flex justify-end" >
                    <Link
                      to={`/admin/schedulings/${s.id}`}
                      className="bg-green-600 hover:bg-green-700 px-3 py-1 rounded-md text-sm"
                    >
                       <PencilIcon className="w-5 h-5 cursor-pointer" />
                    </Link>
                    <button
                      onClick={() => handleDelete(s.id)}
                      disabled={deleting}
                      className="bg-red-600 hover:bg-red-700 px-3 py-1 rounded-md text-sm disabled:opacity-50"
                    >
                      {deleting ? "Excluindo..." : <TrashIcon className="w-5 h-5 cursor-pointer" />}
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>

          {filteredSchedulings.length === 0 && (
            <p className="text-center text-gray-400 py-4">
              Nenhum agendamento encontrado.
            </p>
          )}
        </div>
      )}
    </div>
  );
}