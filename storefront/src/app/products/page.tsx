import Link from "next/link";
import Image from "next/image";
import { getProducts, productImageUrl } from "@/lib/api";

export const dynamic = "force-dynamic";

export const metadata = {
  title: "Products",
  description: "Browse YUMINE products.",
};

export default async function ProductsPage({
  searchParams,
}: {
  searchParams: Promise<Record<string, string | string[] | undefined>>;
}) {
  const sp = await searchParams;
  const search = typeof sp.search === "string" ? sp.search : undefined;
  const pageNumber = typeof sp.page === "string" ? Number(sp.page) : 1;

  const result = await getProducts({
    pageNumber: Number.isFinite(pageNumber) && pageNumber > 0 ? pageNumber : 1,
    pageSize: 24,
    searchName: search,
    orderBy: "price",
  });

  return (
    <div className="mx-auto w-full max-w-6xl px-4 py-10">
      <div className="flex items-end justify-between gap-4">
        <div>
          <h1 className="text-xl font-semibold tracking-tight text-ink">Products</h1>
          <p className="mt-1 text-sm text-ink/70">
            {search ? (
              <>
                Showing results for <span className="font-semibold text-ink">{search}</span>
              </>
            ) : (
              "Browse the catalog."
            )}
          </p>
        </div>
        <div className="text-sm text-ink/70">
          {result.totalCount} item{result.totalCount === 1 ? "" : "s"}
        </div>
      </div>

      <div className="mt-6 grid gap-5 sm:grid-cols-2 lg:grid-cols-4">
        {result.items.map((p) => (
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

      <div className="mt-10 flex items-center justify-center gap-2">
        {result.hasPrevious ? (
          <Link
            href={`/products?page=${Math.max(1, result.currentPage - 1)}${
              search ? `&search=${encodeURIComponent(search)}` : ""
            }`}
            className="inline-flex h-10 items-center rounded-md border border-black/10 bg-white px-4 text-sm font-semibold text-ink hover:border-accent"
          >
            Previous
          </Link>
        ) : null}
        {result.hasNext ? (
          <Link
            href={`/products?page=${result.currentPage + 1}${
              search ? `&search=${encodeURIComponent(search)}` : ""
            }`}
            className="inline-flex h-10 items-center rounded-md border border-black/10 bg-white px-4 text-sm font-semibold text-ink hover:border-accent"
          >
            Next
          </Link>
        ) : null}
      </div>
    </div>
  );
}

