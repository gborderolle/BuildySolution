import React, { Suspense } from "react";
import { Navigate, Route, Routes } from "react-router-dom";
import { CContainer, CSpinner } from "@coreui/react";

import routes from "../../routes";

// redux imports
import { useSelector } from "react-redux";

const AppContent = () => {
  //#region Consts ***********************************

  // Redux get
  const userRole = useSelector((state) => state.auth.userRole);

  const routeMap = {
    Admin: "estates",
    User: "estates",
  };
  const defaultRoute = routeMap[userRole] || "login";

  //#endregion Consts ***********************************

  const renderRoutes = () => {
    return routes.map(
      (route, idx) =>
        route.element && (
          <Route
            key={idx}
            path={route.path}
            exact={route.exact}
            name={route.name}
            element={<route.element />}
          />
        )
    );
  };

  return (
    <CContainer fluid>
      <Suspense fallback={<CSpinner color="primary" />}>
        <Routes>
          {renderRoutes()}
          <Route path="/" element={<Navigate to={defaultRoute} replace />} />
        </Routes>
      </Suspense>
    </CContainer>
  );
};

export default React.memo(AppContent);
