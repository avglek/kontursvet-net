<template>
  <div class="scene-motion">
    <span role="status">{{ statusText }}</span>
    <div class="scene-motion-actions">
      <button
        class="motion-button"
        type="button"
        :disabled="staticOn || !hasActive || replayBusy || reducedMotion"
        @click="emit('replay')"
      >
        ↻ Повторить включение
      </button>
      <button
        class="motion-button"
        type="button"
        :aria-pressed="staticOn"
        :disabled="reducedMotion"
        @click="staticClickHandler()"
      >
        {{ onStatic ? 'Анимация' : 'Статично' }}
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
defineProps<{
  replayBusy: boolean;
  hasActive: boolean;
  staticOn: boolean;
  reducedMotion: boolean;
  statusText: string;
}>();

const onStatic = ref<boolean>(false);

const emit = defineEmits<{
  replay: [];
  'toggle-static': [];
}>();

const staticClickHandler = () => {
  onStatic.value = !onStatic.value;
  emit('toggle-static');
};
</script>
