// Контент страницы. Вынесен из разметки, чтобы тексты и картинки правились
// в одном месте, а секции оставались простыми циклами по данным.

export interface Project {
  image: string;
  alt: string;
  position: string;
  title: string;
  description: string;
  featured?: boolean;
  id?: number;
}

export interface Service {
  number: string;
  title: string;
  description: string;
  image: string;
  alt: string;
  position: string;
}

export interface CaseItem {
  image: string;
  alt: string;
  location: string;
  title: string;
  facts: string[];
  bullets: string[];
}

export interface ValueStep {
  step: string;
  tag: string;
  title: string;
  description: string;
}

export const valueSteps: ValueStep[] = [
  {
    step: '01',
    tag: 'Геометрия и стиль',
    title: 'Архитектура',
    description: 'Анализируем форму дома, пропорции и выносы кровли',
  },
  {
    step: '02',
    tag: 'Световые акценты',
    title: 'Фасад',
    description: 'Мягко подчёркиваем фактуру стен, колонны и входную группу',
  },
  {
    step: '03',
    tag: 'Графика линий',
    title: 'Кровля',
    description: 'Чёткий контурный flex neon по скатам и тёплая бахрома',
  },
  {
    step: '04',
    tag: 'Связь с природой',
    title: 'Участок',
    description: 'Световое оформление вековых сосен, дорожек и въезда',
  },
  {
    step: '05',
    tag: 'Единое управление',
    title: 'Световой сценарий',
    description: 'Парадный вечерний и деликатный ночной режимы',
  },
];

export const projects: Project[] = [
  {
    image: '/images/home/001.webp',
    alt: 'Световое оформление, Гатчина',
    position: '50% 48%',
    title: 'Гатчина',
    description: 'Два дома · flex neon · бахрома · световые мотивы',
    featured: true,
    id: 1,
  },
  {
    image: '/images/home/002.webp',
    alt: 'Световое оформление, Васкелово',
    position: '50% 50%',
    title: 'Васкелово',
    description: 'Дом, зона барбекю и деревья',
    id: 2,
  },
  {
    image: '/images/home/003.webp',
    alt: 'Световое оформление, Репино',
    position: '50% 42%',
    title: 'Репино',
    description: 'Контурное освещение частного дома',
    id: 7,
  },
  {
    image: '/images/home/004.webp',
    alt: 'Световое оформление, Всеволожск',
    position: '50% 46%',
    title: 'Всеволожск',
    description: 'Дом, барбекю и световые растяжки',
    id: 3,
  },
  {
    image: '/images/home/005.webp',
    alt: 'Световое оформление, Flex neon',
    position: '50% 50%',
    title: 'Flex neon',
    description: 'Усадьба и ландшафт в единой линии света',
  },
];

export const services: Service[] = [
  {
    number: '01 · КРОВЛЯ / ФАСАД',
    title: 'Контурная подсветка flex neon',
    description:
      'Чёткая световая линия подчёркивает форму кровли и архитектурную геометрию дома, не перегружая фасад.',
    image: '/images/home/007.webp',
    alt: 'Контурная подсветка flex neon',
    position: '50% 44%',
  },
  {
    number: '02 · КАРНИЗ / ТЕРРАСА',
    title: 'Гирлянда «бахрома»',
    description:
      'Декоративное оформление кровли, карнизов и отдельных зон фасада добавляет глубину вечернему образу.',
    image: '/images/home/008.webp',
    alt: 'Гирлянда «бахрома»',
    position: '50% 48%',
  },
  {
    number: '03 · ДЕРЕВЬЯ / ЛАНДШАФТ',
    title: 'Световое оформление деревьев',
    description:
      'Свет на стволах, кроне и отдельных акцентах связывает архитектуру дома с пространством участка.',
    image: '/images/home/009.webp',
    alt: 'Световое оформление деревьев',
    position: '50% 55%',
  },
  {
    number: '04 · ПОД КЛЮЧ',
    title: 'Комплексное оформление',
    description:
      'Дом, дополнительные строения, деревья и территория работают как единая световая композиция.',
    image: '/images/home/010.webp',
    alt: 'Комплексное оформление',
    position: '50% 50%',
  },
];

export const includedItems = [
  {
    number: '01',
    title: 'Анализ объекта',
    description:
      'Архитектура дома, участок, точки монтажа и особенности объекта.',
  },
  {
    number: '02',
    title: 'Подбор решения',
    description:
      'Определяем, где уместны flex neon, бахрома, деревья или комплекс.',
  },
  {
    number: '03',
    title: 'Материалы',
    description: 'Комплектующие и световые решения для наружной эксплуатации.',
  },
  {
    number: '04',
    title: 'Профессиональный монтаж',
    description: 'Работа бригады, в том числе на сложных и высотных точках.',
  },
  {
    number: '05',
    title: 'Гарантия 1 год',
    description:
      'Гарантия на выполненные работы в течение года после реализации.',
  },
  {
    number: '06',
    title: 'Постобслуживание',
    description:
      'После гарантии доступно постгарантийное обслуживание объекта.',
  },
];

export const cases: CaseItem[] = [
  {
    image: '/images/home/13.webp',
    alt: 'Кейс КонтурСвет: Гатчина',
    location: 'Гатчина',
    title: 'Два дома в одной световой стилистике',
    facts: ['2 дня', '6 человек', 'flex neon + бахрома'],
    bullets: [
      'Большой дом: около 100 м flex neon и около 100 м бахромы',
      '12 мотивов «Полярис»',
      'Малый дом: около 24 м flex neon и около 24 м бахромы',
      'Снежинки и световые мотивы',
    ],
  },
  {
    image: '/images/home/14.webp',
    alt: 'Кейс КонтурСвет: Васкелово',
    location: 'Васкелово',
    title: 'Дом и участок в единой композиции',
    facts: ['2 дня', '5 человек', 'точки выше 15 м'],
    bullets: [
      'Более 170 м flex neon и бахромы на доме',
      '35 м на зоне барбекю',
      'Световые занавесы',
      '70 м гирлянды-нити на ели',
    ],
  },
];

export const processSteps = [
  {
    number: '01',
    title: 'Вы отправляете фото',
    description: 'Дом, участок и населённый пункт.',
  },
  {
    number: '02',
    title: 'Александр уточняет задачу',
    description: 'Что хотите оформить и какой результат ожидаете.',
  },
  {
    number: '03',
    title: 'Подбираем решение',
    description: 'Формируем направление и предварительный объём.',
  },
  {
    number: '04',
    title: 'Согласуем проект',
    description: 'Фиксируем состав работ и дальнейшие действия.',
  },
  {
    number: '05',
    title: 'Выполняем монтаж',
    description: 'Реализуем согласованное световое оформление.',
  },
];

export const navLinks = [
  { label: 'Проекты', href: '#projects' },
  { label: 'Решения', href: '#solutions' },
  { label: 'Кейсы', href: '#cases' },
  { label: 'Как работаем', href: '#process' },
  { label: 'Контакты', href: '#contact' },
];

export const portfolioStats = {
  cases: 7,
  photos: 40,
};
