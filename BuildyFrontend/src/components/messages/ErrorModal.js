import ReactDOM from 'react-dom'

import Button from '../UI/Button/Button'

import classes from './ErrorModal.module.css'

const Backdrop = (props) => {
  return <div className={classes.backdrop} onClick={props.onClick}></div>
}

const ModalOverlay = (props) => {
  return (
    <div className="modal show d-block" tabIndex="-1">
      <div className="modal-dialog">
        <div className="modal-content">
          <div className="modal-header">
            <h5 className="modal-title">{props.title}</h5>
          </div>
          <div className="modal-body">
            <p>{props.message}</p>
          </div>
          <div className="modal-footer">
            <Button class="btn btn-primary" onClick={props.onClick} text="Ok" />
          </div>
        </div>
      </div>
    </div>
  )
}

const portalElement = document.getElementById('overlays')

const ErrorModal = (props) => {
  return (
    <>
      {ReactDOM.createPortal(<Backdrop onClick={props.onClick} />, portalElement)}
      {ReactDOM.createPortal(
        <ModalOverlay title={props.title} message={props.message} onClick={props.onClick} />,
        portalElement,
      )}
    </>
  )
}

export default ErrorModal
