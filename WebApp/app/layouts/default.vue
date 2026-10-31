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

<style></style>
