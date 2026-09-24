<template>
  <div :id="card.link">
    <a class="p_case-card" href="#" @click.prevent="openCase(card.id)">
      <NuxtImg :src="card.img.src" :alt="card.img.alt" />
      <div class="case-card-body">
        <div class="eyebrow">{{ card.title }}</div>
        <h2>{{ card.subTitle }}</h2>
        <p>{{ card.description }}</p>
        <span class="card-link">Посмотреть фотографии ↓</span>
      </div>
    </a>
  </div>
</template>

<script lang="ts" setup>
import { type ICard } from '~/types/CardView';

const props = defineProps<{ card: ICard }>();
const card = props.card;

const router = useRouter();

const openCase = (id: number) => {
  router.push('/portfolio/' + id);
};
</script>

<style></style>
