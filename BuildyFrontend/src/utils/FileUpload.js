import React, { useState, useEffect, useRef } from "react";

const styles = {
  // Asegúrate de que estos estilos correspondan a los definidos en tu CSS
  inputWrapper: "input-wrapper",
  inputCover: "input-cover",
  helpText: "help-text",
  fileName: "file-name",
  fileNameStretch: "file-name spacer",
  fileExt: "file-ext",
  fileDrag: "file-drag",
  input: "input",
  loader: "loader",
  disabled: "disabled",
  loading: "loading",
  loaderItem: "loader-item",
  spacer: "spacer",
  button: "button",
  hover: "hover",
  imagePreview: "image-preview",
  preview: "preview",
  previewItem: "preview-item",
  previews: "previews",
};

export const uploadFileToServer = (file) => {
  if (!file) {
    console.error("Archivo no proporcionado a uploadFileToServer");
    return Promise.reject("Archivo no proporcionado");
  }

  const delay = file.size / 100; // Asegúrate de que 'file' es un objeto File
  return new Promise((resolve, reject) => {
    setTimeout(() => {
      resolve();
    }, delay);
  });
};

const getExtFromType = (type) => {
  const parts = type.split("/");
  return parts[parts.length - 1];
};

const getExtFromName = (name) => {
  const parts = name.split(".");
  return parts[parts.length - 1];
};

const Loader = () => {
  return (
    <div className={styles.loader}>
      <span className={styles.loaderItem}></span>
      <span className={styles.loaderItem}></span>
      <span className={styles.loaderItem}></span>
    </div>
  );
};

const FilePreview = ({ data, onRemove, onUpload }) => {
  const [loading, setLoading] = useState(true);
  const [src, setSrc] = useState(null);
  const [type, setType] = useState(null);

  useEffect(() => {
    loadData(data);
  }, [data]);

  const loadData = (data) => {
    if (!data) return;

    const fileType = data.type.match("application/pdf")
      ? "pdf"
      : data.type.match("image")
      ? "image"
      : "other";

    if (fileType === "image") {
      const reader = new FileReader();
      reader.onload = (e) => {
        setSrc(e.target.result);
        setType("image");
        setLoading(false);
      };
      reader.readAsDataURL(data);
    } else if (fileType === "application/pdf") {
      // Para PDF, podrías mostrar un icono o el propio PDF embebido si es necesario
      setType("pdf");
      setSrc("/path/to/default/pdf-icon.png"); // Ruta a un ícono de PDF o lo que prefieras
      setLoading(false);
    } else {
      // Otros tipos de archivos pueden ser manejados aquí
      setSrc(false);
      setType("other");
      setLoading(false);
    }
  };

  const loadingDisplay = loading ? "loading data..." : null;
  const uploading = data.loading ? <Loader /> : null;

  // En FilePreview, ajustar la renderización según el tipo de archivo
  const preview =
    !loading && !data.loading ? (
      type === "text" ? (
        <pre className={styles.preview}>{src}</pre>
      ) : type === "image" ? (
        <img alt="preview" src={src} className={styles.imagePreview} />
      ) : type === "pdf" ? (
        // Mostrar un ícono de PDF, o incluso el PDF embebido según tu elección
        <img alt="PDF icon" src={src} className={styles.imagePreview} />
      ) : (
        <pre className={styles.preview}>no preview</pre>
      )
    ) : null;

  const classes = `${styles.previewItem} ${
    data.loading ? styles.disabled : ""
  }`.trim();

  return (
    <div className={classes}>
      {uploading}
      {loadingDisplay}
      {preview}
      <div className={styles.fileNameStretch}>{data.name}</div>
      <button className={styles.button} onClick={onRemove}>
        remove
      </button>
      <button className={styles.button} onClick={onUpload}>
        upload
      </button>
    </div>
  );
};

