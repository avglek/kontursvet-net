import type { LeadTokenAttachment } from '~/types/api';
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

  const uploadUrl = '/api/lead/upload';
  const sendUrl = '/api/lead/messages';

  const uidMessage = crypto.randomUUID();

  // Опции сжатия для изображений
  const compressionOptions = {
    maxSizeMB: 1,
    maxWidthOrHeight: 1920,
    useWebWorker: true,
  };

  /**
   * Загружает один файл и возвращает его token.
   */
  const uploadFile = async (file: File): Promise<LeadTokenAttachment> => {
    const formData = new FormData();
    const compressedFile = await imageCompression(file, compressionOptions);
    formData.append('file', compressedFile, compressedFile.name);
    //formData.append('uuid', uidMessage);

    return api.post<LeadTokenAttachment>(uploadUrl, formData);
  };

  /**
   * Загружает все файлы параллельно.
   * Если хотя бы один упал — выбрасываем ошибку, ничего не отправляем.
   */
  const uploadAll = async (files: File[]): Promise<LeadTokenAttachment[]> => {
    if (files.length === 0) return [];

    const results = await Promise.all(files.map(uploadFile));

    return results.map((r: LeadTokenAttachment, i: number) => ({
      fileName: files[i]!.name,
      token: r.token,
      contentType: r.contentType,
      leadId: r.leadId,
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
