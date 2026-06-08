import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";

type Role = {
  id: number;
  name: string;
};

type User = {
  id?: number;
  name: string;
  email: string;
  password?: string;
  roleId: number;
  role?: Role;
};

export default function UsersForm() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();

  const [user, setUser] = useState<User>({
    name: "",
    email: "",
    password: "",
    roleId: 0,
  });
  const [roles, setRoles] = useState<Role[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  const token = localStorage.getItem("token");

  // Buscar roles disponíveis
  useEffect(() => {
    const fetchRoles = async () => {
      try {
        const res = await fetch("/api/roles", {
          headers: { Authorization: `Bearer ${token}` },
        });
        if (!res.ok) throw new Error("Erro ao carregar roles");
        const data: Role[] = await res.json();
        setRoles(data);
      } catch {
        setError("Erro ao carregar roles");
      }
    };
    fetchRoles();
  }, [token]);

  // Se for edição, buscar o utilizador
  useEffect(() => {
    if (!id) return;
    const fetchUser = async () => {
      try {
        const res = await fetch(`/api/users/${id}`, {
          headers: { Authorization: `Bearer ${token}` },
        });
        if (!res.ok) throw new Error("Erro ao carregar utilizador");
        const data: User = await res.json();
        setUser({ ...data, password: "" }); // não traz senha
      } catch {
        setError("Erro ao carregar utilizador");
      }
    };
    fetchUser();
  }, [id, token]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError(null);

    try {
      const method = id ? "PUT" : "POST";
      const url = id ? `/api/users/${id}` : "/api/users";

      const res = await fetch(url, {
        method,
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify(user),
      });

      if (!res.ok) throw new Error("Erro ao salvar utilizador");

      navigate("/admin/users");
    } catch {
      setError("Erro ao salvar utilizador");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="flex-1 p-8 bg-gray-900 overflow-y-auto">
      <div className="max-w-4xl mx-auto">
        <h2 className="text-3xl font-bold text-gray-100 mb-10 border-b border-gray-700 pb-4">
          {id ? "Editar Utilizador" : "Novo Utilizador"}
        </h2>

        {error && (
          <p className="text-red-500 bg-red-900/30 p-2 rounded mb-4">{error}</p>
        )}

        <form onSubmit={handleSubmit} className="grid grid-cols-2 gap-6">
          <div className="col-span-2">
            <label className="block text-sm font-medium text-gray-300 mb-2">Nome</label>
            <input
              type="text"
              placeholder="Nome"
              value={user.name}
              onChange={(e) => setUser({ ...user, name: e.target.value })}
              className="w-full px-4 py-2 rounded-md bg-gray-700 text-gray-200 border border-gray-600 focus:ring-2 focus:ring-indigo-500"
              required
            />
          </div>

          <div className="col-span-2">
            <label className="block text-sm font-medium text-gray-300 mb-2">E-mail</label>
            <input
              type="email"
              placeholder="Email"
              value={user.email}
              onChange={(e) => setUser({ ...user, email: e.target.value })}
              className="w-full px-4 py-2 rounded-md bg-gray-700 text-gray-200 border border-gray-600 focus:ring-2 focus:ring-indigo-500"
              required
            />
          </div>

          {!id && (
            <div className="col-span-2">
              <label className="block text-sm font-medium text-gray-300 mb-2">Senha</label>
              <input
                type="password"
                placeholder="Senha"
                value={user.password}
                onChange={(e) => setUser({ ...user, password: e.target.value })}
                className="w-full px-4 py-2 rounded-md bg-gray-700 text-gray-200 border border-gray-600 focus:ring-2 focus:ring-indigo-500"
                required
              />
            </div>
          )}

          <div className="col-span-2">
            <label className="block text-sm font-medium text-gray-300 mb-2">Role</label>
            <select
              value={user.roleId}
              onChange={(e) =>
                setUser({ ...user, roleId: parseInt(e.target.value) })
              }
              className="w-full px-4 py-2 rounded-md bg-gray-700 text-gray-200 border border-gray-600 focus:ring-2 focus:ring-indigo-500"
              required
            >
              <option value="" >Selecione uma role</option>
              {roles.map((role) => (
                <option key={role.id} value={role.id}>
                  {role.name}
                </option>
              ))}
            </select>
          </div>

          <div className="col-span-2 flex justify-end space-x-4">
           
            <button
              type="submit"
              disabled={loading}
              className="px-6 py-2 rounded-md bg-indigo-600 hover:bg-indigo-700 text-white font-semibold shadow-md"
            >
              {loading ? "Salvando..." : "Salvar"}
            </button>
             <button
              type="button"
              onClick={() => navigate("/admin/users")}
              className="px-6 py-2 rounded-md bg-gray-600 hover:bg-gray-700 text-gray-200"
            >
              Cancelar
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
