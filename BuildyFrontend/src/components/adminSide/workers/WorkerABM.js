import React, { useEffect, useState } from "react";
import { useNavigate, useLocation } from "react-router-dom";

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
} from "@coreui/react";
import useInput from "../../../hooks/use-input";
import useAPI from "../../../hooks/use-API";

// redux imports
import { useSelector, useDispatch } from "react-redux";
import { fetchWorkerList } from "../../../store/generalData-actions";
import { urlWorker } from "../../../endpoints";
import { authActions } from "../../../store/auth-slice";

const WorkerABM = () => {
  //#region Const ***********************************

  const location = useLocation();
  const worker = location.state?.worker;
  const editMode = location.state?.editMode ? location.state?.editMode : false;
  const fromJobABM = location.state?.from === "JobABM";
  const job = location.state?.job;

  const [isValidForm, setIsValidForm] = useState(true);
  const {
    isLoading,
    isSuccess,
    error: errorAPI,
    uploadData,
    removeData,
  } = useAPI();

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
    worker && worker.name ? worker.name : ""
  );

  const {
    value: phone,
    isValid: inputIsValidPhone,
    hasError: inputHasErrorPhone,
    valueChangeHandler: inputChangeHandlerPhone,
    inputBlurHandler: inputBlurHandlerPhone,
    reset: inputResetPhone,
  } = useInput(
    (value) => /^[0-9]{9}$/.test(value.trim()), // validateValue function
    null, // onChangeCallback
    worker ? worker.phone : ""
  );

  const {
    value: email,
    isValid: inputIsValidEmail,
    hasError: inputHasErrorEmail,
    valueChangeHandler: inputChangeHandlerEmail,
    inputBlurHandler: inputBlurHandlerEmail,
    reset: inputResetEmail,
  } = useInput(
    (value) => true,
    null, // onChangeCallback
    worker ? worker.email : ""
  );

  const {
    value: document,
    isValid: inputIsValidDocument,
    hasError: inputHasErrorDocument,
    valueChangeHandler: inputChangeHandlerDocument,
    inputBlurHandler: inputBlurHandlerDocument,
    reset: inputResetDocument,
  } = useInput(
    (value) => true,
    null, // onChangeCallback
    worker ? worker.document : ""
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
    worker ? worker.comments : ""
  );

  const handleCancel = () => {
    if (fromJobABM) {
      navigate("/job-abm", { state: { job: job } });
    } else {
      navigate("/workers"); // La ruta por defecto en caso de que no venga de RentABM
    }
  };

  const handleDelete = async () => {
    if (worker && worker.id) {
      const confirmDelete = window.confirm(
        "¿Estás seguro de que quieres eliminar este trabajador?"
      );
      if (confirmDelete) {
        await removeData(urlWorker, worker.id);
        dispatch(fetchWorkerList());
        navigate("/workers");
      }
    }
  };

  const requiredFieldStyle = {
    borderColor: "violet",
  };

  //#endregion Const ***********************************

  //#region Hooks ***********************************

  //#endregion Hooks ***********************************

  //#region Events ***********************************

  const formSubmitHandler = async (event) => {
    event.preventDefault();

    // Validación: Nombre y Teléfono son los campos obligatorios
    const isNameValid = name.trim() !== "";
    const isPhoneValid = phone.trim() !== "" && /^[0-9]{9}$/.test(phone.trim()); // Asumiendo que el teléfono debe tener 9 dígitos

    setIsValidForm(isNameValid && isPhoneValid);

    if (!isValidForm) {
      return;
    }

    // Si todos los campos requeridos son válidos, procede con la lógica de envío
    const dataToUpload = {
      Name: name,
      Phone: phone,
      Email: email.trim() !== "" ? email : null, // Asigna null si el campo está vacío
      IdentityDocument: document.trim() !== "" ? document : null, // Asigna null si el campo está vacío
      Comments: comments.trim() !== "" ? comments : null, // Asigna null si el campo está vacío
    };

    try {
      await uploadData(dataToUpload, urlWorker, editMode, worker?.id);
      dispatch(fetchWorkerList());

      //if (isSuccess) {
      setTimeout(() => {
        if (fromJobABM) {
          navigate("/job-abm", { state: { job: job } });
        } else {
          navigate("/workers"); // La ruta por defecto en caso de que no venga de RentABM
        }
      }, 1000);
      //}
    } catch (error) {
      console.error("Error al enviar los datos:", error);
    }
  };

  //#endregion Events ***********************************

  //#region Functions ***********************************

  const validatePhone1Input = (input) => {
    const maxDigits = 9;
    const inputValue = input.trim();
    const isValid = /^\d{0,9}$/.test(inputValue);
    // return isValid ? inputValue : inputValue.slice(0, maxDigits);
    return isValid ? inputValue : false;
  };

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
                    {editMode
                      ? "Modificar un trabajador"
                      : "Agregar un trabajador"}
                  </CCardTitle>
                </div>
                {editMode && worker && worker.id && (
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
                  Nombre completo
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
                <CInputGroupText className="cardItem custom-input-group-text">
                  Celular [9 dígitos]
                </CInputGroupText>
                <CFormInput
                  type="number"
                  className="cardItem"
                  onChange={(event) => {
                    const validatedInput = validatePhone1Input(
                      event.target.value
                    );
                    if (validatedInput) {
                      inputChangeHandlerPhone(event);
                    }
                  }}
                  onBlur={inputBlurHandlerPhone}
                  value={phone}
                  style={requiredFieldStyle}
                />
                {inputHasErrorPhone && (
                  <CAlert color="danger" className="w-100">
                    Entrada inválida
                  </CAlert>
                )}
              </CInputGroup>
              <br />
              <CInputGroup>
                <CInputGroupText className="cardItem custom-input-group-text">
                  Email @
                </CInputGroupText>
                <CFormInput
                  type="text"
                  className="cardItem"
                  onChange={inputChangeHandlerEmail}
                  onBlur={inputBlurHandlerEmail}
                  value={email}
                />
                {inputHasErrorEmail && (
                  <CAlert color="danger" className="w-100">
                    Entrada inválida
                  </CAlert>
                )}
              </CInputGroup>
              <br />
              <CInputGroup>
                <CInputGroupText className="cardItem custom-input-group-text">
                  Cédula
                </CInputGroupText>
                <CFormInput
                  type="text"
                  className="cardItem"
                  onChange={inputChangeHandlerDocument}
                  onBlur={inputBlurHandlerDocument}
                  value={document}
                />
                {inputHasErrorDocument && (
                  <CAlert color="danger" className="w-100">
                    Entrada inválida
                  </CAlert>
                )}
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

export default WorkerABM;
