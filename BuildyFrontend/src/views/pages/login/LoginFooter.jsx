import React from "react";

const Footer = ({ isMobileDevice = false }) => {
  return (
    <div
      style={{
        marginTop: "auto",
        backgroundColor: "#f8f9fa",
        textAlign: "center",
        padding: "10px 0",
        width: "100%",
        fontSize: isMobileDevice ? "12px" : "16px",
      }}
    >
      Buildy v3.0 - © {new Date().getFullYear()} Todos los derechos reservados.
    </div>
  );
};

export default Footer;
