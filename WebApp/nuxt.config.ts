export default defineNuxtConfig({
  compatibilityDate: "2026-09-01",

  // Проксирование API на .NET backend (в dev-режиме)
  $development: {
    runtimeConfig: {
      public: {
        apiBase: "", // пусто — значит, все запросы идут на /api/** и проксируются
      },
    },
    routeRules: {
      "/api/**": { proxy: "http://localhost:5096/api/**" },
      "/uploads/**": { proxy: "http://localhost:5096/uploads/**" },
    },
  },

  // Прод: apiBase из env
  runtimeConfig: {
    public: {
      apiBase: process.env.NUXT_PUBLIC_API_BASE || "",
    },
  },

  modules: [// опционально, если позже понадобится
  "@pinia/nuxt", "@nuxt/ui"],

  css: ["~/assets/css/main.css"],

  devtools: { enabled: true },
});