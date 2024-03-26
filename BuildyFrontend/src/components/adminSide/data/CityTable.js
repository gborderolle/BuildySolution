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
  fetchCityList,
  fetchProvinceList,
} from "../../../store/generalData-actions";
import { urlCity } from "../../../endpoints";

const CityTable = (props) => {
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
  const [currentCity, setCurrentCity] = useState(null);

  const [isDeleteModalVisible, setIsDeleteModalVisible] = useState(false);
  const [idToDelete, setIdToDelete] = useState(null);

  const [ddlSelectedProvince, setDdlSelectedProvince] = useState(null);
  const [inputHasErrorProvince, setInputHasErrorProvince] = useState(false);

  // redux
  const dispatch = useDispatch();

  // Redux
  const cityList = useSelector((state) => state.generalData.cityList);
  const provinceList = useSelector((state) => state.generalData.provinceList);

  const [sortConfig, setSortConfig] = useState({
    key: null,
    direction: "ascending",
  });

  const itemsPerPage = 25;
  const [currentPage, setCurrentPage] = useState(1);
  const [pageCount, setPageCount] = useState(0);

  useEffect(() => {
    dispatch(fetchCityList());
    dispatch(fetchProvinceList());
  }, [dispatch]);

  useEffect(() => {
    setPageCount(Math.ceil(cityList.length / itemsPerPage));
  }, [cityList, itemsPerPage]);

  const {
    value: cityName,
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

  const combinedCityList = useMemo(() => {
    return cityList.map((city) => {
      const province = provinceList.find((p) => p.id === city.provinceDSId);
      return {
        ...city,
        provinceName: province ? province.name : "No definido",
      };
    });
  }, [cityList, provinceList]);

  const sortedList = useMemo(() => {
    let sortableList = [...combinedCityList];
    if (sortConfig.key !== null) {
      sortableList.sort((a, b) => {
        if (sortConfig.key === "provinceName") {
          const provinceA = a.provinceName.toLowerCase();
          const provinceB = b.provinceName.toLowerCase();
          if (provinceA < provinceB) {
            return sortConfig.direction === "ascending" ? -1 : 1;
          }
          if (provinceA > provinceB) {
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
  }, [combinedCityList, sortConfig]);

  //#endregion Hooks ***********************************

  //#region Functions ***********************************

  const requestSort = (key) => {
    let direction = "ascending";
    if (sortConfig.key === key && sortConfig.direction === "ascending") {
      direction = "descending";
    }
    setSortConfig({ key, direction });
  };

  const openModal = (city = null) => {
    setCurrentCity(city);
    if (city && city.name && city.nominatimCityCode) {
      inputReset1(city.name);
      inputReset2(city.nominatimCityCode);

      // Busca el departamento (province) correspondiente y lo establece como seleccionado
      const province = provinceList.find((p) => p.id === city.provinceDSId);
      if (province) {
        setDdlSelectedProvince(province);
      } else {
        inputResetProvince(); // Resetea el selector de departamentos si no se encuentra uno correspondiente
      }
    } else {
      inputReset1();
      inputReset2();
      inputResetProvince();
    }
    setIsModalVisible(true);
  };

  const closeModal = () => {
    setIsModalVisible(false);
    setCurrentCity(null);
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
      await removeData(urlCity, idToDelete);
      dispatch(fetchCityList());
      closeDeleteModal();
    }
  };

  const inputResetProvince = () => {
    setDdlSelectedProvince(null);
    setInputHasErrorProvince(false);
  };

  //#endregion Functions ***********************************

  //#region Events ***********************************

  const formSubmitHandler = async (event) => {
    event.preventDefault();

    // Verificar si se seleccionó una provincia
    const inputIsValidProvince = ddlSelectedProvince !== null;
    if (!inputIsValidProvince) {
      setInputHasErrorProvince(true);
      return;
    }

    setIsValidForm(inputIsValid1 && inputIsValid2);

    if (!isValidForm) {
      return;
    }

    const dataToUpload = {
      Name: cityName,
      NominatimCityCode: nominatimCode,
      ProvinceDSId: ddlSelectedProvince.id,
    };

    try {
      let response;
      if (currentCity) {
        // Actualizar el rol de usuario
        response = await uploadData(
          dataToUpload,
          urlCity, // Endpoint para actualizar roles
          true,
          currentCity.id
        );
      } else {
        // Crear un nuevo rol de usuario
        response = await uploadData(
          dataToUpload,
          urlCity // Endpoint para crear roles
        );
      }
      if (response) {
        dispatch(fetchCityList());
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

  const handleSelectDdlProvince = (item) => {
    setDdlSelectedProvince(item);
  };

  const handlePageChange = (pageNumber) => {
    setCurrentPage(pageNumber);
  };

  //#endregion Events ***********************************

  const indexOfLastItem = currentPage * itemsPerPage;
  const indexOfFirstItem = indexOfLastItem - itemsPerPage;
  const currentCities = cityList.slice(indexOfFirstItem, indexOfLastItem);

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
            <CTableHeaderCell onClick={() => requestSort("nominatimCityCode")}>
              Código nominatim
            </CTableHeaderCell>
            <CTableHeaderCell onClick={() => requestSort("provinceName")}>
              Departamento
            </CTableHeaderCell>
            <CTableHeaderCell>Acciones</CTableHeaderCell>
          </CTableRow>
        </CTableHead>
        <CTableBody>
          {sortedList.map((city, index) => {
            const province = provinceList.find(
              (province) => province.id === city.provinceDSId
            );
            const provinceName = province ? province.name : "No definido";

            return (
              <CTableRow key={city.id}>
                <CTableDataCell>{index + 1}</CTableDataCell>
                <CTableDataCell>{city.name}</CTableDataCell>
                <CTableDataCell>{city.nominatimCityCode}</CTableDataCell>
                <CTableDataCell>{provinceName}</CTableDataCell>
                <CTableDataCell>
                  <CButton
                    color="dark"
                    size="sm"
                    onClick={() => openModal(city)}
                  >
                    Editar
                  </CButton>
                  <CButton
                    color="danger"
                    size="sm"
                    onClick={() => openDeleteModal(city.id)}
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
            {currentCity ? "Editar ciudad" : "Agregar ciudad"}
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
                    value={cityName}
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
                    Departamento
                  </CInputGroupText>
                  <CDropdown>
                    <CDropdownToggle id="ddlProvince" color="secondary">
                      {ddlSelectedProvince
                        ? ddlSelectedProvince.name
                        : "Seleccionar"}
                    </CDropdownToggle>
                    <CDropdownMenu>
                      {provinceList &&
                        provinceList.length > 0 &&
                        provinceList.map((province) => (
                          <CDropdownItem
                            key={province.id}
                            onClick={() => handleSelectDdlProvince(province)}
                            style={{ cursor: "pointer" }}
                            value={province.id}
                          >
                            {province.id}: {province.name}
                          </CDropdownItem>
                        ))}
                    </CDropdownMenu>
                  </CDropdown>
                  {inputHasErrorProvince && (
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
              {currentCity ? "Actualizar" : "Guardar"}
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

export default CityTable;
