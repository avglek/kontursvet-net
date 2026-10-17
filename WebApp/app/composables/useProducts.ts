import type { ProductCardDto } from "~/types/api";

/**
 * Загружает активные товары с ненулевой ценой и возвращает
 * список + словарь code → price для быстрого доступа.
 */
export function useProducts() {
  const api = useApi();
  const products = ref<ProductCardDto[]>([]);
  const priceByCode = computed(() => {
    const map = new Map<string, number>();
    for (const p of products.value) {
      if (p.isActive && p.price > 0) map.set(p.code, p.price);
    }
    return map;
  });
  const loading = ref(true);
  const error = ref<string | null>(null);

  const fetchProducts = async () => {
    try {
      products.value = await api.get<ProductCardDto[]>(
        "/api/products?skip=0&take=200"
      );
    } catch (err) {
      error.value =
        err instanceof Error ? err.message : "Не удалось загрузить прайс";
    } finally {
      loading.value = false;
    }
  };

  return { products, priceByCode, loading, error, fetchProducts };
}
