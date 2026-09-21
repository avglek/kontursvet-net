<script setup lang="ts">
const { user, logout } = useAuth();
const router = useRouter();

const links = [
  { to: "/admin", label: "Дашборд", icon: "i-heroicons-home" },
  {
    to: "/admin/portfolio",
    label: "Портфолио",
    icon: "i-heroicons-rectangle-stack",
  },
  { to: "/admin/leads", label: "Лиды", icon: "i-heroicons-envelope" },
];

const handleLogout = async () => {
  logout();
  await router.push("/admin/login");
};
</script>

<template>
  <div class="min-h-screen flex bg-gray-50 dark:bg-gray-950">
    <!-- Sidebar -->
    <aside
      class="w-64 border-r border-gray-200 dark:border-gray-800 bg-white dark:bg-gray-900 flex flex-col"
    >
      <div class="px-6 py-5 border-b border-gray-200 dark:border-gray-800">
        <h1 class="text-lg font-semibold">Kontursvet</h1>
        <p class="text-xs text-gray-500">Админка</p>
      </div>

      <nav class="flex-1 px-3 py-4 space-y-1">
        <NuxtLink
          v-for="link in links"
          :key="link.to"
          :to="link.to"
          class="flex items-center gap-3 px-3 py-2 rounded-md text-sm text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-800"
          active-class="bg-primary-50 dark:bg-primary-950 text-primary-700 dark:text-primary-300"
        >
          {{ link.label }}
        </NuxtLink>
      </nav>

      <div class="px-3 py-4 border-t border-gray-200 dark:border-gray-800">
        <div class="px-3 mb-2 text-xs text-gray-500">
          {{ user?.username ?? "—" }} ({{ user?.role ?? "—" }})
        </div>
        <button
          class="w-full text-left px-3 py-2 rounded-md text-sm text-red-600 hover:bg-red-50 dark:hover:bg-red-950"
          @click="handleLogout"
        >
          Выйти
        </button>
      </div>
    </aside>

    <!-- Content -->
    <div class="flex-1 flex flex-col">
      <header
        class="h-14 border-b border-gray-200 dark:border-gray-800 bg-white dark:bg-gray-900 flex items-center px-6"
      >
        <h2 class="text-sm font-medium text-gray-700 dark:text-gray-300">
          {{ $route.meta.title ?? "" }}
        </h2>
      </header>

      <main class="flex-1 p-6 overflow-auto">
        <slot />
      </main>
    </div>
  </div>
</template>
