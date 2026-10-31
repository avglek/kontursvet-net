<template>
  <footer class="site-footer">
    <div class="container footer-row">
      <NuxtImg src="/images/icon.png" alt="КонтурСвет — Архитектура света" />
      <p>Санкт-Петербург и Ленинградская область</p>
      <p>Гарантия 1 год · далее постгарантийное обслуживание</p>
      <NuxtLink class="footer-nav-link" to="/privacy">
        {{ t('footer.privacy') }}
      </NuxtLink>
      <!-- Отзыв согласия: сбрасываем выбор и перезагружаемся — баннер
           появляется снова, а счётчик гарантированно останавливается.
           ClientOnly: решение лежит в localStorage, на SSR его нет — без
           обёртки получилась бы рассинхронизация гидрации -->
      <ClientOnly>
        <button
          v-if="consentWasGiven"
          type="button"
          class="footer-nav-link"
          @click="onCookieSettings"
        >
          {{ t('footer.cookieSettings') }}
        </button>
      </ClientOnly>
    </div>
  </footer>
</template>

<script setup lang="ts">
const { t } = useI18n();
const { choice, reset } = useCookieConsent();

/** Кнопка нужна, только если посетитель уже что-то выбрал. */
const consentWasGiven = computed(() => choice.value !== null);

function onCookieSettings() {
  reset();
  location.reload();
}
</script>
