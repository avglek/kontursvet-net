/**
 * Загруженные через админку файлы живут в общем volume и наружу их раздаёт
 * nginx (в dev — прокси Nuxt на API). Их нет в public-бандле Nuxt, поэтому
 * <NuxtImg> проксирует такой путь через ipx, который не находит файл и
 * отдаёт 404. Для /uploads/** рендерим обычный <img> — тогда браузер идёт
 * напрямую по /uploads/**, откуда файл приходит со стороны nginx.
 */
export const isUploaded = (src?: string | null): boolean =>
  !!src && src.startsWith('/uploads/');
