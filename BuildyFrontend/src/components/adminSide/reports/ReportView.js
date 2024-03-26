import React from "react";
import { useNavigate, useLocation } from "react-router-dom";
import ImageGallery from "react-image-gallery";
import "react-image-gallery/styles/css/image-gallery.css";

import { CButton } from "@coreui/react";

const ReportGallery = ({ photos }) => {
  return <ImageGallery items={photos} />;
};

const ReportView = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const report = location.state?.report;
  const estate = report?.estate;

  // Verificar si el reporte fue recibido correctamente
  if (!report || !Array.isArray(report.listPhotosURL)) {
    return <div>No se encontró el reporte</div>;
  }

  // Convertir listPhotos a la estructura requerida por ImageGallery
  const photos = report.listPhotosURL.map((url) => ({
    original: url,
    thumbnail: url, // O la URL de la miniatura si está disponible
  }));

  const goBack = () => {
    navigate(-1); // Esto hace que el navegador vuelva a la página anterior
  };

  return (
    <div>
      <h3>
        Reporte: {report.name} [dir. {estate?.address}]
      </h3>
      <CButton size="sm" color="secondary" onClick={goBack}>
        Volver
      </CButton>
      <br />
      <br />
      <div>
        <ReportGallery photos={photos} />
      </div>
    </div>
  );
};

export default ReportView;
