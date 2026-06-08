import { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import {  PlusIcon, PencilIcon, TrashIcon } from "@heroicons/react/24/solid";
//import { useFetch } from "../../../hooks/useFetch";
//import { useDelete } from "../../../hooks/useDelete";

interface Service {
  id: number;
  name: string;
  description: string;
  price: number;
}

export default function ServicesList() {
  const [services, setServices] = useState<Service[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [search, setSearch] = useState("");
  
  // const { data: services, setData: setServices, loading, error} = useFetch<Service[]>("/api/services");

   const token = localStorage.getItem("token"); // pega token do localStorage

//    const { handleDelete, loading: deleting, error: deleteError } = useDelete<Service>(
//   "/api/services",
//   (id) => setServices((prev) => prev?.filter((s) => s.id !== id) || [])
// );

  const fetchServices = async () => {
    try {
      setLoading(true);
      const res = await fetch("/api/services", {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });
      if (!res.ok) throw new Error("Erro ao buscar serviços");
      const data: Service[] = await res.json();
      setServices(data);
    } catch (err) {
      setError("Erro ao carregar serviços.");
    } finally {
      setLoading(false);
    }
  };
  

  const handleDelete = async (id: number) => {
    if (!confirm("Tem certeza que deseja excluir este serviço?")) return;
    try {
      const res = await fetch(`/api/services/${id}`, {
        method: "DELETE",
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });
      if (!res.ok) throw new Error("Erro ao deletar serviço");
      setServices(services.filter((s) => s.id !== id));
    } catch {
      setError("Erro ao excluir serviço.");
    }
  };

  useEffect(() => {
    fetchServices();
  }, []);
  

  const filteredServices = services.filter((s) =>
    s.name.toLowerCase().includes(search.toLowerCase())
  );

  return (
    // <div className="min-h-screen p-6 bg-gray-900">
    <div className="p-6">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold  text-gray-100">Serviços</h1>
        <Link
          to="/admin/services/new"
          className="bg-indigo-600 hover:bg-indigo-700 px-4 py-2 rounded-md font-semibold transition"
          
        >
          <PlusIcon className="w-5 h-5 cursor-pointer" />
          {/* Novo Serviço */}
        </Link>
      </div>

      <input
        type="text"
        placeholder="Buscar serviço..."
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
                <th className="px-4 py-2">Descrição</th>
                <th className="px-4 py-2">Preço</th>
                <th className="px-4 py-2 text-right">Ações</th>
              </tr>
            </thead>
            <tbody>
              {filteredServices.map((service) => (
                <tr key={service.id} className="border-b border-gray-700">
                  <td className="px-4 py-2">{service.name}</td>
                  <td className="px-4 py-2">{service.description}</td>
                  <td className="px-4 py-2">R$ {service.price.toFixed(2)}</td>
                  <td className="px-4 py-2 text-right space-x-2 flex justify-end">
                    <Link
                      to={`/admin/services/${service.id}`}
                      className="bg-green-600 hover:bg-green-700 px-3 py-1 rounded-md text-sm"
                    >
                      {/* Editar */}
                      <PencilIcon className="w-5 h-5 cursor-pointer" />
                    </Link>
                    <button
                      onClick={() => handleDelete(service.id)}
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
