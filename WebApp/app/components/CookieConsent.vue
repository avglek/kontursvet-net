<template>
  <ClientOnly>
    <aside
      v-if="isPending"
      class="cookie-consent"
      :class="{ 'cookie-consent--hidden': isInputFocused }"
      role="region"
      aria-label="Cookie and analytics"
    >
      <div class="cookie-consent__body">
        <!-- Короткий текст + ссылка «Подробнее» — в одной строке -->
        <p class="cookie-consent__text">
          {{ t('cookieConsent.text') }}
          <NuxtLink class="cookie-consent__link" to="/privacy">
            {{ t('cookieConsent.link') }}
          </NuxtLink>
        </p>
      </div>

      <div class="cookie-consent__actions">
        <button
          type="button"
          class="button button-primary button-small cookie-consent__btn"
          @click="accept"
        >
          {{ t('cookieConsent.accept') }}
        </button>
        <button
          type="button"
          class="button button-ghost button-small cookie-consent__btn"
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
const { isPending, accept, decline } = useCookieConsent();

// Прячем баннер, когда пользователь печатает в поле формы —
// иначе он перекрывает клавиатуру и мешает вводу.
// Небольшая задержка нужна, чтобы избежать мерцания при переходе
// фокуса между полями (blur старого → focusin нового).
const isInputFocused = ref(false);
let focusTimer: ReturnType<typeof setTimeout> | null = null;

function checkFocus() {
  const el = document.activeElement;
  isInputFocused.value =
    el?.tagName === 'INPUT' ||
    el?.tagName === 'TEXTAREA' ||
    el?.tagName === 'SELECT';
}

function onInputFocus() {
  if (focusTimer) clearTimeout(focusTimer);
  focusTimer = setTimeout(checkFocus, 50);
}

onMounted(() => {
  document.addEventListener('focusin', onInputFocus, true);
  document.addEventListener('blur', onInputFocus, true);
});

onBeforeUnmount(() => {
  if (focusTimer) clearTimeout(focusTimer);
  document.removeEventListener('focusin', onInputFocus, true);
  document.removeEventListener('blur', onInputFocus, true);
});
</script>
