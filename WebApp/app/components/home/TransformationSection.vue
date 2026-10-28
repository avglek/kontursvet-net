<template>
  <section id="transformation" class="tf-section" aria-labelledby="tf-title">
    <div class="container">
      <div class="tf-heading">
        <p class="tf-kicker"><span aria-hidden="true" />The Transformation</p>
        <h2 id="tf-title">
          Один дом. Два состояния.<br />Совершенно разное впечатление.
        </h2>
        <p id="tf-description" class="tf-description">
          Используйте слайдер, чтобы сравнить дом без подсветки и с готовым
          световым сценарием.
        </p>
      </div>

      <div ref="surface" class="tf-comparison" :style="surfaceStyle">
        <img
          class="tf-image"
          :src="beforeSrc"
          width="1448"
          height="1086"
          loading="lazy"
          decoding="async"
          alt="Дом без наружной подсветки: кровля, фасад и участок в вечернем свете"
        />
        <img
          class="tf-image tf-after"
          :src="afterSrc"
          width="1448"
          height="1086"
          loading="lazy"
          decoding="async"
          alt="Тот же дом с подсветкой кровли, фасада и деревьев"
        />

        <span class="tf-label tf-label-before">Без подсветки</span>
        <span class="tf-label tf-label-after">С подсветкой</span>
        <span class="tf-divider" aria-hidden="true" />
        <span class="tf-handle" aria-hidden="true">
          <svg
            viewBox="0 0 32 24"
            width="30"
            height="24"
            fill="none"
            stroke="currentColor"
            stroke-width="1.5"
          >
            <path d="m10 7-5 5 5 5M22 7l5 5-5 5M16 4v16" />
          </svg>
        </span>

        <input
          ref="range"
          class="tf-range"
          type="range"
          min="0"
          max="100"
          step="1"
          :value="rangeValue"
          aria-label="Граница сравнения: слева без подсветки, справа с подсветкой"
          aria-describedby="tf-description tf-help"
          :aria-valuetext="valueText"
        />
      </div>

      <div class="tf-toolbar">
        <p id="tf-help">
          <span aria-hidden="true">↔</span> Потяните разделитель — рассмотрите,
          как меняется дом
        </p>
        <button class="tf-replay" type="button" @click="animate">
          <span aria-hidden="true">↻</span> Повторить анимацию
        </button>
      </div>

      <div class="tf-actions">
        <NuxtLink class="button button-primary tf-primary" to="/calculator">
          Рассчитать стоимость и подобрать подсветку
          <span aria-hidden="true">↗</span>
        </NuxtLink>
        <NuxtLink class="button tf-secondary" to="/lead">
          Отправить фото для бесплатной визуализации
          <span aria-hidden="true">↗</span>
        </NuxtLink>
      </div>
    </div>
  </section>

  <!-- <dialog
    ref="calculatorDialog"
    class="tf-dialog"
    aria-labelledby="tf-calc-title"
    @close="onDialogClose"
  >
    <button
      class="tf-close"
      type="button"
      aria-label="Закрыть окно"
      @click="calculatorDialog?.close()"
    >
      ×
    </button>
    <p class="tf-kicker">Калькулятор стоимости</p>
    <h2 id="tf-calc-title">Подберите подсветку для своего дома</h2>
    <p>
      Готовый калькулятор откроется на отдельной странице. В этой версии макета
      ссылка ещё не подключена.
    </p>
    <button
      class="button tf-secondary"
      type="button"
      @click="calculatorDialog?.close()"
    >
      Вернуться к сравнению
    </button>
  </dialog>

  <dialog
    ref="photoDialog"
    class="tf-dialog"
    data-lenis-prevent
    aria-labelledby="tf-photo-title"
    @close="onDialogClose"
  >
    <button
      class="tf-close"
      type="button"
      aria-label="Закрыть форму"
      @click="photoDialog?.close()"
    >
      ×
    </button>
    <p class="tf-kicker">Бесплатная визуализация</p>
    <h2 id="tf-photo-title">Покажите нам ваш дом</h2>
    <p class="tf-demo">
      Демонстрация формы: фотография и телефон не отправляются.
    </p>

    <form class="tf-form" @submit="onSubmit">
      <label for="tf-photo-file"
        >Фотография дома <span aria-hidden="true">*</span></label
      >

      <div
        class="tf-upload"
        :class="{ 'tf-dragover': dragOver }"
        @dragenter="onDragEnter"
        @dragover="onDragOver"
        @dragleave="onDragLeave"
        @drop="onDrop"
      >
        <label class="tf-file-label" for="tf-photo-file">
          <span aria-hidden="true">＋</span> Выбрать или перетащить фотографию
          <small>JPG, PNG, WEBP · до 10 МБ</small>
        </label>
        <input
          id="tf-photo-file"
          ref="photoFileInput"
          type="file"
          accept="image/jpeg,image/png,image/webp"
          required
          @change="onChange"
        />
        <img
          v-if="previewUrl"
          class="tf-preview"
          :src="previewUrl"
          alt="Предпросмотр выбранной фотографии дома"
        />
        <p v-if="fileName" class="tf-filename">{{ fileName }}</p>
      </div>

      <label for="tf-phone">Телефон <span aria-hidden="true">*</span></label>
      <input
        id="tf-phone"
        ref="phoneInput"
        v-model="phone"
        type="tel"
        name="phone"
        autocomplete="tel"
        inputmode="tel"
        placeholder="+7 (999) 123-45-67"
        required
      />

      <label for="tf-place">
        Где находится объект <span class="tf-optional">необязательно</span>
      </label>
      <input
        id="tf-place"
        v-model="place"
        type="text"
        name="place"
        autocomplete="address-level2"
        placeholder="Например, Всеволожск"
      />

      <p
        class="tf-feedback"
        :class="{ 'tf-error': feedbackIsError }"
        role="status"
        aria-live="polite"
      >
        {{ feedback }}
      </p>

      <button class="button button-primary" type="submit">
        Проверить заполнение <span aria-hidden="true">→</span>
      </button>
    </form>
  </dialog> -->
