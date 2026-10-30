<template>
  <div v-if="isLoading"><Spinner :is-overlay="true" /></div>
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
            ><strong>{{
              quantityCaseStringRef ? quantityCaseStringRef[0] : '0'
            }}</strong
            >{{
              quantityCaseStringRef ? quantityCaseStringRef[1] : 'кейсов'
            }}</span
          >
          <span
            ><strong>{{
              quantityCasePhotosRef ? quantityCasePhotosRef[0] : '0'
            }}</strong
            >{{
              quantityCasePhotosRef ? quantityCasePhotosRef[1] : 'кадров'
            }}</span
          >
        </div>
        <span class="portfolio-entry-action">
          Открыть портфолио <b aria-hidden="true">→</b>
        </span>
      </NuxtLink>
    </div>
  </section>
</template>

<script setup lang="ts">
import { projects, type Project } from '~/data/content';

const { openLightbox } = useLightbox();
const router = useRouter();

const quantityCaseStringRef = ref<string[]>();
const quantityCasePhotosRef = ref<string[]>();
const isLoading = ref<boolean>(true);
const error = ref<string | null>(null);

const openImage = (project: Project) => {
  if (project.id) {
    router.push('/portfolio/' + project.id);
  } else {
    openLightbox(project.image);
  }
};

onMounted(async () => {
  try {
    const { quantityCaseString, quantityCasePhotos } = await useCounters();
    {
      console.log(quantityCaseString, quantityCasePhotos);
      if (quantityCasePhotos && quantityCaseString) {
        quantityCaseStringRef.value = quantityCaseString.split(' ');
        quantityCasePhotosRef.value = quantityCasePhotos.split(' ');
      }
    }
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Неизвестная ошибка';
  } finally {
    isLoading.value = false;
  }
});
</script>
