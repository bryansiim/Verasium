import { useState } from "react";

const NAV_LINKS = [
  { id: "analisar", label: "Analisar" },
  { id: "como-funciona", label: "Como funciona" },
  { id: "formatos", label: "Formatos" },
  { id: "sobre", label: "Sobre" },
];

function scrollTo(e, id, onDone) {
  e.preventDefault();
  const el = document.getElementById(id);
  if (el) {
    el.scrollIntoView({ behavior: "smooth", block: "start" });
    history.replaceState(null, "", "/" + id);
  }
  onDone?.();
}

export default function Navbar() {
  const [open, setOpen] = useState(false);

  return (
    <nav className={`navbar${open ? " navbar-open" : ""}`}>
      <div className="navbar-inner">
        <div className="navbar-links">
          {NAV_LINKS.map((link) => (
            <a
              key={link.id}
              href={`/${link.id}`}
              className="navbar-link"
              onClick={(e) => scrollTo(e, link.id)}
            >
              {link.label}
            </a>
          ))}
        </div>
        <button
          className="navbar-toggle"
          onClick={() => setOpen(!open)}
          aria-label="Menu"
        >
          <span className="navbar-toggle-bar" />
          <span className="navbar-toggle-bar" />
          <span className="navbar-toggle-bar" />
        </button>
      </div>

      {open && (
        <div className="navbar-mobile">
          {NAV_LINKS.map((link) => (
            <a
              key={link.id}
              href={`/${link.id}`}
              className="navbar-mobile-link"
              onClick={(e) => scrollTo(e, link.id, () => setOpen(false))}
            >
              {link.label}
            </a>
          ))}
        </div>
      )}
    </nav>
  );
}
