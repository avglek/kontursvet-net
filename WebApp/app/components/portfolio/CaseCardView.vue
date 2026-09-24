<template>
  <!-- Состояние загрузки -->
  <div v-if="isLoading"><Spinner /></div>

  <!-- Ошибка -->
  <div v-else-if="error" class="error">{{ error }}</div>
  <section class="section concept-case" :id="card?.name">
    <div class="shell">
      <div class="case-heading">
        <div>
          <div class="eyebrow">{{ card?.case }}</div>
          <h2>{{ card?.title }}</h2>
        </div>
        <NuxtLink :to="'/portfolio/#' + card?.name">К списку ↑</NuxtLink>
      </div>
      <p class="section-intro">
        {{ card?.description }}
      </p>
      <div class="facts-wrap" v-if="card?.task">
        <div class="facts-task">
          <strong>Задача заказчика</strong>
          <p>
            {{ card?.task }}
          </p>
        </div>
        <div class="facts-grid">
          <div class="fact fact-wide">
            <span class="fact-label">Объём работ</span>
            <ul>
              <li v-for="work in card?.works">{{ work }}</li>
            </ul>
          </div>
          <div class="fact">
            <span class="fact-label">Локация</span>
            <div class="fact-value">{{ card?.location }}</div>
          </div>
          <div class="fact">
            <span class="fact-label">Срок</span>
            <div class="fact-value">{{ card?.term }}</div>
          </div>
          <div class="fact">
            <span class="fact-label">Команда</span>
            <div class="fact-value">{{ card?.team }}</div>
          </div>
          <div class="fact">
            <span class="fact-label">Период</span>
            <div class="fact-value">{{ card?.period }}</div>
          </div>
          <div class="fact fact-wide">
            <span class="fact-label">Особенности монтажа</span>
            <div class="fact-value">
              {{ card?.features }}
            </div>
          </div>
        </div>
      </div>
      <div class="meta">
        <span>{{ card?.meta[0] }}</span
        ><span> {{ card?.meta[1] }}</span>
        <span> {{ card?.meta[2] }}</span>
      </div>
      <div class="gallery">
        <figure v-for="photo in card?.photos">
          <NuxtImg
            loading="lazy"
            :src="photo.src"
            :alt="photo.alt"
            @click="openLightbox(photo.src)"
          />
          <figcaption>{{ photo.figcaption }}</figcaption>
        </figure>
      </div>
      <NuxtLink class="link" :to="'/portfolio/#' + card?.name"
        >К списку ↑</NuxtLink
      >
    </div>
  </section>
</template>

<script lang="ts" setup>
import { type ICardView, type IPhoto } from '~/types/CardView';
import { ref, onMounted } from 'vue';
import Spinner from '../Spinner.vue';
import { useLightbox } from '~/composables/useLightbox';

// Подключаем функцию открытия
const { openLightbox } = useLightbox();

const cardViews = ref<ICardView[]>([]);
const photos = ref<IPhoto[]>([]);
const card = ref<ICardView | null>(null);
const isLoading = ref<boolean>(true);
const error = ref<string | null>(null);

const props = defineProps<{
  id: number;
}>();
const id: number = props.id;

onMounted(async () => {
  try {
    const cardResponse = await fetch('/api/data?file=cards-view.json');
    const photoResponse = await fetch('/api/data?file=photo-view.json');

    if (!cardResponse.ok || !photoResponse.ok) {
      throw new Error('Ошибка при загрузке данных');
    }
    cardViews.value = (await cardResponse.json()) as ICardView[];
    photos.value = (await photoResponse.json()) as IPhoto[];

    card.value = cardViews.value[id - 1]!;
    card.value.photos = photos.value[id - 1]?.gallery;
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Неизвестная ошибка';
  } finally {
    isLoading.value = false;
  }
});
</script>

<style scoped>
.link {
  color: var(--accent);
  text-decoration: none;
  white-space: nowrap;
  margin-top: 1.5rem;
  display: flex;
  justify-content: end;
}
</style>
