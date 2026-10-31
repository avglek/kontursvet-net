/**
 * Пути служебных разделов сайта.
 */

/**
 * Админ-панель (`/admin`, `/admin/portfolio`, локализованная `/en/admin/...`).
 * Визиты туда в статистику Яндекс Метрики не отправляются, и баннер согласия
 * на аналитические cookie там не показывается.
 */
export function isAdminPath(path: string): boolean {
  return /(^|\/)admin(\/|$)/.test(path);
}
