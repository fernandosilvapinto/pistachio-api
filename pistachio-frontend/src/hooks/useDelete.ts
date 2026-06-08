import { useState } from "react";

export function useDelete(urlBase: string, onSuccess?: (id: number) => void) {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleDelete = async (id: number) => {
    if (!confirm("Tem certeza que deseja excluir este item?")) return;

    setLoading(true);
    try {
         const token = localStorage.getItem("token");
      const res = await fetch(`${urlBase}/${id}`, {
        method: "DELETE",
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });

      if (!res.ok) throw new Error("Erro ao deletar item");

      if (onSuccess) onSuccess(id); // Atualiza o state do componente
    } catch (err) {
      setError((err as Error).message || "Erro ao deletar item");
    } finally {
      setLoading(false);
    }
  };

  return { handleDelete, loading, error, setError };
}
