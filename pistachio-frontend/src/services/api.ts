import axios from "axios";

const api = axios.create({
  baseURL: "http://localhost:5150", // ajuste conforme seu backend
});

export default api;
