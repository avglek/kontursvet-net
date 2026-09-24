export interface ICardView {
  id: number;
  name: string;
  case: string;
  title: string;
  description: string;
  task: string;
  works: string[];
  location: string;
  term: string;
  team: string;
  period: string;
  features: string;
  meta: string[];
  photos?: IGallery[];
}

export interface ICard {
  id: number;
  link: string;
  title: string;
  subTitle: string;
  description: string;
  img: {
    src: string;
    alt: string;
  };
}

export interface IPhoto {
  id: number;
  case: string;
  gallery: IGallery[];
}

interface IGallery {
  key: number;
  src: string;
  alt: string;
  figcaption: string;
}

export interface IModalLeadPanel {
  title: string;
  message: string;
}
