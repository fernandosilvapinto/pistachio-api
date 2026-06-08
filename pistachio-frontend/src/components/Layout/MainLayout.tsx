import React from 'react';
import type { ReactNode } from 'react';
import { Outlet } from 'react-router-dom';
import Header from './Header';
import Footer from './Footer';

const MainLayout: React.FC = () => {
  return (
    <div className="flex flex-col min-h-screen bg-gray-900 text-gray-100">
      <Header />
      <main className="flex-grow container mx-auto px-6 py-8">
        <Outlet /> {/* Renderiza a página filha */}
      </main>
      <Footer />
    </div>
  );
};

export default MainLayout;
