import React from 'react'
import {createPortal} from 'react-dom'

export function ConfirmModal({ id, title, body, confirmText = 'Підтвердити', onConfirm }) {
  return createPortal(
      <div className="modal fade" id={id} tabIndex="-1" aria-hidden="true">
        <div className="modal-dialog modal-dialog-centered">
          <div className="modal-content bg-dark text-light border border-secondary">
            <div className="modal-header border-secondary">
              <h5 className="modal-title">{title}</h5>
              <button type="button" className="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div className="modal-body">
              {body}
            </div>
            <div className="modal-footer border-secondary">
              <button type="button" className="btn btn-outline-light" data-bs-dismiss="modal">Скасувати</button>
              <button type="button" className="btn btn-danger" data-bs-dismiss="modal" onClick={onConfirm}>
                {confirmText}
              </button>
            </div>
          </div>
        </div>
      </div>,
      document.body
  )
}
