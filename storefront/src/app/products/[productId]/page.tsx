import Image from "next/image";
import Link from "next/link";
import { getProductById, productImageUrl } from "@/lib/api";

export const dynamic = "force-dynamic";

export default async function ProductDetailsPage({
  params,
}: {
  params: Promise<{ productId: string }>;
}) {
  const { productId } = await params;
  const id = Number(productId);
  const product = await getProductById(id);

  return (
    <div className="mx-auto w-full max-w-6xl px-4 py-10">
      <div className="text-sm text-ink/70">
        <Link href="/products" className="hover:underline underline-offset-4">
          Products
        </Link>{" "}
        <span className="mx-2">/</span>
        <span className="text-ink">{product.name}</span>
      </div>

      <div className="mt-6 grid gap-10 lg:grid-cols-2">
        <div className="rounded-md border border-black/10 bg-white p-4 shadow-soft">
          <div className="relative aspect-square overflow-hidden rounded-md bg-black/5">
            <Image
              src={productImageUrl(product.productId)}
              alt={product.name}
              fill
              sizes="(max-width: 1024px) 100vw, 50vw"
              className="object-cover"
              priority
            />
          </div>
        </div>

        <div>
          <h1 className="text-2xl font-semibold tracking-tight text-ink">{product.name}</h1>
          <div className="mt-2 text-lg font-semibold text-ink">${product.price.toFixed(2)}</div>

          <div className="mt-6 grid grid-cols-2 gap-4 text-sm">
            <Spec label="Weight" value={`${product.weight} g`} />
            <Spec label="Size" value={product.size ? `${product.size}` : "—"} />
            <Spec label="Manufacturer" value={product.manufacturer ?? "—"} />
            <Spec label="CategoryId" value={`${product.categoryId}`} />
          </div>

          <div className="mt-8 flex gap-3">
            <button
              type="button"
              className="inline-flex h-11 items-center rounded-md bg-accent px-5 text-sm font-semibold text-ink hover:opacity-95 focus:outline-none focus:ring-2 focus:ring-accent/50"
            >
              Add to cart
            </button>
            <Link
              href="/cart"
              className="inline-flex h-11 items-center rounded-md border border-black/10 bg-white px-5 text-sm font-semibold text-ink hover:border-accent"
            >
              View cart
            </Link>
          </div>

          <div className="mt-10 rounded-md border border-black/10 bg-white p-5 shadow-soft">
            <div className="text-sm font-semibold text-ink">About</div>
            <p className="mt-2 text-sm leading-6 text-ink/80">
              Minimalist design with emphasis on proportion and finish. Choose from a variety of our products and sizes to find the perfect fit.
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}

function Spec({ label, value }: { label: string; value: string }) {
  return (
    <div className="rounded-md border border-black/10 bg-white p-4 shadow-soft">
      <div className="text-xs font-semibold uppercase tracking-wide text-ink/60">{label}</div>
      <div className="mt-1 text-sm font-semibold text-ink">{value}</div>
    </div>
  );
}

