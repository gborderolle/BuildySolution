import React, { useEffect, useState } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import Modal from "react-modal";

import {
  CRow,
  CCol,
  CButton,
  CSpinner,
  CFormInput,
  CInputGroup,
  CInputGroupText,
  CAlert,
  CForm,
  CCard,
  CCardTitle,
  CCardBody,
  CCardFooter,
  CDropdown,
  CDropdownItem,
  CDropdownToggle,
  CDropdownMenu,
} from "@coreui/react";
import useInput from "../../../hooks/use-input";
import useAPI from "../../../hooks/use-API";

// redux imports
import { useSelector, useDispatch } from "react-redux";
import {
  fetchJobList,
  fetchEstateList,
} from "../../../store/generalData-actions";
import { urlJob } from "../../../endpoints";
import { authActions } from "../../../store/auth-slice";

import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";
import "../../../utils/FileUpload.css";
import FileUpload from "../../../utils/FileUpload.js";

const JobABM = () => {
  //#region Const ***********************************

  const location = useLocation();
  const job = location.state?.job;
  const editMode = location.state?.editMode ? location.state?.editMode : false;

  const [isValidForm, setIsValidForm] = useState(true);
  const {
    isLoading,
    isSuccess,
    error: errorAPI,
    uploadData,
    removeData,
  } = useAPI();

  // DDLs
  const [inputHasErrorEstate, setInputHasErrorEstate] = useState(false);

  const monthString = job?.month;
  const monthDate = monthString ? new Date(monthString) : new Date();
  const [month, setMonth] = useState(monthDate);

  const [loadedPhotos, setLoadedPhotos] = useState([]);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [selectedImage, setSelectedImage] = useState(null);
  const [estateJobCount, setEstateJobCount] = useState(0);

  // redux
  const dispatch = useDispatch();

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

  // Redux
  const estateList = useSelector((state) => state.generalData.estateList);
  const workerList = useSelector((state) => state.generalData.workerList);

  const defaultEstateId = job?.estate?.id || null;
  const defaultEstate = estateList.find(
    (estate) => job?.estate?.id === defaultEstateId
  );
  const [ddlSelectedEstate, setDdlSelectedEstate] = useState(
    defaultEstate || null
  );

  const defaultWorker =
    job?.listWorkers?.length > 0
      ? workerList.find(
          (worker) =>
            worker.id === job.listWorkers[job.listWorkers.length - 1].id
        )
      : null;

  const [ddlSelectedWorker, setDdlSelectedWorker] = useState(
    defaultWorker || null
  );

  const {
    value: name,
    isValid: inputIsValidName,
    hasError: inputHasErrorName,
    valueChangeHandler: inputChangeHandlerName,
    inputBlurHandler: inputBlurHandlerName,
    reset: inputResetName,
  } = useInput(
    (value) => value.trim() !== "", // validateValue function
    null, // onChangeCallback
    job && job.name ? job.name : ""
  );

  const {
    value: comments,
    isValid: inputIsValidComments,
    hasError: inputHasErrorComments,
    valueChangeHandler: inputChangeHandlerComments,
    inputBlurHandler: inputBlurHandlerComments,
    reset: inputResetComments,
  } = useInput(
    (value) => true,
    null, // onChangeCallback
    job && job.comments ? job.comments : ""
  );

  const {
    value: cost,
    isValid: inputIsValidCost,
    hasError: inputHasErrorCost,
    valueChangeHandler: inputChangeHandlerCost,
    inputBlurHandler: inputBlurHandlerCost,
    reset: inputResetCost,
  } = useInput(
    (value) => !isNaN(value) && value.trim() !== "", // validación actualizada
    null, // onChangeCallback
    job && job.labourCost ? job.labourCost.toString() : ""
  );

  const openModal = (imageUrl) => {
    setSelectedImage(imageUrl);
    setIsModalOpen(true);
  };

  const closeModal = () => {
    setIsModalOpen(false);
    setSelectedImage(null);
  };

  const handleCancel = () => {
    navigate("/jobs"); // Reemplaza con la ruta deseada
  };

  const handleDelete = async () => {
    if (job && job.id) {
      const confirmDelete = window.confirm(
        "¿Estás seguro de que quieres eliminar esta obra?"
      );
      if (confirmDelete) {
        await removeData(urlJob, job.id);
        dispatch(fetchJobList());
        dispatch(fetchEstateList());
        navigate("/jobs");
      }
    }
  };

  const requiredFieldStyle = {
    borderColor: "violet",
  };

  const getJobCountForEstate = (estate) => {
    return estate.listJobs?.length;
  };

  //#endregion Const ***********************************

  //#region Hooks ***********************************

  useEffect(() => {
    if (editMode && job?.listPhotosURL) {
      const existingPhotos = job.listPhotosURL.map((url) => ({
        url, // URL de la foto existente
        isExisting: true, // Marca para identificar que es una foto ya existente
      }));
      setLoadedPhotos(existingPhotos);
    }
  }, [editMode, job]);

  //#endregion Hooks ***********************************

  //#region Events ***********************************

  const formSubmitHandler = async (event) => {
    event.preventDefault();

    // Verificar si se seleccionó una provincia
    const inputIsValidEstate = ddlSelectedEstate !== null;
    if (!inputIsValidEstate) {
      setInputHasErrorEstate(true);
      return;
    }

    setIsValidForm(
      inputIsValidName &&
        inputIsValidComments &&
        inputIsValidCost &&
        inputIsValidEstate
    );

    if (isValidForm) {
      try {
        // Subir cada foto en loadedPhotos
        const formData = new FormData();
        loadedPhotos.forEach((photo, index) => {
          formData.append(`ListPhotos`, photo.file); // Aquí no uses índice en el nombre del campo
        });

        formData.append("Name", name);
        formData.append("Month", month.toISOString()); // Asegúrate de enviar la fecha en un formato adecuado
        formData.append("Comments", comments);
        formData.append("LabourCost", cost);
        formData.append("EstateId", ddlSelectedEstate.id);

        if (ddlSelectedWorker) {
          formData.append("WorkerIds", ddlSelectedWorker.id);
        }

        await uploadData(formData, urlJob, editMode, job?.id);
        dispatch(fetchJobList());
        dispatch(fetchEstateList());

        //if (isSuccess) {
        setTimeout(() => {
          navigate("/jobs");
        }, 1000);
        //}
      } catch (error) {
        console.error("Error al enviar los datos:", error);
      }
    }
  };

  const handleSelectDdlEstate = (item) => {
    setDdlSelectedEstate(item);
    const jobCount = getJobCountForEstate(item);
    setEstateJobCount(jobCount);
  };

  const handleSelectDdlWorker = (item) => {
    setDdlSelectedWorker(item);
  };

  //#endregion Events ***********************************

  //#region Functions ***********************************

  // Esta función se llama cuando se cargan nuevos archivos
  const handleFileUpload = (newFiles) => {
    setLoadedPhotos((currentFiles) => [
      ...currentFiles,
      ...newFiles.map((file) => ({
        file,
        type: file.type,
        name: file.name,
      })),
    ]);
  };

  // Modificar la función de renderizado para manejar fotos existentes
  const renderPhotoPreviews = () => {
    return (
      <div style={{ display: "flex", overflowX: "auto", gap: "10px" }}>
        {loadedPhotos.map((photo, index) => (
          <div
            key={index}
            style={{ flex: "0 0 auto" }}
            onClick={() => openModal(photo.url ? photo.url.url : "")}
          >
            <img
              src={photo.url ? photo.url.url : "/placeholder-image-url"}
              alt={`Foto ${index}`}
              style={{ width: "100px", height: "100px", cursor: "pointer" }}
            />
          </div>
        ))}
      </div>
    );
  };

  function navigateToWorker() {
    navigate("/worker-abm", { state: { from: "JobABM", job: job } });
  }

  //#endregion Functions ***********************************

  return (
    <CRow>
      <CCol xs>
        <CForm onSubmit={formSubmitHandler}>
          <CCard>
            <CCardBody>
              <div
                style={{
                  display: "flex",
                  justifyContent: "space-between",
                  alignItems: "center",
                  width: "100%",
                }}
              >
                <div>
                  <CCardTitle>
                    {editMode ? "Modificar la obra" : "Agregar una obra"}
                  </CCardTitle>
                </div>
                {editMode && job && job.id && (
                  <CButton
                    color="danger"
                    size="sm"
                    onClick={handleDelete}
                    style={{ marginLeft: "auto" }}
                  >
                    Eliminar
                  </CButton>
                )}
              </div>
              <br />
              <CInputGroup>
                <CInputGroupText className="cardItem custom-input-group-text">
                  Propiedad
                </CInputGroupText>
                <CDropdown>
                  <CDropdownToggle
                    id="ddlEstate"
                    color="secondary"
                    style={requiredFieldStyle}
                  >
                    {ddlSelectedEstate ? ddlSelectedEstate.name : "Seleccionar"}
                  </CDropdownToggle>
                  <CDropdownMenu>
                    {estateList &&
                      estateList.length > 0 &&
                      estateList.map((estate) => (
                        <CDropdownItem
                          key={estate.id}
                          onClick={() => handleSelectDdlEstate(estate)}
                          style={{ cursor: "pointer" }}
                          value={estate.id}
                        >
                          {estate.id}: {estate.address} ({estate.name})
                        </CDropdownItem>
                      ))}
                  </CDropdownMenu>
                </CDropdown>
                <CInputGroupText>{estateJobCount} obras</CInputGroupText>
                {/*  */}
                {inputHasErrorEstate && (
                  <CAlert color="danger" className="w-100">
                    Entrada inválida
                  </CAlert>
                )}
              </CInputGroup>
              <br />
              <CInputGroup>
                <div
                  style={{
                    display: "flex",
                    alignItems: "center",
                    width: "100%",
                  }}
                >
                  <CInputGroupText className="cardItem custom-input-group-text">
                    Trabajador
                  </CInputGroupText>
                  <CDropdown>
                    <CDropdownToggle id="ddlWorker" color="secondary">
                      {ddlSelectedWorker
                        ? ddlSelectedWorker.name
                        : "Seleccionar"}
                    </CDropdownToggle>
                    <CDropdownMenu>
                      {workerList &&
                        workerList.length > 0 &&
                        workerList.map((worker) => (
                          <CDropdownItem
                            key={worker.id}
                            onClick={() => handleSelectDdlWorker(worker)}
                            style={{ cursor: "pointer" }}
                            value={worker.id}
                          >
                            {worker.id}: {worker.name}
                          </CDropdownItem>
                        ))}
                    </CDropdownMenu>
                  </CDropdown>
                  <CButton
                    color="dark"
                    style={{ marginLeft: "10px" }}
                    onClick={() => navigateToWorker()}
                  >
                    Nuevo
                  </CButton>
                </div>
              </CInputGroup>
              <br />
              <CInputGroup>
                <CInputGroupText className="cardItem custom-input-group-text">
                  Nombre
                </CInputGroupText>
                <CFormInput
                  type="text"
                  className="cardItem"
                  onChange={inputChangeHandlerName}
                  onBlur={inputBlurHandlerName}
                  value={name}
                  style={requiredFieldStyle}
                />
                {inputHasErrorName && (
                  <CAlert color="danger" className="w-100">
                    Entrada inválida
                  </CAlert>
                )}
              </CInputGroup>
              <br />
              <CInputGroup>
                <CInputGroupText>Fecha</CInputGroupText>
                <DatePicker
                  selected={month}
                  onChange={(date) => setMonth(date)}
                  dateFormat="MM/yyyy"
                  showMonthYearPicker
                  className="form-control"
                />
              </CInputGroup>
              <br />
              <CInputGroup>
                <CInputGroupText className="cardItem custom-input-group-text">
                  Comentarios
                </CInputGroupText>
                <CFormInput
                  type="text"
                  className="cardItem"
                  onChange={inputChangeHandlerComments}
                  onBlur={inputBlurHandlerComments}
                  value={comments}
                />
                {inputHasErrorComments && (
                  <CAlert color="danger" className="w-100">
                    Entrada inválida
                  </CAlert>
                )}
              </CInputGroup>
              <br />
              <CInputGroup>
                <CInputGroupText className="cardItem custom-input-group-text">
                  Costo de la obra $
                </CInputGroupText>
                <CFormInput
                  type="number"
                  className="cardItem"
                  onChange={inputChangeHandlerCost}
                  onBlur={inputBlurHandlerCost}
                  value={cost}
                />
                {inputHasErrorCost && (
                  <CAlert color="danger" className="w-100">
                    Entrada inválida
                  </CAlert>
                )}
              </CInputGroup>
              <br />
              <FileUpload
                multiple={true}
                name="example-upload"
                maxSize={5000000}
                onUpload={handleFileUpload}
                label="Cargar fotos"
              />
              {editMode && (
                <>
                  <br />
                  <div>{renderPhotoPreviews()}</div>
                  <Modal
                    appElement={document.getElementById("root")}
                    isOpen={isModalOpen}
                    onRequestClose={closeModal}
                    contentLabel="Imagen Ampliada"
                    style={{
                      overlay: {
                        zIndex: 1000, // Un valor alto para asegurar que esté por encima de todo
                      },
                      content: {
                        display: "flex",
                        flexDirection: "column",
                        alignItems: "center", // Centrar horizontalmente
                        justifyContent: "center", // Centrar verticalmente (opcional)
                        background: "white", // Fondo blanco
                        padding: "20px", // Espaciado interno
                        borderRadius: "10px", // Bordes redondeados
                        boxShadow: "0 4px 8px rgba(0, 0, 0, 0.1)", // Sombra para dar efecto de elevación
                      },
                    }}
                  >
                    <div
                      style={{
                        display: "flex",
                        flexDirection: "column",
                        alignItems: "center",
                        justifyContent: "center",
                      }}
                    >
                      <div
                        style={{
                          border: "4px solid rgb(60 75 100)", // Un borde marrón oscuro para simular el marco
                          padding: "5px", // Espacio entre el borde y la imagen
                          boxShadow: "0 4px 8px rgba(0, 0, 0, 0.5)", // Sombra para dar efecto de profundidad
                          margin: "20px", // Margen alrededor del marco
                        }}
                      >
                        <img
                          src={selectedImage}
                          alt="Imagen"
                          style={{ width: "440px", marginTop: "60px" }}
                        />
                        <div
                          style={{
                            display: "flex",
                            justifyContent: "center",
                            marginTop: "10px",
                          }}
                        >
                          <a href={selectedImage} download>
                            <CButton
                              size="sm"
                              color="secondary"
                              style={{ marginRight: "10px" }}
                            >
                              Descargar
                            </CButton>
                          </a>
                          <CButton
                            size="sm"
                            color="secondary"
                            onClick={closeModal}
                          >
                            Cerrar
                          </CButton>
                        </div>
                      </div>
                    </div>
                  </Modal>
                </>
              )}
              <br />
              <CRow className="justify-content-center">
                {isLoading && (
                  <div className="text-center">
                    <CSpinner />
                  </div>
                )}
              </CRow>
              <br />
              <CButton type="submit" color="dark">
                Confirmar
              </CButton>
              <CButton
                type="button"
                color="secondary"
                onClick={handleCancel}
                style={{ marginLeft: "10px" }}
              >
                Cancelar
              </CButton>
              <br />
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
        </CForm>
        <br />
        <br />
      </CCol>
    </CRow>
  );
};

export default JobABM;
