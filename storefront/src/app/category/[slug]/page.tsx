import { redirect } from "next/navigation";

const slugToQuery: Record<string, string> = {
  rings: "Rings",
  pendants: "Pendants",
  necklaces: "Necklaces",
  bracelets: "Bracelets",
  earrings: "Earrings",
};

export const dynamic = "force-dynamic";

export default async function CategoryPage({
  params,
}: {
  params: Promise<{ slug: string }>;
}) {
  const { slug } = await params;
  const query = slugToQuery[slug.toLowerCase()];

  // Catalog categories in DB may not exactly match these 5 UI categories.
  // For the deadline we keep this simple and filter by name via `searchName`.
  redirect(`/products?search=${encodeURIComponent(query ?? slug)}`);
}

