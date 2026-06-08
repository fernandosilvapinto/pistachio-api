import { useParams } from "react-router-dom";

export default function ServiceDetailsPage() {
  const { id } = useParams();

  return (
    <section className="max-w-3xl mx-auto p-6 text-gray-100">
      <h1 className="text-3xl font-bold mb-4">Detalhes do Serviço #{id}</h1>
      <p className="mb-6">
        Aqui você verá a descrição detalhada do serviço selecionado.
      </p>
      <button className="bg-blue-600 hover:bg-blue-700 px-6 py-3 rounded font-semibold">
        Agendar este serviço
      </button>
    </section>
  );
}
