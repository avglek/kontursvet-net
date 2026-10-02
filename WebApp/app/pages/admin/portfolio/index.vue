<template>
  <div>
    <div class="admin-head">
      <div>
        <div class="eyebrow">Управление</div>
        <h1 class="admin-title">Портфолио</h1>
      </div>
      <NuxtLink class="button button-primary" to="/admin/portfolio/new">
        + Добавить объект
      </NuxtLink>
    </div>

    <p v-if="error" class="admin-alert" role="alert">{{ error }}</p>

    <p v-if="loading" class="admin-state">Загрузка…</p>
    <p v-else-if="!cards.length" class="admin-state">
      Пока нет ни одного объекта. Добавьте первый.
    </p>

    <ul v-else class="admin-list">
      <li v-for="c in cards" :key="c.id" class="admin-row">
        <img
          class="admin-row__thumb"
          :src="c.img.src"
          :alt="c.img.alt"
          loading="lazy"
        />

        <div class="admin-row__body">
          <span class="admin-row__kicker">{{ c.title }}</span>
          <strong>{{ c.subTitle }}</strong>
          <span class="admin-row__meta">ID {{ c.id }} · {{ c.link }}</span>
        </div>

        <div class="admin-row__actions">
          <NuxtLink
            class="button button-small button-ghost"
            :to="`/portfolio/${c.id}`"
            target="_blank"
          >
            Открыть
          </NuxtLink>
          <NuxtLink
            class="button button-small button-ghost"
            :to="`/admin/portfolio/${c.id}`"
          >
            Редактировать
          </NuxtLink>
          <button
            type="button"
            class="button button-small button-danger"
            :disabled="deletingId === c.id"
            @click="remove(c)"
          >
            Удалить
          </button>
        </div>
      </li>
    </ul>
  </div>
</template>

<script setup lang="ts">
import { usePortfolioAdmin } from '~/composables/usePortfolioAdmin';
import type { PortfolioCardDto } from '~/types/api';

const admin = usePortfolioAdmin();

const cards = ref<PortfolioCardDto[]>([]);
const loading = ref(true);
const error = ref<string | null>(null);
const deletingId = ref<number | null>(null);

const errMsg = (err: unknown, fallback: string) =>
  err instanceof Error && err.message ? err.message : fallback;

onMounted(async () => {
  try {
    cards.value = await admin.listCards();
  } catch (err) {
    error.value = errMsg(err, 'Не удалось загрузить список');
  } finally {
    loading.value = false;
  }
});

const remove = async (card: PortfolioCardDto) => {
  if (!confirm(`Удалить «${card.subTitle || card.title}»? Это необратимо.`))
    return;

  error.value = null;
  deletingId.value = card.id;
  try {
    await admin.deleteCard(card.id);
    cards.value = cards.value.filter((c) => c.id !== card.id);
  } catch (err) {
    error.value = errMsg(err, 'Не удалось удалить объект');
  } finally {
    deletingId.value = null;
  }
};
</script>