const FileUpload = ({ maxSize, name, multiple, label, onUpload }) => {
  const [fileList, setFileList] = useState([]);
  const [hoverState, setHoverState] = useState(null);
  const fileInputRef = useRef(null);

  const handleDragOver = (e) => {
    e.stopPropagation();
    e.preventDefault();
    setHoverState(e.type === "dragover" ? styles.hover : null);
  };

  // Esta función ahora solo establece los archivos en el estado fileList
  const handleFileSelect = (e) => {
    e.preventDefault(); // Prevenir el comportamiento por defecto
    e.stopPropagation(); // Detener la propagación del evento

    const newFiles = e.target.files || e.dataTransfer.files; // Obtener los archivos seleccionados

    // Asegúrate de que los archivos no excedan el tamaño máximo
    const validFiles = Array.from(newFiles).filter(
      (file) => !maxSize || file.size <= maxSize
    );

    // Actualiza el estado fileList con los nuevos archivos, además de los ya existentes
    setFileList((currentFiles) => [
      ...currentFiles,
      ...validFiles.map((file) => ({
        ...file,
        loading: false,
        // Incorporamos una propiedad adicional para manejar la URL de previsualización de manera condicional más adelante
        previewUrl: file.type.startsWith("image/")
          ? URL.createObjectURL(file)
          : null,
      })),
    ]);

    // Llama al callback onUpload con los nuevos archivos
    onUpload(validFiles);
  };

  const removeItem = (index) => {
    const updatedList = [...fileList];
    updatedList.splice(index, 1);
    setFileList(updatedList);
  };

  const uploadFile = (file) => {
    return new Promise((resolve, reject) => {
      const index = fileList.indexOf(file);
      const updatedList = [...fileList];
      updatedList[index] = { ...file, loading: true };
      setFileList(updatedList);

      if (typeof file !== "file" || !("size" in file)) {
        reject(new Error("No file size"));
        return;
      }

      onUpload(file).then(() => {
        resolve();
      });
    });
  };

  const previews = () => {
    return fileList.map((file, index) => (
      <FilePreview
        key={index}
        data={file}
        onRemove={() => removeItem(index)}
        onUpload={() => uploadFile(file).then(() => removeItem(index))}
      />
    ));
  };

  const selectFile = (e) => {
    e.preventDefault();
    if (fileInputRef.current) {
      fileInputRef.current.click();
    }
  };

  const dragClasses = `${styles.fileDrag} ${hoverState}`.trim();
  const fileExt =
    fileList.length === 1
      ? fileList[0].type
        ? `.${getExtFromType(fileList[0].type)}`
        : `.${getExtFromName(fileList[0].name)}`
      : null;

  const fileNames =
    fileList.length > 1
      ? `${fileList.length} Files`
      : fileList.length === 1
      ? fileList[0].name.replace(fileExt, "")
      : "No file chosen";

  return (
    <div>
      <input type="hidden" name={`${name}:maxSize`} value={maxSize} />
      <div>
        <label>
          <span>{label}</span>
          <div
            className={dragClasses}
            onDragOver={handleDragOver}
            onDragLeave={handleDragOver}
            onDrop={handleFileSelect}
          >
            <div className={styles.inputWrapper}>
              <input
                type="file"
                tabIndex="-1"
                ref={fileInputRef}
                className={styles.input}
                name={name}
                multiple={multiple}
                onChange={handleFileSelect}
              />
              <div className={styles.inputCover}>
                <button
                  className={styles.button}
                  type="button"
                  onClick={selectFile}
                >
                  Choose Files
                </button>
                <span className={styles.fileName}>{fileNames}</span>
                {fileExt && <span className={styles.fileExt}>{fileExt}</span>}
              </div>
            </div>
            <span className={styles.helpText}>or drop files here</span>
          </div>
          <button
            className={styles.button}
            type="button"
            onClick={() => fileList.forEach(uploadFile)}
          >
            Upload All
          </button>
          <div className={styles.previews}>{previews()}</div>
        </label>
      </div>
    </div>
  );
};

export default FileUpload;
