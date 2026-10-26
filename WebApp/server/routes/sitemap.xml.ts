// Допустимые значения <changefreq> по схеме sitemaps.org 0.9
type ChangeFreq =
  | 'always'
  | 'hourly'
  | 'daily'
  | 'weekly'
  | 'monthly'
  | 'yearly'
  | 'never';

interface StaticUrl {
  path: string;
  priority: number;
  changefreq: ChangeFreq;
}

const staticUrls: StaticUrl[] = [
  { path: '/', priority: 1.0, changefreq: 'weekly' },
  { path: '/portfolio', priority: 0.9, changefreq: 'weekly' },
  { path: '/calculator', priority: 0.8, changefreq: 'monthly' },
  { path: '/lead', priority: 0.7, changefreq: 'monthly' },
];
