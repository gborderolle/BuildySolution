import React, { useState, useEffect, useMemo } from "react";
import { useNavigate } from "react-router-dom";
import {
  CCard,
  CCardBody,
  CCardHeader,
  CRow,
  CFormInput,
  CTable,
  CPagination,
  CPaginationItem,
  CPopover,
  CModal,
  CModalBody,
  CModalHeader,
  CModalTitle,
  CModalFooter,
  CButton,
  CDropdown,
  CDropdownToggle,
  CDropdownMenu,
  CDropdownItem,
} from "@coreui/react";

import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faPrint,
  faPlus,
  faRefresh,
  faEye,
  faPaintRoller,
  faNewspaper,
  faMoneyBillWave,
  faInfoCircle,
  faTriangleExclamation,
  faMapMarkerAlt,
} from "@fortawesome/free-solid-svg-icons";

// redux imports
import { useDispatch, useSelector, batch } from "react-redux";
import { authActions } from "../../../store/auth-slice";
import {
  fetchEstateList,
  fetchRentList,
  fetchTenantList,
} from "../../../store/generalData-actions";

import { jsPDF } from "jspdf";
import autoTable from "jspdf-autotable";
import * as XLSX from "xlsx";
import { saveAs } from "file-saver";

import "./EstateMenu.css";

