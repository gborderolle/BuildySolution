import React, { useState, useEffect, useMemo } from "react";
import {
  CSpinner,
  CRow,
  CTable,
  CTableHead,
  CTableRow,
  CTableHeaderCell,
  CTableBody,
  CTableDataCell,
  CButton,
  CModal,
  CModalHeader,
  CModalTitle,
  CModalBody,
  CForm,
  CFormInput,
  CModalFooter,
  CInputGroup,
  CInputGroupText,
  CDropdown,
  CDropdownToggle,
  CDropdownMenu,
  CDropdownItem,
  CCard,
  CCardBody,
  CCardFooter,
  CAlert,
  CPagination,
  CPaginationItem,
} from "@coreui/react";
import useInput from "../../../hooks/use-input";
import useAPI from "../../../hooks/use-API";

// redux imports
import { useSelector, useDispatch } from "react-redux";
import {
  fetchProvinceList,
  fetchCountryList,
} from "../../../store/generalData-actions";
import { urlProvince } from "../../../endpoints";

const ProvinceTable = (props) => {
  //#region Consts ***********************************

  const [isValidForm, setIsValidForm] = useState(true);
  const {
    isLoading,
    isSuccess,
    error: errorAPI,
    uploadData,
    removeData,
  } = useAPI();

  const [isModalVisible, setIsModalVisible] = useState(false);
  const [currentProvince, setCurrentProvince] = useState(null);

  const [isDeleteModalVisible, setIsDeleteModalVisible] = useState(false);
  const [idToDelete, setIdToDelete] = useState(null);

  const [ddlSelectedCountry, setDdlSelectedCountry] = useState(null);
  const [inputHasErrorCountry, setInputHasErrorCountry] = useState(false);

  // redux
  const dispatch = useDispatch();

  // Redux
  const provinceList = useSelector((state) => state.generalData.provinceList);
  const countryList = useSelector((state) => state.generalData.countryList);

  const [sortConfig, setSortConfig] = useState({
    key: null,
    direction: "ascending",
  });

  const itemsPerPage = 25;
  const [currentPage, setCurrentPage] = useState(1);
  const [pageCount, setPageCount] = useState(0);

  useEffect(() => {
    dispatch(fetchProvinceList());
    dispatch(fetchCountryList());
  }, [dispatch]);

  useEffect(() => {
    setPageCount(Math.ceil(provinceList.length / itemsPerPage));
  }, [provinceList, itemsPerPage]);

  const {
    value: provinceName,
    isValid: inputIsValid1,
    hasError: inputHasError1,
    valueChangeHandler: inputChangeHandler1,
    inputBlurHandler: inputBlurHandler1,
    reset: inputReset1,
  } = useInput((value) => value.trim() !== "");

  const {
    value: nominatimCode,
    isValid: inputIsValid2,
    hasError: inputHasError2,
    valueChangeHandler: inputChangeHandler2,
    inputBlurHandler: inputBlurHandler2,
    reset: inputReset2,
  } = useInput((value) => true);

  //#endregion Consts ***********************************

  //#region Hooks ***********************************

  const combinedProvinceList = useMemo(() => {
    return provinceList.map((province) => {
      const country = countryList.find((c) => c.id === province.countryDSId);
      return {
        ...province,
        countryName: country ? country.name : "No definido",
      };
    });
  }, [provinceList, countryList]);

  // Función de ordenamiento adaptada
  const sortedList = useMemo(() => {
    let sortableList = [...combinedProvinceList];
    if (sortConfig.key !== null) {
      sortableList.sort((a, b) => {
        if (sortConfig.key === "countryName") {
          const countryA = a.countryName.toLowerCase();
          const countryB = b.countryName.toLowerCase();
          if (countryA < countryB) {
            return sortConfig.direction === "ascending" ? -1 : 1;
          }
          if (countryA > countryB) {
            return sortConfig.direction === "ascending" ? 1 : -1;
          }
          return 0;
        } else {
          // Ordenamiento estándar para otras propiedades
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
  }, [combinedProvinceList, sortConfig]);

  //#endregion Hooks ***********************************

  //#region Functions ***********************************

  const requestSort = (key) => {
    let direction = "ascending";
    if (sortConfig.key === key && sortConfig.direction === "ascending") {
      direction = "descending";
    }
    setSortConfig({ key, direction });
  };

  const openModal = (province = null) => {
    setCurrentProvince(province);
    if (province && province.name && province.nominatimProvinceCode) {
      inputReset1(province.name);
      inputReset2(province.nominatimProvinceCode);

      // Busca el departamento (province) correspondiente y lo establece como seleccionado
      const country = countryList.find((p) => p.id === province.countryDSId);
      if (country) {
        setDdlSelectedCountry(country);
      } else {
        inputResetCountry(); // Resetea el selector de departamentos si no se encuentra uno correspondiente
      }
    } else {
      inputReset1();
      inputReset2();
    }
    setIsModalVisible(true);
  };

  const closeModal = () => {
    setIsModalVisible(false);
    setCurrentProvince(null);
  };

  const closeDeleteModal = () => {
    setIsDeleteModalVisible(false);
    setIdToDelete(null);
  };

  const openDeleteModal = (id) => {
    setIdToDelete(id);
    setIsDeleteModalVisible(true);
  };

  const confirmDelete = async () => {
    if (idToDelete) {
      await removeData(urlProvince, idToDelete);
      dispatch(fetchProvinceList());
      closeDeleteModal();
    }
  };

  const inputResetCountry = () => {
    setDdlSelectedCountry(null);
    setInputHasErrorCountry(false);
  };

  //#endregion Functions ***********************************

  //#region Events ***********************************

  const formSubmitHandler = async (event) => {
    event.preventDefault();

    // Verificar si se seleccionó una provincia
    const inputIsValidCountry = ddlSelectedCountry !== null;
    if (!inputIsValidCountry) {
      setInputHasErrorCountry(true);
      return;
    }

    setIsValidForm(inputIsValid1 && inputIsValid2);

    if (!isValidForm) {
      return;
    }

    const dataToUpload = {
      Name: provinceName,
      NominatimProvinceCode: nominatimCode,
      CountryDSId: ddlSelectedCountry.id,
    };

    try {
      let response;
      if (currentProvince) {
        // Actualizar el rol de usuario
        response = await uploadData(
          dataToUpload,
          urlProvince, // Endpoint para actualizar roles
          true,
          currentProvince.id
        );
      } else {
        // Crear un nuevo rol de usuario
        response = await uploadData(
          dataToUpload,
          urlProvince // Endpoint para crear roles
        );
      }
      if (response) {
        dispatch(fetchProvinceList());
        inputReset1();
        inputReset2();

        setTimeout(() => {
          closeModal();
        }, 1000);
      }
    } catch (error) {
      console.error("Error al enviar los datos:", error);
    }
  };

  const handleSelectDdlCountry = (item) => {
    setDdlSelectedCountry(item);
  };

  const handlePageChange = (pageNumber) => {
    setCurrentPage(pageNumber);
  };

  //#endregion Events ***********************************

  const indexOfLastItem = currentPage * itemsPerPage;
  const indexOfFirstItem = indexOfLastItem - itemsPerPage;
  const currentProvinces = provinceList.slice(
    indexOfFirstItem,
    indexOfLastItem
  );

  return (
    <div>
      <CButton color="dark" size="sm" onClick={() => openModal()}>
        Agregar
      </CButton>
      <CTable striped>
        <CTableHead>
          <CTableRow>
            <CTableHeaderCell>#</CTableHeaderCell>
            <CTableHeaderCell onClick={() => requestSort("name")}>
              Nombre
            </CTableHeaderCell>
            <CTableHeaderCell
              onClick={() => requestSort("nominatimProvinceCode")}
            >
              Código nominatim
            </CTableHeaderCell>
            <CTableHeaderCell onClick={() => requestSort("countryName")}>
              País
            </CTableHeaderCell>
            <CTableHeaderCell>Acciones</CTableHeaderCell>
          </CTableRow>
        </CTableHead>
        <CTableBody>
          {sortedList.map((province, index) => {
            const country = countryList.find(
              (countryItem) => countryItem.id === province.countryDSId
            );
            const countryName = country ? country.name : "No definido";

            return (
              <CTableRow key={province.id}>
                <CTableDataCell>{index + 1}</CTableDataCell>
                <CTableDataCell>{province.name}</CTableDataCell>
                <CTableDataCell>
                  {province.nominatimProvinceCode}
                </CTableDataCell>
                <CTableDataCell>{countryName}</CTableDataCell>
                <CTableDataCell>
                  <CButton
                    color="dark"
                    size="sm"
                    onClick={() => openModal(province)}
                  >
                    Editar
                  </CButton>
                  <CButton
                    color="danger"
                    size="sm"
                    onClick={() => openDeleteModal(province.id)}
                    style={{ marginLeft: 10 }}
                  >
                    Eliminar
                  </CButton>
                </CTableDataCell>
              </CTableRow>
            );
          })}
        </CTableBody>
      </CTable>

      <CPagination align="center">
        {currentPage > 1 && (
          <CPaginationItem onClick={() => handlePageChange(currentPage - 1)}>
            Anterior
          </CPaginationItem>
        )}
        {[...Array(pageCount)].map((_, i) => (
          <CPaginationItem
            key={i + 1}
            active={i + 1 === currentPage}
            onClick={() => handlePageChange(i + 1)}
          >
            {i + 1}
          </CPaginationItem>
        ))}
        {currentPage < pageCount && (
          <CPaginationItem onClick={() => handlePageChange(currentPage + 1)}>
            Siguiente
          </CPaginationItem>
        )}
      </CPagination>

      <CModal visible={isModalVisible} onClose={closeModal}>
        <CModalHeader>
          <CModalTitle>
            {currentProvince ? "Editar departamento" : "Agregar departamento"}
          </CModalTitle>
        </CModalHeader>
        <CForm onSubmit={formSubmitHandler}>
          <CModalBody>
            <CCard>
              <CCardBody>
                <CInputGroup>
                  <CInputGroupText className="cardItem custom-input-group-text">
                    {props.inputName}
                  </CInputGroupText>
                  <CFormInput
                    type="text"
                    className="cardItem"
                    onChange={inputChangeHandler1}
                    onBlur={inputBlurHandler1}
                    value={provinceName}
                  />
                  {inputHasError1 && (
                    <CAlert color="danger" className="w-100">
                      Entrada inválida
                    </CAlert>
                  )}
                </CInputGroup>
                <br />
                <CInputGroup>
                  <CInputGroupText className="cardItem custom-input-group-text">
                    {props.nominatimCode}
                  </CInputGroupText>
                  <CFormInput
                    type="text"
                    className="cardItem"
                    onChange={inputChangeHandler2}
                    onBlur={inputBlurHandler2}
                    value={nominatimCode}
                  />
                  {inputHasError2 && (
                    <CAlert color="danger" className="w-100">
                      Entrada inválida
                    </CAlert>
                  )}
                </CInputGroup>
                <br />
                <CInputGroup>
                  <CInputGroupText className="cardItem custom-input-group-text">
                    País
                  </CInputGroupText>
                  <CDropdown>
                    <CDropdownToggle id="ddlProvince" color="secondary">
                      {ddlSelectedCountry
                        ? ddlSelectedCountry.name
                        : "Seleccionar"}
                    </CDropdownToggle>
                    <CDropdownMenu>
                      {countryList &&
                        countryList.length > 0 &&
                        countryList.map((country) => (
                          <CDropdownItem
                            key={country.id}
                            onClick={() => handleSelectDdlCountry(country)}
                            style={{ cursor: "pointer" }}
                            value={country.id}
                          >
                            {country.id}: {country.name}
                          </CDropdownItem>
                        ))}
                    </CDropdownMenu>
                  </CDropdown>
                  {inputHasErrorCountry && (
                    <CAlert color="danger" className="w-100">
                      Entrada inválida
                    </CAlert>
                  )}
                </CInputGroup>
                <br />
                <CRow className="justify-content-center">
                  {isLoading && (
                    <div className="text-center">
                      <CSpinner />
                    </div>
                  )}
                </CRow>
                <br />
                <CCardFooter className="text-medium-emphasis">
                  {!isValidForm && (
                    <CAlert color="danger" className="w-100">
                      El formulario no es válido
                    </CAlert>
                  )}
                  {isSuccess && (
                    <CAlert color="success" className="w-100">
                      Datos ingresados correctamente
                    </CAlert>
                  )}
                  {errorAPI && (
                    <CAlert color="danger" className="w-100">
                      {errorAPI}
                    </CAlert>
                  )}
                </CCardFooter>
              </CCardBody>
            </CCard>
          </CModalBody>
          <CModalFooter>
            <CButton type="submit" color="dark" size="sm">
              {currentProvince ? "Actualizar" : "Guardar"}
            </CButton>
            <CButton color="secondary" size="sm" onClick={closeModal}>
              Cancelar
            </CButton>
          </CModalFooter>
        </CForm>
      </CModal>

      <CModal visible={isDeleteModalVisible} onClose={closeDeleteModal}>
        <CModalHeader>
          <CModalTitle>Confirmar</CModalTitle>
        </CModalHeader>
        <CModalBody>
          ¿Estás seguro de que deseas eliminar este elemento?
        </CModalBody>
        <CModalFooter>
          <CButton color="danger" size="sm" onClick={confirmDelete}>
            Eliminar
          </CButton>
          <CButton color="secondary" size="sm" onClick={closeDeleteModal}>
            Cancelar
          </CButton>
        </CModalFooter>
      </CModal>
    </div>
  );
};

export default ProvinceTable;
