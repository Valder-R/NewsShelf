import React from 'react'
import {useForm} from 'react-hook-form'
import {toast} from 'react-toastify'

export function CommentForm({ onSubmit }) {
  const { register, handleSubmit, reset, formState: { isSubmitting } } = useForm({ defaultValues: { text: '' } })

  return (
    <form
      className="ns-card p-3"
      onSubmit={handleSubmit(async (values) => {
        if (!values.text?.trim()) {
          toast.info('Напишіть коментар')
          return
        }
        await onSubmit(values)
        reset()
      })}
    >
      <div className="d-flex gap-2">
        <textarea
          className="form-control ns-input"
          rows={2}
          placeholder="Ваш коментар..."
          {...register('text')}
        />
        <button className="btn btn-light" disabled={isSubmitting} type="submit" style={{ minWidth: 140 }}>
          <i className="bi bi-send me-2"></i>
          Надіслати
        </button>
      </div>
      <div className="ns-muted small mt-2">
        Дотримуйтесь правил спільноти. Коментарі можуть проходити модерацію.
      </div>
    </form>
  )
}
