<template>
  <Teleport to="body">
    <Transition name="callback-modal">
      <!-- Диалог поверх страницы: открывается плавающей кнопкой на главной.
           Клик по затемнённому фону закрывает окно, клик по карточке — нет. -->
      <div v-if="isOpen" class="callback-modal" @click.self="close">
        <div
          ref="cardRef"
          class="callback-modal__card"
          role="dialog"
          aria-modal="true"
          aria-labelledby="callback-modal-title"
          @keydown="onKeydown"
        >
          <button
            type="button"
            class="callback-modal__close"
            :aria-label="t('callback.close')"
            @click="close"
          >
            &times;
          </button>

          <div v-if="sent" class="modal-success">
            <div class="modal-success__icon">✅</div>
            <h2 class="modal-success__title">
              {{ t('callback.successTitle') }}
            </h2>
            <p class="modal-success__text">{{ t('callback.successText') }}</p>
          </div>

          <template v-else>
            <p class="callback-modal__kicker">{{ t('callback.kicker') }}</p>
            <h2 id="callback-modal-title" class="callback-modal__title">
              {{ t('callback.title') }}
            </h2>
            <p class="callback-modal__text">{{ t('callback.text') }}</p>

            <form class="callback-modal__form" @submit.prevent="submit">
              <div class="field">
                <label for="callback-name">{{ t('callback.nameLabel') }}</label>
                <input
                  id="callback-name"
                  ref="nameInputRef"
                  v-model="form.name"
                  name="name"
                  type="text"
                  autocomplete="name"
                  maxlength="60"
                  required
                  :placeholder="t('callback.namePlaceholder')"
                />
              </div>

              <div class="field">
                <label for="callback-phone">
                  {{ t('callback.phoneLabel') }}
                </label>
                <input
                  id="callback-phone"
                  v-model="form.phoneFormat"
                  v-maska
                  name="phone"
                  type="tel"
                  autocomplete="tel"
                  data-maska="+7(###)###-##-##"
                  required
                  :placeholder="t('callback.phonePlaceholder')"
                  :aria-invalid="phoneError ? 'true' : undefined"
                  @maska="form.phoneDigital = $event.detail.unmasked"
                />
                <span v-if="phoneError" class="field__error">
                  {{ phoneError }}
                </span>
              </div>

              <p v-if="errorMessage" class="form__error" role="alert">
                {{ errorMessage }}
              </p>

              <button
                type="submit"
                class="submit-button"
                :disabled="isSubmitting"
              >
                {{
                  isSubmitting ? t('callback.submitting') : t('callback.submit')
                }}
              </button>

              <p class="callback-modal__privacy">{{ t('callback.privacy') }}</p>
            </form>
          </template>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<script setup lang="ts">
import { nextTick, reactive, ref, watch } from 'vue';
import { vMaska } from 'maska/vue';
import { useLeads } from '~/composables/useLeads';

const { t } = useI18n();
const { isCallbackOpen, closeCallback } = useModals();
const { sendCallback } = useLeads();

// Цели Яндекс Метрики: как в основной форме заявки (см. LeadCard.vue).
const metrika = useYandexMetrika();

const isOpen = isCallbackOpen;

const cardRef = ref<HTMLElement | null>(null);
const nameInputRef = ref<HTMLInputElement | null>(null);

const form = reactive({
  name: '',
  phoneFormat: '',
  phoneDigital: '',
});

const isSubmitting = ref(false);
const sent = ref(false);
const phoneError = ref('');
const errorMessage = ref('');

// Маска даёт +7(999)123-45-67 — считаем полными все 11 набранных цифр.
function isPhoneComplete(value: string) {
  return value.replace(/\D/g, '').length === 11;
}

function reset() {
  Object.assign(form, { name: '', phoneFormat: '', phoneDigital: '' });
  sent.value = false;
  phoneError.value = '';
  errorMessage.value = '';
  isSubmitting.value = false;
}

async function submit() {
  phoneError.value = isPhoneComplete(form.phoneFormat)
    ? ''
    : t('callback.phoneError');
  if (phoneError.value) return;

  errorMessage.value = '';
  isSubmitting.value = true;

  try {
    await sendCallback({
      name: form.name.trim(),
      phoneDigital: form.phoneDigital,
      phoneFormat: form.phoneFormat,
    });
    sent.value = true;
    metrika.reachGoal('callback_request');
  } catch {
    errorMessage.value = t('callback.error');
  } finally {
    isSubmitting.value = false;
  }
}

function close() {
  closeCallback();
}

const FOCUSABLE =
  'a[href], button:not([disabled]), input:not([disabled]), select:not([disabled]), textarea:not([disabled]), [tabindex]:not([tabindex="-1"])';

// Лёгкая ловушка фокуса: Tab с последнего элемента возвращает на первый.
function onKeydown(event: KeyboardEvent) {
  if (event.key !== 'Tab') return;

  const card = cardRef.value;
  if (!card) return;

  const items = Array.from(
    card.querySelectorAll<HTMLElement>(FOCUSABLE),
  ).filter((el) => el.offsetParent !== null);
  if (items.length === 0) return;

  const first = items[0]!;
  const last = items[items.length - 1]!;
  const active = document.activeElement as HTMLElement | null;

  if (event.shiftKey && (active === first || !card.contains(active))) {
    event.preventDefault();
    last.focus();
  } else if (!event.shiftKey && active === last) {
    event.preventDefault();
    first.focus();
  }
}

function onEscape(event: KeyboardEvent) {
  if (event.key !== 'Escape') return;
  event.preventDefault();
  close();
}

// Элемент, который открыл диалог: на него возвращаем фокус после закрытия.
let opener: HTMLElement | null = null;

watch(
  isOpen,
  async (open) => {
    if (!import.meta.client) return;

    if (open) {
      opener = document.activeElement as HTMLElement | null;
      document.body.style.overflow = 'hidden';
      document.addEventListener('keydown', onEscape);
      await nextTick();
      nameInputRef.value?.focus();
      return;
    }

    document.body.style.overflow = '';
    document.removeEventListener('keydown', onEscape);
    opener?.focus?.();
    opener = null;
    reset();
  },
  { immediate: true },
);

onBeforeUnmount(() => {
  if (!import.meta.client) return;
  document.body.style.overflow = '';
  document.removeEventListener('keydown', onEscape);
});
</script>
