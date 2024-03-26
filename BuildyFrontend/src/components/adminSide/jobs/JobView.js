import React from "react";
import { useNavigate, useLocation } from "react-router-dom";
import ImageGallery from "react-image-gallery";
import "react-image-gallery/styles/css/image-gallery.css";

import { CButton } from "@coreui/react";

const JobGallery = ({ photos }) => {
  return <ImageGallery items={photos} />;
};

const JobView = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const job = location.state?.job;
  const estate = job?.estate;

  // Verificar si el jobe fue recibido correctamente
  if (!job || !Array.isArray(job.listPhotosURL)) {
    return <div>No se encontró la obra</div>;
  }

  // Convertir listPhotos a la estructura requerida por ImageGallery
  const photos = job.listPhotosURL.map((url) => ({
    original: url,
    thumbnail: url, // O la URL de la miniatura si está disponible
  }));

  const goBack = () => {
    navigate(-1); // Esto hace que el navegador vuelva a la página anterior
  };

  return (
    <div>
      <h3>
        Obra: {job.name} [dir. {estate?.address}]
      </h3>
      <CButton size="sm" color="secondary" onClick={goBack}>
        Volver
      </CButton>
      <br />
      <br />
      <div>
        <JobGallery photos={photos} />
      </div>
    </div>
  );
};

export default JobView;
