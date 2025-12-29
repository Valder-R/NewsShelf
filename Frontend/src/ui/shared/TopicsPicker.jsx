import React from 'react'
import clsx from 'clsx'

export function TopicsPicker({
                                 label = 'Topics',
                                 options = [],
                                 value = [],
                                 onChange,
                                 disabled = false
                             }) {
    const selected = Array.isArray(value) ? value : []

    function toggle(opt) {
        if (disabled) return
        const has = selected.includes(opt)
        const next = has
            ? selected.filter(x => x !== opt)
            : [...selected, opt]
        onChange?.(next)
    }

    function remove(opt) {
        if (disabled) return
        onChange?.(selected.filter(x => x !== opt))
    }

    return (
        <div>
            <label className="form-label">{label}</label>

            <input
                className="form-control ns-input mb-2"
                readOnly
                value={
                    selected.length > 0
                        ? selected.join(', ')
                        : 'No topics selected'
                }
            />

            {selected.length > 0 && (
                <div className="d-flex flex-wrap gap-2 mb-2">
                    {selected.map(t => (
                        <span
                            key={t}
                            className={clsx(
                                'ns-badge d-inline-flex align-items-center gap-2',
                                disabled && 'opacity-75'
                            )}
                        >
              <span>{t}</span>
                            {!disabled && (
                                <button
                                    type="button"
                                    className="btn btn-sm btn-link p-0 text-light"
                                    onClick={() => remove(t)}
                                    aria-label="remove topic"
                                    style={{lineHeight: 1, textDecoration: 'none'}}
                                >
                                    <i className="bi bi-x-lg"></i>
                                </button>
                            )}
            </span>
                    ))}
                </div>
            )}

            <div className="ns-card p-3" style={{maxHeight: 220, overflow: 'auto'}}>
                {options.length === 0 ? (
                    <div className="ns-muted small">Немає доступних тем</div>
                ) : (
                    <div className="d-flex flex-column gap-2">
                        {options.map(opt => {
                            const checked = selected.includes(opt)
                            return (
                                <label
                                    key={opt}
                                    className={clsx(
                                        'd-flex align-items-center gap-2',
                                        disabled && 'opacity-75'
                                    )}
                                >
                                    <input
                                        type="checkbox"
                                        className="form-check-input"
                                        checked={checked}
                                        onChange={() => toggle(opt)}
                                        disabled={disabled}
                                    />
                                    <span>{opt}</span>
                                </label>
                            )
                        })}
                    </div>
                )}
            </div>

            <div className="ns-muted small mt-2">
                Можна обрати декілька тем.
            </div>
        </div>
    )
}
