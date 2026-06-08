import React from "react";
import { Navigate } from "react-router-dom";

interface PrivateRouteClientProps {
  children: React.ReactNode;
}

const PrivateRouteClient: React.FC<PrivateRouteClientProps> = ({ children }) => {
  const token = localStorage.getItem("token");
  const role = localStorage.getItem("role"); // futuramente vindo do backend

  if (!token || role !== "User") {
    return <Navigate to="/login" replace />;
  }

  return <>{children}</>;
};

export default PrivateRouteClient;
