import Link from "next/link";

const navItems = [
  { label: "Rings", href: "/category/rings" },
  { label: "Pendants", href: "/category/pendants" },
  { label: "Necklaces", href: "/category/necklaces" },
  { label: "Bracelets", href: "/category/bracelets" },
  { label: "Earrings", href: "/category/earrings" },
] as const;

export function SiteFooter() {
  return (
    <footer className="bg-ink text-white">
      <div className="mx-auto w-full max-w-6xl px-4 py-12">
        <div className="grid gap-10 md:grid-cols-3">
          <div>
            <div className="text-sm font-semibold tracking-[0.25em]">YUMINE</div>
            <p className="mt-3 text-sm text-white/80 leading-6">
              Minimalist jewelry, crafted to be timeless. Simple forms. Quiet luxury.
            </p>
          </div>

          <div>
            <div className="text-sm font-semibold">Navigation</div>
            <div className="mt-3 grid grid-cols-2 gap-2 text-sm">
              {navItems.map((item) => (
                <Link key={item.href} href={item.href} className="text-white/80 hover:text-white">
                  {item.label}
                </Link>
              ))}
              <Link href="/products" className="text-white/80 hover:text-white">
                All Products
              </Link>
            </div>
          </div>

          <div>
            <div className="text-sm font-semibold">Contact</div>
            <div className="mt-3 space-y-2 text-sm text-white/80">
              <div>
                Phone:{" "}
                <a className="text-white hover:underline" href="tel:+10000000000">
                  +1 (000) 000-0000
                </a>
              </div>
              <div>
                Email:{" "}
                <a className="text-white hover:underline" href="mailto:hello@yumine.example">
                  hello@yumine.example
                </a>
              </div>
              <div>
                Instagram:{" "}
                <a
                  className="text-white hover:underline"
                  href="https://instagram.com/"
                  target="_blank"
                  rel="noreferrer"
                >
                  @yumine
                </a>
              </div>
            </div>
          </div>
        </div>

        <div className="mt-10 border-t border-white/10 pt-6 text-xs text-white/60">
          © {new Date().getFullYear()} YUMINE. All rights reserved.
        </div>
      </div>
    </footer>
  );
}

