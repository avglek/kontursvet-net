<template>
  <div>
    <p v-if="loading" class="admin-state">Загрузка…</p>
    <p v-else-if="loadError" class="admin-alert" role="alert">{{ loadError }}</p>

    <form v-else class="admin-form" novalidate @submit.prevent="save">
      <p v-if="formError" class="admin-alert" role="alert">{{ formError }}</p>

      <section class="admin-card">
        <h2 class="admin-card__title">Карточка товара</h2>

        <div class="admin-grid">
          <div class="field">
            <label for="p-code">Артикул</label>
            <input id="p-code" v-model.trim="form.code" placeholder="KS-001" />
            <span v-if="errors.code" class="field__error">{{ errors.code }}</span>
          </div>

          <div class="field">
            <label for="p-group">Группа</label>
            <input id="p-group" v-model.trim="form.group" placeholder="Светодиодная лента" />
            <span v-if="errors.group" class="field__error">{{ errors.group }}</span>
          </div>

          <div class="field field-wide">
            <label for="p-name">Название</label>
            <input
              id="p-name"
              v-model.trim="form.name"
              placeholder="Гибкая светодиодная лента 24 В"
            />
            <span v-if="errors.name" class="field__error">{{ errors.name }}</span>
          </div>

          <div class="field field-wide">
            <label for="p-desc">Описание</label>
            <textarea id="p-desc" v-model="form.description" rows="5"></textarea>
          </div>

          <div class="field">
            <label for="p-price">Цена, ₽</label>
            <input
              id="p-price"
              v-model.number="form.price"
              type="number"
              min="0"
              step="0.01"
              placeholder="0"
            />
            <span v-if="errors.price" class="field__error">{{ errors.price }}</span>
          </div>

          <div class="field">
            <label for="p-active">Статус</label>
            <label class="admin-checkbox">
              <input id="p-active" v-model="form.isActive" type="checkbox" />
              <span>Активен (виден на сайте)</span>
            </label>
          </div>

          <div class="field field-wide">
            <label>Изображение</label>
            <AdminImageUpload v-model="imgSrc" />
          </div>
        </div>
      </section>

      <div class="admin-actions">
        <button type="submit" class="button button-primary" :disabled="saving">
          {{ saving ? 'Сохранение…' : isEdit ? 'Сохранить' : 'Создать товар' }}
        </button>
        <NuxtLink class="button button-ghost" to="/admin/products">Отмена</NuxtLink>
        <button
          v-if="isEdit"
          type="button"
          class="button button-danger"
          :disabled="deleting || saving"
          @click="removeProduct"
        >
          {{ deleting ? 'Удаление…' : 'Удалить товар' }}
        </button>
      </div>
    </form>
  </div>
</template>

<script lang="ts" setup>
import { ApiError } from '~/composables/useApi';
import { useProductsAdmin } from '~/composables/useProductsAdmin';
import type { ProductCardDto } from '~/types/api';

const props = defineProps<{ id?: number }>();

const admin = useProductsAdmin();

const currentId = ref<number | undefined>(props.id);
const isEdit = computed(() => currentId.value !== undefined);

const loading = ref(props.id !== undefined);
const loadError = ref<string | null>(null);
const saving = ref(false);
const deleting = ref(false);
const formError = ref<string | null>(null);

const form = reactive({
  code: '',
  name: '',
  description: '',
  group: '',
  price: 0 as number,
  isActive: true,
});

const imgSrc = ref('');
const errors = reactive<Record<string, string>>({});

const errMsg = (err: unknown, fallback: string) =>
  err instanceof Error && err.message ? err.message : fallback;

const scrollTop = () => window.scrollTo({ top: 0, behavior: 'smooth' });

onMounted(async () => {
  if (props.id === undefined) return;
  try {
    const p = await admin.getProduct(props.id);
    Object.assign(form, {
      code: p.code,
      name: p.name,
      description: p.description,
      group: p.group,
      price: p.price,
      isActive: p.isActive,
    });
    imgSrc.value = p.imgSrc ?? '';
  } catch (err) {
    loadError.value =
      err instanceof ApiError && err.status === 404
        ? 'Товар не найден'
        : errMsg(err, 'Не удалось загрузить товар');
  } finally {
    loading.value = false;
  }
});

const validate = () => {
  Object.keys(errors).forEach((k) => delete errors[k]);

  if (!form.code) errors.code = 'Укажите артикул';
  if (!form.name) errors.name = 'Укажите название';
  if (!form.group) errors.group = 'Укажите группу';
  if (form.price < 0 || Number.isNaN(form.price))
    errors.price = 'Цена не может быть отрицательной';

  return Object.keys(errors).length === 0;
};

const buildDto = (id: number): ProductCardDto => ({
  id,
  code: form.code,
  name: form.name,
  description: form.description.trim(),
  group: form.group,
  price: form.price,
  imgSrc: imgSrc.value || null,
  isActive: form.isActive,
});

const save = async () => {
  formError.value = null;
  if (!validate()) {
    formError.value = 'Заполните обязательные поля.';
    scrollTop();
    return;
  }

  saving.value = true;
  try {
    if (currentId.value === undefined) {
      const created = await admin.createProduct(buildDto(0));
      currentId.value = created.id;
    } else {
      await admin.updateProduct(currentId.value, buildDto(currentId.value));
    }
    await navigateTo('/admin/products');
  } catch (err) {
    formError.value = errMsg(err, 'Не удалось сохранить');
    scrollTop();
  } finally {
    saving.value = false;
  }
};

const removeProduct = async () => {
  if (currentId.value === undefined) return;
  if (!confirm(`Удалить «${form.name}»? Это необратимо.`)) return;

  deleting.value = true;
  formError.value = null;
  try {
    await admin.deleteProduct(currentId.value);
    await navigateTo('/admin/products');
  } catch (err) {
    formError.value = errMsg(err, 'Не удалось удалить товар');
    scrollTop();
  } finally {
    deleting.value = false;
  }
};
</script>
