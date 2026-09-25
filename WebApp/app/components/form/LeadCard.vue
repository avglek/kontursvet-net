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
              v-model="form.phoneFormat"
              data-maska="+7(###)###-##-##"
              @maska="form.phoneDigital = $event.detail.unmasked"
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
            ><input type="checkbox" required v-model="check" /><span
              >Согласен на обработку данных для обратной связи</span
            ></label
          >
          <button class="submit-button" type="submit" :disabled="!check">
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

<script setup lang="ts">
import type { ILead, ILeadPhone, ILeadForm } from '~/types/ILead';
import { type IModalLeadPanel } from '~/types/CardView.ts';
import { ref } from 'vue';
import { vMaska } from 'maska/vue';
import { useLeads } from '~/composables/useLeads';

const form: ILeadForm = reactive({
  name: '',
  phoneDigital: '',
  phoneFormat: '',
  home: '',
  location: '',
  message: '',
});

const check = ref(false);
const modalMessage: Partial<IModalLeadPanel> = {};
const isFormDisabled = ref(false);
const isModalView = ref(false);

let selected: File[] = [];
const { t } = useI18n();

const handleModalClose = () => {
  isModalView.value = false;
};

const handleSubmit = async () => {
  isFormDisabled.value = true;

  try {
    const { sendLead } = useLeads();
    await sendLead(form, selected);
    modalMessage.title = t('modal.success.title');
    modalMessage.message = t('modal.success.message');
    isModalView.value = true;
    // Сброс формы
    Object.assign(form, {
      name: '',
      phoneDigital: '',
      phoneFormat: '',
      home: '',
      location: '',
      message: '',
    });
    selected = [];
  } catch (e: any) {
    modalMessage.title = t('modal.error.title');
    modalMessage.message = t('modal.error.message');
    isModalView.value = true;
  } finally {
    isFormDisabled.value = false;
  }
};

const handleFileChange = (e: Event) => {
  const target = e.target as HTMLInputElement;
  selected = Array.from(target.files ?? []);
};
</script>
