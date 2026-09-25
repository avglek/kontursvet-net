import type { StoredFile } from '~/types/api';
import { useApi } from './useApi';
import imageCompression from 'browser-image-compression';

interface LeadForm {
  name: string;
  phoneDigital: string;
  phoneFormat: string;
  home: string;
  location: string;
  message: string;
}

interface LeadAttachmentPayload {
  filename: string;
  url: string;
  contentType: string;
}

export const useLeads = () => {
  const api = useApi();

  const uploadUrl = '/api/files/upload';
  const sendUrl = '/api/messages';

  // Опции сжатия для изображений
  const compressionOptions = {
    maxSizeMB: 1,
    maxWidthOrHeight: 1920,
    useWebWorker: true,
  };

  /**
   * Загружает один файл и возвращает его URL.
   */
  const uploadFile = async (file: File): Promise<StoredFile> => {
    const formData = new FormData();
    const compressedFile = await imageCompression(file, compressionOptions);
    formData.append('files', compressedFile, compressedFile.name);

    return api.post<StoredFile>(uploadUrl, formData);
  };

  /**
   * Загружает все файлы параллельно.
   * Если хотя бы один упал — выбрасываем ошибку, ничего не отправляем.
   */
  const uploadAll = async (files: File[]): Promise<LeadAttachmentPayload[]> => {
    if (files.length === 0) return [];

    const results = await Promise.all(files.map(uploadFile));
    return results.map((r: LeadAttachmentPayload, i: number) => ({
      filename: files[i]!.name,
      url: r.url,
      contentType: r.contentType,
    }));
  };

  /**
   * Единая точка входа: загрузка файлов → отправка лида.
   */
  const sendLead = async (form: LeadForm, files: File[]) => {
    // 1. Загрузка файлов (если есть)
    const attachments = await uploadAll(files);

    // 2. Отправка лида одним JSON-запросом
    const payload = {
      text: {
        name: form.name,
        phone: { digital: form.phoneDigital, format: form.phoneFormat },
        home: form.home,
        location: form.location,
        message: form.message,
      },
      attachments,
    };

    return api.post<{ id: number }>(sendUrl, payload);
  };

  return { sendLead, uploadFile };
};
