<template>
  <div>
    <p v-if="loading" class="admin-state">Загрузка…</p>
    <p v-else-if="loadError" class="admin-alert" role="alert">{{ loadError }}</p>

    <form v-else class="admin-form" novalidate @submit.prevent="save">
      <p v-if="formError" class="admin-alert" role="alert">{{ formError }}</p>

      <!-- ===== Карточка в списке ===== -->
      <section class="admin-card">
        <h2 class="admin-card__title">Карточка в списке</h2>
        <p class="admin-card__lead">
          Так объект выглядит на странице «Портфолио».
        </p>

        <div class="admin-grid">
          <div class="field">
            <label for="f-link">Идентификатор</label>
            <input id="f-link" v-model.trim="card.link" placeholder="case-08" />
            <span class="field__hint">
              Латиница, цифры и дефис. Используется как якорь в списке.
            </span>
            <span v-if="errors.link" class="field__error">{{ errors.link }}</span>
          </div>

          <div class="field">
            <label for="f-title">Заголовок над названием</label>
            <input
              id="f-title"
              v-model.trim="card.title"
              placeholder="Кейс 08 · Город или посёлок"
            />
            <span v-if="errors.title" class="field__error">{{ errors.title }}</span>
          </div>

          <div class="field field-wide">
            <label for="f-subtitle">Название объекта</label>
            <input
              id="f-subtitle"
              v-model.trim="card.subTitle"
              placeholder="Частный дом в Репино"
            />
          </div>

          <div class="field field-wide">
            <label for="f-desc">Короткое описание</label>
            <textarea
              id="f-desc"
              v-model="card.description"
              placeholder="Что сделали: контурная подсветка flex neon, бахрома…"
            ></textarea>
          </div>

          <div class="field field-wide">
            <label>Обложка</label>
            <AdminImageUpload v-model="card.img.src" />
            <span v-if="errors.imgSrc" class="field__error">{{ errors.imgSrc }}</span>
          </div>

          <div class="field field-wide">
            <label for="f-img-alt">Описание обложки (alt)</label>
            <input
              id="f-img-alt"
              v-model="card.img.alt"
              placeholder="Дом в Репино с контурной подсветкой"
            />
          </div>
        </div>
      </section>

      <!-- ===== Страница проекта ===== -->
      <section class="admin-card">
        <h2 class="admin-card__title">Страница проекта</h2>
        <p class="admin-card__lead">
          Если поле «Задача заказчика» пустое, блок с задачей и фактами на
          странице не показывается — останутся только описание и фото.
        </p>

        <div class="admin-grid">
          <div class="field">
            <label for="v-part">Метка кейса</label>
            <input id="v-part" v-model.trim="view.part" placeholder="Кейс 08" />
            <span v-if="errors.part" class="field__error">{{ errors.part }}</span>
          </div>

          <div class="field">
            <label for="v-title">Заголовок страницы</label>
            <input
              id="v-title"
              v-model.trim="view.title"
              placeholder="Контурная подсветка для дома в Репино"
            />
            <span v-if="errors.viewTitle" class="field__error">
              {{ errors.viewTitle }}
            </span>
          </div>

          <div class="field field-wide">
            <label for="v-desc">Описание</label>
            <textarea id="v-desc" v-model="view.description" rows="5"></textarea>
          </div>

          <div class="field field-wide">
            <label for="v-task">Задача заказчика</label>
            <textarea id="v-task" v-model="view.task"></textarea>
          </div>

          <div class="field field-wide">
            <label for="v-works">Объём работ</label>
            <textarea
              id="v-works"
              v-model="view.worksText"
              rows="4"
              placeholder="Каждый пункт с новой строки"
            ></textarea>
          </div>

          <div class="field">
            <label for="v-location">Локация</label>
            <input id="v-location" v-model.trim="view.location" />
          </div>

          <div class="field">
            <label for="v-term">Срок</label>
            <input id="v-term" v-model.trim="view.term" placeholder="2 дня" />
          </div>

          <div class="field">
            <label for="v-team">Команда</label>
            <input
              id="v-team"
              v-model.trim="view.team"
              placeholder="бригада из 5 человек"
            />
          </div>

          <div class="field">
            <label for="v-period">Период</label>
            <input id="v-period" v-model.trim="view.period" placeholder="декабрь" />
          </div>

          <div class="field field-wide">
            <label for="v-features">Особенности монтажа</label>
            <textarea id="v-features" v-model="view.features"></textarea>
          </div>
        </div>
      </section>

      <!-- ===== Строка под заголовком страницы ===== -->
      <section class="admin-card">
        <h2 class="admin-card__title">Краткая сводка</h2>
        <p class="admin-card__lead">
          Три коротких значения над галереей на странице проекта.
        </p>

        <div class="admin-grid admin-grid--3">
          <div class="field">
            <label for="m-0">Место</label>
            <input id="m-0" v-model.trim="view.meta[0]" placeholder="Репино" />
          </div>
          <div class="field">
            <label for="m-1">Тип объекта</label>
            <input id="m-1" v-model.trim="view.meta[1]" placeholder="частный дом" />
          </div>
          <div class="field">
            <label for="m-2">Состав работ</label>
            <input
              id="m-2"
              v-model.trim="view.meta[2]"
              placeholder="контурная подсветка flex neon"
            />
          </div>
        </div>
      </section>

      <!-- ===== Галерея ===== -->
      <section class="admin-card">
        <h2 class="admin-card__title">Галерея</h2>
        <p class="admin-card__lead">
          Порядок фото на сайте — как в этом списке. Нумерация (key)
          проставляется автоматически при сохранении.
        </p>

        <ol v-if="gallery.length" class="admin-gallery">
          <li v-for="(photo, i) in gallery" :key="photo.uid" class="admin-photo">
            <div class="admin-photo__img">
              <img :src="photo.src" :alt="photo.alt" loading="lazy" />
            </div>

            <div class="admin-photo__fields">
              <div class="field">
                <label :for="`g-alt-${photo.uid}`">Описание (alt)</label>
                <input :id="`g-alt-${photo.uid}`" v-model="photo.alt" />
              </div>
              <div class="field">
                <label :for="`g-cap-${photo.uid}`">Подпись под фото</label>
                <input :id="`g-cap-${photo.uid}`" v-model="photo.figcaption" />
              </div>
              <p class="admin-photo__path">{{ photo.src }}</p>
            </div>

            <div class="admin-photo__tools">
              <button
                type="button"
                class="icon-button"
                :disabled="i === 0"
                aria-label="Поднять выше"
                @click="move(i, -1)"
              >
                ↑
              </button>
              <button
                type="button"
                class="icon-button"
                :disabled="i === gallery.length - 1"
                aria-label="Опустить ниже"
                @click="move(i, 1)"
              >
                ↓
              </button>
              <button
                type="button"
                class="icon-button icon-button--danger"
                aria-label="Убрать фото"
                @click="removePhoto(i)"
              >
                ×
              </button>
            </div>
          </li>
        </ol>
        <p v-else class="admin-state">В галерее пока нет фотографий.</p>

        <p v-if="galleryError" class="field__error">{{ galleryError }}</p>

        <label
          class="button button-ghost admin-add-photos"
          :class="{ 'is-busy': galleryBusy }"
        >
          {{ galleryBusy ? 'Загрузка…' : '+ Добавить фото' }}
          <input
            type="file"
            accept="image/*"
            multiple
            hidden
            :disabled="galleryBusy"
            @change="addPhotos"
          />
        </label>
      </section>

      <!-- ===== Действия ===== -->
      <div class="admin-actions">
        <button type="submit" class="button button-primary" :disabled="saving">
          {{ saving ? 'Сохранение…' : isEdit ? 'Сохранить' : 'Создать объект' }}
        </button>
        <NuxtLink class="button button-ghost" to="/admin/portfolio">
          Отмена
        </NuxtLink>
        <button
          v-if="isEdit"
          type="button"
          class="button button-danger"
          :disabled="deleting || saving"
          @click="removeCard"
        >
          {{ deleting ? 'Удаление…' : 'Удалить объект' }}
        </button>
      </div>
    </form>
  </div>
