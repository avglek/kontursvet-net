<template>
  <section class="lead-section" id="request">
    <div class="shell position">
      <FormModal
        v-if="isModalView.valueOf()"
        @modal-close="handleModalClose()"
        :title="modalMessage.title!"
        :message="modalMessage.message!"
      />
      <Spinner v-if="isFormDisabled.valueOf()" />
      <div class="lead-card">
        <div class="lead-copy">
          <div class="eyebrow">Предварительная оценка</div>
          <h2>Обсудим ваш объект</h2>
          <p>
            Оставьте контакты и приложите фотографии. Специалист КонтурСвет
            уточнит задачу и предложит следующий шаг.
          </p>
          <div class="lead-steps">
            <div class="lead-step">
              <b>01</b><span>Получаем фотографии и адрес объекта</span>
            </div>
            <div class="lead-step">
              <b>02</b><span>Уточняем задачу и желаемый эффект</span>
            </div>
            <div class="lead-step">
              <b>03</b><span>Готовим предварительную оценку</span>
            </div>
          </div>
        </div>
        <form
          class="lead-form"
          data-concept-form
          @submit.prevent="handleSubmit"
        >
          <div class="field">
            <label>Имя</label
            ><input
              v-model="form.name"
              name="name"
              autocomplete="name"
              required
              placeholder="Как к вам обращаться"
            />
          </div>
          <div class="field">
            <label>Телефон</label
            ><input
              v-maska
              v-model="phoneFormatted"
              data-maska="+7(###)###-##-##"
              @maska="phoneRaw = $event.detail.unmasked"
              placeholder="+7(999)000-00-00"
            />
          </div>
          <div class="field">
            <label>Тип объекта</label
            ><select name="object" v-model="form.home">
              <option>Частный дом</option>
              <option>Коммерческий объект</option>
              <option>Территория или участок</option>
              <option>Другое</option>
            </select>
          </div>
          <div class="field">
            <label>Где находится объект</label
            ><input
              v-model="form.location"
              name="location"
              placeholder="Город или район"
            />
          </div>
          <div class="field field-wide">
            <label>Коротко о задаче</label
            ><textarea
              v-model="form.message"
              name="message"
              placeholder="Что хотите подсветить и к какому сроку"
            ></textarea>
          </div>
          <div class="field field-wide">
            <label>Фотографии объекта</label
            ><input
              name="photos"
              type="file"
              accept="image/*"
              multiple
              @change="handleFileChange"
              ref="fileInput"
            />
          </div>
          <label class="check"
            ><input type="checkbox" required v-model="form.check" /><span
              >Согласен на обработку данных для обратной связи</span
            ></label
          >
          <button class="submit-button" type="submit" :disabled="!form.check">
            Получить предварительную оценку
          </button>
          <p class="form-status" aria-live="polite"></p>
        </form>
      </div>
    </div>
  </section>
</template>

<style scoped>
.position {
  position: relative;
}
</style>

<script lang="ts" setup>
import imageCompression from 'browser-image-compression';
import type { ILead, ILeadPhone } from '#shared/types/ILead';
import { type IModalLeadPanel } from '~/types/CardView.ts';
import { ref } from 'vue';
import { vMaska } from 'maska/vue';

const { $clientLog } = useNuxtApp();

const { t, locale, setLocale } = useI18n();

const url = '/api/send-message';
const modalMessage: Partial<IModalLeadPanel> = {};

const form = reactive({
  name: '',
  home: '',
  location: '',
  message: '',
  check: false,
});

// Опции сжатия для изображений
const compressionOptions = {
  maxSizeMB: 1,
  maxWidthOrHeight: 1920,
  useWebWorker: true,
};

const fileInput = ref(null);
const isFormDisabled = ref(false);
const isModalView = ref(false);
const phoneFormatted = ref('');
const phoneRaw = ref('');

let selectedFile: File[] = [];

const handleModalClose = () => {
  isModalView.value = false;
};

const handleFileChange = (e: Event) => {
  const target = e.target as HTMLInputElement;
  if (target.files && target.files?.length > 0) {
    selectedFile = Array.from(target.files);
  }
};

const handleSubmit = async () => {
  form.check = false;
  isFormDisabled.value = true;
  const phone: ILeadPhone = {
    digital: phoneRaw.value,
    format: phoneFormatted.value,
  };
  const body: ILead = {
    name: form.name,
    phone,
    home: form.home,
    message: form.message,
    location: form.location,
  };

  const formData = new FormData();

  for (let file of selectedFile) {
    if (file.type.startsWith('image/')) {
      try {
        const compressedFile = await imageCompression(file, compressionOptions);

        // Добавляем сжатый файл в formData
        formData.append('files', compressedFile, compressedFile.name);
        $clientLog.info(
          `add file: ${compressedFile.name} : ${compressedFile.size}`,
        );
      } catch (err) {
        $clientLog.warn(`Compress failed: ${file.name}`, err);
        console.error('Ошибка сжатия файла:', file.name, err);
      }
    }
  }

  formData.append('json', JSON.stringify(body));

  try {
    const response = await $fetch(url, {
      method: 'POST',
      body: formData,
    });
    modalMessage.title = t('modal.success.title');
    modalMessage.message = t('modal.success.message');
    $clientLog.info('Upload success:', response);
    isModalView.value = true;
  } catch (error) {
    $clientLog.warn('Upload failed:', error);
    modalMessage.title = t('modal.error.title');
    modalMessage.message = t('modal.error.message');
    isModalView.value = true;
  } finally {
    isFormDisabled.value = false;
    clearForm();
  }
};

const clearForm = () => {
  form.check = false;
  form.home = '';
  form.location = '';
  form.message = '';
  form.name = '';
  phoneFormatted.value = '';
  phoneRaw.value = '';

  if (fileInput.value) {
    // @ts-ignore
    fileInput.value.value = '';
  }
  selectedFile = [];
};
</script>
