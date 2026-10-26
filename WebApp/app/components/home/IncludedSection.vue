<script setup lang="ts">
import { includedItems } from '~/data/content';
</script>

<template>
  <section class="section included-section">
    <div class="container">
      <div class="section-head">
        <div>
          <div class="eyebrow">
            За что вы платите
          </div>
          <h2 class="section-title">
            Что входит в проект<span class="glow-line" />
          </h2>
        </div>
        <p class="section-lead">
          Продуманное решение, материалы, монтаж и дальнейшая поддержка — не
          только метры светового материала.
        </p>
      </div>

      <div class="included-grid">
        <article v-for="item in includedItems" :key="item.number" class="included-card">
          <span>{{ item.number }}</span>
          <h3>{{ item.title }}</h3>
          <p>{{ item.description }}</p>
        </article>
      </div>
    </div>
  </section>
</template>
