import type { error } from "winston";

export default defineNuxtPlugin((nuxtApp) => {
  // Функция для отправки логов на сервер
  const sendLogToServer = async (
    level: string,
    message: string,
    details: any,
  ) => {
    try {
      // Используем стандартный fetch, чтобы не вызывать циклических зависимостей $fetch
      await fetch('/api/logger', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          level,
          message,
          details,
          url: window.location.href,
        }),
      });
    } catch (e) {
      console.error('Не удалось отправить лог на сервер:', e);
    }
  };

  // 1. Перехват ошибок Vue (внутри компонентов, хуков жизненного цикла, методов)
  nuxtApp.vueApp.config.errorHandler = (error: any, instance, info) => {
    console.error(error); // Оставляем в консоли разработчика

    sendLogToServer('error', error.message || 'Vue Error', {
      stack: error.stack,
      info: info, // Например: "mounted hook", "template render"
    });
  };

  // 2. Перехват глобальных ошибок браузера (скрипты, сторонние библиотеки)
  window.addEventListener('error', (event) => {
    sendLogToServer('error', event.message, {
      filename: event.filename,
      lineno: event.lineno,
    });
  });

  // 3. Перехват неотловленных промисов на фронте
  window.addEventListener('unhandledrejection', (event) => {
    sendLogToServer('error', 'Unhandled Promise Rejection', {
      reason: event.reason?.message || event.reason,
    });
  });

  // Предоставляем глобальный хелпер $clientLog для ручного логирования событий
  return {
    provide: {
      clientLog: {
        info: (msg: string, data?: any) => sendLogToServer('info', msg, data),
        warn: (msg: string, data?: any) => sendLogToServer('warn', msg, data),
        err: (msg: string, data?: any) => sendLogToServer('error', msg, data),
      },
    },
  };
});
