import { useState, useEffect, useMemo } from "react";

export function useFetch<T>(url: string, options?: RequestInit) {
  const [data, setData] = useState<T | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // Memoiza options para não disparar o efeito toda vez que a referência mudar
  const memoOptions = useMemo(() => options, [JSON.stringify(options)]);

  useEffect(() => {
    const fetchData = async () => {
      setLoading(true);
      try {
        const token = localStorage.getItem("token");
        const res = await fetch(url, {
          ...memoOptions,
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
            ...(memoOptions?.headers || {}),
          },
        });

        if (!res.ok) throw new Error("Erro ao buscar dados");

        const json = await res.json();
        setData(json);
      } catch (err) {
        setError((err as Error).message || "Erro ao carregar dados");
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [url, memoOptions]);

  return { data, setData, loading, error, setError };
}
