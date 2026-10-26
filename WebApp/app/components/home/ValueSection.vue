<script setup lang="ts">
import { valueSteps } from '~/data/content';

// Первый шаг подсвечен по умолчанию, как в исходном макете.
const activeStep = ref('01');
</script>

<template>
  <section class="value-section" aria-labelledby="value-title">
    <div class="container value-layout">
      <div class="value-head-col">
        <div class="eyebrow">
          Архитектура света
        </div>
        <h2 id="value-title" class="value-title">
          Создаём цельный световой образ дома и участка
        </h2>
        <p class="value-subtitle">
          Комплексный сценарий: от выразительной геометрии кровли до
          ландшафтных акцентов на деревьях.
        </p>
      </div>

      <div class="value-track-wrapper">
        <div class="value-light-beam">
          <div class="beam-glow-pulse" />
        </div>

        <div class="value-track" aria-label="Состав светового образа">
          <div
            v-for="(item, index) in valueSteps"
            :key="item.step"
            class="value-item"
            :class="{ 'is-active': activeStep === item.step }"
            :data-step="index + 1"
            @mouseenter="activeStep = item.step"
            @click="activeStep = item.step"
          >
            <div class="value-step-indicator">
              <span class="value-num">{{ item.step }}</span>
              <span class="value-dot" />
            </div>
            <div class="value-item-content">
              <span class="value-tag">{{ item.tag }}</span>
              <strong>{{ item.title }}</strong>
              <p class="value-desc">
                {{ item.description }}
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>
