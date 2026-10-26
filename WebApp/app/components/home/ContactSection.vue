<script setup lang="ts">
import { ref, computed } from "vue";

const phone = ref("");
const fileInput = ref<HTMLInputElement | null>(null);
const photo = ref<File | null>(null);
const submitted = ref(false);

const {
  dragOver,
  openPicker,
  onKeydown,
  onDragEnter,
  onDragOver,
  onDragLeave,
  onDrop,
  onChange,
  reset,
} = useImageDrop({
  input: fileInput,
  validate: (file) => validatePhoto(file, 25),
  onFile: (file) => {
    photo.value = file;
  },
});

const previewName = computed(() =>
  photo.value ? `✓ Выбран файл: ${photo.value.name}` : "",
);

function onSubmit(event: Event) {
  event.preventDefault();
  submitted.value = true;
}

function onReset() {
  reset();
  phone.value = "";
  submitted.value = false;
}

const advantages = [
  "Бесплатная визуализация подсветки по фото вашего дома",
  "Официальный договор и гарантия 1 год на монтаж и оборудование",
  "Монтаж за 1–2 дня без повреждения и сверления кровли",
];
</script>

<template>
  <section class="final-cta" id="contact" aria-labelledby="contact-title">
    <div class="container final-layout">
      <div class="final-copy">
        <div class="eyebrow">Следующий шаг</div>
        <h2 id="contact-title">Покажите дом — начнём с фотографии</h2>
        <p>
          Для предварительной оценки достаточно фотографий объекта. Александр
          рассчитает метраж flex neon и бахромы, подготовит варианты сценария и
          пришлёт расчёт в течение 30 минут.
        </p>

        <div class="contact-advantages">
          <div class="contact-adv-item" v-for="(adv, i) in advantages" :key="i">
            <span class="adv-dot" />
            <span>{{ adv }}</span>
          </div>
        </div>

        <div class="phone-row">
          <a href="tel:+79650473553">+7 (965) 047-35-53</a>
          <a href="tel:+78127150505">8 (812) 715-05-05</a>
        </div>

        <div class="contact-direct-call">
          <a
            href="tel:+79650473553"
            class="button button-ghost button-call-direct"
            >Позвонить Александру →</a
          >
        </div>
      </div>

      <div class="final-form-card" id="inline-lead-card">
        <div class="form-card-badge">Оценка стоимости</div>
        <h3 class="form-card-title">Отправить фото на расчёт</h3>
        <p class="form-card-subtitle">
          Прикрепите фото дома или участка для точной сметы
        </p>

        <form
          v-if="!submitted"
          class="inline-lead-form"
          id="inline-lead-form"
          @submit="onSubmit"
        >
          <div class="form-group">
            <label for="contact-phone">Ваш номер телефона</label>
            <input
              type="tel"
              id="contact-phone"
              v-model="phone"
              name="phone"
              required
              placeholder="+7 (___) ___-__-__"
              autocomplete="tel"
            />
          </div>

          <div class="form-group">
            <label>Фотография дома / участка</label>
            <div
              class="drop-zone"
              :class="{ dragover: dragOver }"
              tabindex="0"
              role="button"
              aria-label="Прикрепить фото дома"
              @click="openPicker"
              @keydown="onKeydown"
              @dragenter="onDragEnter"
              @dragover="onDragOver"
              @dragleave="onDragLeave"
              @drop="onDrop"
            >
              <div class="drop-icon">📷</div>
              <div class="drop-main-text">
                Нажмите для выбора или перетащите фото
              </div>
              <div class="drop-sub-text">JPG, PNG, WEBP до 25 МБ</div>
              <input
                type="file"
                id="contact-lead-file"
                ref="fileInput"
                accept="image/*"
                style="display: none"
                @change="onChange"
              />
              <div v-if="previewName" class="file-preview-badge">
                {{ previewName }}
              </div>
            </div>
          </div>

          <button
            type="submit"
            class="button button-primary button-submit-form"
            id="contact-submit-btn"
          >
            Отправить на расчет →
          </button>

          <div class="form-privacy-note">
            Демонстрационная форма: контакты и фотографии никуда не
            отправляются.
          </div>
        </form>

        <div v-else class="lead-success-screen">
          <div class="success-icon">✨</div>
          <h4>Файл выбран — демонстрационный режим</h4>
          <p>
            Макет не отправляет заявки. В рабочей версии форму нужно подключить
            к каналам обработки обращений.
          </p>
          <button
            type="button"
            class="button button-ghost button-small"
            @click="onReset"
          >
            Отправить ещё одно фото
          </button>
        </div>
      </div>
    </div>
  </section>
</template>
