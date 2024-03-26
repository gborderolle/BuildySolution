import React, { useState, useEffect, useMemo } from "react";
import {
  CCard,
  CCardBody,
  CCardHeader,
  CRow,
  CTable,
  CTableHead,
  CTableRow,
  CTableHeaderCell,
  CTableBody,
  CTableDataCell,
  CPagination,
  CPaginationItem,
  CFormInput,
} from "@coreui/react";

// redux imports
import { useSelector, useDispatch } from "react-redux";
import { fetchLogsList } from "../../../store/generalData-actions";

import "./LogsTable.css";

const LogsTable = () => {
  //#region Consts ***********************************

  const [logs, setLogs] = useState([]); // Esto debería ser reemplazado por tu lógica de obtención de datos
  const [currentPage, setCurrentPage] = useState(1);
  const [itemsPerPage] = useState(10);

  const [pageCount, setPageCount] = useState(0); // Añade esto
  const [searchTerm, setSearchTerm] = useState("");

  // redux
  const dispatch = useDispatch();

  // Redux
  const logsList = useSelector((state) => state.generalData.logsList);

  const [sortConfig, setSortConfig] = useState({
    key: "creation",
    direction: "descending",
  });

  const filteredLogsList = logsList.filter((log) => {
    const match1 = log.entity
      ? log.entity.toLowerCase().includes(searchTerm.toLowerCase())
      : false;
    const match2 = log.action
      ? log.action.toLowerCase().includes(searchTerm.toLowerCase())
      : false;
    const match3 = log.data
      ? log.data.toLowerCase().includes(searchTerm.toLowerCase())
      : false;
    const match4 = log.username
      ? log.username.toLowerCase().includes(searchTerm.toLowerCase())
      : false;

    return match1 || match2 || match3 || match4;
  });

  useEffect(() => {
    dispatch(fetchLogsList());
  }, [dispatch]);

  useEffect(() => {
    setPageCount(Math.ceil(filteredLogsList.length / itemsPerPage));
  }, [filteredLogsList, itemsPerPage]);

  // Función para verificar si la fecha del log es la actual
  const isToday = (logDate) => {
    const today = new Date();
    const logCreationDate = new Date(logDate);
    return logCreationDate.setHours(0, 0, 0, 0) === today.setHours(0, 0, 0, 0);
  };

  //#endregion Consts ***********************************

  //#region Hooks ***********************************

  const sortedList = useMemo(() => {
    let sortableList = [...filteredLogsList];
    if (sortConfig.key !== null) {
      sortableList.sort((a, b) => {
        // Ordenamiento estándar para otras propiedades
        if (a[sortConfig.key] < b[sortConfig.key]) {
          return sortConfig.direction === "ascending" ? -1 : 1;
        }
        if (a[sortConfig.key] > b[sortConfig.key]) {
          return sortConfig.direction === "ascending" ? 1 : -1;
        }
        return 0;
      });
    }
    return sortableList;
  }, [filteredLogsList, sortConfig]);

  const indexOfLastLog = currentPage * itemsPerPage;
  const indexOfFirstLog = indexOfLastLog - itemsPerPage;
  const currentLogs = sortedList.slice(indexOfFirstLog, indexOfLastLog);

  //#endregion Hooks ***********************************

  //#region Functions ***********************************

  const requestSort = (key) => {
    let direction = "ascending";
    if (sortConfig.key === key && sortConfig.direction === "ascending") {
      direction = "descending";
    }
    setSortConfig({ key, direction });
  };

  const formatUruguayDate = (dateString) => {
    const date = new Date(dateString);
    return new Intl.DateTimeFormat("es-UY", {
      year: "numeric",
      month: "long",
      day: "numeric",
      hour: "2-digit",
      minute: "2-digit",
      second: "2-digit",
      timeZone: "America/Montevideo",
    }).format(date);
  };

  //#endregion Functions ***********************************

  //#region Events ***********************************

  const handleSearchChange = (event) => {
    setSearchTerm(event.target.value);
  };

  const handlePageChange = (pageNumber) => {
    setCurrentPage(pageNumber);
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
            Tabla de logs
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
            <div className="custom-table-responsive">
              <CTable striped>
                <CTableHead>
                  <CTableRow>
                    <CTableHeaderCell>#</CTableHeaderCell>
                    <CTableHeaderCell onClick={() => requestSort("entity")}>
                      Entidad
                    </CTableHeaderCell>
                    <CTableHeaderCell onClick={() => requestSort("action")}>
                      Acción
                    </CTableHeaderCell>
                    <CTableHeaderCell onClick={() => requestSort("data")}>
                      Datos
                    </CTableHeaderCell>
                    <CTableHeaderCell onClick={() => requestSort("username")}>
                      Usuario
                    </CTableHeaderCell>
                    <CTableHeaderCell onClick={() => requestSort("creation")}>
                      Creación
                    </CTableHeaderCell>
                  </CTableRow>
                </CTableHead>
                <CTableBody>
                  {currentLogs.map((log, index) => {
                    const isLogToday = isToday(log.creation);
                    const rowClass = isLogToday ? "log-text-warning" : "";

                    return (
                      <CTableRow key={log.id}>
                        <CTableDataCell className={rowClass}>
                          {indexOfFirstLog + index + 1}
                        </CTableDataCell>
                        <CTableDataCell className={rowClass}>
                          {log.entity}
                        </CTableDataCell>
                        <CTableDataCell className={rowClass}>
                          {log.action}
                        </CTableDataCell>
                        <CTableDataCell className={rowClass}>
                          {log.username}
                        </CTableDataCell>
                        <CTableDataCell className={rowClass}>
                          {log.data}
                        </CTableDataCell>
                        <CTableDataCell className={rowClass}>
                          {formatUruguayDate(log.creation)}
                        </CTableDataCell>
                      </CTableRow>
                    );
                  })}
                </CTableBody>
              </CTable>
            </div>
          </CRow>
          <CPagination align="center">
            {Array.from({ length: pageCount }, (_, i) => i + 1).map((page) => (
              <CPaginationItem
                key={page}
                active={page === currentPage}
                onClick={() => handlePageChange(page)}
              >
                {page}
              </CPaginationItem>
            ))}
          </CPagination>
        </CCardBody>
      </CCard>
    </>
  );
};

export default LogsTable;
