export default defineNuxtConfig({
  compatibilityDate: "2026-09-01",
  devtools: { enabled: false },

  // Отключаем загрузку шрифтов по сети
  // Устанавливаем шрифт npm install @fontsource-variable/inter
  modules: ["@nuxt/fonts"],

  fonts: {
    // 1. Указываем, какие шрифты брать из установленных npm-пакетов
    families: [{ name: "Inter", provider: "npm" }],
    // 2. Отключаем сетевой поиск unifont по реестру Fontsource, чтобы убрать ошибку
    // 1. Отключаем ВСЕ внешние сетевые API, которые вызывают ошибки
    providers: {
      google: false,
      fontsource: false,
      fontshare: false,
      bunny: false,
    },
  },
  // Проксирование API на .NET backend (в dev-режиме)
  $development: {
    runtimeConfig: {
      public: {
        apiBase: "", // пусто — значит, все запросы идут на /api/** и проксируются
      },
    },
    routeRules: {
      "/api/**": { proxy: "http://localhost:5136/api/**" },
      "/uploads/**": { proxy: "http://localhost:5136/uploads/**" },
    },
  },

  // Прод: apiBase из env
  runtimeConfig: {
    public: {
      apiBase: process.env.NUXT_PUBLIC_API_BASE || "",
    },
  },

  // Глобальные стили
  css: ["~/assets/scss/main.scss"],
});
