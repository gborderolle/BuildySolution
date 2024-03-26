import React, { useEffect } from "react";
import { useNavigate } from "react-router-dom";

import {
  AppContent,
  AppSidebar,
  AppFooterMobileAdmin,
  AppHeader,
} from "../index";

// Redux import
import { authActions } from "../../store/auth-slice";

import classes from "./DefaultLayout.module.css";

// Redux imports
import { batch, useDispatch, useSelector } from "react-redux";
import {
  fetchEstateList,
  fetchJobList,
  fetchRentList,
  fetchReportList,
  fetchWorkerList,
  fetchTenantList,
  fetchCountryList,
  fetchProvinceList,
  fetchCityList,
  fetchOwnerList,
  fetchUserList,
  fetchUserRoleList,
  fetchLogsList,
} from "../../store/generalData-actions";

const DefaultLayout = () => {
  //#region Consts

  const navigate = useNavigate();

  // Redux get
  const userRole = useSelector((state) => state.auth.userRole);

  // LocalStorage get
  const isMobile = JSON.parse(localStorage.getItem("isMobile"));

  // Redux fetch DB
  const dispatch = useDispatch();

  //#endregion Consts

  //#region Hooks

  // Hook para revisar expiración del token
  useEffect(() => {
    const checkTokenExpiration = () => {
      const isLoggedInData = JSON.parse(localStorage.getItem("isLoggedIn"));
      if (!isLoggedInData || new Date().getTime() >= isLoggedInData.expiry) {
        dispatch(authActions.logout());
        navigate("/login");
      }
    };

    const intervalId = setInterval(() => {
      checkTokenExpiration();
      // }, 300000); // 300000 ms son 5 minutos
    }, 3600000); // 3600000 ms son 1 hora

    // Limpieza al desmontar el componente
    return () => clearInterval(intervalId);
  }, []);

  // Redux fetch DB (carga inicial)
  useEffect(() => {
    if (isMobile) {
      // Aplicar estilos al montar el componente
      document.documentElement.style.fontSize = "small";
      document.body.style.fontSize = "small";
    }

    const fetchGeneralData = async () => {
      batch(() => {
        dispatch(fetchEstateList());
        dispatch(fetchJobList());
        dispatch(fetchRentList());
        dispatch(fetchReportList());
        dispatch(fetchWorkerList());
        dispatch(fetchTenantList());
        dispatch(fetchCountryList());
        dispatch(fetchProvinceList());
        dispatch(fetchCityList());
        dispatch(fetchOwnerList());
        dispatch(fetchUserList());
        dispatch(fetchUserRoleList());
        dispatch(fetchLogsList());
      });
    };
    fetchGeneralData();

    // Limpiar estilos al desmontar el componente
    return () => {
      document.documentElement.style.fontSize = "";
      document.body.style.fontSize = "";
    };
  }, [dispatch]);

  //#endregion Hooks

  return (
    <div>
      {!isMobile && <AppSidebar />}
      <div className="wrapper d-flex flex-column min-vh-100 bg-light">
        <AppHeader />
        &nbsp;
        <div className="body flex-grow-1 px-3">
          <AppContent />
        </div>
        {/* {userRole == "Admin" && !isMobile && (
          <AppFooter className={classes.AppFooter} />
        )} */}
        {isMobile && (
          <AppFooterMobileAdmin
            userRole={userRole}
            className={classes.AppFooter}
          />
        )}
      </div>
    </div>
  );
};

export default DefaultLayout;
