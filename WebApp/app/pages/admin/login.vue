<template>
  <div class="admin">
    <aside class="admin__sidebar">
      <div class="admin__brand">
        <h1 class="admin__brand-title">Kontursvet</h1>
        <p class="admin__brand-subtitle">Админка</p>
      </div>

      <nav class="admin__nav">
        <NuxtLink
          v-for="link in links"
          :key="link.to"
          :to="link.to"
          class="admin__nav-link"
          active-class="admin__nav-link--active"
        >
          {{ link.label }}
        </NuxtLink>
      </nav>

      <div class="admin__footer">
        <div class="admin__user">
          {{ user?.username ?? "—" }} ({{ user?.role ?? "—" }})
        </div>
        <button class="admin__logout" @click="handleLogout">Выйти</button>
      </div>
    </aside>

    <div class="admin__content">
      <header class="admin__topbar">
        <h2 class="admin__page-title">{{ $route.meta.title ?? "" }}</h2>
      </header>

      <main class="admin__main">
        <slot />
      </main>
    </div>
  </div>
</template>

<script setup lang="ts">
const { user, logout } = useAuth();
const router = useRouter();

const links = [
  { to: "/admin", label: "Дашборд" },
  { to: "/admin/portfolio", label: "Портфолио" },
  { to: "/admin/leads", label: "Лиды" },
];

const handleLogout = async () => {
  logout();
  await router.push("/admin/login");
};
</script>
