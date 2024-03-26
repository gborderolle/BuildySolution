import React from "react";

const EstateMenu = React.lazy(() =>
  import("./components/adminSide/estates/EstateMenu")
);
const EstateABM = React.lazy(() =>
  import("./components/adminSide/estates/EstateABM")
);

const ReportMenu = React.lazy(() =>
  import("./components/adminSide/reports/ReportMenu")
);
const ReportABM = React.lazy(() =>
  import("./components/adminSide/reports/ReportABM")
);
const ReportView = React.lazy(() =>
  import("./components/adminSide/reports/ReportView")
);

const WorkerMenu = React.lazy(() =>
  import("./components/adminSide/workers/WorkerMenu")
);
const WorkerABM = React.lazy(() =>
  import("./components/adminSide/workers/WorkerABM")
);

const TenantMenu = React.lazy(() =>
  import("./components/adminSide/tenants/TenantMenu")
);
const TenantABM = React.lazy(() =>
  import("./components/adminSide/tenants/TenantABM")
);

const JobMenu = React.lazy(() => import("./components/adminSide/jobs/JobMenu"));
const JobABM = React.lazy(() => import("./components/adminSide/jobs/JobABM"));
const JobView = React.lazy(() => import("./components/adminSide/jobs/JobView"));

const RentABM = React.lazy(() =>
  import("./components/adminSide/rents/RentABM")
);

const DataMenu = React.lazy(() =>
  import("./components/adminSide/data/DataMenu")
);

const MapMenu = React.lazy(() =>
  import("./components/adminSide/maps/MapMenu")
);

const LogsTable = React.lazy(() =>
  import("./components/adminSide/data/LogsTable")
);

const routes = [
  { path: "/", exact: true, name: "Home" },
  { path: "/estates", name: "Propiedades", element: EstateMenu },
  { path: "/estate-abm", name: "Agregar propiedades", element: EstateABM },

  { path: "/reports", name: "Reportes", element: ReportMenu },
  { path: "/report-abm", name: "Agregar reportes", element: ReportABM },
  { path: "/report-view", name: "Ver álbum", element: ReportView },

  { path: "/jobs", name: "Obras", element: JobMenu },
  { path: "/job-abm", name: "Agregar obras", element: JobABM },
  { path: "/job-view", name: "Ver álbum", element: JobView },

  { path: "/tenants", name: "Inquilinos", element: TenantMenu },
  { path: "/tenant-abm", name: "Agregar inquilinos", element: TenantABM },

  { path: "/workers", name: "Trabajadores", element: WorkerMenu },
  { path: "/worker-abm", name: "Agregar trabajadores", element: WorkerABM },

  { path: "/rent-abm", name: "Agregar alquiler", element: RentABM },

  { path: "/map", name: "Mapa", element: MapMenu },

  { path: "/data", name: "Datos base", element: DataMenu },

  { path: "/logs", name: "Logs", element: LogsTable },
];

export default routes;
