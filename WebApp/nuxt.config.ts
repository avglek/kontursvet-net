// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  compatibilityDate: "2025-07-15",
  devtools: { enabled: true },

  routeRules: {
    "/api/**": {
      proxy: "http://localhost:5096/api/**", // Замените 5096 на порт вашего .NET API
    },
  },
});
