import React from "react";
import { Navigate } from "react-router-dom";

interface PrivateRouteAdminProps {
  children: React.ReactNode;
}

const PrivateRouteAdmin: React.FC<PrivateRouteAdminProps> = ({ children }) => {
  const token = localStorage.getItem("token");
  const role = localStorage.getItem("role"); // futuramente vindo do backend

  if (!token || role !== "Admin") {
    return <Navigate to="/admin/login" replace />;
  }

  return <>{children}</>;
};

export default PrivateRouteAdmin;
