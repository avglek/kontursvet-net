<script setup lang="ts">
import { processSteps } from '~/data/content';
</script>

<template>
  <section class="section process-section" id="process">
    <div class="container">
      <div class="section-head">
        <div>
          <div class="eyebrow">
            Как работаем
          </div>
          <h2 class="section-title">
            От фотографии до готового света<span class="glow-line" />
          </h2>
        </div>
        <p class="section-lead">
          Для первого шага не нужно считать метраж и выбирать оборудование.
        </p>
      </div>

      <ol class="process-list">
        <li v-for="step in processSteps" :key="step.number">
          <span class="process-number">{{ step.number }}</span>
          <div>
            <strong>{{ step.title }}</strong>
            <p>{{ step.description }}</p>
          </div>
        </li>
      </ol>
    </div>
  </section>
</template>
