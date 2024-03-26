import { useState } from "react";
import axios from "axios";

const useAPI = () => {
  const [state, setState] = useState({
    isLoading: false,
    isSuccess: false,
    error: null,
    data: null,
  });

  const uploadData = async (dataToUpload, apiUrl, editMode, id) => {
    setState({ isLoading: true, isSuccess: false, error: null });

    // Determinar si dataToUpload es FormData o JSON
    const isFormData = dataToUpload instanceof FormData;

    // Configurar encabezados HTTP
    const authToken = localStorage.getItem("authToken");
    const headers = {
      Authorization: `Bearer ${authToken}`,
      "x-version": "1",
    };

    // Si no es FormData, establecer el encabezado 'Content-Type' como 'application/json'
    if (!isFormData) {
      headers["Content-Type"] = "application/json";
    }

    // Determinar el método HTTP y la URL basados en editMode
    const method = editMode ? "put" : "post";
    const url = editMode ? `${apiUrl}/${id}` : apiUrl;

    try {
      const response = await axios({
        method: method,
        url: url,
        data: dataToUpload,
        headers: headers,
      });

      setState({
        isLoading: false,
        isSuccess: true,
        error: null,
        data: response.data,
      });

      return response.data;
    } catch (error) {
      let errorMessage = "Ocurrió un error desconocido";

      if (error.response) {
        if (typeof error.response.data === "object") {
          errorMessage = JSON.stringify(error.response.data, null, 2);
        } else {
          errorMessage = error.response.data;
        }
      } else if (error.message) {
        errorMessage = error.message;
      }

      console.error("Error en uploadData:", errorMessage);
      setState({ isLoading: false, isSuccess: false, error: errorMessage });
    }
  };

  const removeData = async (apiUrl, id) => {
    setState({ isLoading: true, isSuccess: false, error: null });

    const authToken = localStorage.getItem("authToken");
    const headers = {
      Authorization: `Bearer ${authToken}`,
      "x-version": "1",
    };

    try {
      await axios.delete(`${apiUrl}/${id}`, { headers });
      setState({ isLoading: false, isSuccess: true, error: null });
    } catch (error) {
      let errorMessage = "Error al eliminar";
      if (error.response && error.response.data) {
        errorMessage = error.response.data;
      }
      setState({ isLoading: false, isSuccess: false, error: errorMessage });
    }
  };

  return { ...state, uploadData, removeData };
};

export default useAPI;
