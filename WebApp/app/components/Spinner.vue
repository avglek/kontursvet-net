<template>
  <div :class="{ 'overlay-loader': true, 'overlay-loader-bg': isOverlay }">
    <div class="lds-spinner">
      <div></div>
      <div></div>
      <div></div>
      <div></div>
      <div></div>
      <div></div>
      <div></div>
      <div></div>
      <div></div>
      <div></div>
      <div></div>
      <div></div>
    </div>
  </div>
</template>

<script lang="ts" setup>
const props = defineProps<{
  isOverlay?: {
    type: boolean;
    default: false;
  };
}>();
</script>

<style scoped>
/* 1. Задний фон-подложка на весь экран */
.overlay-loader {
  position: fixed;
  top: 0;
  left: 0;
  width: 100vw;
  /* dynamic vh (dvh) идеально подходит для мобильных, 
     так как учитывает появление/скрытие адресной строки браузера */
  height: 100dvh;

  /* Центрирование спиннера */
  display: flex;
  justify-content: center;
  align-items: center;

  /* Гарантирует, что лоадер будет поверх модалок, меню и шапки */
  z-index: 9999;
}
.overlay-loader-bg {
  background-color: rgba(255, 255, 255, 0.5); /* Полупрозрачный белый фон */
}

.lds-spinner,
.lds-spinner div,
.lds-spinner div:after {
  box-sizing: border-box;
}
.lds-spinner {
  color: #0b213b;
  display: inline-block;
  position: relative;
  width: 80px;
  height: 80px;
}
.lds-spinner div {
  transform-origin: 40px 40px;
  animation: lds-spinner 1.2s linear infinite;
}
.lds-spinner div:after {
  content: ' ';
  display: block;
  position: absolute;
  top: 3.2px;
  left: 36.8px;
  width: 6.4px;
  height: 17.6px;
  border-radius: 20%;
  background: #0b213b;
}
.lds-spinner div:nth-child(1) {
  transform: rotate(0deg);
  animation-delay: -1.1s;
}
.lds-spinner div:nth-child(2) {
  transform: rotate(30deg);
  animation-delay: -1s;
}
.lds-spinner div:nth-child(3) {
  transform: rotate(60deg);
  animation-delay: -0.9s;
}
.lds-spinner div:nth-child(4) {
  transform: rotate(90deg);
  animation-delay: -0.8s;
}
.lds-spinner div:nth-child(5) {
  transform: rotate(120deg);
  animation-delay: -0.7s;
}
.lds-spinner div:nth-child(6) {
  transform: rotate(150deg);
  animation-delay: -0.6s;
}
.lds-spinner div:nth-child(7) {
  transform: rotate(180deg);
  animation-delay: -0.5s;
}
.lds-spinner div:nth-child(8) {
  transform: rotate(210deg);
  animation-delay: -0.4s;
}
.lds-spinner div:nth-child(9) {
  transform: rotate(240deg);
  animation-delay: -0.3s;
}
.lds-spinner div:nth-child(10) {
  transform: rotate(270deg);
  animation-delay: -0.2s;
}
.lds-spinner div:nth-child(11) {
  transform: rotate(300deg);
  animation-delay: -0.1s;
}
.lds-spinner div:nth-child(12) {
  transform: rotate(330deg);
  animation-delay: 0s;
}
@keyframes lds-spinner {
  0% {
    opacity: 1;
  }
  100% {
    opacity: 0;
  }
}

@media (max-width: 820px) {
  .full-screen-page {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    /* 100vh может баговать в мобильных браузерах из-за адресной строки, dvh решает это */
    height: 100dvh;
    background-color: rgba(255, 255, 255, 0.7); /* Полупрозрачный фон */

    /* Центрирование контента */
    display: flex;
    justify-content: center;
    align-items: center;
  }
}
</style>
