<template>
  <div class="upload">
    <div class="upload__preview">
      <img v-if="src" :src="src" alt="" />
      <span v-else>Изображение не выбрано</span>
    </div>

    <div class="upload__controls">
      <input
        v-model.trim="src"
        class="upload__path"
        type="text"
        placeholder="/uploads/… или /images/…"
        aria-label="Путь к изображению"
      />
      <label class="button button-small button-ghost" :class="{ 'is-busy': busy }">
        {{ busy ? 'Загрузка…' : 'Загрузить файл' }}
        <input type="file" accept="image/*" hidden :disabled="busy" @change="onPick" />
      </label>
    </div>

    <p v-if="error" class="field__error">{{ error }}</p>
  </div>
</template>

<script lang="ts" setup>
const src = defineModel<string>({ default: '' });

const admin = usePortfolioAdmin();
const busy = ref(false);
const error = ref<string | null>(null);

const onPick = async (e: Event) => {
  const input = e.target as HTMLInputElement;
  const file = input.files?.[0];
  input.value = '';
  if (!file) return;

  error.value = null;
  busy.value = true;
  try {
    const stored = await admin.uploadImage(file);
    src.value = stored.url;
  } catch (err) {
    error.value =
      err instanceof Error && err.message
        ? err.message
        : 'Не удалось загрузить файл';
  } finally {
    busy.value = false;
  }
};
</script>
