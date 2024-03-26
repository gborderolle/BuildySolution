import React, { useEffect } from "react";

// redux imports
import { useSelector, useDispatch } from "react-redux";

import {
  CSidebar,
  CSidebarBrand,
  CSidebarNav,
  CSidebarToggler,
  CImage,
} from "@coreui/react";
import CIcon from "@coreui/icons-react";

import { AppSidebarNav } from "./AppSidebarNav";

import logoBig from "src/assets/images/Buildyv2-h.png";
import { sygnet } from "src/assets/brand/sygnet";

import SimpleBar from "simplebar-react";
import "simplebar/dist/simplebar.min.css";

// sidebar nav config
import navigation from "../../_nav";

const AppSidebar = () => {
  //#region Consts

  // redux get
  const dispatch = useDispatch();

  const unfoldable = useSelector((state) => state.oldState.sidebarUnfoldable);
  const sidebarShow = useSelector((state) => state.oldState.sidebarShow);
  const userRole = useSelector((state) => state.auth.userRole);

  //#endregion Consts

  //#region Hooks

  const filterByRoleId = (roleId) => {
    return navigation.filter((item) => {
      return Array.isArray(item.roles) && item.roles.includes(roleId);
    });
  };

  useEffect(() => {
    const handleVisibilityChange = (visible) => {
      if (sidebarShow !== visible) {
        dispatch({ type: "set", sidebarShow: visible });
      }
    };
    handleVisibilityChange(sidebarShow);

    return () => {};
  }, [sidebarShow]);

  //#endregion Hooks

  //#region Methods

  const handleVisibilityChange = (visible) => {
    if (sidebarShow !== visible) {
      // dispatch({ type: "set", sidebarShow: visible });
    }
  };

  //#endregion Methods

  const filteredNavigation = filterByRoleId(userRole);

  return (
    <CSidebar
      position="fixed"
      unfoldable={unfoldable}
      visible={sidebarShow}
      onVisibleChange={handleVisibilityChange}
    >
      <CSidebarBrand className="d-none d-md-flex" to="/">
        <CImage
          fluid
          src={logoBig}
          width={150}
          className="p-2"
          style={{ opacity: "0.86" }}
        />
        <CIcon className="sidebar-brand-narrow" icon={sygnet} height={35} />
      </CSidebarBrand>
      <CSidebarNav>
        <SimpleBar>
          {/* <AppSidebarNav items={navigation} /> */}
          <AppSidebarNav items={filteredNavigation} />
        </SimpleBar>
      </CSidebarNav>
      <CSidebarToggler
        className="d-none d-lg-flex"
        onClick={
          () => dispatch({ type: "set", sidebarUnfoldable: !unfoldable })
          // dispatch({ type: "set" })
        }
      />
    </CSidebar>
  );
};

export default React.memo(AppSidebar);
