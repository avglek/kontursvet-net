<template>
  <div>
    <header class="site-header admin-bar">
      <div class="container nav-row">
        <NuxtLink class="brand" to="/admin" aria-label="КонтурСвет — админка">
          <NuxtImg src="/images/icon.png" alt="КонтурСвет" />
        </NuxtLink>

        <nav class="admin-nav" aria-label="Разделы админки">
          <NuxtLink to="/admin" exact-active-class="is-active">Дашборд</NuxtLink>
          <NuxtLink to="/admin/portfolio" active-class="is-active">
            Портфолио
          </NuxtLink>
          <NuxtLink to="/" target="_blank">Сайт</NuxtLink>
        </nav>

        <div class="admin-user">
          <span v-if="user">{{ user.username }}</span>
          <button
            type="button"
            class="button button-small button-ghost"
            @click="onLogout"
          >
            Выйти
          </button>
        </div>
      </div>
    </header>

    <main class="container admin-main">
      <slot />
    </main>
  </div>
</template>

<script lang="ts" setup>
const { user, logout } = useAuth();

const onLogout = async () => {
  logout();
  await navigateTo('/admin/login');
};
</script>
