import { useEffect, useState } from "react";
import icon from "../../assets/icon.png";
import BrandSection from "./sections/BrandSection";
import TypographySection from "./sections/TypographySection";
import ColorsSection from "./sections/ColorsSection";
import SpacingSection from "./sections/SpacingSection";
import ComponentsSection from "./sections/ComponentsSection";
import LayoutsSection from "./sections/LayoutsSection";
import MotionSection from "./sections/MotionSection";
import AccessibilitySection from "./sections/AccessibilitySection";
import DontsSection from "./sections/DontsSection";
import "./DesignSystemScreen.css";

const NAV_ITEMS = [
  { id: "ds-brand", label: "Marca" },
  { id: "ds-typography", label: "Tipografia" },
  { id: "ds-colors", label: "Cores" },
  { id: "ds-spacing", label: "Espacamento" },
  { id: "ds-components", label: "Componentes" },
  { id: "ds-layouts", label: "Layouts" },
  { id: "ds-motion", label: "Motion" },
  { id: "ds-accessibility", label: "Acessibilidade" },
  { id: "ds-donts", label: "Nao fazer" },
];

export default function DesignSystemScreen({ darkMode, onToggleTheme, onBack }) {
  const [activeSection, setActiveSection] = useState("ds-brand");

  useEffect(() => {
    window.scrollTo(0, 0);
  }, []);

  // Intersection observer to track active section
  useEffect(() => {
    const observer = new IntersectionObserver(
      (entries) => {
        for (const entry of entries) {
          if (entry.isIntersecting) {
            setActiveSection(entry.target.id);
          }
        }
      },
      { rootMargin: "-20% 0px -60% 0px" }
    );

    NAV_ITEMS.forEach(({ id }) => {
      const el = document.getElementById(id);
      if (el) observer.observe(el);
    });

    return () => observer.disconnect();
  }, []);

  const scrollToSection = (id) => {
    const el = document.getElementById(id);
    if (el) {
      el.scrollIntoView({ behavior: "smooth", block: "start" });
    }
  };

  return (
    <div className="ds-page">
      {/* Top navbar */}
      <nav className="ds-topbar">
        <div className="ds-topbar-inner">
          <button className="ds-back-btn" onClick={onBack} title="Voltar ao Verasium">
            <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
              <line x1="19" y1="12" x2="5" y2="12" />
              <polyline points="12 19 5 12 12 5" />
            </svg>
            <img src={icon} alt="Verasium" className="ds-topbar-logo" draggable={false} />
            <span className="ds-topbar-title">Design System</span>
          </button>

          <button
            className="ds-theme-toggle"
            onClick={onToggleTheme}
            title={darkMode ? "Modo claro" : "Modo escuro"}
            aria-label={darkMode ? "Ativar modo claro" : "Ativar modo escuro"}
          >
            {darkMode ? (
              <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                <circle cx="12" cy="12" r="5" />
                <line x1="12" y1="1" x2="12" y2="3" />
                <line x1="12" y1="21" x2="12" y2="23" />
                <line x1="4.22" y1="4.22" x2="5.64" y2="5.64" />
                <line x1="18.36" y1="18.36" x2="19.78" y2="19.78" />
                <line x1="1" y1="12" x2="3" y2="12" />
                <line x1="21" y1="12" x2="23" y2="12" />
                <line x1="4.22" y1="19.78" x2="5.64" y2="18.36" />
                <line x1="18.36" y1="5.64" x2="19.78" y2="4.22" />
              </svg>
            ) : (
              <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                <path d="M21 12.79A9 9 0 1 1 11.21 3 7 7 0 0 0 21 12.79z" />
              </svg>
            )}
          </button>
        </div>
      </nav>

      <div className="ds-layout">
        {/* Sidebar navigation */}
        <aside className="ds-sidebar">
          <nav className="ds-sidebar-nav">
            {NAV_ITEMS.map(({ id, label }) => (
              <button
                key={id}
                className={`ds-sidebar-link ${activeSection === id ? "active" : ""}`}
                onClick={() => scrollToSection(id)}
              >
                {label}
              </button>
            ))}
          </nav>
        </aside>

        {/* Main content */}
        <main className="ds-main">
          <div className="ds-hero">
            <h1 className="ds-hero-title">Verasium Design System</h1>
            <p className="ds-hero-desc">
              Fonte unica de verdade visual do produto. Todas as definicoes de marca, tipografia,
              cores, componentes e padroes de interface do Verasium.
            </p>
          </div>

          <BrandSection />
          <TypographySection />
          <ColorsSection />
          <SpacingSection />
          <ComponentsSection />
          <LayoutsSection />
          <MotionSection />
          <AccessibilitySection />
          <DontsSection />
        </main>
      </div>
    </div>
  );
}
