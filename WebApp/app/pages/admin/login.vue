<script setup lang="ts">
import { ApiError } from "~/composables/useApi";

definePageMeta({ layout: false });

const { login } = useAuth();
const route = useRoute();
const router = useRouter();

const form = reactive({ username: "", password: "" });
const pending = ref(false);
const error = ref<string | null>(null);

const onSubmit = async () => {
  error.value = null;
  pending.value = true;

  try {
    await login(form);
    const redirect = (route.query.redirect as string) || "/admin";
    await router.push(redirect);
  } catch (err) {
    if (err instanceof ApiError) {
      error.value = err.problem?.detail || err.problem?.title || err.message;
    } else {
      error.value = "Не удалось войти";
    }
  } finally {
    pending.value = false;
  }
};
</script>

<template>
  <div
    class="min-h-screen flex items-center justify-center bg-gray-50 dark:bg-gray-950 px-4"
  >
    <form
      class="w-full max-w-sm bg-white dark:bg-gray-900 rounded-lg shadow-sm border border-gray-200 dark:border-gray-800 p-6 space-y-4"
      @submit.prevent="onSubmit"
    >
      <div>
        <h1 class="text-xl font-semibold">Вход в админку</h1>
        <p class="text-sm text-gray-500 mt-1">Kontursvet</p>
      </div>

      <div
        v-if="error"
        class="text-sm text-red-600 bg-red-50 dark:bg-red-950 rounded-md px-3 py-2"
      >
        {{ error }}
      </div>

      <div class="space-y-1">
        <label for="username" class="text-sm font-medium">Логин</label>
        <input
          id="username"
          v-model="form.username"
          type="text"
          autocomplete="username"
          required
          class="w-full rounded-md border border-gray-300 dark:border-gray-700 bg-transparent px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-primary-500"
        />
      </div>

      <div class="space-y-1">
        <label for="password" class="text-sm font-medium">Пароль</label>
        <input
          id="password"
          v-model="form.password"
          type="password"
          autocomplete="current-password"
          required
          class="w-full rounded-md border border-gray-300 dark:border-gray-700 bg-transparent px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-primary-500"
        />
      </div>

      <button
        type="submit"
        :disabled="pending"
        class="w-full rounded-md bg-primary-600 hover:bg-primary-700 disabled:opacity-50 text-white text-sm font-medium py-2 transition"
      >
        {{ pending ? "Вход…" : "Войти" }}
      </button>
    </form>
  </div>
</template>