const EstateMenu = () => {
  //#region Consts ***********************************

  const [searchTerm, setSearchTerm] = useState("");
  const [selectedEstate, setSelectedEstate] = useState(null);
  const [showModal, setShowModal] = useState(false);
  const [selectedOwner, setSelectedOwner] = useState("");

  const dispatch = useDispatch();

  // redux gets
  const [estateList, setEstateList] = useState([]);
  const reduxEstateList =
    useSelector((state) => state.generalData.estateList) || [];

  useEffect(() => {
    let sortedList = [...reduxEstateList];
    sortedList.sort((a, b) => {
      if (a.name.toLowerCase() < b.name.toLowerCase()) {
        return -1;
      }
      if (a.name.toLowerCase() > b.name.toLowerCase()) {
        return 1;
      }
      return 0;
    });
    setEstateList(sortedList);
  }, [reduxEstateList]);

  const itemsPerPage = 25;
  const [currentPage, setCurrentPage] = useState(1);
  const [pageCount, setPageCount] = useState(0);

  const [sortConfig, setSortConfig] = useState({
    key: "address",
    direction: "ascending",
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

  const isMobile = useSelector((state) => state.auth.isMobile);

  //#endregion Consts ***********************************

  //#region Hooks ***********************************

  const uniqueOwners = useMemo(() => {
    // Extrae los propietarios y elimina duplicados
    const ownersMap = new Map(
      estateList
        .filter((estate) => estate.owner) // Filtra propiedades que tienen propietario definido
        .map((estate) => [estate.owner.id, estate.owner.name]) // Crea pares de [id, nombre]
    );

    // Convierte el Map en un array de objetos con id y name, y ordena por id
    const ownersArray = Array.from(ownersMap, ([id, name]) => ({
      id,
      name,
    })).sort((a, b) => a.id - b.id);

    // Añade la opción "Todos" al inicio
    return [{ id: "", name: "Todos" }, ...ownersArray];
  }, [estateList]);

  // Scroll to top of the page on startup
  useEffect(() => {
    window.scrollTo(0, 0);
  }, []);

  const filteredEstateList = useMemo(() => {
    return estateList.filter((estate) => {
      // Filtra por propietario si se seleccionó uno, omitiendo si se seleccionó "Todos" o si no se seleccionó nada.
      const matchesOwner =
        selectedOwner === "" || estate.owner?.id.toString() === selectedOwner;

      // Filtra por término de búsqueda si se ha ingresado alguno.
      const matchesSearchTerm =
        searchTerm.trim() === "" ||
        estate.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
        estate.address.toLowerCase().includes(searchTerm.toLowerCase()) ||
        (estate.comments &&
          estate.comments.toLowerCase().includes(searchTerm.toLowerCase()));

      // Verificar si el nombre del inquilino coincide con el término de búsqueda
      const lastRent =
        estate.listRents && estate.listRents.length > 0
          ? estate.listRents[estate.listRents.length - 1]
          : null;
      const tenantName =
        lastRent && lastRent.listTenants && lastRent.listTenants.length > 0
          ? lastRent.listTenants[0].name
          : null;
      const matchesTenantName = tenantName
        ? tenantName.toLowerCase().includes(searchTerm.toLowerCase())
        : false;

      const matchesMonthlyValue =
        lastRent && lastRent.monthlyValue
          ? lastRent.monthlyValue
              .toString()
              .toLowerCase()
              .includes(searchTerm.toLowerCase())
          : false;

      // Combina todos los criterios de coincidencia en una única verificación
      return (
        matchesOwner &&
        (matchesSearchTerm || matchesTenantName || matchesMonthlyValue)
      );
    });
  }, [estateList, selectedOwner, searchTerm]);

  useEffect(() => {
    setPageCount(Math.ceil(filteredEstateList.length / itemsPerPage));
  }, [filteredEstateList, itemsPerPage]);

  const handleSearchChange = (event) => {
    setSearchTerm(event.target.value);
  };

  const handlePageChange = (pageNumber) => {
    setCurrentPage(pageNumber);
  };

  const addHandler = () => {
    dispatch(fetchEstateList());

    setTimeout(() => {
      navigate("/estate-abm");
    }, 200); // Asegúrate de que este tiempo coincida o sea ligeramente mayor que la duración de tu animación
  };

  const printHandler = () => {
    setShowModal(true);
  };

  const handleClose = () => {
    setShowModal(false);
  };

  const updateHandler = () => {
    const fetchGeneralData = async () => {
      batch(() => {
        dispatch(fetchEstateList());
        dispatch(fetchRentList());
        dispatch(fetchTenantList());
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
    let sortableList = [...filteredEstateList];
    if (sortConfig.key !== null) {
      sortableList.sort((a, b) => {
        // Propiedades anidadas
        if (sortConfig.key === "monthlyValue") {
          const lastRentA =
            a.listRents?.[a.listRents.length - 1]?.monthlyValue || 0;
          const lastRentB =
            b.listRents?.[b.listRents.length - 1]?.monthlyValue || 0;

          if (lastRentA < lastRentB) {
            return sortConfig.direction === "ascending" ? -1 : 1;
          }
          if (lastRentA > lastRentB) {
            return sortConfig.direction === "ascending" ? 1 : -1;
          }
          return 0;
        } else if (sortConfig.key === "rentComments") {
          const lastRentA =
            a.listRents?.[a.listRents.length - 1]?.comments || "";
          const lastRentB =
            b.listRents?.[b.listRents.length - 1]?.comments || "";

          if (lastRentA.toLowerCase() < lastRentB.toLowerCase()) {
            return sortConfig.direction === "ascending" ? -1 : 1;
          }
          if (lastRentA.toLowerCase() > lastRentB.toLowerCase()) {
            return sortConfig.direction === "ascending" ? 1 : -1;
          }
          return 0;
        } else if (sortConfig.key === "tenant") {
          // Obtiene el nombre del primer inquilino del último alquiler
          const tenantNameA =
            a.listRents?.[a.listRents.length - 1]?.listTenants?.[0]?.name || "";
          const tenantNameB =
            b.listRents?.[b.listRents.length - 1]?.listTenants?.[0]?.name || "";

          if (tenantNameA.toLowerCase() < tenantNameB.toLowerCase()) {
            return sortConfig.direction === "ascending" ? -1 : 1;
          }
          if (tenantNameA.toLowerCase() > tenantNameB.toLowerCase()) {
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
  }, [filteredEstateList, sortConfig]);

  const totalMonthlyRent = useMemo(() => {
    return filteredEstateList.reduce((total, estate) => {
      const lastRent =
        estate.listRents && estate.listRents[estate.listRents.length - 1];
      return total + (lastRent ? lastRent.monthlyValue : 0);
    }, 0);
  }, [filteredEstateList]);

  //#endregion Hooks ***********************************

  //#region Functions ***********************************

  const renderEstateRows = () => {
    const indexOfLastItem = currentPage * itemsPerPage;
    const indexOfFirstItem = indexOfLastItem - itemsPerPage;
    const currentEstates = sortedList.slice(indexOfFirstItem, indexOfLastItem);

    return currentEstates.map((estate, index) => (
      <tr key={estate.id}>
        <td
          style={estate.id === selectedEstate?.id ? { color: "blue" } : null}
          onClick={() => handleSelectEstate(estate)}
        >
          {" "}
          <button
            style={{
              border: "none",
              background: "none",
              padding: 0,
              color: "blue",
              textDecoration: "underline",
              cursor: "pointer",
            }}
            onClick={() => handleSelectEstate(estate)}
          >
            {index + 1}
          </button>
        </td>
        <td
          style={estate.id === selectedEstate?.id ? { color: "blue" } : null}
          onClick={() => handleSelectEstate(estate)}
        >
          {estate.listRents &&
          estate.listRents.length > 0 &&
          estate.listRents[estate.listRents.length - 1].listTenants &&
          estate.listRents[estate.listRents.length - 1].listTenants.length >
            0 ? (
            estate.name
          ) : (
            <>
              <FontAwesomeIcon icon={faTriangleExclamation} color="gold" />{" "}
              {estate.name}
            </>
          )}
        </td>
        <td
          style={estate.id === selectedEstate?.id ? { color: "blue" } : null}
          onClick={() => handleSelectEstate(estate)}
        >
          {estate.address}
        </td>
        <td
          style={estate.id === selectedEstate?.id ? { color: "blue" } : null}
          onClick={() => handleSelectEstate(estate)}
        >
          {estate.listRents &&
          estate.listRents.length > 0 &&
          estate.listRents[estate.listRents.length - 1].listTenants &&
          estate.listRents[estate.listRents.length - 1].listTenants.length > 0
            ? estate.listRents[estate.listRents.length - 1].listTenants[0].name
            : ""}
        </td>
        <td
          style={estate.id === selectedEstate?.id ? { color: "blue" } : null}
          onClick={() => handleSelectEstate(estate)}
        >
          {estate.presentRentId > 0 && estate.listRents.length > 0
            ? formatToDollars(
                estate.listRents?.[estate.listRents.length - 1].monthlyValue
              )
            : "-"}
        </td>
        <td
          style={estate.id === selectedEstate?.id ? { color: "blue" } : null}
          onClick={() => handleSelectEstate(estate)}
        >
          {estate.presentRentId > 0 && estate.listRents.length > 0
            ? estate.listRents?.[estate.listRents.length - 1].comments
            : "-"}
        </td>
        <td
          style={estate.id === selectedEstate?.id ? { color: "blue" } : null}
          onClick={() => handleSelectEstate(estate)}
        >
          {estate.comments}
        </td>
        <td style={estate.id === selectedEstate?.id ? { color: "blue" } : null}>
          <button
            onClick={() => navigateToEstate(estate)}
            style={{ border: "none", background: "none" }}
            title="Ver propiedad"
          >
            <FontAwesomeIcon icon={faEye} color="#697588" />
          </button>
          <button
            onClick={() => navigateToRent(estate)}
            style={{ border: "none", background: "none" }}
            title={
              estate.presentRentId > 0 ? "Ver alquiler" : "Agregar alquiler"
            }
          >
            <FontAwesomeIcon
              icon={faMoneyBillWave}
              color={estate.presentRentId > 0 ? "#697588" : "lightgray"}
            />
          </button>
          <button
            onClick={() => navigateToJobs(estate)}
            style={{ border: "none", background: "none" }}
            title="Ver obras"
          >
            <FontAwesomeIcon
              icon={faPaintRoller}
              color={
                estate.listJobs && estate.listJobs.length > 0
                  ? "#697588"
                  : "lightgray"
              }
            />
          </button>
          <button
            onClick={() => navigateToReports(estate)}
            style={{ border: "none", background: "none" }}
            title="Ver reportes"
          >
            <FontAwesomeIcon
              icon={faNewspaper}
              color={
                estate.listReports && estate.listReports.length > 0
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
              if (estate.googleMapsURL) {
                window.open(estate.googleMapsURL, "_blank");
              } else {
                alert("No hay Google Maps disponible.");
              }
            }}
            title="Ver en Google Maps"
          >
            <FontAwesomeIcon
              icon={faMapMarkerAlt}
              color={estate.googleMapsURL ? "#697588" : "lightgray"}
            />
          </button>
          <CPopover
            content={estate.owner?.name || "N/A"}
            placement="top"
            trigger={["hover", "focus"]}
          >
            <button style={{ border: "none", background: "none" }}>
              <FontAwesomeIcon
                icon={faInfoCircle}
                color={estate.owner?.color || "lightgray"}
              />
            </button>
          </CPopover>
        </td>
      </tr>
    ));
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

  function navigateToEstate(estate) {
    navigate("/estate-abm", { state: { estate, editMode: true } });
  }

  function navigateToJobs(estate) {
    navigate("/jobs", { state: { estate, listMode: true } });
  }

  function navigateToReports(estate) {
    navigate("/reports", { state: { estate, listMode: true } });
  }

  function navigateToRent(estate) {
    if (estate.presentRentId > 0) {
      navigate("/rent-abm", { state: { estate, editMode: true } });
    } else {
      navigate("/rent-abm", { state: { estate } });
    }
  }

  const formatToDollars = (number) => {
    return new Intl.NumberFormat("en-US", {
      style: "currency",
      currency: "USD",
      minimumFractionDigits: 0,
      maximumFractionDigits: 0,
    }).format(number);
  };

  const formatDate = () => {
    const d = new Date();
    const year = d.getFullYear();
    const month = `${d.getMonth() + 1}`.padStart(2, "0"); // Los meses comienzan desde 0
    const day = `${d.getDate()}`.padStart(2, "0");
    return `${year}${month}${day}`;
  };

  // Función para exportar a PDF
  const exportPDF = () => {
    const doc = new jsPDF();

    // Suponiendo que 'estateList' es tu lista de objetos a exportar
    const tableColumn = [
      "ID",
      "Nombre",
      "Dirección",
      "Inquilino",
      "Mensualidad",
      "Dueño",
      "Comentarios",
    ]; // Asegúrate de ajustar estos encabezados
    const tableRows = estateList.map((estate) => [
      estate.id,
      estate.name,
      estate.address,
      estate.tenantName,
      estate.monthlyValue,
      estate.owner?.name || "N/A",
      estate.comments,
    ]);

    autoTable(doc, {
      head: [tableColumn],
      body: tableRows,
      startY: 20,
    });

    const dateStr = formatDate(); // Utiliza la función formatDate que proporcioné anteriormente
    doc.save(`buildy_${dateStr}.pdf`);
  };

  // Función para exportar a CSV
  const exportCSV = () => {
    const header = [
      "ID,Nombre,Dirección,Inquilino,Mensualidad,Dueño,Comentarios",
    ]; // Ajusta estos encabezados según sea necesario
    const rows = estateList.map((estate) =>
      [
        estate.id,
        estate.name,
        estate.address,
        estate.tenantName,
        estate.monthlyValue,
        estate.owner?.name || "N/A",
        estate.comments,
        // Excluyendo la columna "Opciones"
      ].join(",")
    );

    const csvContent = [header, ...rows].join("\n");
    const blob = new Blob([csvContent], { type: "text/csv;charset=utf-8;" });
    const dateStr = formatDate(); // Utiliza la función formatDate que proporcioné anteriormente
    saveAs(blob, `buildy_${dateStr}.csv`);
  };

  // Función para exportar a XLSX
  const exportXLSX = () => {
    // Crear un nuevo libro de trabajo
    const wb = XLSX.utils.book_new();

    // Convertir datos a hoja de trabajo (asumiendo que estateList es tu lista de datos)
    const dataForExport = estateList.map(
      ({ id, name, address, tenantName, monthlyValue, owner, comments }) => ({
        ID: id,
        Nombre: name,
        Dirección: address,
        Inquilino: tenantName,
        Mensualidad: monthlyValue,
        Dueño: owner?.name || "N/A",
        Comentarios: comments,
      })
    );

    const ws = XLSX.utils.json_to_sheet(dataForExport);

    // Añadir la hoja de trabajo al libro con un nombre específico
    XLSX.utils.book_append_sheet(wb, ws, "Propiedades");

    // Generar un nombre de archivo con la fecha actual
    const dateStr = formatDate(); // Utiliza la función formatDate
    XLSX.writeFile(wb, `buildy_${dateStr}.xlsx`);
  };

  //#endregion Functions ***********************************

  //#region Events ***********************************

  const handleSelectEstate = (estate) => {
    setSelectedEstate(estate);
  };

  //#endregion Events ***********************************

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
            {!isMobile && "Panel de propiedades"}
            <div
              style={{
                display: "flex",
                alignItems: "center",
              }}
            >
              <CDropdown className="mb-3">
                <CDropdownToggle color="secondary">
                  {uniqueOwners.find((o) => o.id.toString() === selectedOwner)
                    ?.name || "Propietario"}
                </CDropdownToggle>
                <CDropdownMenu>
                  {uniqueOwners.map((owner) => (
                    <CDropdownItem
                      key={owner.id}
                      onClick={() => setSelectedOwner(owner.id.toString())} // Guarda el id del propietario seleccionado
                    >
                      {owner.name}
                    </CDropdownItem>
                  ))}
                </CDropdownMenu>
              </CDropdown>
            </div>
            <div style={{ display: "flex", justifyContent: "flex-end" }}>
              <button
                onClick={printHandler}
                style={{ border: "none", background: "none", margin: "2px" }}
              >
                <FontAwesomeIcon icon={faPrint} color="#697588" />
              </button>
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
              <CTable striped id="dataTable">
                <thead>
                  <tr>
                    <th>#</th>
                    <th
                      className="table-header"
                      onClick={() => requestSort("name")}
                    >
                      Nombre
                    </th>
                    <th
                      className="table-header"
                      onClick={() => requestSort("address")}
                    >
                      Dirección
                    </th>
                    <th
                      className="table-header"
                      onClick={() => requestSort("tenant")}
                    >
                      Inquilino
                    </th>
                    <th
                      className="table-header"
                      onClick={() => requestSort("monthlyValue")}
                    >
                      $Renta
                    </th>
                    <th
                      className="table-header"
                      onClick={() => requestSort("rentComments")}
                    >
                      Forma
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
                <tbody>{renderEstateRows()}</tbody>
                <tfoot>
                  <tr>
                    <td
                      colSpan="7"
                      style={{ textAlign: "right", fontWeight: "bold" }}
                    >
                      Total Mensualidad:
                    </td>
                    <td style={{ fontWeight: "bold" }}>
                      {formatToDollars(totalMonthlyRent)}
                    </td>
                  </tr>
                </tfoot>
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

      <CModal visible={showModal} onClose={handleClose}>
        <CModalHeader>
          <CModalTitle>Descargar datos</CModalTitle>
        </CModalHeader>
        <CModalBody>
          <p>Elige el formato para descargar la tabla:</p>
          <CButton color="primary" onClick={exportPDF} className="me-2">
            PDF
          </CButton>
          <CButton color="success" onClick={exportCSV} className="me-2">
            CSV
          </CButton>
          <CButton color="warning" onClick={exportXLSX}>
            XLSX
          </CButton>
        </CModalBody>
        <CModalFooter>
          <CButton color="secondary" onClick={handleClose}>
            Cerrar
          </CButton>
        </CModalFooter>
      </CModal>
    </>
  );
};

export default EstateMenu;
