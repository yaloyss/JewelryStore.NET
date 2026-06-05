"use client";

import Image from "next/image";
import { useEffect, useMemo, useRef, useState } from "react";
import { cn } from "@/lib/utils";

type Slide = {
  title: string;
  subtitle: string;
  cta: { label: string; href: string };
  imageSrc: string;
};

const slides: Slide[] = [
  {
    title: "Quiet luxury, everyday.",
    subtitle: "Minimal forms. Soft accents. Crafted to last.",
    cta: { label: "Shop rings", href: "/category/rings" },
    imageSrc: "/hero/slide-1.jpg",
  },
  {
    title: "A small detail, a big feeling.",
    subtitle: "Pieces designed to stay with you.",
    cta: { label: "Explore pendants", href: "/category/pendants" },
    imageSrc: "/hero/slide-2.jpg",
  },
  {
    title: " ",
    subtitle: " ",
    cta: { label: "Shop necklaces", href: "/category/necklaces" },
    imageSrc: "/hero/slide-3.jpg",
  },
];

export function HeroCarousel() {
  const [active, setActive] = useState(0);
  const containerRef = useRef<HTMLDivElement | null>(null);

  const max = slides.length;
  const intervalMs = 5000;

  useEffect(() => {
    const id = window.setInterval(() => {
      setActive((v) => (v + 1) % max);
    }, intervalMs);
    return () => window.clearInterval(id);
  }, [max]);

  useEffect(() => {
    const el = containerRef.current;
    if (!el) return;
    el.scrollTo({ left: el.clientWidth * active, behavior: "smooth" });
  }, [active]);

  const ariaLabel = useMemo(() => `Hero banner, slide ${active + 1} of ${max}`, [active, max]);

  return (
    <section className="mx-auto w-full max-w-6xl px-4 pt-8" aria-label={ariaLabel}>
      <div className="overflow-hidden rounded-md border border-black/10 bg-white shadow-soft">
        <div
          ref={containerRef}
          className={cn(
            "flex w-full snap-x snap-mandatory overflow-x-auto scroll-smooth",
            "[scrollbar-width:none] [&::-webkit-scrollbar]:hidden"
          )}
        >
          {slides.map((s, idx) => (
            <div key={s.title} className="relative w-full flex-none snap-center">
              <div className="relative h-[360px] md:h-[420px]">
                <Image
                  src={s.imageSrc}
                  alt=""
                  fill
                  priority={idx === 0}
                  className="object-cover"
                />
                <div className="absolute inset-0 bg-gradient-to-r from-white/85 via-white/35 to-transparent" />
                <div className="absolute inset-0 flex items-center">
                  <div className="max-w-xl px-8 md:px-12">
                    <h1 className="text-3xl md:text-4xl font-semibold tracking-tight text-ink">
                      {s.title}
                    </h1>
                    <p className="mt-3 text-base md:text-lg leading-7 text-ink/80">
                      {s.subtitle}
                    </p>
                    <a
                      href={s.cta.href}
                      className="mt-6 inline-flex h-11 items-center rounded-md bg-accent px-5 text-sm font-semibold text-ink hover:opacity-95 focus:outline-none focus:ring-2 focus:ring-accent/50"
                    >
                      {s.cta.label}
                    </a>
                  </div>
                </div>
              </div>
            </div>
          ))}
        </div>

        <div className="flex items-center justify-center gap-2 py-3">
          {slides.map((_, idx) => (
            <button
              key={idx}
              type="button"
              className={cn(
                "h-2.5 w-2.5 rounded-full border border-black/20 transition",
                idx === active ? "bg-accent" : "bg-white hover:bg-black/5"
              )}
              aria-label={`Go to slide ${idx + 1}`}
              aria-current={idx === active}
              onClick={() => setActive(idx)}
            />
          ))}
        </div>
      </div>
    </section>
  );
}

