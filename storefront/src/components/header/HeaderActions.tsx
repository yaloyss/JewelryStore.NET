"use client";

import { useMemo, useState } from "react";
import { useRouter } from "next/navigation";
import * as Popover from "@radix-ui/react-popover";
import { MagnifyingGlassIcon, BackpackIcon } from "@radix-ui/react-icons";
import { cn } from "@/lib/utils";

function getCartCount() {
  try {
    const raw = localStorage.getItem("yumine_cart_v1");
    if (!raw) return 0;
    const parsed = JSON.parse(raw) as { items?: Array<{ quantity: number }> };
    return parsed.items?.reduce((sum, i) => sum + (i.quantity ?? 0), 0) ?? 0;
  } catch {
    return 0;
  }
}

export function HeaderActions() {
  const router = useRouter();
  const [query, setQuery] = useState("");
  const [cartCount, setCartCount] = useState(0);

  useMemo(() => {
    // client-only hydration of cart count
    setCartCount(getCartCount());
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  function submitSearch() {
    const trimmed = query.trim();
    if (!trimmed) {
      router.push("/products");
      return;
    }
    router.push(`/products?search=${encodeURIComponent(trimmed)}`);
  }

  return (
    <div className="flex items-center gap-3">
      <div className="hidden sm:block">
        <div className="flex h-10 items-center gap-2 rounded-md border border-black/10 bg-white px-3 focus-within:border-accent focus-within:ring-2 focus-within:ring-accent/30">
          <MagnifyingGlassIcon className="h-4 w-4 text-ink/70" />
          <input
            value={query}
            onChange={(e) => setQuery(e.target.value)}
            onKeyDown={(e) => {
              if (e.key === "Enter") submitSearch();
            }}
            placeholder="Search"
            className="w-48 bg-transparent text-sm text-ink placeholder:text-ink/40 outline-none"
            aria-label="Search products"
          />
        </div>
      </div>

      <Popover.Root>
        <Popover.Trigger asChild>
          <button
            type="button"
            className="relative inline-flex h-10 w-10 items-center justify-center rounded-md border border-black/10 bg-white hover:border-accent hover:ring-2 hover:ring-accent/30 transition"
            aria-label="Shopping cart"
          >
            <BackpackIcon className="h-5 w-5 text-ink" />
            {cartCount > 0 ? (
              <span className="absolute -right-1 -top-1 inline-flex h-5 min-w-5 items-center justify-center rounded-full bg-accent px-1 text-xs font-semibold text-ink">
                {cartCount}
              </span>
            ) : null}
          </button>
        </Popover.Trigger>

        <Popover.Portal>
          <Popover.Content
            align="end"
            sideOffset={10}
            className={cn(
              "w-[320px] rounded-md border border-black/10 bg-white p-4 shadow-soft",
              "data-[state=open]:animate-in data-[state=closed]:animate-out"
            )}
          >
            <div className="flex items-center justify-between">
              <div className="text-sm font-semibold text-ink">Cart</div>
              <Popover.Close
                className="rounded-md px-2 py-1 text-xs text-ink/70 hover:text-ink"
                aria-label="Close cart"
              >
                Close
              </Popover.Close>
            </div>
            <div className="mt-3 text-sm text-ink/70">
              Cart UI comes next (localStorage + checkout). For now this icon is wired for the
              minimalist layout.
            </div>
          </Popover.Content>
        </Popover.Portal>
      </Popover.Root>
    </div>
  );
}

