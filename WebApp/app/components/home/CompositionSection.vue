<template>
  <section class="section composition-section">
    <div class="container composition-card">
      <div class="composition-photo">
        <img
          :src="photo.src"
          :alt="photo.alt"
          loading="lazy"
          decoding="async"
          @click="openLightbox(photo.src, photo.alt)"
        />
        <span
          v-for="pin in pins"
          :key="pin.name"
          class="photo-pin"
          :class="`pin-${pin.name}`"
          >{{ pin.label }}</span
        >
      </div>

      <div class="composition-copy">
        <div class="eyebrow">Единая световая композиция</div>
        <h2 class="section-title">Свет подчинён архитектуре</h2>
        <p>
          Мы смотрим на объект целиком: что подчеркнуть, где оставить темноту и
          как связать дом с участком.
        </p>

        <ol class="architecture-list">
          <li v-for="step in steps" :key="step.number">
            <span>{{ step.number }}</span>
            <div>
              <strong>{{ step.title }}</strong>
              <p>{{ step.text }}</p>
            </div>
          </li>
        </ol>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
const { openLightbox } = useModals();

const photo = {
  src: "/images/home/006.webp",
  alt: "Дом с оформленной кровлей, фасадом и террасой",
};

const pins = [
  { name: "roof", label: "Кровля" },
  { name: "facade", label: "Фасад" },
  { name: "terrace", label: "Терраса" },
];

const steps = [
  {
    number: "01",
    title: "Читаем архитектуру",
    text: "Определяем главные линии дома и точки акцента.",
  },
  {
    number: "02",
    title: "Строим сценарий",
    text: "Соединяем контур, бахрому и локальные решения.",
  },
  {
    number: "03",
    title: "Собираем образ",
    text: "Дом и территория работают как одна композиция.",
  },
];
</script>
