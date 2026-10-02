export default defineNuxtConfig({
  compatibilityDate: '2026-09-01',
  devtools: { enabled: false },

  // Отключаем загрузку шрифтов по сети
  // Устанавливаем шрифт npm install @fontsource-variable/inter
  modules: [
    '@nuxt/fonts',
    '@nuxt/image',
    '@stolbov.r/nuxt-font-loader',
    '@nuxtjs/i18n',
  ],

  fonts: {
    // 1. Указываем, какие шрифты брать из установленных npm-пакетов
    families: [{ name: 'Inter', provider: 'npm' }],
    // 2. Отключаем сетевой поиск unifont по реестру Fontsource, чтобы убрать ошибку
    // 1. Отключаем ВСЕ внешние сетевые API, которые вызывают ошибки
    providers: {
      google: false,
      fontsource: false,
      fontshare: false,
      bunny: false,
    },
  },

  // Загрузка локальных шрифтов.
  fontLoader: {
    local: [
      {
        family: 'Inter Display',
        src: '/fonts/InterDisplay-Bold.woff2',
        weight: '700',
        preload: true,
        display: 'swap',
        style: 'normal',
      },
      {
        family: 'Inter Display',
        src: '/fonts/InterDisplay-SemiBold.woff2',
        weight: '600',
        preload: true,
        display: 'swap',
        style: 'normal',
      },
      {
        family: 'Open Sans',
        src: '/fonts/OpenSans-Regular.woff2',
        weight: '400',
        preload: true,
        display: 'swap',
        style: 'normal',
      },
      {
        family: 'Open Sans',
        src: '/fonts/OpenSans-SemiBold.woff2',
        weight: '600',
        preload: true,
        display: 'swap',
        style: 'normal',
      },
    ],
  },
  // Проксирование API на .NET backend (в dev-режиме)
  $development: {
    runtimeConfig: {
      public: {
        apiBase: '', // пусто — значит, все запросы идут на /api/** и проксируются
      },
    },
    routeRules: {
      '/api/**': { proxy: 'http://localhost:5136/api/**' },
      '/uploads/**': { proxy: 'http://localhost:5136/uploads/**' },
    },
  },

  // Прод: apiBase из env
  runtimeConfig: {
    public: {
      apiBase: process.env.NUXT_PUBLIC_API_BASE || '',
    },
  },

  // Глобальные стили
  css: ['~/assets/scss/main.scss'],

  // i18
  i18n: {
    defaultLocale: 'ru',
    langDir: 'locales',
    locales: [
      { code: 'ru', language: 'ru-RU', file: 'ru.json' },
      { code: 'en', language: 'en-US', file: 'en.json' },
    ],
  },

  // nuxt.config.ts
  routeRules: {
    '/admin/**': { appLayout: 'admin', ssr: false },
  },
});