</template>

<script lang="ts" setup>
import { ApiError } from '~/composables/useApi';
import type {
  GalleryItemDto,
  PortfolioCardDto,
  PortfolioCardViewDto,
} from '~/types/api';

const props = defineProps<{ id?: number }>();

const admin = usePortfolioAdmin();

// После успешного создания карточки id запоминается:
// если сохранение страницы проекта не удалось, повторное нажатие обновит, а не создаст дубль
const currentId = ref<number | undefined>(props.id);
const isEdit = computed(() => currentId.value !== undefined);

const loading = ref(props.id !== undefined);
const loadError = ref<string | null>(null);
const saving = ref(false);
const deleting = ref(false);
const formError = ref<string | null>(null);
const galleryBusy = ref(false);
const galleryError = ref<string | null>(null);

interface GalleryRow extends GalleryItemDto {
  uid: number;
}
let uidCounter = 0;
const toRow = (g: GalleryItemDto): GalleryRow => ({ ...g, uid: ++uidCounter });

const card = reactive({
  link: '',
  title: '',
  subTitle: '',
  description: '',
  img: { src: '', alt: '' },
});

const view = reactive({
  part: '',
  title: '',
  description: '',
  task: '',
  worksText: '',
  location: '',
  term: '',
  team: '',
  period: '',
  features: '',
  meta: ['', '', ''] as string[],
});

const gallery = ref<GalleryRow[]>([]);
const errors = reactive<Record<string, string>>({});

const errMsg = (err: unknown, fallback: string) =>
  err instanceof Error && err.message ? err.message : fallback;

const scrollTop = () => window.scrollTo({ top: 0, behavior: 'smooth' });

