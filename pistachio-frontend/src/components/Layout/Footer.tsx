import React from "react";

const Footer: React.FC = () => {

  return (
    <footer className="bg-gray-900 text-gray-500 py-6 mt-12">
      <div className="container mx-auto px-6 text-center text-sm">
        &copy; {new Date().getFullYear()} PistachioApp. Todos os direitos
        reservados.
      </div>
    
    </footer>
  );
};

export default Footer;
