import React from 'react'
import clsx from 'clsx'

export function LikeButton({ liked, count, disabled, onToggle }) {
  return (
    <button
      type="button"
      className={clsx('btn btn-sm', liked ? 'btn-light' : 'btn-outline-light')}
      disabled={disabled}
      onClick={onToggle}
    >
      <i className={clsx('bi me-2', liked ? 'bi-heart-fill' : 'bi-heart')}></i>
      {count ?? 0}
    </button>
  )
}
