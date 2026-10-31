<template>
  <ClientOnly>
    <aside
      v-if="isPending"
      class="cookie-consent"
      role="region"
      :aria-label="t('cookieConsent.title')"
    >
      <div class="cookie-consent__body">
        <h2 class="cookie-consent__title">{{ t('cookieConsent.title') }}</h2>
        <p class="cookie-consent__text">{{ t('cookieConsent.text') }}</p>
        <!-- Ссылка на страницу политики конфиденциальности -->
        <NuxtLink class="cookie-consent__link" to="/privacy">
          {{ t('cookieConsent.link') }}
        </NuxtLink>
      </div>

      <div class="cookie-consent__actions">
        <button
          type="button"
          class="button button-primary button-small"
          @click="accept"
        >
          {{ t('cookieConsent.accept') }}
        </button>
        <button
          type="button"
          class="button button-ghost button-small"
          @click="decline"
        >
          {{ t('cookieConsent.decline') }}
        </button>
      </div>
    </aside>
  </ClientOnly>
</template>

<script setup lang="ts">
const { t } = useI18n();
// Баннер показывается, только пока ответа не было, поэтому в момент отказа
// счётчик заведомо не запущен — перезагрузка для вступления отказа в силу
// не нужна (она нужна при отзыве уже данного согласия — см. Footer.vue).
const { isPending, accept, decline } = useCookieConsent();
</script>
