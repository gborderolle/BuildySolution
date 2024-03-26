import React from "react";
import { NavLink } from "react-router-dom";

// redux imports
import { useSelector, useDispatch } from "react-redux";

import {
  CContainer,
  CHeader,
  CHeaderNav,
  CHeaderToggler,
  CNavLink,
  CNavItem,
  CImage,
} from "@coreui/react";
import CIcon from "@coreui/icons-react";
import { cilMenu } from "@coreui/icons";

import { AppHeaderDropdown } from "../header/index";

import logoSmall from "src/assets/images/buildyTxt.png";
import classes from "./AppHeader.css";

const AppHeader = () => {
  //#region Consts ***********************************

  // redux get
  const dispatch = useDispatch();

  // Redux get
  const sidebarShow = useSelector((state) => state.oldState.sidebarShow);
  const userRole = useSelector((state) => state.loggedUser?.userRole || 0);

  // LocalStorage get
  const isMobile = JSON.parse(localStorage.getItem("isMobile"));

  //#endregion Consts ***********************************

  //#region Hooks ***********************************

  //#endregion Hooks ***********************************

  //#region Functions ***********************************

  //#endregion Functions ***********************************

  //#region JSX ***********************************

  const headerStyle = isMobile
    ? {
        "--cui-header-bg": "#697588",
        color: "whitesmoke",
      }
    : {};

  const iconStyle = isMobile
    ? {
        "--ci-primary-color": "whitesmoke",
      }
    : {};

  //#endregion JSX ***********************************

  return (
    <CHeader position="sticky" className="mb-1" style={headerStyle}>
      <CContainer fluid>
        {isMobile ? (
          <CImage fluid src={logoSmall} className={classes.CImage} width={70} />
        ) : (
          <CHeaderToggler
            className="ps-1"
            onClick={() => dispatch({ type: "set", sidebarShow: !sidebarShow })}
            style={iconStyle}
          >
            <CIcon icon={cilMenu} size="lg" />
          </CHeaderToggler>
        )}

        <CHeaderNav className="d-none d-md-flex me-auto">
          <CNavItem>
            <CNavLink to="/estates" component={NavLink}>
              Propiedades
            </CNavLink>
          </CNavItem>
        </CHeaderNav>

        <CHeaderNav className="ms-3">
          <AppHeaderDropdown />
        </CHeaderNav>
      </CContainer>
    </CHeader>
  );
};

export default AppHeader;
