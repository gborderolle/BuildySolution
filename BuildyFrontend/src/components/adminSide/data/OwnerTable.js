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
import { fetchOwnerList } from "../../../store/generalData-actions";
import { urlOwner } from "../../../endpoints";

const OwnerTable = (props) => {
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
  const [currentOwner, setCurrentOwner] = useState(null);

  const [isDeleteModalVisible, setIsDeleteModalVisible] = useState(false);
  const [idToDelete, setIdToDelete] = useState(null);

  // redux
  const dispatch = useDispatch();

  // Redux
  const ownerList = useSelector((state) => state.generalData.ownerList);

  const [sortConfig, setSortConfig] = useState({
    key: null,
    direction: "ascending",
  });

  const itemsPerPage = 25;
  const [currentPage, setCurrentPage] = useState(1);
  const [pageCount, setPageCount] = useState(0);

  useEffect(() => {
    dispatch(fetchOwnerList());
    dispatch(fetchOwnerList());
  }, [dispatch]);

  useEffect(() => {
    setPageCount(Math.ceil(ownerList.length / itemsPerPage));
  }, [ownerList, itemsPerPage]);

  const {
    value: ownerName,
    isValid: inputIsValid1,
    hasError: inputHasError1,
    valueChangeHandler: inputChangeHandler1,
    inputBlurHandler: inputBlurHandler1,
    reset: inputReset1,
  } = useInput((value) => value.trim() !== "");

  const {
    value: color,
    isValid: inputIsValid2,
    hasError: inputHasError2,
    valueChangeHandler: inputChangeHandler2,
    inputBlurHandler: inputBlurHandler2,
    reset: inputReset2,
  } = useInput((value) => true);

  //#endregion Consts ***********************************

  //#region Hooks ***********************************

  const sortedList = useMemo(() => {
    let sortableList = [...ownerList];
    if (sortConfig.key !== null) {
      sortableList.sort((a, b) => {
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
  }, [ownerList, sortConfig]);

  //#endregion Hooks ***********************************

  //#region Functions ***********************************

  const requestSort = (key) => {
    let direction = "ascending";
    if (sortConfig.key === key && sortConfig.direction === "ascending") {
      direction = "descending";
    }
    setSortConfig({ key, direction });
  };

  const openModal = (owner = null) => {
    setCurrentOwner(owner);
    if (owner && owner.name && owner.color) {
      inputReset1(owner.name);
      inputReset2(owner.color);
    } else {
      inputReset1();
      inputReset2();
    }
    setIsModalVisible(true);
  };

  const closeModal = () => {
    setIsModalVisible(false);
    setCurrentOwner(null);
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
      await removeData(urlOwner, idToDelete);
      dispatch(fetchOwnerList());
      closeDeleteModal();
    }
  };

  //#endregion Functions ***********************************

  //#region Events ***********************************

  const formSubmitHandler = async (event) => {
    event.preventDefault();

    setIsValidForm(inputIsValid1 && inputIsValid2);

    if (!isValidForm) {
      return;
    }

    const dataToUpload = {
      Name: ownerName,
      Color: color,
    };

    try {
      let response;
      if (currentOwner) {
        // Actualizar el rol de usuario
        response = await uploadData(
          dataToUpload,
          urlOwner, // Endpoint para actualizar roles
          true,
          currentOwner.id
        );
      } else {
        // Crear un nuevo rol de usuario
        response = await uploadData(
          dataToUpload,
          urlOwner // Endpoint para crear roles
        );
      }
      if (response) {
        dispatch(fetchOwnerList());
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

  const handlePageChange = (pageNumber) => {
    setCurrentPage(pageNumber);
  };

  //#endregion Events ***********************************

  const indexOfLastItem = currentPage * itemsPerPage;
  const indexOfFirstItem = indexOfLastItem - itemsPerPage;
  const currentOwners = ownerList.slice(indexOfFirstItem, indexOfLastItem);

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
            <CTableHeaderCell onClick={() => requestSort("color")}>
              Color
            </CTableHeaderCell>
            <CTableHeaderCell>Acciones</CTableHeaderCell>
          </CTableRow>
        </CTableHead>
        <CTableBody>
          {sortedList.map((owner, index) => (
            <CTableRow key={owner.id}>
              <CTableDataCell>{index + 1}</CTableDataCell>
              <CTableDataCell>{owner.name}</CTableDataCell>
              <CTableDataCell>{owner.color}</CTableDataCell>
              <CTableDataCell>
                <CButton
                  color="dark"
                  size="sm"
                  onClick={() => openModal(owner)}
                >
                  Editar
                </CButton>
                <CButton
                  color="danger"
                  size="sm"
                  onClick={() => openDeleteModal(owner.id)}
                  style={{ marginLeft: 10 }}
                >
                  Eliminar
                </CButton>
              </CTableDataCell>
            </CTableRow>
          ))}
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
            {currentOwner ? "Editar dueño" : "Agregar dueño"}
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
                    value={ownerName}
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
                    {props.color}
                  </CInputGroupText>
                  <CFormInput
                    type="text"
                    className="cardItem"
                    onChange={inputChangeHandler2}
                    onBlur={inputBlurHandler2}
                    value={color}
                  />
                  {inputHasError2 && (
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
              {currentOwner ? "Actualizar" : "Guardar"}
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

export default OwnerTable;
