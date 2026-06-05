import Link from "next/link";
import { HeaderActions } from "@/components/header/HeaderActions";

const navItems = [
  { label: "Rings", href: "/category/rings" },
  { label: "Pendants", href: "/category/pendants" },
  { label: "Necklaces", href: "/category/necklaces" },
  { label: "Bracelets", href: "/category/bracelets" },
  { label: "Earrings", href: "/category/earrings" },
] as const;

export function SiteHeader() {
  return (
    <header className="sticky top-0 z-50 bg-white/80 backdrop-blur supports-[backdrop-filter]:bg-white/60">
      <div className="mx-auto w-full max-w-6xl px-4">
        <div className="grid h-20 grid-cols-[1fr_auto_1fr] items-center gap-4 border-b border-black/10">
          <nav className="hidden justify-self-start md:flex md:items-center md:gap-5">
            {navItems.map((item) => (
              <Link
                key={item.href}
                href={item.href}
                className="text-sm font-medium text-ink/90 hover:text-ink transition-colors"
              >
                {item.label}
              </Link>
            ))}
          </nav>

          <Link
            href="/"
            className="justify-self-center text-xl font-semibold tracking-[0.25em] text-ink"
            aria-label="YUMINE home"
          >
            YUMINE
          </Link>

          <div className="justify-self-end">
            <HeaderActions />
          </div>
        </div>
      </div>
    </header>
  );
}

