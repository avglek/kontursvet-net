import { useState } from '#app';

export const useLightbox = () => {
  const activeImageSrc = useState<string | null>('lightbox-src', () => null);

  const img = useImage();

  const openLightbox = (src: any) => {
    activeImageSrc.value = src;
  };

  const closeLightbox = () => {
    activeImageSrc.value = null;
  };

  return {
    activeImageSrc,
    openLightbox,
    closeLightbox,
  };
};
