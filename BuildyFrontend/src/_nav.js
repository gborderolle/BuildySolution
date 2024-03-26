import React from "react";
import CIcon from "@coreui/icons-react";
import {
  cilHome,
  cilMap,
  cilImage,
  cilPaint,
  cilPeople,
  cilBriefcase,
  cilStorage,
  cilBellExclamation,
} from "@coreui/icons";
import { CNavItem } from "@coreui/react";

const _nav = [
  {
    roles: ["Admin", "User"],
    component: CNavItem,
    name: "Propiedades",
    to: "/estates",
    icon: <CIcon icon={cilHome} customClassName="nav-icon" />,
  },
  {
    roles: ["Admin", "User"],
    component: CNavItem,
    name: "Mapa",
    to: "/map",
    icon: <CIcon icon={cilMap} customClassName="nav-icon" />,
  },
  {
    roles: ["Admin", "User"],
    component: CNavItem,
    name: "Reportes",
    to: "/reports",
    icon: <CIcon icon={cilImage} customClassName="nav-icon" />,
  },
  {
    roles: ["Admin", "User"],
    component: CNavItem,
    name: "Obras",
    to: "/jobs",
    icon: <CIcon icon={cilPaint} customClassName="nav-icon" />,
  },
  {
    roles: ["Admin", "User"],
    component: CNavItem,
    name: "Inquilinos",
    to: "/tenants",
    icon: <CIcon icon={cilPeople} customClassName="nav-icon" />,
  },
  {
    roles: ["Admin", "User"],
    component: CNavItem,
    name: "Trabajadores",
    to: "/workers",
    icon: <CIcon icon={cilBriefcase} customClassName="nav-icon" />,
  },
  {
    roles: ["Admin"],
    component: CNavItem,
    name: "Datos base",
    to: "/data",
    icon: <CIcon icon={cilStorage} customClassName="nav-icon" />,
  },
  {
    roles: ["Admin"],
    component: CNavItem,
    name: "Logs",
    to: "/logs",
    icon: <CIcon icon={cilBellExclamation} customClassName="nav-icon" />,
  },
];

export default _nav;
