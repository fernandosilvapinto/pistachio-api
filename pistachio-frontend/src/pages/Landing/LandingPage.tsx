import React from 'react';

const LandingPage: React.FC = () => {
  const services = [
    {
      id: 1,
      title: "Consultoria Técnica",
      description: "Análise detalhada e soluções personalizadas para seu projeto.",
    },
    {
      id: 2,
      title: "Desenvolvimento Personalizado",
      description: "Criação de sistemas sob medida para atender suas necessidades.",
    },
    {
      id: 3,
      title: "Suporte e Manutenção",
      description: "Assistência contínua para manter seus sistemas atualizados e funcionais.",
    },
  ];

  return (
    <section className="max-w-5xl mx-auto p-6 text-gray-100">
      <h1 className="text-4xl font-bold mb-8">Bem-vindo ao Pistachio</h1>
      <p className="mb-12 max-w-3xl">
        Conheça os serviços oferecidos pelo nosso profissional, pensados para entregar o melhor para você.
      </p>

      <div className="grid gap-8 grid-cols-1 md:grid-cols-3">
        {services.map(({ id, title, description }) => (
          <div
            key={id}
            className="flex flex-col justify-between bg-gray-800 p-6 rounded-lg shadow-lg hover:shadow-xl transition-shadow duration-300"
          >
            <div>
              <h2 className="text-2xl font-semibold mb-4">{title}</h2>
              <p className="text-gray-300 mb-6">{description}</p>
            </div>
            <button
              className="mt-auto bg-blue-600 hover:bg-blue-700 text-white font-semibold py-3 rounded transition-colors duration-300"
              onClick={() => alert(`Agendar serviço: ${title}`)}
            >
              Agendar Serviço
            </button>
          </div>
        ))}
      </div>
    </section>
  );
};

export default LandingPage;
