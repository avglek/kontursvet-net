// Общее состояние модальных окон страницы.
// Состояние живёт в useState, поэтому переживает SSR-рендер и доступно
// из любого компонента без пробрасывания пропсов.

export type NoticeKind = 'portfolio' | 'calc';

export const NOTICE_CONTENT: Record<NoticeKind, { title: string, text: string }> = {
  portfolio: {
    title: 'Полное портфолио проектов',
    text: 'Все 5 ключевых реализованных объектов (Гатчина, Васкелово, Репино, Всеволожск, Flex neon) представлены в интерактивной галерее ниже на странице.',
  },
  calc: {
    title: 'Калькулятор стоимости',
    text: 'Готовый калькулятор будет подключён на отдельной странице рабочего сайта. В этом макете ссылка пока демонстрационная.',
  },
};

export function useModals() {
  const lightboxSrc = useState<string>('modal:lightbox-src', () => '');
  const lightboxAlt = useState<string>('modal:lightbox-alt', () => '');
  const isLightboxOpen = useState<boolean>('modal:lightbox', () => false);
  const isLeadOpen = useState<boolean>('modal:lead', () => false);
  const noticeKind = useState<NoticeKind | ''>('modal:notice', () => '');

  const noticeContent = computed(() =>
    noticeKind.value ? NOTICE_CONTENT[noticeKind.value] : null,
  );

  const isAnyOpen = computed(
    () => isLightboxOpen.value || isLeadOpen.value || noticeKind.value !== '',
  );

  function openLightbox(src: string, alt = 'Фотография проекта') {
    lightboxSrc.value = src;
    lightboxAlt.value = alt;
    isLightboxOpen.value = true;
  }

  function closeLightbox() {
    isLightboxOpen.value = false;
    lightboxSrc.value = '';
  }

  function openLead() {
    isLeadOpen.value = true;
  }

  function closeLead() {
    isLeadOpen.value = false;
  }

  function openNotice(kind: NoticeKind) {
    noticeKind.value = kind;
  }

  function closeNotice() {
    noticeKind.value = '';
  }

  function closeAll() {
    closeLightbox();
    closeLead();
    closeNotice();
  }

  return {
    lightboxSrc,
    lightboxAlt,
    isLightboxOpen,
    isLeadOpen,
    noticeKind,
    noticeContent,
    isAnyOpen,
    openLightbox,
    closeLightbox,
    openLead,
    closeLead,
    openNotice,
    closeNotice,
    closeAll,
  };
}
