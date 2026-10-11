import { useApi } from './useApi';
import type { ProductCardDto } from '~/types/api';

export const useProductsAdmin = () => {
  const api = useApi();

  const listProducts = () =>
    api.get<ProductCardDto[]>('/api/products?skip=0&take=200');

  const getProduct = (id: number) =>
    api.get<ProductCardDto>(`/api/products/${id}`);

  const createProduct = (dto: ProductCardDto) =>
    api.post<{ id: number }>('/api/products', dto);

  const updateProduct = (id: number, dto: ProductCardDto) =>
    api.put<void>(`/api/products/${id}`, dto);

  const deleteProduct = (id: number) =>
    api.delete<void>(`/api/products/${id}`);

  return { listProducts, getProduct, createProduct, updateProduct, deleteProduct };
};
