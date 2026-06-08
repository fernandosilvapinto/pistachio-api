import React from "react";

const AdminDashboardPage: React.FC = () => {
  return (
    <div className="p-6">
      <h1 className="text-3xl font-bold text-purple-400 mb-4">
        Painel Administrativo
      </h1>
      <p className="text-gray-300">
        Bem-vindo ao painel de administração do Pistachio.  
        Use o menu lateral para gerenciar usuários, serviços, agendamentos e pagamentos.
      </p>
    </div>
  );
};

export default AdminDashboardPage;
