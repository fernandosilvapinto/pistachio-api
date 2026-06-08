import { useState, useEffect } from "react";
import { useNavigate, useParams } from "react-router-dom";

interface Service {
  id?: number;
  name: string;
  description: string;
  price: number;
  isActive: boolean;
  isFeatured: boolean;
}

export default function ServiceForm() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();

  const [service, setService] = useState<Service>({
    name: "",
    description: "",
    price: 0,
    isActive: true,
    isFeatured: false,
  });

  const token = localStorage.getItem("token"); // pega token do localStorage

  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (id) {
      fetch(`/api/services/${id}`, {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      })
        .then((res) => res.json())
        .then((data) => setService(data))
        .catch(() => setError("Erro ao carregar serviço."));
    }
  }, [id]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      const method = id ? "PUT" : "POST";
      const url = id ? `/api/services/${id}` : "/api/services";
      await fetch(url, {
        method,
        headers: { 
          "Content-Type": "application/json", 
          Authorization: `Bearer ${token}` 
        },
        body: JSON.stringify(service),
      });
      navigate("/admin/services");
    } catch {
      setError("Erro ao salvar serviço.");
    }
  };

  return (
    <div className="flex-1 p-8 bg-gray-900 overflow-y-auto">
      <div className="max-w-4xl mx-auto">
        <h2 className="text-3xl font-bold text-gray-100 mb-10 border-b border-gray-700 pb-4">
          {id ? "Editar Serviço" : "Novo Serviço"}
        </h2>

        {error && (
          <div className="bg-red-600/20 text-red-400 px-4 py-2 rounded-md mb-6">
            {error}
          </div>
        )}

        <form
          onSubmit={handleSubmit}
          className="grid grid-cols-1 md:grid-cols-2 gap-8"
        >
          {/* Nome */}
          <div className="col-span-2">
            <label
              htmlFor="name"
              className="block text-sm font-medium text-gray-300 mb-2"
            >
              Nome
            </label>
            <input
              id="name"
              type="text"
              placeholder="Nome do serviço"
              className="w-full px-5 py-3 rounded-md bg-gray-800 text-gray-200 placeholder-gray-500 border border-gray-700 focus:outline-none focus:ring-2 focus:ring-indigo-500 transition"
              required
              value={service.name}
              onChange={(e) => setService({ ...service, name: e.target.value })}
            />
          </div>

          {/* Descrição */}
          <div className="col-span-2">
            <label
              htmlFor="description"
              className="block text-sm font-medium text-gray-300 mb-2"
            >
              Descrição
            </label>
            <textarea
              id="description"
              rows={5}
              placeholder="Descrição detalhada do serviço"
              className="w-full px-5 py-3 rounded-md bg-gray-800 text-gray-200 placeholder-gray-500 border border-gray-700 focus:outline-none focus:ring-2 focus:ring-indigo-500 transition"
              required
              value={service.description}
              onChange={(e) =>
                setService({ ...service, description: e.target.value })
              }
            />
          </div>

          {/* Preço */}
          <div>
            <label
              htmlFor="price"
              className="block text-sm font-medium text-gray-300 mb-2"
            >
              Preço
            </label>
            <input
              id="price"
              type="number"
              placeholder="0.00"
              className="w-full px-5 py-3 rounded-md bg-gray-800 text-gray-200 placeholder-gray-500 border border-gray-700 focus:outline-none focus:ring-2 focus:ring-indigo-500 transition"
              required
              value={service.price}
              onChange={(e) =>
                setService({ ...service, price: parseFloat(e.target.value) })
              }
            />
          </div>

          {/* Checkboxes */}
          <div className="flex flex-col space-y-4">
            <label className="flex items-center space-x-3 text-gray-300">
              <input
                type="checkbox"
                checked={service.isActive || false}
                onChange={(e) =>
                  setService({ ...service, isActive: e.target.checked })
                }
                className="h-5 w-5 accent-indigo-500"
              />
              <span>Ativo</span>
            </label>

            <label className="flex items-center space-x-3 text-gray-300">
              <input
                type="checkbox"
                checked={service.isFeatured || false}
                onChange={(e) =>
                  setService({ ...service, isFeatured: e.target.checked })
                }
                className="h-5 w-5 accent-indigo-500"
              />
              <span>Destaque</span>
            </label>
          </div>

          {/* Botão ocupa largura total */}
          <div className="col-span-2 flex justify-end space-x-4">
            <button
              type="submit"
              className="px-6 py-3 bg-indigo-600 hover:bg-indigo-700 text-white font-semibold rounded-lg shadow-md transition transform hover:-translate-y-0.5 focus:outline-none focus:ring-4 focus:ring-indigo-400"
            >
              Salvar
            </button>
            <button
              type="button"
              onClick={() => navigate("/admin/services")} // ou outra rota de retorno
              className="px-6 py-3 rounded-lg border border-gray-600 text-gray-300 hover:bg-gray-700 hover:text-white transition"
            >
              Cancelar
            </button>

           
          </div>
        </form>
      </div>
    </div>
  );
}
