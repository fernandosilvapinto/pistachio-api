import React, { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import {  PlusIcon, PencilIcon, TrashIcon } from "@heroicons/react/24/solid";

type Role = {
  id: number;
  name: string;
};

type User = {
  id: number;
  name: string;
  email: string;
  createdAt: string;
  roleId: number;
  role?: Role;
};

export default function UsersList() {
  const [users, setUsers] = useState<User[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [search, setSearch] = useState("");

  const fetchUsers = async () => {
    try {
      const token = localStorage.getItem("token");
      setLoading(true);
      const res = await fetch("/api/users", {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });

      if (!res.ok) throw new Error("Erro ao buscar usuários");

      const data: User[] = await res.json();
      setUsers(data);
    } catch (err) {
      setError("Erro ao carregar usuários.");
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id: number) => {
    if (!confirm("Tem certeza que deseja excluir este usuário?")) return;

    try {
      const token = localStorage.getItem("token");
      const res = await fetch(`/api/users/${id}`, {
        method: "DELETE",
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });

      if (!res.ok) throw new Error("Erro ao excluir usuário");

      setUsers((prev) => prev.filter((u) => u.id !== id));
    } catch (err) {
      alert("Erro ao excluir usuário.");
    }
  };

  useEffect(() => {
    fetchUsers();
  }, []);

  const filteredUsers = users.filter((u) =>
    u.name.toLowerCase().includes(search.toLowerCase())
  );

  return (
    <div className="p-6">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold text-gray-100">Usuários</h1>
        <Link
          to="/admin/users/new"
          className="bg-indigo-600 hover:bg-indigo-700 font-semibold px-4 py-2 rounded-md shadow"
        >
           <PlusIcon className="w-5 h-5 cursor-pointer" />
        </Link>
      </div>

        <input
          type="text"
          placeholder="Buscar utilizador..."
          className="w-full mb-4 px-4 py-2 rounded-md bg-gray-700 text-gray-200 placeholder-gray-400 border border-gray-600 focus:outline-none focus:ring-2 focus:ring-indigo-500"
          value={search}
          onChange={(e) => setSearch(e.target.value)}
        />

      <div className="bg-gray-800 rounded-lg shadow p-4 overflow-x-auto">
        {loading ? (
          <p className="text-gray-300">Carregando...</p>
        ) : error ? (
          <p className="text-red-500">{error}</p>
        ) : (
          <table className="min-w-full text-left text-gray-200">
            <thead>
              <tr className="border-b border-gray-700">
                <th className="px-4 py-2">ID</th>
                <th className="px-4 py-2">Nome</th>
                <th className="px-4 py-2">E-mail</th>
                <th className="px-4 py-2">Tipo</th>
                <th className="px-4 py-2 text-right">Ações</th>
              </tr>
            </thead>
            <tbody>
              {filteredUsers.map((u) => (
                <tr
                  key={u.id}
                  className="border-b border-gray-700 hover:bg-gray-700"
                >
                  <td className="px-4 py-2">{u.id}</td>
                  <td className="px-4 py-2">{u.name}</td>
                  <td className="px-4 py-2">{u.email}</td>
                  <td className="px-4 py-2">{u.role?.name || "—"}</td>

                  <td className="px-4 py-2 text-right space-x-2 flex justify-end">
                    <Link
                      to={`/admin/users/${u.id}`}
                      className="bg-green-600 hover:bg-green-700 text-white px-3 py-1 rounded-md text-sm"
                    >
                      {/* Editar */}
                       <PencilIcon className="w-5 h-5 cursor-pointer" />
                    </Link>
                    <button
                      onClick={() => handleDelete(u.id)}
                      className="bg-red-600 hover:bg-red-700 text-white px-3 py-1 rounded-md text-sm"
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
