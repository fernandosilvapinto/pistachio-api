import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
// import type { User } from "@/types/User";
// import type { Service } from "@/types/Service";
// import type { Scheduling } from "@/types/Scheduling";

type Scheduling = {
  id: number;
  scheduledDate: string;
  userId: number;
  serviceId: number;
};

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

type Service = {
  id: number;
  name: string;
  description: string;
  price: number;
};


export default function SchedulingsForm() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const token = localStorage.getItem("token");

  const [scheduling, setScheduling] = useState<Partial<Scheduling>>({
    userId: 0,
    serviceId: 0,
    scheduledDate: "",
  });
  const [users, setUsers] = useState<User[]>([]);
  const [services, setServices] = useState<Service[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // Buscar usuários e serviços
  useEffect(() => {
    const fetchData = async () => {
      try {
        const [usersRes, servicesRes] = await Promise.all([
          fetch("/api/users", { headers: { Authorization: `Bearer ${token}` } }),
          fetch("/api/services", { headers: { Authorization: `Bearer ${token}` } }),
        ]);

        if (!usersRes.ok || !servicesRes.ok) throw new Error("Erro ao carregar dados");

        setUsers(await usersRes.json());
        setServices(await servicesRes.json());
      } catch {
        setError("Erro ao carregar usuários ou serviços");
      }
    };
    fetchData();
  }, [token]);

  // Buscar agendamento se for edição
  useEffect(() => {
    if (!id) return;
    const fetchScheduling = async () => {
      try {
        const res = await fetch(`/api/schedulings/${id}`, {
          headers: { Authorization: `Bearer ${token}` },
        });
        if (!res.ok) throw new Error("Erro ao carregar agendamento");
        const data: Scheduling = await res.json();
        setScheduling({
          userId: data.userId,
          serviceId: data.serviceId,
          scheduledDate: data.scheduledDate.slice(0, 16), // YYYY-MM-DDTHH:mm
        });
      } catch {
        setError("Erro ao carregar agendamento");
      }
    };
    fetchScheduling();
  }, [id, token]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError(null);

    try {
      const method = id ? "PUT" : "POST";
      const url = id ? `/api/schedulings/${id}` : "/api/schedulings";

      const res = await fetch(url, {
        method,
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify(scheduling),
      });

      if (!res.ok) throw new Error("Erro ao salvar agendamento");

      navigate("/admin/schedulings");
    } catch {
      setError("Erro ao salvar agendamento");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="p-8 w-full">
      <div className="bg-gray-800 rounded-lg shadow-2xl max-w-2xl mx-auto p-8">
        <h2 className="text-3xl font-bold text-gray-100 mb-6">
          {id ? "Editar Agendamento" : "Novo Agendamento"}
        </h2>

        {error && (
          <p className="text-red-500 bg-red-900/30 p-2 rounded mb-4">{error}</p>
        )}

        <form onSubmit={handleSubmit} className="grid grid-cols-2 gap-6">
          <div className="col-span-2">
            <label className="block text-gray-300 mb-1">Usuário</label>
            <select
              value={scheduling.userId}
              onChange={(e) =>
                setScheduling({ ...scheduling, userId: parseInt(e.target.value) })
              }
              className="w-full px-4 py-2 rounded-md bg-gray-700 text-gray-200 border border-gray-600 focus:ring-2 focus:ring-indigo-500"
              required
            >
              <option value={0}>Selecione um usuário</option>
             {users.map((u) => (
                <option key={u.id} value={u.id}>
                  {u.name}
                </option>
              ))}
            </select>
          </div>

          <div className="col-span-2">
            <label className="block text-gray-300 mb-1">Serviço</label>
            <select
              value={scheduling.serviceId}
              onChange={(e) =>
                setScheduling({ ...scheduling, serviceId: parseInt(e.target.value) })
              }
              className="w-full px-4 py-2 rounded-md bg-gray-700 text-gray-200 border border-gray-600 focus:ring-2 focus:ring-indigo-500"
              required
            >
              <option value={0}>Selecione um serviço</option>
              {services.map((s) => (
                <option key={s.id} value={s.id}>
                  {s.name}
                </option>
              ))}
            </select>
          </div>

          <div className="col-span-2">
            <label className="block text-gray-300 mb-1">Data e Hora</label>
            <input
              type="datetime-local"
              value={scheduling.scheduledDate}
              onChange={(e) =>
                setScheduling({ ...scheduling, scheduledDate: e.target.value })
              }
              className="w-full px-4 py-2 rounded-md bg-gray-700 text-gray-200 border border-gray-600 focus:ring-2 focus:ring-indigo-500"
              required
            />
          </div>

          <div className="col-span-2 flex justify-end space-x-4">
            <button
              type="button"
              onClick={() => navigate("/admin/schedulings")}
              className="px-6 py-2 rounded-md bg-gray-600 hover:bg-gray-700 text-gray-200"
            >
              Cancelar
            </button>
            <button
              type="submit"
              disabled={loading}
              className="px-6 py-2 rounded-md bg-indigo-600 hover:bg-indigo-700 text-white font-semibold shadow-md"
            >
              {loading ? "Salvando..." : "Salvar"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}