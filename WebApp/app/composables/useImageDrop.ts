// Общий обработчик drag-and-drop для зон загрузки фотографий.
// Возвращает состояние и обработчики, которые шаблон вешает прямо на разметку.

interface UseImageDropOptions {
  input: Ref<HTMLInputElement | null>;
  /** Возвращает текст ошибки или null, если файл подходит. */
  validate?: (file: File) => string | null;
  onFile: (file: File | null) => void;
}

export function useImageDrop({ input, validate, onFile }: UseImageDropOptions) {
  const dragOver = ref(false);
  const error = ref("");

  function accept(files: FileList | null) {
    const file = files?.[0] ?? null;
    error.value = "";

    if (!file) {
      onFile(null);
      return;
    }

    const message = validate?.(file) ?? null;
    if (message) {
      error.value = message;
      if (input.value) input.value.value = "";
      onFile(null);
      return;
    }

    onFile(file);
  }

  function openPicker() {
    input.value?.click();
  }

  function onKeydown(event: KeyboardEvent) {
    if (event.key === "Enter" || event.key === " ") {
      event.preventDefault();
      openPicker();
    }
  }

  function onDragEnter(event: DragEvent) {
    event.preventDefault();
    dragOver.value = true;
  }

  function onDragOver(event: DragEvent) {
    event.preventDefault();
    dragOver.value = true;
  }

  function onDragLeave(event: DragEvent) {
    event.preventDefault();
    dragOver.value = false;
  }

  function onDrop(event: DragEvent) {
    event.preventDefault();
    dragOver.value = false;

    const dropped = event.dataTransfer?.files[0];
    if (!dropped || !input.value) return;

    const transfer = new DataTransfer();
    transfer.items.add(dropped);
    input.value.files = transfer.files;
    accept(input.value.files);
  }

  function onChange() {
    accept(input.value?.files ?? null);
  }

  function reset() {
    if (input.value) input.value.value = "";
    dragOver.value = false;
    error.value = "";
    onFile(null);
  }

  return {
    dragOver,
    error,
    openPicker,
    onKeydown,
    onDragEnter,
    onDragOver,
    onDragLeave,
    onDrop,
    onChange,
    reset,
  };
}

export const ACCEPTED_IMAGE_TYPES = ["image/jpeg", "image/png", "image/webp"];

export function validatePhoto(file: File, maxMb = 10) {
  if (
    !ACCEPTED_IMAGE_TYPES.includes(file.type) ||
    file.size > maxMb * 1024 * 1024
  ) {
    return `Выберите JPG, PNG или WEBP размером до ${maxMb} МБ.`;
  }
  return null;
}
