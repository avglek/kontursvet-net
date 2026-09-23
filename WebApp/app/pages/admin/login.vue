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

<style lang="scss" scoped>
.admin {
  min-height: 100vh;
  display: flex;
  background: $color-bg;
}

.admin__sidebar {
  width: 16rem;
  display: flex;
  flex-direction: column;
  background: $color-surface;
  border-right: 1px solid $color-border;
}

.admin__brand {
  padding: 1.25rem 1.5rem;
  border-bottom: 1px solid $color-border;
}

.admin__brand-title {
  font-size: 1.125rem;
  font-weight: 600;
}

.admin__brand-subtitle {
  font-size: 0.75rem;
  color: $color-text-muted;
  margin-top: 0.125rem;
}

.admin__nav {
  flex: 1;
  padding: 1rem 0.75rem;
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.admin__nav-link {
  display: block;
  padding: 0.5rem 0.75rem;
  font-size: 0.875rem;
  color: $color-text;
  border-radius: $radius-md;
  transition: background 0.15s;

  &:hover {
    background: rgba(0, 0, 0, 0.04);
  }

  &--active {
    background: $color-primary-light;
    color: $color-primary;
    font-weight: 500;
  }
}

.admin__footer {
  padding: 1rem 0.75rem;
  border-top: 1px solid $color-border;
}

.admin__user {
  padding: 0 0.75rem 0.5rem;
  font-size: 0.75rem;
  color: $color-text-muted;
}

.admin__logout {
  width: 100%;
  padding: 0.5rem 0.75rem;
  text-align: left;
  font-size: 0.875rem;
  color: $color-error;
  border-radius: $radius-md;
  transition: background 0.15s;

  &:hover {
    background: rgba($color-error, 0.08);
  }
}

.admin__content {
  flex: 1;
  display: flex;
  flex-direction: column;
  min-width: 0;
}

.admin__topbar {
  height: 3.5rem;
  display: flex;
  align-items: center;
  padding: 0 1.5rem;
  background: $color-surface;
  border-bottom: 1px solid $color-border;
}

.admin__page-title {
  font-size: 0.875rem;
  font-weight: 500;
  color: $color-text-muted;
}

.admin__main {
  flex: 1;
  padding: 1.5rem;
  overflow: auto;
}
</style>
