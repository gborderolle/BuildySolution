import React from "react";
import { CFooter } from "@coreui/react";

const AppFooter = (props) => {
  return (
    <CFooter>
      <div className={props.className} style={{ textAlign: "center" }}>
        <a
          href="https://buildy.uy"
          target="_blank"
          rel="noopener noreferrer"
        >
          Buildy 2.0
        </a>
        <span className="ms-1">
          &copy; Todos los derechos reservados, 2024.
        </span>
      </div>
    </CFooter>
  );
};

export default React.memo(AppFooter);
