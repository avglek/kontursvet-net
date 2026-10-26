// Управление видеопрезентацией в герое: синхронизация основного и размытого
// слоёв, HUD-таймер и подстраховка, если браузер заблокировал автовоспроизведение.

const DURATION_FALLBACK = 47; // 00:47 — показывает, пока метаданные не загружены

function format(seconds: number) {
  const mm = String(Math.floor(seconds / 60)).padStart(2, '0');
  const ss = String(Math.floor(seconds % 60)).padStart(2, '0');
  return `${mm}:${ss}`;
}

export function useHeroVideo() {
  const mainVideo = ref<HTMLVideoElement | null>(null);
  const ambientVideo = ref<HTMLVideoElement | null>(null);

  const playing = ref(false);
  const status = ref('Видео на паузе');
  const timeText = ref(`00:00 / ${format(DURATION_FALLBACK)}`);
  const progress = ref(0);

  const toggleLabel = computed(() =>
    playing.value ? 'Приостановить видео' : 'Воспроизвести видео',
  );

  let mobile: MediaQueryList | null = null;

  // Размытый слой не нужен на телефонах — там он не виден, а кодек один.
  function syncAmbient() {
    const video = mainVideo.value;
    const ambient = ambientVideo.value;
    if (!video || !ambient) return;

    if (mobile?.matches || video.paused) {
      ambient.pause();
      return;
    }
    if (Math.abs(ambient.currentTime - video.currentTime) > 0.35) {
      ambient.currentTime = video.currentTime;
    }
    if (ambient.paused) ambient.play().catch(() => {});
  }

  function play() {
    const video = mainVideo.value;
    if (!video) return Promise.resolve();
    if (video.error) video.load();
    video.muted = true;

    return video.play().catch(() => {
      playing.value = false;
      status.value = 'Нажмите ▶ для запуска';
    });
  }

  function pause() {
    mainVideo.value?.pause();
    ambientVideo.value?.pause();
  }

  function toggle() {
    if (mainVideo.value?.paused) play();
    else pause();
  }

  function onPlaying() {
    playing.value = true;
    status.value = 'Шоурил проектов';
    syncAmbient();
  }

  function onPause() {
    playing.value = false;
    syncAmbient();
  }

  function onError() {
    playing.value = false;
    status.value = 'Повторите запуск видео';
  }

  function onTimeUpdate() {
    syncAmbient();
    const video = mainVideo.value;
    if (!video || !Number.isFinite(video.duration)) return;

    progress.value = (video.currentTime / video.duration) * 100;
    timeText.value = `${format(video.currentTime)} / ${format(video.duration)}`;
  }

  function onVisibilityChange() {
    if (!document.hidden && mainVideo.value && !mainVideo.value.paused) {
      play();
      syncAmbient();
    }
  }

  onMounted(() => {
    const video = mainVideo.value;
    if (!video) return;

    mobile = window.matchMedia('(max-width: 640px)');
    video.muted = true;
    video.defaultMuted = true;
    video.playsInline = true;
    if (ambientVideo.value) ambientVideo.value.muted = true;

    video.addEventListener('playing', onPlaying);
    video.addEventListener('pause', onPause);
    video.addEventListener('error', onError);
    video.addEventListener('timeupdate', onTimeUpdate);
    mobile.addEventListener('change', syncAmbient);
    document.addEventListener('visibilitychange', onVisibilityChange);

    play();
  });

  onBeforeUnmount(() => {
    const video = mainVideo.value;
    if (video) {
      video.removeEventListener('playing', onPlaying);
      video.removeEventListener('pause', onPause);
      video.removeEventListener('error', onError);
      video.removeEventListener('timeupdate', onTimeUpdate);
    }
    mobile?.removeEventListener('change', syncAmbient);
    document.removeEventListener('visibilitychange', onVisibilityChange);
  });

  return {
    mainVideo,
    ambientVideo,
    playing,
    status,
    timeText,
    progress,
    toggleLabel,
    play,
    pause,
    toggle,
  };
}
