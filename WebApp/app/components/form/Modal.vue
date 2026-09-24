<template>
  <!-- <div class="full-screen-page"> -->
  <div id="modal" @click="$emit('modalClose')">
    <div class="modal__window">
      <span class="modal__close" @click="$emit('modalClose')">X</span>
      <h2>{{ title }}</h2>
      <p>{{ message }}</p>
    </div>
  </div>
  <!-- </div> -->
</template>

<style scoped>
#modal {
  position: absolute;
  top: 0;
  left: 0;
  height: 100%;
  width: 100%;
  display: flex;
  justify-content: center;
  align-items: center;
  background-color: rgba(255, 255, 255, 0.3);
}
.modal__window {
  position: relative;
  background-color: #081b33;
  border: 1px solid rgba(22, 168, 224, 0.32);
  padding: 4em 2em;
}

.modal__close {
  position: absolute;
  cursor: pointer;
  top: 5px;
  right: 15px;
  color: #fefefe;
  font-weight: 800;
}
.modal__window p {
  color: var(--accent);
  font-weight: 400;
  letter-spacing: 0.16em;
}
.modal__window h2 {
  font-size: 2.4rem;
  line-height: 1.08;
  margin: 0 0 3.8rem 0;
}
@media (max-width: 820px) {
  #modal {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    /* 100vh может баговать в мобильных браузерах из-за адресной строки, dvh решает это */
    height: 100dvh;
    background-color: rgba(255, 255, 255, 0.3); /* Полупрозрачный фон */

    /* Центрирование контента */
    display: flex;
    justify-content: center;
    align-items: center;
    z-index: 9999; /* Чтобы спиннер был поверх всех элементов */
  }
}
</style>

<script lang="ts" setup>
defineProps<{
  message: string;
  title: string;
}>();
</script>
