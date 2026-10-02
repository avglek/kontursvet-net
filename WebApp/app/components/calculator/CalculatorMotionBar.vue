<script setup lang="ts">
defineProps<{
  replayBusy: boolean;
  hasActive: boolean;
  staticOn: boolean;
  reducedMotion: boolean;
  statusText: string;
}>();

const emit = defineEmits<{
  replay: [];
  "toggle-static": [];
}>();
</script>

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
        @click="emit('toggle-static')"
      >
        Статично
      </button>
    </div>
  </div>
</template>