</template>

<script setup lang="ts">
const { surface, range, surfaceStyle, rangeValue, valueText, animate } =
  useTransformation();

const beforeSrc = '/images/home/transformation/before.png';
const afterSrc = '/images/home/transformation/after.png';

const calculatorDialog = ref<HTMLDialogElement | null>(null);
const photoDialog = ref<HTMLDialogElement | null>(null);

const photoFileInput = ref<HTMLInputElement | null>(null);
const phoneInput = ref<HTMLInputElement | null>(null);
const phone = ref('');
const place = ref('');
const previewUrl = ref<string | null>(null);
const fileName = ref('');
const feedback = ref('');
const feedbackIsError = ref(false);

// function openDialog(dialog: HTMLDialogElement | null) {
//   if (!dialog) return;
//   dialog.showModal();
//   document.documentElement.style.overflow = 'hidden';
// }

// function onDialogClose() {
//   document.documentElement.style.overflow = '';
// }

const {
  dragOver,
  error: fileError,
  openPicker,
  onKeydown,
  onDragEnter,
  onDragOver,
  onDragLeave,
  onDrop,
  onChange,
  reset: resetFile,
} = useImageDrop({
  input: photoFileInput,
  validate: validatePhoto,
  onFile: (file) => {
    if (previewUrl.value) URL.revokeObjectURL(previewUrl.value);

    if (!file) {
      previewUrl.value = null;
      fileName.value = '';
      feedback.value = '';
      feedbackIsError.value = false;
      return;
    }

    previewUrl.value = URL.createObjectURL(file);
    fileName.value = file.name;
    feedback.value = 'Фотография выбрана локально. Файл не отправлен.';
    feedbackIsError.value = false;
  },
});

watch(fileError, (message) => {
  if (message) {
    feedback.value = message;
    feedbackIsError.value = true;
  }
});

function onSubmit(event: Event) {
  event.preventDefault();

  const digits = phoneInput.value?.value.replace(/\D/g, '') ?? '';
  if (!photoFileInput.value?.files?.length || digits.length < 10) {
    feedback.value =
      'Выберите фотографию и укажите телефон минимум из 10 цифр.';
    feedbackIsError.value = true;
    return;
  }

  feedback.value = 'Поля заполнены. Это демонстрация: заявка не отправлена.';
  feedbackIsError.value = false;
}

function revokePreview() {
  if (previewUrl.value) URL.revokeObjectURL(previewUrl.value);
}

onMounted(() => window.addEventListener('pagehide', revokePreview));
onBeforeUnmount(() => {
  window.removeEventListener('pagehide', revokePreview);
  revokePreview();
});
</script>
