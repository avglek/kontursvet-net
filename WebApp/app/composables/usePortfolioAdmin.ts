import imageCompression from 'browser-image-compression';
import { ApiError, useApi } from './useApi';
import type {
  PortfolioCardDto,
  PortfolioCardViewDto,
  StoredFile,
} from '~/types/api';

const MAX_SIZE = 10 * 1024 * 1024; // как на бэке: 10 МБ
const ALLOWED_TYPES = [
  'image/jpeg',
  'image/png',
  'image/webp',
  'image/gif',
  'image/svg+xml',
  'image/avif',
];

export const usePortfolioAdmin = () => {
  const api = useApi();

  const listCards = () =>
    api.get<PortfolioCardDto[]>('/api/profile/cards?skip=0&take=200');

  const getCard = (id: number) =>
    api.get<PortfolioCardDto>(`/api/profile/cards/${id}`);

  // У карточки может не быть представления — тогда бэк отдаёт 404, это не ошибка
  const getView = async (id: number): Promise<PortfolioCardViewDto | null> => {
    try {
      return await api.get<PortfolioCardViewDto>(
        `/api/profile/cardviews/${id}`,
        { skipErrorHandling: true },
      );
    } catch (err) {
      if (err instanceof ApiError && err.status === 404) return null;
      throw err;
    }
  };

  const createCard = (card: PortfolioCardDto) =>
    api.post<{ id: number }>('/api/profile/cards', card);

  const updateCard = (id: number, card: PortfolioCardDto) =>
    api.put<void>(`/api/profile/cards/${id}`, card);

  // PUT — upsert: создаёт представление, если его ещё нет
  const saveView = (id: number, view: PortfolioCardViewDto) =>
    api.put<void>(`/api/profile/cardviews/${id}`, view);

  // Каскадно удаляет и представление
  const deleteCard = (id: number) =>
    api.delete<void>(`/api/profile/cards/${id}`);

  /**
   * Загружает картинку на сервер. JPG/PNG сжимаются и конвертируются в WebP —
   * как и остальные фото на сайте.
   */
  const uploadImage = async (file: File): Promise<StoredFile> => {
    if (!ALLOWED_TYPES.includes(file.type)) {
      throw new Error('Допустимы форматы JPG, PNG, WebP, GIF, SVG и AVIF');
    }

    let body = file;
    if (file.type === 'image/jpeg' || file.type === 'image/png') {
      try {
        const compressed = await imageCompression(file, {
          maxWidthOrHeight: 2400,
          maxSizeMB: 2,
          useWebWorker: true,
          fileType: 'image/webp',
        });
        const name = file.name.replace(/\.[^.]+$/, '') + '.webp';
        body = new File([compressed], name, { type: 'image/webp' });
      } catch {
        body = file; // не вышло сжать — отправляем как есть
      }
    }

    if (body.size > MAX_SIZE) throw new Error('Файл больше 10 МБ');

    const formData = new FormData();
    formData.append('file', body, body.name);
    return api.post<StoredFile>('/api/files/upload', formData);
  };

  return {
    listCards,
    getCard,
    getView,
    createCard,
    updateCard,
    saveView,
    deleteCard,
    uploadImage,
  };
};