// ---------- Загрузка (режим редактирования) ----------
onMounted(async () => {
  if (props.id === undefined) return;

  try {
    const [c, v] = await Promise.all([
      admin.getCard(props.id),
      admin.getView(props.id),
    ]);

    Object.assign(card, {
      link: c.link,
      title: c.title,
      subTitle: c.subTitle,
      description: c.description,
      img: { src: c.img?.src ?? '', alt: c.img?.alt ?? '' },
    });

    if (v) {
      Object.assign(view, {
        part: v.part,
        title: v.title,
        description: v.description,
        task: v.task,
        worksText: (v.works ?? []).join('\n'),
        location: v.location,
        term: v.term,
        team: v.team,
        period: v.period,
        features: v.features,
        meta: [v.meta?.[0] ?? '', v.meta?.[1] ?? '', v.meta?.[2] ?? ''],
      });
      gallery.value = (v.gallery ?? []).map(toRow);
    } else {
      // Карточка есть, страницы проекта ещё нет — подставляем то, что известно
      view.part = '';
      view.title = c.subTitle;
    }
  } catch (err) {
    loadError.value =
      err instanceof ApiError && err.status === 404
        ? 'Объект не найден'
        : errMsg(err, 'Не удалось загрузить объект');
  } finally {
    loading.value = false;
  }
});

// ---------- Галерея ----------
const addPhotos = async (e: Event) => {
  const input = e.target as HTMLInputElement;
  const files = Array.from(input.files ?? []);
  input.value = '';
  if (!files.length) return;

  galleryError.value = null;
  galleryBusy.value = true;
  try {
    for (const file of files) {
      const stored = await admin.uploadImage(file);
      gallery.value.push(
        toRow({ key: 0, src: stored.url, alt: card.subTitle, figcaption: '' }),
      );
    }
  } catch (err) {
    galleryError.value = errMsg(err, 'Не удалось загрузить фото');
  } finally {
    galleryBusy.value = false;
  }
};

const move = (index: number, dir: -1 | 1) => {
  const target = index + dir;
  const list = gallery.value;
  if (target < 0 || target >= list.length) return;
  [list[index], list[target]] = [list[target]!, list[index]!];
};

const removePhoto = (index: number) => {
  gallery.value.splice(index, 1);
};

// ---------- Валидация и сборка payload ----------
const validate = () => {
  Object.keys(errors).forEach((k) => delete errors[k]);

  if (!card.link) errors.link = 'Укажите идентификатор';
  else if (!/^[a-z0-9-]+$/.test(card.link))
    errors.link = 'Только строчная латиница, цифры и дефис';

  if (!card.title) errors.title = 'Укажите заголовок карточки';
  if (!card.img.src) errors.imgSrc = 'Добавьте обложку';
  if (!view.part) errors.part = 'Укажите метку кейса';
  if (!view.title) errors.viewTitle = 'Укажите заголовок страницы';

  return Object.keys(errors).length === 0;
};

const buildCard = (id: number): PortfolioCardDto => ({
  id,
  link: card.link,
  title: card.title,
  subTitle: card.subTitle,
  description: card.description.trim(),
  img: { src: card.img.src, alt: card.img.alt.trim() },
});

const buildView = (id: number): PortfolioCardViewDto => ({
  id,
  name: card.link, // на сайте name страницы совпадает с идентификатором карточки
  part: view.part,
  title: view.title,
  description: view.description.trim(),
  task: view.task.trim(),
  works: view.worksText
    .split('\n')
    .map((s) => s.trim())
    .filter(Boolean),
  location: view.location,
  term: view.term,
  team: view.team,
  period: view.period,
  features: view.features.trim(),
  meta: view.meta.map((s) => s.trim()),
  gallery: gallery.value.map((g, i) => ({
    key: i + 1,
    src: g.src,
    alt: g.alt.trim(),
    figcaption: g.figcaption.trim(),
  })),
});

// ---------- Сохранение / удаление ----------
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
      const created = await admin.createCard(buildCard(0));
      currentId.value = created.id;
    } else {
      await admin.updateCard(currentId.value, buildCard(currentId.value));
    }

    await admin.saveView(currentId.value, buildView(currentId.value));
    await navigateTo('/admin/portfolio');
  } catch (err) {
    const msg = errMsg(err, 'Не удалось сохранить');
    formError.value =
      props.id === undefined && currentId.value !== undefined
        ? `Карточка создана, но страницу проекта сохранить не удалось: ${msg}. Нажмите «Сохранить», чтобы повторить.`
        : msg;
    scrollTop();
  } finally {
    saving.value = false;
  }
};

const removeCard = async () => {
  if (currentId.value === undefined) return;
  if (!confirm('Удалить объект вместе со страницей проекта? Это необратимо.'))
    return;

  deleting.value = true;
  formError.value = null;
  try {
    await admin.deleteCard(currentId.value);
    await navigateTo('/admin/portfolio');
  } catch (err) {
    formError.value = errMsg(err, 'Не удалось удалить объект');
    scrollTop();
  } finally {
    deleting.value = false;
  }
};
</script>
