import React, { useState, useEffect, useMemo } from "react";
import { useNavigate } from "react-router-dom";

import {
  CCard,
  CCardBody,
  CCol,
  CCardHeader,
  CRow,
  CFormInput,
} from "@coreui/react";
import { motion } from "framer-motion";

import { MapContainer, TileLayer, useMap, Popup, Marker } from "react-leaflet";
import MarkerClusterGroup from "react-leaflet-cluster";
import iconBlue from "leaflet/dist/images/marker-icon.png";
import iconGreen from "leaflet/dist/images/marker-icon-completed.png";
import iconShadow from "leaflet/dist/images/marker-shadow.png";
import "leaflet/dist/leaflet.css";
import L, { divIcon } from "leaflet";

// redux imports
import { useSelector, useDispatch, batch } from "react-redux";
import { authActions } from "../../../store/auth-slice";
import {
  fetchEstateList,
  fetchProvinceList,
} from "../../../store/generalData-actions";

import "./MapMenu.css";
import classes from "./Map.module.css";

// Componente Popup personalizado
const CustomPopup = ({ estate, rentList }) => {
  const currentRent = rentList.find((rent) => rent.id === estate.presentRentId);
  const primaryTenant = currentRent ? currentRent.listTenants[0] : null;

  const formatToDollars = (number) => {
    return new Intl.NumberFormat("en-US", {
      style: "currency",
      currency: "USD",
      minimumFractionDigits: 0,
      maximumFractionDigits: 0,
    }).format(number);
  };

  return (
    <Popup>
      <strong>Nombre: </strong>
      {estate.name}
      <br />
      <strong>Dirección: </strong>
      {estate.address}
      <br />
      <strong>Inquilino: </strong>
      {primaryTenant ? primaryTenant.name : "-"}
      <br />
      <strong>Renta: </strong>
      {currentRent ? formatToDollars(currentRent.monthlyValue) : "-"}
      <br />
      {estate.googleMapsURL && (
        <div>
          <a
            href={estate.googleMapsURL}
            target="_blank"
            rel="noopener noreferrer"
          >
            Ver casa
          </a>
        </div>
      )}
    </Popup>
  );
};

L.icon({
  iconUrl: iconBlue,
  shadowUrl: iconShadow,
  iconAnchor: [16, 37],
  iconSize: [20, 30],
});

// Opciones de marcador por defecto con icono azul
const defaultIcon = L.icon({
  iconUrl: iconBlue,
  shadowUrl: iconShadow,
  iconAnchor: [16, 37],
  iconSize: [20, 30],
});

// Opciones de marcador con icono rojo
const findedIcon = L.icon({
  iconUrl: iconGreen,
  shadowUrl: iconShadow,
  iconAnchor: [16, 37],
  iconSize: [20, 30],
});

