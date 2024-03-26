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
} from "@coreui/react";

import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faPlus,
  faRefresh,
  faEye,
  faPaperPlane,
} from "@fortawesome/free-solid-svg-icons";

// redux imports
import { useDispatch, useSelector, batch } from "react-redux";
import { authActions } from "../../../store/auth-slice";
import {
  fetchTenantList,
  fetchEstateList,
} from "../../../store/generalData-actions";

import "./TenantMenu.css";

const TenantMenu = () => {
  //#region Consts ***********************************

  const [searchTerm, setSearchTerm] = useState("");
  const [selectedTenant, setSelectedTenant] = useState(null);

  const dispatch = useDispatch();

  const [tenantList, setTenantList] = useState([]);
  const reduxTenantList =
    useSelector((state) => state.generalData.tenantList) || [];

  const [rentList, setRentList] = useState([]);
  const reduxRentList =
    useSelector((state) => state.generalData.rentList) || [];

  const [estateList, setEstateList] = useState([]);
  const reduxEstateList =
    useSelector((state) => state.generalData.estateList) || [];

  useEffect(() => {
    let sortedList = [...reduxTenantList];
    sortedList.sort((a, b) => {
      if (a.name.toLowerCase() < b.name.toLowerCase()) {
        return -1;
      }
      if (a.name.toLowerCase() > b.name.toLowerCase()) {
        return 1;
      }
      return 0;
    });
    setTenantList(sortedList);
  }, [reduxTenantList]);

  useEffect(() => {
    setRentList(reduxRentList);
  }, [reduxRentList]);

  useEffect(() => {
    setEstateList(reduxEstateList);
  }, [reduxEstateList]);

  const itemsPerPage = 25;
  const [currentPage, setCurrentPage] = useState(1);
  const [pageCount, setPageCount] = useState(0);

  const [sortConfig, setSortConfig] = useState({
    key: null,
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

  const handleSelectTenant = (tenant) => {
    setSelectedTenant(tenant);
  };

  //#endregion Consts ***********************************

  //#region Hooks ***********************************

  // Scroll to top of the page on startup
  useEffect(() => {
    window.scrollTo(0, 0);
  }, []);

  const filteredTenantList = tenantList.filter((tenant) => {
    const match1 = tenant.name
      ? tenant.name.toLowerCase().includes(searchTerm.toLowerCase())
      : false;
    const match2 = tenant.phone1
      ? tenant.phone1.toLowerCase().includes(searchTerm.toLowerCase())
      : false;
    const match3 = tenant.email
      ? tenant.email.toLowerCase().includes(searchTerm.toLowerCase())
      : false;
    const match5 = tenant.comments
      ? tenant.comments.toLowerCase().includes(searchTerm.toLowerCase())
      : false;
    const match6 = tenant.phone2
      ? tenant.phone2.toLowerCase().includes(searchTerm.toLowerCase())
      : false;
    return match1 || match2 || match3 || match4 || match5 || match6;
  });

  useEffect(() => {
    setPageCount(Math.ceil(filteredTenantList.length / itemsPerPage));
  }, [filteredTenantList, itemsPerPage]);

  const handleSearchChange = (event) => {
    setSearchTerm(event.target.value);
  };

  const handlePageChange = (pageNumber) => {
    setCurrentPage(pageNumber);
  };

  const addHandler = () => {
    dispatch(fetchTenantList());

    setTimeout(() => {
      navigate("/tenant-abm");
    }, 200); // Asegúrate de que este tiempo coincida o sea ligeramente mayor que la duración de tu animación
  };

  const updateHandler = () => {
    const fetchGeneralData = async () => {
      batch(() => {
        dispatch(fetchTenantList());
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
    let sortableList = [...filteredTenantList];
    if (sortConfig.key !== null) {
      sortableList.sort((a, b) => {
        if (sortConfig.key === "tenant") {
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
  }, [filteredTenantList, sortConfig]);

  //#endregion Hooks ***********************************

  //#region Functions ***********************************

  const renderTenantRows = () => {
    const indexOfLastItem = currentPage * itemsPerPage;
    const indexOfFirstItem = indexOfLastItem - itemsPerPage;
    const currentTenants = sortedList.slice(indexOfFirstItem, indexOfLastItem);

    return currentTenants.map((tenant, index) => {
      const rent = getRentByTenantId(tenant.id);
      const estate = rent ? getEstateByRentId(rent.id) : null;

      return (
        <tr key={tenant.id}>
          <td
            style={tenant.id === selectedTenant?.id ? { color: "blue" } : null}
            onClick={() => handleSelectTenant(tenant)}
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
              onClick={() => handleSelectTenant(tenant)}
            >
              {index + 1}
            </button>
          </td>
          <td
            style={tenant.id === selectedTenant?.id ? { color: "blue" } : null}
            onClick={() => handleSelectTenant(tenant)}
          >
            {tenant.name}
          </td>
          <td
            style={tenant.id === selectedTenant?.id ? { color: "blue" } : null}
            onClick={() => handleSelectTenant(tenant)}
          >
            {tenant.phone1 ? formatPhoneNumber(tenant.phone1) : ""}
          </td>
          <td
            style={tenant.id === selectedTenant?.id ? { color: "blue" } : null}
            onClick={() => handleSelectTenant(tenant)}
          >
            {tenant.phone2 ? formatPhoneNumber(tenant.phone2) : ""}
          </td>
          <td
            style={tenant.id === selectedTenant?.id ? { color: "blue" } : null}
            onClick={() => handleSelectTenant(tenant)}
          >
            {tenant.email}
          </td>
          <td
            style={tenant.id === selectedTenant?.id ? { color: "blue" } : null}
            onClick={() => handleSelectTenant(tenant)}
          >
            {estate ? estate.name + " (" + estate.address + ")" : "No tiene"}
          </td>
          <td
            style={tenant.id === selectedTenant?.id ? { color: "blue" } : null}
            onClick={() => handleSelectTenant(tenant)}
          >
            {tenant.comments}
          </td>
          <td>
            <button
              onClick={() => navigateToTenant(tenant)}
              style={{ border: "none", background: "none" }}
              title="Ver detalles"
            >
              <FontAwesomeIcon icon={faEye} color="#697588" />
            </button>
            <button
              onClick={() => sendWhatsApp(tenant.phone1)}
              style={{ border: "none", background: "none" }}
              title="Contactar por WhatsApp"
            >
              <FontAwesomeIcon icon={faPaperPlane} color="#697588" />
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

  function navigateToTenant(tenant) {
    navigate("/tenant-abm", { state: { tenant, editMode: true } });
  }

  const sendWhatsApp = (phone) => {
    let whatsappLink;
    let phoneString = String(phone);

    // Si el número tiene 9 dígitos, quita el primer dígito y agrega el prefijo
    if (phoneString.length === 9) {
      whatsappLink = `https://api.whatsapp.com/send/?phone=598${phoneString.substring(
        1
      )}`;
    }
    // Si el número tiene 8 dígitos, se usa como viene
    else if (phoneString.length === 8) {
      whatsappLink = `https://api.whatsapp.com/send/?phone=598${phoneString}`;
    }
    // En caso de que el número no tenga 8 o 9 dígitos, muestra un mensaje de error
    else {
      return;
    }

    // Abrir el enlace de WhatsApp
    window.open(whatsappLink, "_blank");
  };

  const getRentByTenantId = (tenantId) => {
    // Asume que cada tenant tiene un rentId único
    return rentList.find((rent) =>
      rent.listTenants.some((tenant) => tenant.id === tenantId)
    );
  };

  const getEstateByRentId = (rentId) => {
    // Asume que cada rent tiene un estateId único
    return estateList.find((estate) => estate.presentRentId === rentId);
  };

  const formatPhoneNumber = (phoneNumber) => {
    const cleaned = ("" + phoneNumber).replace(/\D/g, "");
    const match = cleaned.match(/^(\d{3})(\d{3})(\d{3})$/);

    if (match) {
      return `${match[1]} ${match[2]} ${match[3]}`;
    }

    return null;
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
            Panel de inquilinos
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
                      onClick={() => requestSort("name")}
                    >
                      Nombre
                    </th>
                    <th
                      className="table-header"
                      onClick={() => requestSort("phone1")}
                    >
                      Celular 1
                    </th>
                    <th
                      className="table-header"
                      onClick={() => requestSort("phone2")}
                    >
                      Celular 2
                    </th>
                    <th
                      className="table-header"
                      onClick={() => requestSort("email")}
                    >
                      Email
                    </th>
                    <th
                      className="table-header"
                      onClick={() => requestSort("address")}
                    >
                      Casa (dir)
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
                <tbody>{renderTenantRows()}</tbody>
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

export default TenantMenu;
