export default function Navbar({ onNavigateDesignSystem }) {
  return (
    <nav className="navbar">
      <div className="navbar-inner">
        <div className="navbar-links">
          <a href="#analisar" className="navbar-link">Analisar</a>
          <a href="#como-funciona" className="navbar-link">Como funciona</a>
          <a href="#formatos" className="navbar-link">Formatos</a>
          <a href="#sobre" className="navbar-link">Sobre</a>
          {onNavigateDesignSystem && (
            <a href="#" className="navbar-link" onClick={(e) => { e.preventDefault(); onNavigateDesignSystem(); }}>Design System</a>
          )}
        </div>
      </div>
    </nav>
  );
}
