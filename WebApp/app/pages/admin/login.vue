<template>
  <section class="login-section">
    <div class="shell position">
      <div class="lead-card">
        <div class="lead-copy">
          <header class="login__header">
            <h1 class="login__title">Вход в админку</h1>
            <p class="login__subtitle">Kontursvet</p>
          </header>
        </div>
        <form class="login-form" @submit.prevent="onSubmit">
          <p v-if="error" class="form__error">{{ error }}</p>

          <div class="field login">
            <label for="username" class="form__label">Логин</label>
            <input
              id="username"
              v-model="form.username"
              type="text"
              autocomplete="username"
              required
            />
          </div>

          <div class="field login">
            <label for="password" class="form__label">Пароль</label>
            <input
              id="password"
              v-model="form.password"
              type="password"
              autocomplete="current-password"
              required
            />
          </div>

          <button
            type="submit"
            :disabled="pending"
            class="submit-button login hulf"
          >
            {{ pending ? 'Вход…' : 'Войти' }}
          </button>
        </form>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { ApiError } from '~/composables/useApi';

definePageMeta({ layout: false });

const { login } = useAuth();
const route = useRoute();
const router = useRouter();

const form = reactive({ username: '', password: '' });
const pending = ref(false);
const error = ref<string | null>(null);

const onSubmit = async () => {
  error.value = null;
  pending.value = true;

  try {
    await login(form);
    const redirect = (route.query.redirect as string) || '/admin';
    await router.push(redirect);
  } catch (err) {
    if (err instanceof ApiError) {
      error.value = err.problem?.detail || err.problem?.title || err.message;
    } else {
      error.value = 'Не удалось войти';
    }
  } finally {
    pending.value = false;
  }
};
</script>

<style lang="scss" scoped>
// .login {
//   min-height: 100vh;
//   display: flex;
//   align-items: center;
//   justify-content: center;
//   padding: 1rem;
//   background: $color-bg;
// }

// .login__form {
//   width: 100%;
//   max-width: 24rem;
//   padding: 1.5rem;
//   @include card;
// }

// .login__header {
//   margin-bottom: 0.5rem;
// }

// .login__title {
//   font-size: 1.25rem;
//   font-weight: 600;
//   color: $color-text;
// }

// .login__subtitle {
//   font-size: 0.875rem;
//   color: $color-text-muted;
//   margin-top: 0.25rem;
// }
</style>
