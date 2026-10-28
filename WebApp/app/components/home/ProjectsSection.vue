<script setup lang="ts">
import { portfolioStats, projects, type Project } from '~/data/content';

const { openLightbox } = useLightbox();
const router = useRouter();

const openImage = (project: Project) => {
  if (project.id) {
    router.push('/portfolio/' + project.id);
  } else {
    openLightbox(project.image);
  }
};
</script>

<template>
  <section class="section" id="projects">
    <div class="container">
      <div class="section-head">
        <div>
          <div class="eyebrow">Реальные объекты</div>
          <h2 class="section-title">
            Свет, который меняет архитектуру<span class="glow-line" />
          </h2>
        </div>
        <p class="section-lead">
          Не декоративный эффект в вакууме, а реальные дома и участки после
          монтажа.
        </p>
      </div>

      <div class="project-grid">
        <article
          v-for="project in projects"
          :key="project.image"
          class="project-card"
          :class="{ 'project-card-featured': project.featured }"
        >
          <img
            :src="project.image"
            :alt="project.alt"
            :style="{ objectPosition: project.position }"
            loading="lazy"
            decoding="async"
            @click="openImage(project)"
          />
          <div class="project-shade" />
          <div class="project-caption">
            <strong>{{ project.title }}</strong>
            <span>{{ project.description }}</span>
          </div>
        </article>
      </div>

      <NuxtLink class="portfolio-entry" to="/portfolio">
        <div class="portfolio-entry-copy">
          <span class="eyebrow">Полное онлайн-портфолио</span>
          <h3>Смотрите проекты подробно</h3>
          <p>
            Задача, объём работ, сроки и полная фотосерия каждого реализованного
            объекта.
          </p>
        </div>
        <div class="portfolio-entry-stats">
          <span
            ><strong>{{ portfolioStats.cases }}</strong> кейсов</span
          >
          <span
            ><strong>{{ portfolioStats.photos }}</strong> фотография</span
          >
        </div>
        <span class="portfolio-entry-action">
          Открыть портфолио <b aria-hidden="true">→</b>
        </span>
      </NuxtLink>
    </div>
  </section>
</template>
