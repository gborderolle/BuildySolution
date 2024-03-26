// useBumpEffect.js
import { useState } from "react";

const useBumpEffect = () => {
  const [isBumped, setIsBumped] = useState(false);

  const triggerBump = () => {
    // Activa el efecto bump
    setIsBumped(true);
    // Desactiva el efecto bump después de 300ms
    const timer = setTimeout(() => {
      setIsBumped(false);
    }, 300); // Asegúrate de que este tiempo coincida con la duración de tu animación

    // Limpia el timeout si el componente se desmonta
    // o si el efecto se vuelve a activar antes de que el timer termine
    return () => {
      clearTimeout(timer);
    };
  };

  return [isBumped, triggerBump];
};

export default useBumpEffect;
