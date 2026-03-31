function scrollTo(e, id) {
  e.preventDefault();
  const el = document.getElementById(id);
  if (el) {
    el.scrollIntoView({ behavior: "smooth", block: "start" });
    history.replaceState(null, "", "/" + id);
  }
}

export default function Navbar() {
  return (
    <nav className="navbar">
      <div className="navbar-inner">
        <div className="navbar-links">
          <a href="/analisar" className="navbar-link" onClick={(e) => scrollTo(e, "analisar")}>Analisar</a>
          <a href="/como-funciona" className="navbar-link" onClick={(e) => scrollTo(e, "como-funciona")}>Como funciona</a>
          <a href="/formatos" className="navbar-link" onClick={(e) => scrollTo(e, "formatos")}>Formatos</a>
          <a href="/sobre" className="navbar-link" onClick={(e) => scrollTo(e, "sobre")}>Sobre</a>
        </div>
      </div>
    </nav>
  );
}