const MapMenu = () => {
  //#region Consts ***********************************

  // redux init
  const dispatch = useDispatch();

  // redux gets
  const [estateList, setEstateList] = useState([]);
  const reduxEstateList =
    useSelector((state) => state.generalData.estateList) || [];

  const [provinceList, setProvinceList] = useState([]);
  const reduxProvinceList = useSelector(
    (state) => state.generalData.provinceList
  );

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

  const dropdownVariants = {
    open: {
      opacity: 1,
      height: "auto",
      transition: { duration: 0.5, type: "spring", stiffness: 120 },
    },
    closed: { opacity: 0, height: 0, transition: { duration: 0.5 } },
  };

  const [searchTerm, setSearchTerm] = useState("");

  const isMobile = useSelector((state) => state.auth.isMobile);
  const defaultCenter = [-34.91911763324771, -56.15673823330682];

  // redux init

  // redux gets
  const [rentList, setRentList] = useState([]);
  const reduxRentList =
    useSelector((state) => state.generalData.rentList) || [];

  const mapStyle = {
    height: isMobile ? "370px" : "450px",
    width: "100%",
  };

  const fullMapStyle = {
    ...mapStyle,
    marginLeft: "0",
    marginRight: "0",
    paddingLeft: "0",
    paddingRight: "0",
  };

  const handleSearchChange = (event) => {
    setSearchTerm(event.target.value.toLowerCase());
  };

  // Función para verificar si un Estate coincide con la búsqueda
  const estateMatchesSearch = (estate) => {
    const term = searchTerm.toLowerCase();
    const matchesName = estate.name.toLowerCase().includes(term);
    const matchesAddress = estate.address.toLowerCase().includes(term);
    const currentRent = estate.listRents.find(
      (rent) => rent.id === estate.presentRentId
    );
    const primaryTenant = currentRent ? currentRent.listTenants[0] : null;
    const matchesTenant = primaryTenant
      ? primaryTenant.name.toLowerCase().includes(term)
      : false;

    return matchesName || matchesAddress || matchesTenant;
  };

  //#endregion Consts ***********************************

  //#region Hooks ***********************************

  // Scroll to top of the page on startup
  useEffect(() => {
    window.scrollTo(0, 0);
  }, []);

  // redux gets
  useEffect(() => {
    setProvinceList(reduxProvinceList);
    setEstateList(reduxEstateList);
  }, [reduxProvinceList, reduxEstateList]);

  // redux gets
  useEffect(() => {
    setRentList(reduxRentList);
  }, [reduxRentList]);

  useEffect(() => {
    L.Marker.prototype.options.icon = L.icon({
      iconUrl: iconBlue,
      shadowUrl: iconShadow,
      iconAnchor: [16, 37],
      iconSize: [20, 30],
    });
  }, []);

  // Calcula los límites para incluir solo los Estates que coinciden con la búsqueda
  const searchBounds = useMemo(() => {
    const latLngBounds = L.latLngBounds();
    estateList.forEach((estate) => {
      if (estate.latLong && estateMatchesSearch(estate)) {
        const [lat, lon] = estate.latLong.split(",").map(Number);
        latLngBounds.extend([lat, lon]);
      }
    });
    return latLngBounds.isValid() ? latLngBounds : null;
  }, [estateList, searchTerm]);

  //#endregion Hooks ***********************************

  //#region Functions ***********************************

  // Hook para ajustar la vista del mapa
  const ChangeView = ({ bounds }) => {
    const map = useMap();

    useEffect(() => {
      if (map && bounds) {
        map.fitBounds(bounds, { padding: [50, 50] }); // Ajustar la vista del mapa a los límites con padding
      }
    }, [map, bounds]);

    return null;
  };

  // Calcula los límites para incluir todos los circuitos
  const bounds = useMemo(() => {
    const latLngBounds = L.latLngBounds();
    estateList.forEach((estate) => {
      if (estate.latLong) {
        const [lat, lon] = estate.latLong.split(",").map(Number);
        if (!isNaN(lat) && !isNaN(lon)) {
          latLngBounds.extend([lat, lon]);
        }
      }
    });
    return latLngBounds.isValid() ? latLngBounds : null; // Verificar que los límites son válidos
  }, [estateList]);

  // Función para crear el ícono del cluster
  const createCustomClusterIcon = (cluster) => {
    return new divIcon({
      html: `<div class=${
        classes.clusterIcon
      }>${cluster.getChildCount()}</div>`,
      className: "custom-marker-cluster",
      iconSize: L.point(33, 33, true),
    });
  };

  // Calcula el centro de todos los circuitos
  const centerOfEstates = useMemo(() => {
    if (estateList && estateList.length > 0) {
      let totalLat = 0;
      let totalLon = 0;
      let count = 0;

      estateList.forEach((estate) => {
        if (estate.latLong) {
          const [lat, lon] = estate.latLong.split(",").map(Number);
          if (!isNaN(lat) && !isNaN(lon)) {
            totalLat += lat;
            totalLon += lon;
            count++;
          }
        }
      });

      if (count > 0) {
        return [totalLat / count, totalLon / count];
      }
    }
    return defaultCenter; // Utiliza un centro por defecto si no hay circuitos
  }, [estateList]);

  const createMarkers = () => {
    return estateList.map((estate) => {
      if (estate.latLong) {
        const [lat, lon] = estate.latLong.split(",").map(Number);
        if (!isNaN(lat) && !isNaN(lon)) {
          const icon = estateMatchesSearch(estate) ? findedIcon : defaultIcon;

          return (
            <Marker key={estate.id} position={[lat, lon]} icon={icon}>
              <CustomPopup estate={estate} rentList={rentList} />
            </Marker>
          );
        }
      }
      return null;
    });
  };

  //#endregion Functions ***********************************

  //#region Events ***********************************

  const updateHandler = () => {
    const fetchGeneralData = async () => {
      batch(() => {
        dispatch(fetchEstateList());
        dispatch(fetchProvinceList());
      });
    };
    fetchGeneralData();
  };

  //#endregion Events ***********************************

  const filterVariants = {
    hidden: {
      y: -20,
      opacity: 0,
    },
    visible: {
      y: 0,
      opacity: 1,
      transition: {
        duration: 0.3,
        ease: "easeInOut",
      },
    },
  };

  return (
    <>
      <motion.div initial="closed" animate="open" variants={dropdownVariants}>
        <CCard className="mb-4" style={{ paddingBottom: "4rem" }}>
          <CCardHeader>
            <div
              style={{
                display: "flex",
                justifyContent: "space-between",
                alignItems: "center",
              }}
            >
              Distribución geográfica
              <div style={{ display: "flex", justifyContent: "flex-end" }}>
                <CFormInput
                  type="text"
                  placeholder="Buscar..."
                  value={searchTerm}
                  onChange={handleSearchChange}
                  style={{ maxWidth: "300px" }}
                />
              </div>
            </div>
          </CCardHeader>
          <CCardBody>
            <CRow>
              <CCol xs={12} sm={9} md={9} lg={9} xl={9}>
                <MapContainer style={fullMapStyle}>
                  <ChangeView bounds={searchBounds || bounds} />{" "}
                  <TileLayer url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png" />
                  <MarkerClusterGroup
                    chunkedLoading
                    iconCreateFunction={createCustomClusterIcon}
                  >
                    {createMarkers()}
                  </MarkerClusterGroup>
                </MapContainer>
              </CCol>
            </CRow>
          </CCardBody>
        </CCard>
      </motion.div>
    </>
  );
};

export default MapMenu;
