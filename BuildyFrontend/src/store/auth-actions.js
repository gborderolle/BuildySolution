import axios from "axios";
import { authActions } from "./auth-slice";
import showToastMessage from "../components/messages/ShowSuccess";

import { urlAccount } from "../endpoints"; // Asegúrate de ajustar la ruta relativa según sea necesario

export const loginHandler =
  (username, password, navigate, setErrorMessage) => async (dispatch) => {
    try {
      const response = await axios.post(
        `${urlAccount}/login`,
        { username, password },
        { headers: { "x-version": "1" } }
      );

      // Asegúrate de que la respuesta contenga el resultado esperado
      if (response.data && response.data.result && response.data.result.token) {
        const { token, userRoles } = response.data.result;

        // Manejo exitoso del login
        await showToastMessage({
          title: "Login correcto",
          icon: "success",
          callback: () => {
            setTimeout(() => {
              dispatch(
                authActions.login({
                  username,
                  isMobile: isMobileDevice(),
                  authToken: token.token,
                  userRole: userRoles[0],
                })
              );
              navigate("/estates");
            }, 500);
          },
        });
      } else {
        // Manejar el caso donde no se recibe un token válido
        setErrorMessage("Login incorrecto. No se recibió un token válido.");
        await showToastMessage({
          title: "Login incorrecto",
          icon: "error",
        });
      }
    } catch (error) {
      console.error("Error al autenticar:", error);
      showToastMessage({
        title: "Error al autenticar",
        icon: "error",
      });
      setErrorMessage(
        "Error al autenticar. Por favor, revisa tu conexión a Internet."
      );
    }
  };

const isMobileDevice = () => {
  return (
    typeof window.orientation !== "undefined" ||
    navigator.userAgent.indexOf("IEMobile") !== -1
  );
};
