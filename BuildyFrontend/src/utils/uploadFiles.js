console.clear();
const { createClass, PropTypes } = React;
const { render } = ReactDOM;

const styles = {
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

const uploadFileToServer = (file) => {
  const delay = file.size / 100;
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
  return /*#__PURE__*/ React.createElement(
    "div",
    { className: styles.loader } /*#__PURE__*/,
    React.createElement("span", { className: styles.loaderItem }) /*#__PURE__*/,
    React.createElement("span", { className: styles.loaderItem }) /*#__PURE__*/,
    React.createElement("span", { className: styles.loaderItem })
  );
};

const FilePreview = createClass({
  getInitialState() {
    return {
      loading: true,
    };
  },
  getDefaultProps() {
    return {
      onRemove: () => {},
    };
  },
  componentWillMount() {
    this.loadData();
  },
  componentWillReceiveProps(newProps) {
    this.loadData(newProps.data);
  },
  loadData(data = this.props.data) {
    if (!data || !data.type) {
      // Si no hay datos o el tipo de datos no está definido, establecemos un estado por defecto.
      this.setState({
        src: false,
        type: null,
        loading: false,
      });
      return;
    }

    const reader = new FileReader();
    let type;

    // Verifica si el tipo de archivo es texto o imagen y asigna el valor correspondiente.
    if (data.type.match("text")) {
      type = "text";
    } else if (data.type.match("image")) {
      type = "image";
    } else {
      type = data.type;
    }

    reader.onload = (e) => {
      const src = e.target.result;
      this.setState({
        src,
        type,
        loading: false,
      });
    };

    if (type === "text") {
      reader.readAsText(data);
    } else if (type === "image") {
      reader.readAsDataURL(data);
    } else {
      this.setState({
        src: false,
        type,
        loading: false,
      });
    }
  },

  render() {
    const loading = this.state.loading ? "loading data..." : null;

    const uploading = this.props.data.loading /*#__PURE__*/
      ? React.createElement(Loader, null)
      : null;

    const preview =
      !this.state.loading && !this.props.data.loading
        ? this.state.type === "text" /*#__PURE__*/
          ? React.createElement(
              "pre",
              { className: styles.preview },
              this.state.src
            )
          : this.state.type === "image" /*#__PURE__*/
          ? React.createElement("img", {
              alt: "preview",
              src: this.state.src,
              className: styles.imagePreview,
            }) /*#__PURE__*/
          : React.createElement(
              "pre",
              { className: styles.preview },
              "no preview"
            )
        : null;

    const classes = [
      styles.previewItem,
      this.props.data.loading ? styles.disabled : "",
    ]
      .join(" ")
      .trim();
    return /*#__PURE__*/ React.createElement(
      "div",
      { className: classes },
      uploading,
      loading,
      preview /*#__PURE__*/,
      React.createElement(
        "div",
        { className: styles.fileNameStretch },
        this.props.data.name
      ) /*#__PURE__*/,
      React.createElement(
        "button",
        { className: styles.button, onClick: this.props.onRemove },
        "remove"
      ) /*#__PURE__*/,

      React.createElement(
        "button",
        { className: styles.button, onClick: this.props.onUpload },
        "upload"
      )
    );
  },
});

const FileUpload = React.createClass({
  displayName: "FileUpload",
  getInitialState() {
    return {
      fileList: [],
    };
  },

  handleDragOver(e) {
    if ("preventDefault" in e) {
      e.stopPropagation();
      e.preventDefault();
    }
    const hoverState = e.type === "dragover" ? styles.hover : null;
    this.setState({ hoverState });
  },

  handleFileSelect(e) {
    this.handleDragOver(e);
    const files = e.target.files || e.dataTransfer.files;
    const imageTypes = ["image/jpeg", "image/png", "image/webp"]; // Tipos de imagen aceptados
    const filteredFiles = Array.from(files).filter((file) =>
      imageTypes.includes(file.type)
    );
    this.setState({ fileList: this.state.fileList.concat(filteredFiles) });
  },

  removeItem(index) {
    const fileList = this.state.fileList;
    fileList.splice(index, 1);
    this.setState({
      fileList,
    });
  },
  removeFile(file) {
    const fileList = this.state.fileList;
    const index = fileList.indexOf(file);
    this.removeItem(index);
  },
  uploadFile(file) {
    return new Promise((resolve, reject) => {
      const fileList = this.state.fileList;
      const index = fileList.indexOf(file);
      fileList[index].loading = true;
      this.setState({ fileList });
      if (typeof file === "file" || !("size" in file)) {
        return reject(new Error("No file size"));
      }
      this.props.onUpload(file).then((data) => {
        resolve(data);
      });
    });
  },

  previews() {
    return this.state.fileList.map((file, index) => {
      const removeItem = () => {
        this.removeItem(index);
      };
      const uploadFile = () => {
        this.uploadFile(file).then(() => {
          this.removeFile(file);
        });
      };
      return /*#__PURE__*/ React.createElement(FilePreview, {
        key: index,
        data: file,
        onRemove: removeItem,
        onUpload: uploadFile,
      });
    });
  },
  uploadFiles() {
    this.state.fileList.forEach((file) => {
      this.uploadFile(file).then(() => {
        this.removeFile(file);
      });
    });
  },
  selectFile(e) {
    e.preventDefault();
    this.input.click(e);
  },
  render() {
    const { maxSize, name, multiple, label } = this.props;

    const dragClasses = [styles.fileDrag, this.state.hoverState]
      .join(" ")
      .trim();
    const fileExt =
      this.state.fileList.length === 1
        ? this.state.fileList[0].type
          ? `.${getExtFromType(this.state.fileList[0].type)}`
          : `.${getExtFromName(this.state.fileList[0].name)}`
        : null;

    const extTail = fileExt /*#__PURE__*/
      ? React.createElement("span", { className: styles.fileExt }, fileExt)
      : null;

    const fileNames =
      this.state.fileList.length > 1
        ? `${this.state.fileList.length} Files`
        : this.state.fileList.length === 1
        ? this.state.fileList[0].name.replace(fileExt, "")
        : "No hay fotos seleccionadas";

    return /*#__PURE__*/ React.createElement(
      "div",
      null /*#__PURE__*/,
      React.createElement("input", {
        type: "hidden",
        name: `${name}:maxSize`,
        value: maxSize,
      }) /*#__PURE__*/,
      React.createElement(
        "div",
        null /*#__PURE__*/,
        React.createElement(
          "label",
          null /*#__PURE__*/,
          React.createElement("span", null, label) /*#__PURE__*/,
          React.createElement(
            "div",
            {
              className: dragClasses,
              onDragOver: this.handleDragOver,
              onDragLeave: this.handleDragOver,
              onDrop: this.handleFileSelect,
            } /*#__PURE__*/,
            React.createElement(
              "div",
              { className: styles.inputWrapper } /*#__PURE__*/,
              React.createElement("input", {
                type: "file",
                tabIndex: "-1",
                ref: (x) => (this.input = x),
                className: styles.input,
                name: name,
                multiple: multiple,
                onChange: this.handleFileSelect,
                accept: "image/jpeg, image/png, image/webp", // Restricción de tipos de archivo
              }) /*#__PURE__*/,
              React.createElement(
                "div",
                { className: styles.inputCover } /*#__PURE__*/,
                React.createElement(
                  "button",
                  {
                    className: styles.button,
                    type: "button",
                    onClick: this.selectFile,
                  },
                  "Elegir fotos"
                ) /*#__PURE__*/,

                React.createElement(
                  "span",
                  { className: styles.fileName },
                  fileNames
                ),
                extTail
              )
            ) /*#__PURE__*/,

            React.createElement(
              "span",
              { className: styles.helpText },
              "o arrastrar fotos aquí"
            )
          )
        ) /*#__PURE__*/,

        React.createElement(
          "button",
          {
            className: styles.button,
            type: "button",
            onClick: this.uploadFiles,
          },
          "Subir todo"
        ) /*#__PURE__*/,

        React.createElement(
          "div",
          { className: styles.previews },
          this.previews()
        )
      )
    );
  },
});
