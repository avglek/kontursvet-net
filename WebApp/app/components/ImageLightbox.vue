<template>
  <Teleport to="body">
    <Transition name="fade">
      <!-- Показывается только если activeImageSrc не пустой -->
      <div
        v-if="activeImageSrc"
        class="lightbox-overlay"
        @click.self="closeLightbox"
      >
        <button class="close-btn" @click="closeLightbox" aria-label="Закрыть">
          &times;
        </button>
        <img :src="activeImageSrc" alt="Полный размер" class="fullsize-img" />
      </div>
    </Transition>
  </Teleport>
</template>

<script lang="ts" setup>
import { watch } from 'vue';
import { useLightbox } from '~/composables/useLightbox';

const { activeImageSrc, closeLightbox } = useLightbox();

// Блокируем скролл страницы на мобильных и ПК при открытии
watch(activeImageSrc, (newValue) => {
  if (typeof window !== 'undefined') {
    document.body.style.overflow = newValue ? 'hidden' : '';
  }
});
</script>

<style scoped>
.lightbox-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100vw;
  height: 100vh;
  background-color: rgba(0, 0, 0, 0.95);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 99999; /* Поверх всего */
  padding: 16px;
  touch-action: none;
}

.fullsize-img {
  max-width: 100%;
  max-height: 100%;
  object-fit: contain;
  user-select: none;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.5);
}

.close-btn {
  position: absolute;
  top: 16px;
  right: 20px;
  background: none;
  border: none;
  color: #fff;
  font-size: 44px;
  line-height: 1;
  cursor: pointer;
  padding: 10px;
}

.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.2s ease;
}
.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}

@media (max-width: 768px) {
  .close-btn {
    top: 10px;
    right: 12px;
    font-size: 38px;
  }
}
</style>
