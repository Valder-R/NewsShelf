export function AppFooter() {
  return (
    <footer className="ns-footer mt-auto py-3 mt-4">
      <div className="container d-flex flex-column flex-md-row gap-2 justify-content-between align-items-center">
        <div className="ns-muted small">© {new Date().getFullYear()} NewsShelf</div>
        <div className="ns-muted small">React + Bootstrap UI для мікросервісного бекенду</div>
      </div>
    </footer>
  )
}