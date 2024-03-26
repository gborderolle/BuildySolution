import React, { useEffect } from "react";
import { useNavigate } from "react-router-dom";

import {
  CCol,
  CRow,
  CAccordion,
  CAccordionItem,
  CAccordionHeader,
  CAccordionBody,
  CCard,
  CCardHeader,
  CCardBody,
} from "@coreui/react";

// redux imports
import { useSelector, useDispatch } from "react-redux";
import { authActions } from "../../../store/auth-slice";
import CityTable from "./CityTable";
import ProvinceTable from "./ProvinceTable";
import CountryTable from "./CountryTable";
import UserRoleTable from "./UserRoleTable";
import UserTable from "./UserTable";
import OwnerTable from "./OwnerTable";

const DataMenu = () => {
  //#region Const ***********************************

  // redux
  const dispatch = useDispatch();

  //#region RUTA PROTEGIDA
  const navigate = useNavigate();
  const username = useSelector((state) => state.auth.username);
  useEffect(() => {
    if (!username) {
      dispatch(authActions.logout());
      navigate("/login");
    }
  }, [username, navigate, dispatch]);
  //#endregion RUTA PROTEGIDA

  //#endregion Const ***********************************

  //#region Functions ***********************************

  const cityData = async (cityName, provinceId) => {
    return {
      cityName,
      provinceId,
    };
  };

  const provinceData = async (provinceName, countryId) => {
    return {
      provinceName,
      countryId,
    };
  };

  const countryData = async (countryName) => {
    return {
      countryName,
    };
  };

  const userData = async (userName, email, roleId) => {
    return {
      userName,
      email,
      roleId,
    };
  };

  const userRoleData = async (roleName) => {
    return {
      roleName,
    };
  };

  const userOwner = async (name, color) => {
    return {
      name,
      color,
    };
  };

  //#endregion Functions ***********************************

  return (
    <CRow>
      <CCol xs>
        <CAccordion>
          <CAccordionItem itemKey={1}>
            <CAccordionHeader className="custom-accordion-header">
              Menú ciudades
            </CAccordionHeader>
            <CAccordionBody>
              <CCard>
                <CCardHeader>Tabla de datos</CCardHeader>
                <CCardBody>
                  <CityTable
                    title="Menú ciudades"
                    inputName="Nombre"
                    nominatimCode="Código nominatim"
                    createDataToUpload={cityData}
                  />
                </CCardBody>
              </CCard>
            </CAccordionBody>
          </CAccordionItem>
        </CAccordion>
        <br />
        <CAccordion>
          <CAccordionItem itemKey={1}>
            <CAccordionHeader className="custom-accordion-header">
              Menú departamentos
            </CAccordionHeader>
            <CAccordionBody>
              <CCard>
                <CCardHeader>Tabla de datos</CCardHeader>
                <CCardBody>
                  <ProvinceTable
                    title="Menú departamentos"
                    inputName="Nombre"
                    nominatimCode="Código nominatim"
                    createDataToUpload={provinceData}
                  />
                </CCardBody>
              </CCard>
            </CAccordionBody>
          </CAccordionItem>
        </CAccordion>
        <br />
        <CAccordion>
          <CAccordionItem itemKey={1}>
            <CAccordionHeader className="custom-accordion-header">
              Menú países
            </CAccordionHeader>
            <CAccordionBody>
              <CCard>
                <CCardHeader>Tabla de datos</CCardHeader>
                <CCardBody>
                  <CountryTable
                    title="Menú países"
                    inputName="Nombre"
                    nominatimCode="Código nominatim"
                    createDataToUpload={countryData}
                  />
                </CCardBody>
              </CCard>
            </CAccordionBody>
          </CAccordionItem>
        </CAccordion>
        <br />
        <CAccordion>
          <CAccordionItem itemKey={1}>
            <CAccordionHeader className="custom-accordion-header">
              Menú usuarios
            </CAccordionHeader>
            <CAccordionBody>
              <CCard>
                <CCardHeader>Tabla de datos</CCardHeader>
                <CCardBody>
                  <UserTable
                    title="Menú usuarios"
                    inputName="Nombre"
                    email="Email"
                    password="Password"
                    createDataToUpload={userData}
                  />
                </CCardBody>
              </CCard>
            </CAccordionBody>
          </CAccordionItem>
        </CAccordion>
        <br />
        <CAccordion>
          <CAccordionItem itemKey={1}>
            <CAccordionHeader className="custom-accordion-header">
              Menú roles de usuario
            </CAccordionHeader>
            <CAccordionBody>
              <CCard>
                <CCardHeader>Tabla de datos</CCardHeader>
                <CCardBody>
                  <UserRoleTable
                    title="Menú roles de usuario"
                    inputName="Nombre"
                    createDataToUpload={userRoleData}
                  />
                </CCardBody>
              </CCard>
            </CAccordionBody>
          </CAccordionItem>
        </CAccordion>
        <br />
        <CAccordion>
          <CAccordionItem itemKey={1}>
            <CAccordionHeader className="custom-accordion-header">
              Menú dueños
            </CAccordionHeader>
            <CAccordionBody>
              <CCard>
                <CCardHeader>Tabla de datos</CCardHeader>
                <CCardBody>
                  <OwnerTable
                    title="Menú dueños"
                    inputName="Nombre"
                    color="Color"
                    createDataToUpload={userOwner}
                  />
                </CCardBody>
              </CCard>
            </CAccordionBody>
          </CAccordionItem>
        </CAccordion>
        <br />
      </CCol>
    </CRow>
  );
};

export default DataMenu;
