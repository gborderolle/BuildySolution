import React from "react";

// Estates
const EstateMenu = React.lazy(() =>
  import("./components/adminSide/estates/EstateMenu")
);
const EstateABM = React.lazy(() =>
  import("./components/adminSide/estates/EstateABM")
);

// Reports
const ReportMenu = React.lazy(() =>
  import("./components/adminSide/reports/ReportMenu")
);
const ReportABM = React.lazy(() =>
  import("./components/adminSide/reports/ReportABM")
);
const ReportView = React.lazy(() =>
  import("./components/adminSide/reports/ReportView")
);

// Jobs
const JobMenu = React.lazy(() => import("./components/adminSide/jobs/JobMenu"));
const JobABM = React.lazy(() => import("./components/adminSide/jobs/JobABM"));
const JobView = React.lazy(() => import("./components/adminSide/jobs/JobView"));

// Tenants
const TenantMenu = React.lazy(() =>
  import("./components/adminSide/tenants/TenantMenu")
);
const TenantABM = React.lazy(() =>
  import("./components/adminSide/tenants/TenantABM")
);

// Workers
const WorkerMenu = React.lazy(() =>
  import("./components/adminSide/workers/WorkerMenu")
);
const WorkerABM = React.lazy(() =>
  import("./components/adminSide/workers/WorkerABM")
);

// Rents
const RentABM = React.lazy(() =>
  import("./components/adminSide/rents/RentABM")
);

// Others
const DataMenu = React.lazy(() =>
  import("./components/adminSide/data/DataMenu")
);
const MapMenu = React.lazy(() => import("./components/adminSide/maps/MapMenu"));
const LogsTable = React.lazy(() =>
  import("./components/adminSide/data/LogsTable")
);
const RedirectHome = React.lazy(() => import("./utils/RedirectHome"));

const routes = [
  // Estates
  { path: "/estates", name: "Propiedades", element: EstateMenu },
  { path: "/estate-abm", name: "Agregar propiedades", element: EstateABM },

  // Reports
  { path: "/reports", name: "Reportes", element: ReportMenu },
  { path: "/report-abm", name: "Agregar reportes", element: ReportABM },
  { path: "/report-view", name: "Ver álbum", element: ReportView },

  // Jobs
  { path: "/jobs", name: "Obras", element: JobMenu },
  { path: "/job-abm", name: "Agregar obras", element: JobABM },
  { path: "/job-view", name: "Ver álbum", element: JobView },

  // Tenants
  { path: "/tenants", name: "Inquilinos", element: TenantMenu },
  { path: "/tenant-abm", name: "Agregar inquilinos", element: TenantABM },

  // Workers
  { path: "/workers", name: "Trabajadores", element: WorkerMenu },
  { path: "/worker-abm", name: "Agregar trabajadores", element: WorkerABM },

  // Rents
  { path: "/rent-abm", name: "Agregar alquiler", element: RentABM },

  // Others
  { path: "/map", name: "Mapa", element: MapMenu },
  { path: "/data", name: "Datos base", element: DataMenu },
  { path: "/logs", name: "Logs", element: LogsTable },

  { path: "/", exact: true, element: EstateMenu },
  { path: "*", element: RedirectHome },
];

export default routes;
