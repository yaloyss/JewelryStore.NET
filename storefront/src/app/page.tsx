import Link from "next/link";
import { HeroCarousel } from "@/components/home/HeroCarousel";
import { getProducts, productImageUrl } from "@/lib/api";
import Image from "next/image";

export const dynamic = "force-dynamic";

export default async function Home() {
  const productsPromise = getProducts({ pageSize: 8, orderBy: "price" });

  return (
    <div className="pb-14">
      <HeroCarousel />

      <section className="mx-auto w-full max-w-6xl px-4 pt-12">
        <div className="flex items-end justify-between gap-4">
          <div>
            <h2 className="text-lg font-semibold tracking-tight text-ink">Featured pieces</h2>
            <p className="mt-1 text-sm text-ink/70">
              A small selection from the catalog. Built for clarity and product focus.
            </p>
          </div>
          <Link
            href="/products"
            className="text-sm font-semibold text-ink hover:underline underline-offset-4"
          >
            View all
          </Link>
        </div>

        <FeaturedGrid productsPromise={productsPromise} />
      </section>
    </div>
  );
}

async function FeaturedGrid({
  productsPromise,
}: {
  productsPromise: ReturnType<typeof getProducts>;
}) {
  const products = await productsPromise;
  return (
    <div className="mt-6 grid gap-5 sm:grid-cols-2 lg:grid-cols-4">
      {products.items.map((p) => (
        <Link
          key={p.productId}
          href={`/products/${p.productId}`}
          className="group rounded-md border border-black/10 bg-white p-3 shadow-soft transition hover:-translate-y-0.5 hover:border-accent"
        >
          <div className="relative aspect-square overflow-hidden rounded-md bg-black/5">
            <Image
              src={productImageUrl(p.productId)}
              alt={p.name}
              fill
              sizes="(max-width: 1024px) 50vw, 25vw"
              className="object-cover transition group-hover:scale-[1.02]"
            />
          </div>
          <div className="mt-3">
            <div className="line-clamp-1 text-sm font-semibold text-ink">{p.name}</div>
            <div className="mt-1 text-sm text-ink/70">${p.price.toFixed(2)}</div>
          </div>
        </Link>
      ))}
    </div>
  );
}
