import React from "react";
import { useLocation } from "react-router-dom";
import routes from "../../routes";
import { CBreadcrumb, CBreadcrumbItem } from "@coreui/react";

// Redux imports
import { useSelector, useDispatch } from "react-redux";

import Clock from "../clock/Clock"; // Importa el componente Clock

import classes from "./AppBreadcrumb.module.css";

const AppBreadcrumb = (props) => {
  //#region Consts ***********************************

  // Redux get
  const isMobile = useSelector((state) => state.auth.isMobile);

  const currentLocation = useLocation().pathname;

  // redux init
  const dispatch = useDispatch();

  //#endregion Consts ***********************************

  //#region Hooks ***********************************

  //#endregion Hooks ***********************************

  //#region Functions ***********************************

  const getRouteName = (pathname, routes) => {
    const currentRoute = routes.find((route) => route.path === pathname);
    return currentRoute ? currentRoute.name : false;
  };

  const getBreadcrumbs = (location) => {
    const breadcrumbs = [];
    location.split("/").reduce((prev, curr, index, array) => {
      const currentPathname = `${prev}/${curr}`;
      const routeName = getRouteName(currentPathname, routes);
      routeName &&
        breadcrumbs.push({
          pathname: currentPathname,
          name: routeName,
          active: index + 1 === array.length ? true : false,
        });
      return currentPathname;
    });
    return breadcrumbs;
  };

  const breadcrumbs = getBreadcrumbs(currentLocation);

  //#endregion Functions ***********************************

  return (
    <div className="d-flex justify-content-between align-items-center w-100">
      {!isMobile && (
        <CBreadcrumb className={`m-0 ms-2 ${classes.CBreadcrumb}`}>
          <CBreadcrumbItem href="/#/estates">Home</CBreadcrumbItem>
          {breadcrumbs.map((breadcrumb, index) => {
            return (
              <CBreadcrumbItem
                {...(breadcrumb.active
                  ? { active: true }
                  : { href: breadcrumb.pathname })}
                key={index}
              >
                {breadcrumb.name}
              </CBreadcrumbItem>
            );
          })}
        </CBreadcrumb>
      )}
      <div style={{ margin: "auto" }}>{props.children}</div>
      {!isMobile && <Clock />}
    </div>
  );
};

export default React.memo(AppBreadcrumb);
