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
  fetchJobList,
  fetchWorkerList,
} from "../../../store/generalData-actions";

import "./JobMenu.css";

const JobMenu = () => {
  //#region Consts ***********************************

  const location = useLocation();
  const estate = location.state?.estate;
  const listMode = location.state?.listMode ? location.state?.listMode : false;

  const [searchTerm, setSearchTerm] = useState("");
  const [selectedJob, setSelectedJob] = useState(null);

  const dispatch = useDispatch();

  const [jobList, setJobList] = useState([]);
  const reduxJobList = useSelector((state) => state.generalData.jobList) || [];

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

  const handleSelectJob = (job) => {
    setSelectedJob(job);
  };

  //#endregion Consts ***********************************

  //#region Hooks ***********************************

  // Scroll to top of the page on startup
  useEffect(() => {
    window.scrollTo(0, 0);
  }, []);

  useEffect(() => {
    const sortJobs = (jobs) => {
      return jobs.sort((a, b) => {
        const addressA = a.estate?.address?.toLowerCase() || "";
        const addressB = b.estate?.address?.toLowerCase() || "";
        return (
          (addressA < addressB ? -1 : 1) *
          (sortConfig.direction === "ascending" ? 1 : -1)
        );
      });
    };

    const sortedList =
      listMode && estate && estate.listJobs
        ? sortJobs([...estate.listJobs])
        : sortJobs([...reduxJobList]);

    setJobList(sortedList);
  }, [reduxJobList, estate, listMode, sortConfig]);

  const filteredJobList = jobList.filter((job) => {
    const match1 = job.name
      ? job.name.toLowerCase().includes(searchTerm.toLowerCase())
      : false;
    const match2 =
      job.estate && job.estate.name
        ? job.estate.name.toLowerCase().includes(searchTerm.toLowerCase())
        : false;
    const match3 =
      job.estate && job.estate.address
        ? job.estate.address.toLowerCase().includes(searchTerm.toLowerCase())
        : false;
    const match4 = job.comments
      ? job.comments.toLowerCase().includes(searchTerm.toLowerCase())
      : false;
    return match1 || match2 || match3 || match4;
  });

  useEffect(() => {
    setPageCount(Math.ceil(filteredJobList.length / itemsPerPage));
  }, [filteredJobList, itemsPerPage]);

  const handleSearchChange = (event) => {
    setSearchTerm(event.target.value);
  };

  const handlePageChange = (pageNumber) => {
    setCurrentPage(pageNumber);
  };

  const addHandler = () => {
    dispatch(fetchJobList());

    setTimeout(() => {
      navigate("/job-abm");
    }, 200); // Asegúrate de que este tiempo coincida o sea ligeramente mayor que la duración de tu animación
  };

  const updateHandler = () => {
    const fetchGeneralData = async () => {
      batch(() => {
        dispatch(fetchJobList());
        dispatch(fetchWorkerList());
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
    let sortableList = [...filteredJobList];
    if (sortConfig.key !== null) {
      sortableList.sort((a, b) => {
        // Si ordenamos por 'estateName', necesitamos acceder a la propiedad anidada
        if (sortConfig.key === "estateName") {
          const estateNameA = a.estate?.name || "";
          const estateNameB = b.estate?.name || "";

          if (estateNameA < estateNameB) {
            return sortConfig.direction === "ascending" ? -1 : 1;
          }
          if (estateNameA > estateNameB) {
            return sortConfig.direction === "ascending" ? 1 : -1;
          }
          return 0;
        } else if (sortConfig.key === "listPhotosURL") {
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
  }, [filteredJobList, sortConfig]);

  //#endregion Hooks ***********************************

  //#region Functions ***********************************

  const renderJobRows = () => {
    const indexOfLastItem = currentPage * itemsPerPage;
    const indexOfFirstItem = indexOfLastItem - itemsPerPage;
    const currentJobs = sortedList.slice(indexOfFirstItem, indexOfLastItem);

    return currentJobs.map((job, index) => {
      // Parsear la fecha y formatearla como Año/Mes
      const jobDate = new Date(job.month);
      const formattedDate = `${jobDate.getFullYear()}/${(
        "0" +
        (jobDate.getMonth() + 1)
      ).slice(-2)}`; // Añade un cero delante para los meses de un solo dígito

      // Crear una cadena con todos los nombres de los trabajadores
      const workerNames =
        job.listWorkers && job.listWorkers.length > 0
          ? job.listWorkers.map((worker) => worker.name).join(", ")
          : "No asignado";

      return (
        <tr key={job.id}>
          <td
            style={job.id === selectedJob?.id ? { color: "blue" } : null}
            onClick={() => handleSelectJob(job)}
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
              onClick={() => handleSelectJob(job)}
            >
              {index + 1}
            </button>
          </td>
          <td
            style={job.id === selectedJob?.id ? { color: "blue" } : null}
            onClick={() => handleSelectJob(job)}
          >
            {formattedDate}
          </td>
          <td
            style={job.id === selectedJob?.id ? { color: "blue" } : null}
            onClick={() => handleSelectJob(job)}
          >
            {job.estate?.name} ({job.estate?.address})
          </td>
          <td
            style={job.id === selectedJob?.id ? { color: "blue" } : null}
            onClick={() => handleSelectJob(job)}
          >
            {job.name}
          </td>
          <td
            style={job.id === selectedJob?.id ? { color: "blue" } : null}
            onClick={() => handleSelectJob(job)}
          >
            {formatToDollars(job.labourCost)}
          </td>
          <td
            style={job.id === selectedJob?.id ? { color: "blue" } : null}
            onClick={() => handleSelectJob(job)}
          >
            {job.listPhotosURL ? job.listPhotosURL?.length : 0}
          </td>
          <td
            style={job.id === selectedJob?.id ? { color: "blue" } : null}
            onClick={() => handleSelectJob(job)}
          >
            {workerNames}
          </td>
          <td
            style={job.id === selectedJob?.id ? { color: "blue" } : null}
            onClick={() => handleSelectJob(job)}
          >
            {job.comments}
          </td>
          <td>
            <button
              onClick={() => navigateToJob(job)}
              style={{ border: "none", background: "none" }}
              title="Ver detalles"
            >
              <FontAwesomeIcon icon={faEye} color="#697588" />
            </button>

            <button
              onClick={() => navigateToAlbum(job)}
              style={{ border: "none", background: "none" }}
              title="Ver álbum"
            >
              <FontAwesomeIcon
                icon={faCamera}
                color={
                  job.listPhotosURL && job.listPhotosURL.length > 0
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
                if (job.estate?.googleMapsURL) {
                  window.open(job.estate?.googleMapsURL, "_blank");
                } else {
                  alert("No hay Google Maps disponible.");
                }
              }}
              title="Ver en Google Maps"
            >
              <FontAwesomeIcon
                icon={faMapMarkerAlt}
                color={job.estate?.googleMapsURL ? "#697588" : "lightgray"}
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

  function navigateToJob(job) {
    navigate("/job-abm", { state: { job, editMode: true } });
  }

  const navigateToAlbum = (job) => {
    if (job.listPhotosURL && job.listPhotosURL.length > 0) {
      navigate("/job-view", { state: { job } });
    }
  };

  const formatToDollars = (number) => {
    return new Intl.NumberFormat("en-US", {
      style: "currency",
      currency: "USD",
      minimumFractionDigits: 0,
      maximumFractionDigits: 0,
    }).format(number);
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
              ? `Panel de obras de propiedad: ${estate.name} (Dir: ${estate.address})`
              : "Panel de obras (todas)"}
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
                      onClick={() => requestSort("estateName")}
                    >
                      Casa (dir)
                    </th>
                    <th
                      className="table-header"
                      onClick={() => requestSort("name")}
                    >
                      Obra
                    </th>
                    <th
                      className="table-header"
                      onClick={() => requestSort("labourCost")}
                    >
                      $Costo
                    </th>
                    <th
                      className="table-header"
                      onClick={() => requestSort("listPhotosURL")}
                    >
                      #Fotos
                    </th>
                    <th
                      className="table-header"
                      onClick={() => requestSort("phone1")}
                    >
                      Trabajadores
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
                <tbody>{renderJobRows()}</tbody>
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

export default JobMenu;
