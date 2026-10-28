<template>
  <section class="section" id="cases">
    <div class="container">
      <div class="section-head">
        <div>
          <div class="eyebrow">Кейсы</div>
          <h2 class="section-title">
            Два проекта — две задачи<span class="glow-line" />
          </h2>
        </div>
        <p class="section-lead">
          Конкретный объём, состав оформления и реальный результат.
        </p>
      </div>

      <div class="cases-grid">
        <article v-for="item in cases" :key="item.location" class="case-card">
          <div class="case-photo">
            <img
              :src="item.image"
              :alt="item.alt"
              loading="lazy"
              decoding="async"
              @click="openLightbox(item.image, item.alt)"
            />
          </div>
          <div class="case-body">
            <div class="eyebrow">
              {{ item.location }}
            </div>
            <h3>{{ item.title }}</h3>

            <div class="case-facts">
              <span v-for="fact in item.facts" :key="fact">{{ fact }}</span>
            </div>

            <ul>
              <li v-for="bullet in item.bullets" :key="bullet">{{ bullet }}</li>
            </ul>
          </div>
        </article>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { cases } from '@/data/content';

const { openLightbox } = useModals();
</script>
