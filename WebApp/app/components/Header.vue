<template>
  <header class="site-header">
    <div class="container nav-row" ref="menuRef">
      <NuxtLink
        class="brand"
        href="/#top"
        aria-label="КонтурСвет — на главную"
        @click="closeMenu()"
        ><NuxtImg src="/images/icon.png" alt="КонтурСвет — Архитектура света"
      /></NuxtLink>
      <button
        class="mobile-toggle"
        type="button"
        @click="onMobileClick()"
        :aria-expanded="isOpenMenu ? 'true' : 'false'"
      >
        <span></span><span></span>
      </button>
      <nav
        id="main-navigation"
        class="nav-links"
        aria-label="Основная навигация"
        v-if="isOpenMenu || !isMobileScreen"
        @click="closeMenu()"
      >
        <NuxtLink href="/#projects">Проекты</NuxtLink
        ><NuxtLink href="/#solutions">Решения</NuxtLink
        ><NuxtLink href="/#cases">Кейсы</NuxtLink
        ><NuxtLink href="/#process">Как работаем</NuxtLink
        ><NuxtLink href="/#contact">Контакты</NuxtLink>
        <NuxtLink no-prefetch class="portfolio-nav-link" to="/portfolio"
          >Портфолио
          <!-- <span aria-hidden="true">↗</span> -->
        </NuxtLink>
        <NuxtLink class="button button-small button-ghost" href="/lead"
          >Отправить фото</NuxtLink
        >
      </nav>
    </div>
  </header>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useMediaQuery, onClickOutside } from '@vueuse/core';
const isOpenMenu = ref(false);
const menuRef = ref(null);
const isMobileScreen = useMediaQuery('(max-width: 820px)');

const onMobileClick = () => {
  isOpenMenu.value = !isOpenMenu.value;
};

const closeMenu = () => {
  if (isMobileScreen) {
    isOpenMenu.value = false;
  }
};

onClickOutside(menuRef, () => {
  if (isOpenMenu.value) {
    isOpenMenu.value = false;
  }
});
</script>
