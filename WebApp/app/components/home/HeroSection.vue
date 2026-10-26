<script setup lang="ts">
const { openLead } = useModals();
const {
  mainVideo,
  ambientVideo,
  playing,
  status,
  timeText,
  progress,
  toggleLabel,
  play,
  toggle,
} = useHeroVideo();

const videoSrc = "/video/hero-showreel-hd-silent.mp4";
const posterSrc = "/video/hero-poster-hd.webp";
</script>

<template>
  <section class="cinematic-hero" aria-labelledby="hero-title">
    <!-- Двухслойная видеосцена: размытое свечение + чистое видео 16:9 -->
    <div
      class="hero-video-stage"
      aria-label="Видеопрезентация проектов КонтурСвет"
    >
      <video
        ref="ambientVideo"
        class="hero-video-ambient"
        loop
        muted
        playsinline
        aria-hidden="true"
        preload="none"
      >
        <source :src="videoSrc" type="video/mp4" />
      </video>

      <div class="hero-video-wrapper">
        <video
          ref="mainVideo"
          class="hero-video-main"
          autoplay
          loop
          muted
          playsinline
          preload="auto"
          :poster="posterSrc"
        >
          <source :src="videoSrc" type="video/mp4" />
          Ваш браузер не поддерживает встроенное видео.
        </video>
      </div>
    </div>

    <div class="cinematic-overlay" />

    <div class="cinematic-content">
      <div class="cinematic-top-meta">
        <div class="cinematic-badge">Архитектура света</div>
        <div class="cinematic-coords">
          Санкт-Петербург и Ленинградская область
        </div>
      </div>

      <h1 id="hero-title" class="cinematic-title">
        <span class="title-eyebrow-accent">Архитектура света</span>
        <span class="title-main">
          <span class="title-line-1">Вечерний образ </span>
          <span class="title-line-2">вашего дома</span>
        </span>
      </h1>

      <p class="cinematic-lead">
        Проектируем и монтируем премиальное контурное освещение, бахрому и
        ландшафтный свет для частных загородных резиденций. Подчёркиваем
        геометрию кровли и фасада без видимой проводки.
      </p>

      <div class="cinematic-bottom-bar">
        <div class="cinematic-actions">
          <a
            class="button button-primary"
            href="#"
            data-modal="lead"
            @click.prevent="openLead"
          >
            Отправить фото для оценки
            <span aria-hidden="true">→</span>
          </a>
          <a class="button button-ghost" href="#projects">
            Смотреть проекты <span aria-hidden="true">↓</span>
          </a>
        </div>

        <div class="cinematic-hud-group">
          <div class="video-nav-hud">
            <div class="video-badge-indicator">
              <span class="video-pulse-dot" />
              <span class="video-status-text">{{ status }}</span>
            </div>

            <div class="video-time-wrap">
              <span class="video-time-display">{{ timeText }}</span>
              <div class="video-hud-progress-track">
                <div
                  class="video-hud-progress"
                  :style="{ width: `${progress}%` }"
                />
              </div>
            </div>

            <div class="video-hud-actions">
              <button
                class="video-hud-btn"
                type="button"
                :aria-label="toggleLabel"
                :title="toggleLabel"
                @click="toggle"
              >
                <svg
                  v-show="playing"
                  class="icon-pause"
                  width="13"
                  height="13"
                  viewBox="0 0 24 24"
                  fill="currentColor"
                >
                  <rect x="6" y="4" width="4" height="16" rx="1" />
                  <rect x="14" y="4" width="4" height="16" rx="1" />
                </svg>
                <svg
                  v-show="!playing"
                  class="icon-play"
                  width="13"
                  height="13"
                  viewBox="0 0 24 24"
                  fill="currentColor"
                >
                  <polygon points="6,4 20,12 6,20" />
                </svg>
              </button>
            </div>
          </div>

          <div class="cinematic-stats">
            <div class="cinematic-stat-item">
              <span class="cinematic-stat-tag">Индивидуально</span>
              <span class="cinematic-stat-title">Проект под ваш дом</span>
              <span class="cinematic-stat-desc"
                >Световой сценарий с учётом архитектуры и участка</span
              >
            </div>
            <div class="cinematic-stat-item">
              <span class="cinematic-stat-tag">Технология</span>
              <span class="cinematic-stat-title">Аккуратный монтаж</span>
              <span class="cinematic-stat-desc"
                >Алюминиевый профиль без сверления кровли</span
              >
            </div>
            <div class="cinematic-stat-item">
              <span class="cinematic-stat-tag">Эстетика</span>
              <span class="cinematic-stat-title">Без видимой проводки</span>
              <span class="cinematic-stat-desc"
                >Скрытая коммутация и чистый фасад днём</span
              >
            </div>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>
