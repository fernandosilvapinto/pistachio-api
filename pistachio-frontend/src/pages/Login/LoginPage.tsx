import React from "react";
import { useState } from "react";


const Login: React.FC = () => {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");

    try {
      const response = await fetch("http://localhost:5150/api/auth/login", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({ email, password }),
      });

      if (!response.ok) {
        throw new Error("Credenciais inválidas");
      }
      
      const data = await response.json();
      
      if (data.role !== "User") {
        throw new Error("Credenciais inválidas");
      }
      
      // Armazenar token e role no localStorage
      localStorage.setItem("token", data.token);
      localStorage.setItem("role", data.role);

      // Redireciona para a dashboard
      window.location.href = "/";
    } catch (err) {
      setError((err as Error).message);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-900 px-4">
      <div className="bg-gray-800 rounded-lg shadow-2xl max-w-md w-full p-10">
        <h2 className="text-4xl font-bold text-gray-100 mb-8 text-center">
          Login
        </h2>

        {error && <p className="text-red-500 text-center mb-4">{error}</p>}

        <form onSubmit={handleSubmit} className="space-y-6">
          {/* Email */}
          <div>
            <label htmlFor="email" className="sr-only">
              E-mail
            </label>
            <input
              id="email"
              type="email"
              placeholder="E-mail"
              className="w-full px-5 py-3 rounded-md bg-gray-700 text-gray-200 placeholder-gray-400 border border-gray-600 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 transition"
              required
              value={email}
              onChange={(e) => setEmail(e.target.value)}
            />
          </div>

          {/* Senha */}
          <div>
            <label htmlFor="password" className="sr-only">
              Senha
            </label>
            <input
              id="password"
              type="password"
              placeholder="Senha"
              className="w-full px-5 py-3 rounded-md bg-gray-700 text-gray-200 placeholder-gray-400 border border-gray-600 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 transition"
              required
              value={password}
              onChange={(e) => setPassword(e.target.value)}
            />
          </div>

          {/* Botão Login */}
          <button
            type="submit"
            className="w-full bg-indigo-600 hover:bg-indigo-700 text-white font-semibold py-3 rounded-md shadow-md transition transform hover:-translate-y-0.5 focus:outline-none focus:ring-4 focus:ring-indigo-400"
          >
            Entrar
          </button>
        </form>

        {/* Link Esqueci minha senha */}
        <div className="mt-6 text-center">
          <a
            href="#"
            className="text-sm text-indigo-400 hover:text-indigo-600 hover:underline"
          >
            Esqueci minha senha
          </a>
        </div>
        <div className="mt-2 text-center">
          <a
            href="/"
            className="text-sm text-indigo-400 hover:text-indigo-600 hover:underline"
          >
            Voltar
          </a>
        </div>
      </div>
    </div>
  );
};

export default Login;

