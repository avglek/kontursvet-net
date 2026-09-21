// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  compatibilityDate: "2025-07-15",
  devtools: { enabled: true },
  experimental: { appManifest: true },
  routeRules: {
    "/api/**": {
      proxy: "http://localhost:5136/api/**",
    },
  },
});
