import React, { useState, useEffect, useMemo } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import {
  CCard,
  CCardBody,
  CCardHeader,
  CRow,
  CFormInput,
  CTable,
  CPagination,
  CPaginationItem,
} from "@coreui/react";

import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faPlus,
  faRefresh,
  faEye,
  faCamera,
  faMapMarkerAlt,
} from "@fortawesome/free-solid-svg-icons";

// redux imports
import { useDispatch, useSelector, batch } from "react-redux";
import { authActions } from "../../../store/auth-slice";
import {
  fetchReportList,
  fetchEstateList,
} from "../../../store/generalData-actions";

import "./ReportMenu.css";

const ReportMenu = () => {
  //#region Consts ***********************************

  const location = useLocation();
  const estate = location.state?.estate;
  const listMode = location.state?.listMode ? location.state?.listMode : false;

  const [searchTerm, setSearchTerm] = useState("");
  const [selectedReport, setSelectedReport] = useState(null);

  const dispatch = useDispatch();

  const [reportList, setReportList] = useState([]);
  const reduxReportList =
    useSelector((state) => state.generalData.reportList) || [];

  const itemsPerPage = 25;
  const [currentPage, setCurrentPage] = useState(1);
  const [pageCount, setPageCount] = useState(0);

  const [sortConfig, setSortConfig] = useState({
    key: "month",
    direction: "descending",
  });

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

  const handleSelectReport = (report) => {
    setSelectedReport(report);
  };

  //#endregion Consts ***********************************

  //#region Hooks ***********************************

  // Scroll to top of the page on startup
  useEffect(() => {
    window.scrollTo(0, 0);
  }, []);

  useEffect(() => {
    const sortReports = (reports) => {
      return reports.sort((a, b) => {
        const addressA = a.estate?.address?.toLowerCase() || "";
        const addressB = b.estate?.address?.toLowerCase() || "";
        return (
          (addressA < addressB ? -1 : 1) *
          (sortConfig.direction === "ascending" ? 1 : -1)
        );
      });
    };

    const sortedList =
      listMode && estate && estate.listReports
        ? sortReports([...estate.listReports])
        : sortReports([...reduxReportList]);

    setReportList(sortedList);
  }, [reduxReportList, estate, listMode, sortConfig]);

  const filteredReportList = reportList.filter((report) => {
    const match1 = report.name
      ? report.name.toLowerCase().includes(searchTerm.toLowerCase())
      : false;
    const match2 =
      report.estate && report.estate.name
        ? report.estate.name.toLowerCase().includes(searchTerm.toLowerCase())
        : false;
    const match3 =
      report.estate && report.estate.address
        ? report.estate.address.toLowerCase().includes(searchTerm.toLowerCase())
        : false;
    const match4 = report.comments
      ? report.comments.toLowerCase().includes(searchTerm.toLowerCase())
      : false;
    return match1 || match2 || match3 || match4;
  });

  useEffect(() => {
    setPageCount(Math.ceil(filteredReportList.length / itemsPerPage));
  }, [filteredReportList, itemsPerPage]);

  const handleSearchChange = (event) => {
    setSearchTerm(event.target.value);
  };

  const handlePageChange = (pageNumber) => {
    setCurrentPage(pageNumber);
  };

  const addHandler = () => {
    dispatch(fetchReportList());

    setTimeout(() => {
      navigate("/report-abm");
    }, 200); // Asegúrate de que este tiempo coincida o sea ligeramente mayor que la duración de tu animación
  };

  const updateHandler = () => {
    const fetchGeneralData = async () => {
      batch(() => {
        dispatch(fetchReportList());
        dispatch(fetchEstateList());
      });
    };
    fetchGeneralData();
  };

  const requestSort = (key) => {
    let direction = "ascending";
    if (sortConfig.key === key && sortConfig.direction === "ascending") {
      direction = "descending";
    }
    setSortConfig({ key, direction });
  };

  // Aplicar el ordenamiento a los datos
  const sortedList = useMemo(() => {
    let sortableList = [...filteredReportList];
    if (sortConfig.key !== null) {
      sortableList.sort((a, b) => {
        if (sortConfig.key === "listPhotosURL") {
          const photoURLA = a.listPhotosURL?.length || "";
          const photoURLB = b.listPhotosURL?.length || "";

          if (photoURLA < photoURLB) {
            return sortConfig.direction === "ascending" ? -1 : 1;
          }
          if (photoURLA > photoURLB) {
            return sortConfig.direction === "ascending" ? 1 : -1;
          }
          return 0;
        } else {
          // Ordenamiento para las demás propiedades
          if (a[sortConfig.key] < b[sortConfig.key]) {
            return sortConfig.direction === "ascending" ? -1 : 1;
          }
          if (a[sortConfig.key] > b[sortConfig.key]) {
            return sortConfig.direction === "ascending" ? 1 : -1;
          }
          return 0;
        }
      });
    }
    return sortableList;
  }, [filteredReportList, sortConfig]);

  //#endregion Hooks ***********************************

  //#region Functions ***********************************

  const renderReportRows = () => {
    const indexOfLastItem = currentPage * itemsPerPage;
    const indexOfFirstItem = indexOfLastItem - itemsPerPage;
    const currentReports = sortedList.slice(indexOfFirstItem, indexOfLastItem);

    return currentReports.map((report, index) => {
      // Parsear la fecha y formatearla como Año/Mes
      const reportDate = new Date(report.month);
      const formattedDate = `${reportDate.getFullYear()}/${(
        "0" +
        (reportDate.getMonth() + 1)
      ).slice(-2)}`; // Añade un cero delante para los meses de un solo dígito

      return (
        <tr key={report.id}>
          <td
            style={report.id === selectedReport?.id ? { color: "blue" } : null}
            onClick={() => handleSelectReport(report)}
          >
            <button
              style={{
                border: "none",
                background: "none",
                padding: 0,
                color: "blue",
                textDecoration: "underline",
                cursor: "pointer",
              }}
              onClick={() => handleSelectReport(report)}
            >
              {index + 1}
            </button>
          </td>
          <td
            style={report.id === selectedReport?.id ? { color: "blue" } : null}
            onClick={() => handleSelectReport(report)}
          >
            {formattedDate}
          </td>
          <td
            style={report.id === selectedReport?.id ? { color: "blue" } : null}
            onClick={() => handleSelectReport(report)}
          >
            {report.estate?.name} ({report.estate?.address})
          </td>
          <td
            style={report.id === selectedReport?.id ? { color: "blue" } : null}
            onClick={() => handleSelectReport(report)}
          >
            {report.name}
          </td>
          <td
            style={report.id === selectedReport?.id ? { color: "blue" } : null}
            onClick={() => handleSelectReport(report)}
          >
            {report.listPhotosURL ? report.listPhotosURL?.length : 0}
          </td>
          <td
            style={report.id === selectedReport?.id ? { color: "blue" } : null}
            onClick={() => handleSelectReport(report)}
          >
            {report.comments}
          </td>
          <td>
            <button
              onClick={() => navigateToReport(report)}
              style={{ border: "none", background: "none" }}
              title="Ver detalles"
            >
              <FontAwesomeIcon icon={faEye} color="#697588" />
            </button>
            <button
              onClick={() => navigateToAlbum(report)}
              style={{ border: "none", background: "none" }}
              title="Ver álbum"
            >
              <FontAwesomeIcon
                icon={faCamera}
                color={
                  report.listPhotosURL && report.listPhotosURL.length > 0
                    ? "#697588"
                    : "lightgray"
                }
              />
            </button>
            <button
              style={{
                border: "none",
                background: "none",
              }}
              onClick={() => {
                if (report.estate?.googleMapsURL) {
                  window.open(report.estate?.googleMapsURL, "_blank");
                } else {
                  alert("No hay Google Maps disponible.");
                }
              }}
              title="Ver en Google Maps"
            >
              <FontAwesomeIcon
                icon={faMapMarkerAlt}
                color={report.estate?.googleMapsURL ? "#697588" : "lightgray"}
              />
            </button>
          </td>
        </tr>
      );
    });
  };

  // Determinar el rango de páginas a mostrar alrededor de la página actual
  const pagesToShow = 3; // Ajusta este número según sea necesario
  let startPage = Math.max(currentPage - Math.floor(pagesToShow / 2), 1);
  let endPage = Math.min(startPage + pagesToShow - 1, pageCount);

  if (endPage - startPage + 1 < pagesToShow) {
    startPage = Math.max(endPage - pagesToShow + 1, 1);
  }

  const indexOfLastItem = currentPage * itemsPerPage;
  const indexOfFirstItem = indexOfLastItem - itemsPerPage;

  function navigateToReport(report) {
    navigate("/report-abm", { state: { report, editMode: true } });
  }

  const navigateToAlbum = (report) => {
    if (report.listPhotosURL && report.listPhotosURL.length > 0) {
      navigate("/report-view", { state: { report } });
    }
  };

  //#endregion Functions ***********************************

  //#region Events ***********************************

  return (
    <>
      <CCard className="mb-4">
        <CCardHeader>
          <div
            style={{
              display: "flex",
              justifyContent: "space-between",
              alignItems: "center",
            }}
          >
            {listMode && estate
              ? `Panel de reportes de propiedad: ${estate.name} (Dir: ${estate.address})`
              : "Panel de reportes (todos)"}

            <div style={{ display: "flex", justifyContent: "flex-end" }}>
              <CFormInput
                type="text"
                placeholder="Buscar..."
                value={searchTerm}
                onChange={handleSearchChange}
                style={{ maxWidth: "300px" }}
              />
              &nbsp;
              <button
                onClick={addHandler}
                style={{ border: "none", background: "none" }}
              >
                <FontAwesomeIcon icon={faPlus} color="#697588" />
              </button>
              <button
                onClick={updateHandler}
                style={{ border: "none", background: "none", float: "right" }}
              >
                <FontAwesomeIcon icon={faRefresh} color="#697588" />{" "}
              </button>
            </div>
          </div>
        </CCardHeader>
        <CCardBody>
          <CRow>
            <div className="custom-table-responsive">
              <CTable striped>
                <thead>
                  <tr>
                    <th>#</th>
                    <th
                      className="table-header"
                      onClick={() => requestSort("month")}
                    >
                      Fecha/mes
                    </th>
                    <th
                      className="table-header"
                      onClick={() => requestSort("name")}
                    >
                      Casa (dir)
                    </th>
                    <th
                      className="table-header"
                      onClick={() => requestSort("phone1")}
                    >
                      Reporte
                    </th>
                    <th
                      className="table-header"
                      onClick={() => requestSort("listPhotosURL")}
                    >
                      #Fotos
                    </th>
                    <th
                      className="table-header"
                      onClick={() => requestSort("comments")}
                    >
                      Comentarios
                    </th>
                    <th>Opciones</th>
                  </tr>
                </thead>
                <tbody>{renderReportRows()}</tbody>
              </CTable>
            </div>
          </CRow>
          <CPagination align="center" aria-label="Page navigation example">
            {startPage > 1 && (
              <CPaginationItem onClick={() => handlePageChange(1)}>
                1
              </CPaginationItem>
            )}
            {startPage > 2 && <CPaginationItem>...</CPaginationItem>}
            {[...Array(endPage - startPage + 1)].map((_, index) => (
              <CPaginationItem
                key={startPage + index}
                active={startPage + index === currentPage}
                onClick={() => handlePageChange(startPage + index)}
              >
                {startPage + index}
              </CPaginationItem>
            ))}
            {endPage < pageCount - 1 && <CPaginationItem>...</CPaginationItem>}
            {endPage < pageCount && (
              <CPaginationItem onClick={() => handlePageChange(pageCount)}>
                {pageCount}
              </CPaginationItem>
            )}
          </CPagination>
        </CCardBody>
      </CCard>
    </>
  );
};

export default ReportMenu;
