import type { CardCount, CardCountPhotos } from '~/types/api';

export async function useCounters() {
  const caseForms: [string, string, string] = ['кейс', 'кейса', 'кейсов'];
  const photosForms: [string, string, string] = ['кадр', 'кадра', 'кадров'];

  const responseCount = await fetch('/api/profile/cards/count');
  const responsePhotos = await fetch('/api/profile/cards/photos-count');

  let quantityCaseString = '0 кейсов';
  let quantityCasePhotos = '0 кадров';

  if (!responseCount.ok || !responsePhotos.ok) {
    throw new Error('Ошибка при загрузке данных');
  }

  const count = (await responseCount.json()) as CardCount;
  const photosCount = (await responsePhotos.json()) as CardCountPhotos;

  if (count.quantity > 0) {
    quantityCaseString =
      count.quantity + ' ' + pluralize(count.quantity, caseForms);
  }

  if (photosCount.photos > 0) {
    quantityCasePhotos =
      photosCount.photos + ' ' + pluralize(photosCount.photos, photosForms);
  }

  return {
    quantityCaseString,
    quantityCasePhotos,
  };
}
