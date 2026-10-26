// Интерактивное сравнение «до / после» в секции The Transformation.
// Разделитель управляется range-инпутом, указателем и повтором анимации.

const ANIMATION_MS = 2400;
// До 65% времени раскрываем подсвеченный фасад, затем возвращаемся к равному сплиту.
const REVEAL_PHASE = 0.65;

export function useTransformation() {
  const surface = ref<HTMLElement | null>(null);
  const range = ref<HTMLInputElement | null>(null);

  const position = ref(50);

  const surfaceStyle = computed(() => ({ '--tf-position': `${position.value}%` }));
  const rangeValue = computed(() => String(Math.round(position.value)));
  const valueText = computed(() => {
    const left = Math.round(position.value);
    return `Без подсветки — ${left}%, с подсветкой — ${100 - left}%`;
  });

  let frame: number | null = null;
  let takenByUser = false;
  let activePointer: number | null = null;
  let observer: IntersectionObserver | null = null;
  let motion: MediaQueryList | null = null;

  function setPosition(value: number) {
    position.value = Math.max(0, Math.min(100, value));
    if (range.value) range.value.value = String(Math.round(position.value));
  }

  function stopAnimation() {
    if (frame !== null) cancelAnimationFrame(frame);
    frame = null;
    surface.value?.setAttribute('data-animating', 'false');
  }

  function animate() {
    stopAnimation();
    if (motion?.matches) {
      setPosition(50);
      return;
    }

    const started = performance.now();
    surface.value?.setAttribute('data-animating', 'true');

    const tick = (now: number) => {
      const progress = Math.min(1, (now - started) / ANIMATION_MS);
      const phase = progress < REVEAL_PHASE ? progress / REVEAL_PHASE : (progress - REVEAL_PHASE) / (1 - REVEAL_PHASE);
      const eased = 0.5 - 0.5 * Math.cos(Math.PI * phase);

      setPosition(progress < REVEAL_PHASE ? 82 - 64 * eased : 18 + 32 * eased);
      frame = progress < 1 ? requestAnimationFrame(tick) : null;
      if (frame !== null) return;
      stopAnimation();
    };

    frame = requestAnimationFrame(tick);
  }

  function takeControl() {
    takenByUser = true;
    stopAnimation();
  }

  function onInput() {
    takeControl();
    setPosition(Number(range.value?.value ?? 50));
  }

  function positionFromPointer(event: PointerEvent) {
    const el = surface.value;
    if (!el) return;
    const bounds = el.getBoundingClientRect();
    setPosition(((event.clientX - bounds.left) / bounds.width) * 100);
  }

  function onPointerDown(event: PointerEvent) {
    const input = range.value;
    if (!input) return;
    if (!event.isPrimary || (event.pointerType === 'mouse' && event.button !== 0)) return;

    event.preventDefault();
    takeControl();
    activePointer = event.pointerId;
    input.focus({ preventScroll: true });
    input.setPointerCapture(event.pointerId);
    positionFromPointer(event);
  }

  function onPointerMove(event: PointerEvent) {
    if (activePointer === event.pointerId) positionFromPointer(event);
  }

  function releasePointer() {
    activePointer = null;
  }

  function onMotionChange() {
    if (motion?.matches) {
      stopAnimation();
      setPosition(50);
    }
  }

  onMounted(() => {
    const input = range.value;
    const el = surface.value;
    if (!input || !el) return;

    motion = window.matchMedia('(prefers-reduced-motion: reduce)');

    input.addEventListener('input', onInput);
    input.addEventListener('keydown', takeControl);
    input.addEventListener('pointerdown', onPointerDown);
    input.addEventListener('pointermove', onPointerMove);
    for (const event of ['pointerup', 'pointercancel', 'lostpointercapture'] as const) {
      input.addEventListener(event, releasePointer);
    }
    motion.addEventListener('change', onMotionChange);

    // Один раз проигрываем анимацию, когда секция появилась и оба кадра загружены.
    const imagesReady = Promise.all(
      Array.from(el.querySelectorAll('img')).map((image) => {
        if (image.complete && image.naturalWidth) return Promise.resolve();
        return new Promise<void>((resolve) => {
          image.addEventListener('load', resolve, { once: true });
          image.addEventListener('error', resolve, { once: true });
        });
      }),
    );

    observer = new IntersectionObserver((entries) => {
      if (!entries.some((entry) => entry.isIntersecting)) return;
      observer?.disconnect();
      imagesReady.then(() => {
        if (!takenByUser) animate();
      });
    }, { threshold: 0.2 });
    observer.observe(el);
  });

  onBeforeUnmount(() => {
    stopAnimation();
    observer?.disconnect();

    const input = range.value;
    if (input) {
      input.removeEventListener('input', onInput);
      input.removeEventListener('keydown', takeControl);
      input.removeEventListener('pointerdown', onPointerDown);
      input.removeEventListener('pointermove', onPointerMove);
      for (const event of ['pointerup', 'pointercancel', 'lostpointercapture'] as const) {
        input.removeEventListener(event, releasePointer);
      }
    }
    motion?.removeEventListener('change', onMotionChange);
  });

  return {
    surface,
    range,
    position,
    surfaceStyle,
    rangeValue,
    valueText,
    animate,
    setPosition,
  };
}
