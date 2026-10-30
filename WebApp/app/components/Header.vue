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
        aria-controls="main-navigation"
        :aria-expanded="isOpenMenu ? 'true' : 'false'"
        @click="toggleMenu()"
      >
        <span></span><span></span>
      </button>
      <!--
        Навигация всегда в DOM: видимость на мобильном управляется CSS
        (.nav-links { display: none } / .nav-links.is-open { display: flex }).
        Без v-if — нет расхождения SSR и клиента (hydration mismatch).
      -->
      <nav
        id="main-navigation"
        class="nav-links"
        :class="{ 'is-open': isOpenMenu }"
        aria-label="Основная навигация"
        @click="closeMenu()"
      >
        <NuxtLink href="/#projects">Проекты</NuxtLink
        ><NuxtLink href="/#solutions">Решения</NuxtLink
        ><NuxtLink href="/#cases">Кейсы</NuxtLink
        ><NuxtLink href="/#process">Как работаем</NuxtLink
        ><NuxtLink href="/#contact">Контакты</NuxtLink>
        <NuxtLink no-prefetch class="portfolio-nav-link" to="/portfolio"
          >Портфолио
        </NuxtLink>
        <NuxtLink no-prefetch class="portfolio-nav-link" to="/calculator"
          >Калькулятор
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
import { onClickOutside } from '@vueuse/core';

const isOpenMenu = ref(false);
const menuRef = ref<HTMLElement | null>(null);

const toggleMenu = () => {
  isOpenMenu.value = !isOpenMenu.value;
};

// На десктопе класс is-open не влияет на видимость, поэтому закрывать можно всегда.
const closeMenu = () => {
  isOpenMenu.value = false;
};

onClickOutside(menuRef, closeMenu);
</script>
