<template>
  <div>
    <Header />
    <slot />
    <Footer />
    <ImageLightbox />
    <!-- Диалог обратного звонка: открывается плавающей кнопкой (useModals) -->
    <FormCallbackModal />
  </div>
</template>

<script lang="ts" setup></script>

<style lang="scss">
// Баннер cookie-согласия прижат к нижнему краю и занимает ~90px.
// Добавляем отступ снизу, чтобы кнопки отправки форм не перекрывались.
main {
  padding-bottom: 100px;
}
</style>
