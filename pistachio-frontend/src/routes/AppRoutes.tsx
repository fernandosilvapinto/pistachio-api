// src/routes/AppRoutes.tsx
import React from "react";
import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import MainLayout from "../components/Layout/MainLayout";
import LoginPage from "../pages/Login/LoginPage";
import DashboardPage from "../pages/Dashboard/DashboardPage";
import LandingPage from "../pages/Landing/LandingPage";
import ServiceDetailsPage from "../pages/Landing/ServiceDetailsPage";
import SchedulingPage from "../pages/Scheduling/SchedulingPage";
import MySchedulingsPage from "../pages/Scheduling/MySchedulingsPage";
import PaymentPage from "../pages/Payment/PaymentPage";

import AdminLoginPage from "../pages/Admin/Login/AdminLoginPage";
import AdminLayout from "../components/Layout/AdminLayout";
import AdminDashboardPage from "../pages/Admin/Dashboard/AdminDashboardPage";
import RolesPage from "../pages/Admin/Roles/RolesList";
import RolesForm from "../pages/Admin/Roles/RolesForm";
import ServicesPage from "../pages/Admin/Services/ServicesList";
import ServicesForm from "../pages/Admin/Services/ServicesForm";
import UsersPage from "../pages/Admin/Users/UsersList";
import UsersForm from "../pages/Admin/Users/UsersForm";
import SchedulingsPage from "../pages/Admin/Schedulings/SchedulingsList";
import SchedulingsForm from "../pages/Admin/Schedulings/SchedulingsForm";
import PaymentsPage from "../pages/Admin/Payments/PaymentsList";

import PrivateRouteClient from "./PrivateRouteClient";
import PrivateRouteAdmin from "./PrivateRouteAdmin";

export default function AppRoutes() {
  return (
    <BrowserRouter>
      <Routes>
        {/* Rotas públicas com layout */}
        <Route element={<MainLayout />}>
          <Route path="/" element={<LandingPage />} />
          <Route path="/service/:id" element={<ServiceDetailsPage />} />
          {/* Podemos deixar as páginas privadas aqui, sem proteção por enquanto */}
          <Route
            path="/scheduling"
            element={
              <PrivateRouteClient>
                <SchedulingPage />
              </PrivateRouteClient>
            }
          />
          <Route
            path="/my-schedulings"
            element={
              <PrivateRouteClient>
                <MySchedulingsPage />
              </PrivateRouteClient>
            }
          />
          <Route
            path="/payment"
            element={
              <PrivateRouteClient>
                <PaymentPage />
              </PrivateRouteClient>
            }
          />
          <Route
            path="/dashboard"
            element={
              <PrivateRouteClient>
                <DashboardPage />
              </PrivateRouteClient>
            }
          />
        </Route>

        {/* Área admin */}
        <Route path="/admin" element={<AdminLayout />}>
          <Route
            index
            element={
              <PrivateRouteAdmin>
                <AdminDashboardPage />
              </PrivateRouteAdmin>
            }
          />

          <Route
            path="payments"
            element={
              <PrivateRouteAdmin>
                <PaymentsPage />
              </PrivateRouteAdmin>
            }
          />

          <Route
            path="services"
            element={
              <PrivateRouteAdmin>
                <ServicesPage />
              </PrivateRouteAdmin>
            }
          />
          <Route
            path="services/new"
            element={
              <PrivateRouteAdmin>
                <ServicesForm />
              </PrivateRouteAdmin>
            }
          />
          <Route
            path="services/:id"
            element={
              <PrivateRouteAdmin>
                <ServicesForm />
              </PrivateRouteAdmin>
            }
          />
          <Route
            path="users"
            element={
              <PrivateRouteAdmin>
                <UsersPage />
              </PrivateRouteAdmin>
            }
          />
          <Route
            path="users/new"
            element={
              <PrivateRouteAdmin>
                <UsersForm />
              </PrivateRouteAdmin>
            }
          />
          <Route
            path="users/:id"
            element={
              <PrivateRouteAdmin>
                <UsersForm />
              </PrivateRouteAdmin>
            }
          />
          <Route
            path="roles"
            element={
              <PrivateRouteAdmin>
                <RolesPage />
              </PrivateRouteAdmin>
            }
          />
          <Route
            path="roles/new"
            element={
              <PrivateRouteAdmin>
                <RolesForm />
              </PrivateRouteAdmin>
            }
          />
          <Route
            path="roles/:id"
            element={
              <PrivateRouteAdmin>
                <RolesForm />
              </PrivateRouteAdmin>
            }
          />

          <Route
            path="schedulings"
            element={
              <PrivateRouteAdmin>
                <SchedulingsPage />
              </PrivateRouteAdmin>
            }
          />
          <Route
            path="schedulings/new"
            element={
              <PrivateRouteAdmin>
                <SchedulingsForm />
              </PrivateRouteAdmin>
            }
          />
          <Route
            path="schedulings/:id"
            element={
              <PrivateRouteAdmin>
                <SchedulingsForm />
              </PrivateRouteAdmin>
            }
          />


        </Route>

        {/* Login fora do layout */}
        <Route path="/login" element={<LoginPage />} />
        <Route path="/admin/login" element={<AdminLoginPage />} />
      </Routes>
    </BrowserRouter>
  );
}
