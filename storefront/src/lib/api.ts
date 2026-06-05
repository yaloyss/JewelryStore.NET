export type ProductDto = {
  productId: number;
  name: string;
  price: number;
  weight: number;
  size?: number | null;
  manufacturer?: string | null;
  metalId?: number | null;
  categoryId: number;
};

export type PagedResponse<T> = {
  items: T[];
  currentPage: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasNext: boolean;
  hasPrevious: boolean;
};

function apiBaseUrl() {
  return process.env.NEXT_PUBLIC_API_BASE_URL?.replace(/\/$/, "") ?? "http://localhost:5010";
}

export function productImageUrl(productId: number) {
  // Convention-based images (served from backend later):
  // /images/products/{productId}.webp (fallback .jpg)
  const base = apiBaseUrl();
  return `${base}/images/products/${productId}.webp`;
}

export async function getProducts(params?: {
  pageNumber?: number;
  pageSize?: number;
  categoryId?: number;
  metalId?: number;
  minPrice?: number;
  maxPrice?: number;
  searchName?: string;
  orderBy?: string;
}): Promise<PagedResponse<ProductDto>> {
  const base = apiBaseUrl();
  const url = new URL(`${base}/api/Products`);

  const entries: Record<string, string | number | undefined | null> = {
    pageNumber: params?.pageNumber ?? 1,
    pageSize: params?.pageSize ?? 12,
    categoryId: params?.categoryId,
    metalId: params?.metalId,
    minPrice: params?.minPrice,
    maxPrice: params?.maxPrice,
    searchName: params?.searchName,
    orderBy: params?.orderBy,
  };

  for (const [k, v] of Object.entries(entries)) {
    if (v !== undefined && v !== null && `${v}`.length > 0) {
      url.searchParams.set(k, `${v}`);
    }
  }

  const res = await fetch(url, { next: { revalidate: 60 } });
  if (!res.ok) {
    throw new Error(`Failed to fetch products: ${res.status}`);
  }
  return (await res.json()) as PagedResponse<ProductDto>;
}

export async function getProductById(productId: number): Promise<ProductDto> {
  const base = apiBaseUrl();
  const res = await fetch(`${base}/api/Products/${productId}`, { next: { revalidate: 60 } });
  if (!res.ok) {
    throw new Error(`Failed to fetch product ${productId}: ${res.status}`);
  }
  return (await res.json()) as ProductDto;
}

