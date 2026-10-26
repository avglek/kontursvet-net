<script setup lang="ts">
import { services } from "~/data/content";

const { openLightbox } = useModals();
</script>

<template>
  <section class="section" id="solutions">
    <div class="container">
      <div class="section-head">
        <div>
          <div class="eyebrow">Форматы оформления</div>
          <h2 class="section-title">
            Четыре направления света<span class="glow-line" />
          </h2>
        </div>
        <p class="section-lead">
          Сначала оцениваем объект и желаемый результат, затем предлагаем
          подходящее решение.
        </p>
      </div>

      <div class="services-grid">
        <article
          v-for="service in services"
          :key="service.number"
          class="service-card"
        >
          <div class="service-media">
            <img
              :src="service.image"
              :alt="service.alt"
              :style="{ objectPosition: service.position }"
              loading="lazy"
              decoding="async"
              @click="openLightbox(service.image, service.alt)"
            />
          </div>
          <div class="service-body">
            <span class="service-number">{{ service.number }}</span>
            <h3>{{ service.title }}</h3>
            <p>{{ service.description }}</p>
          </div>
        </article>
      </div>
    </div>
  </section>
</template>
