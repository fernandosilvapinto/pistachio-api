 import { useState, useEffect } from "react";
import { useNavigate, useParams } from "react-router-dom";

interface Role {
    id?: number;
    name: string;
}

export default function roleForm() {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();

    const [role, setRole] = useState<Role>({
        name: ""
    });

    const token = localStorage.getItem("token"); // pega token do localStorage

    const [error, setError] = useState<string | null>(null);

     useEffect(() => {
    if (id) {
      fetch(`/api/roles/${id}`, {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      })
        .then((res) => res.json())
        .then((data) => setRole(data))
        .catch(() => setError("Erro ao carregar função."));
    }
  }, [id]);

    const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      const method = id ? "PUT" : "POST";
      const url = id ? `/api/roles/${id}` : "/api/roles";
      await fetch(url, {
        method,
        headers: { 
          "Content-Type": "application/json", 
          Authorization: `Bearer ${token}` 
        },
        body: JSON.stringify(role),
      });
      navigate("/admin/roles");
    } catch {
      setError("Erro ao salvar função.");
    }
  };

  return(

    <div className="flex-1 p-8 bg-gray-900 overflow-y-auto">
      <div className="max-w-4xl mx-auto">
        <h2 className="text-3xl font-bold text-gray-100 mb-10 border-b border-gray-700 pb-4">
          {id ? "Editar Função" : "Novo Função"}
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
              Função
            </label>
            <input
              id="name"
              type="text"
              placeholder="Nome da função"
              className="w-full px-5 py-3 rounded-md bg-gray-800 text-gray-200 placeholder-gray-500 border border-gray-700 focus:outline-none focus:ring-2 focus:ring-indigo-500 transition"
              required
              value={role.name}
              onChange={(e) => setRole({ ...role, name: e.target.value })}
            />
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
              onClick={() => navigate("/admin/roles")} // ou outra rota de retorno
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