<template>
  <!-- Плавающий бокс в правом нижнем углу: анимированная кнопка-gif,
       по клику открывает модальную форму обратного звонка. -->
  <button
    type="button"
    class="floating-lead"
    :class="{ 'floating-lead--lifted': isLifted }"
    :aria-label="t('callback.title')"
    aria-haspopup="dialog"
    @click="openCallback"
  >
    <!-- Картинка лежит в public и отдаётся как есть: NuxtImg прогоняет её
         через ipx, а тот схлопывает gif в первый кадр и теряет анимацию. -->
    <img
      class="floating-lead__image"
      src="/images/ks-button-animated.gif"
      alt=""
      width="404"
      height="238"
      decoding="async"
    />
  </button>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue';

const { isPending } = useCookieConsent();
const { openCallback } = useModals();
const { t } = useI18n();

// Баннер cookie-согласия живёт только на клиенте, поэтому смещение включаем
// после монтирования: серверная разметка остаётся базовой и не «расходится»
// с клиентской при гидрации.
const isMounted = ref(false);
onMounted(() => {
  isMounted.value = true;
});

// Пока посетитель не ответил баннеру, поднимаем бокс над ним — иначе он
// перекроет кнопки «Принять» / «Отклонить».
const isLifted = computed(() => isMounted.value && isPending.value);
</script>
