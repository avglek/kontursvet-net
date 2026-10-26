// Плавный скролл (Lenis) и анимации по скроллу (GSAP + ScrollTrigger).
// Логика перенесена из assets/js/cinematic-motion.js, но без скриншот-хуков
// (?noanim, ?scrollY, ?targetEl) — они были нужны только системе снимков.
//
// Вызывается один раз из корня страницы:
//   useCinematicMotion()

interface MotionHandle {
  destroy: () => void;
}

const HEADER_OFFSET = 80;

async function initMotion(): Promise<MotionHandle> {
  const disposers: Array<() => void> = [];
  const reducedMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;
  const isTouch = window.matchMedia('(pointer: coarse)').matches || window.innerWidth <= 640;
  const animOffset = isTouch ? 16 : 35;

  const { gsap } = await import('gsap');
  const { ScrollTrigger } = await import('gsap/ScrollTrigger');
  gsap.registerPlugin(ScrollTrigger);
  disposers.push(() => {
    ScrollTrigger.getAll().forEach((trigger) => trigger.kill());
  });

  // ─── Lenis: только на десктопе, на тач-устройствах остаётся нативный скролл ─
  if (!reducedMotion && !isTouch) {
    const { default: Lenis } = await import('lenis');
    const lenis = new Lenis({
      duration: 1.2,
      easing: (t: number) => Math.min(1, 1.001 - Math.pow(2, -10 * t)),
      orientation: 'vertical',
      gestureOrientation: 'vertical',
      smoothWheel: true,
      wheelMultiplier: 1,
      touchMultiplier: 1,
    });

    lenis.on('scroll', ScrollTrigger.update);
    gsap.ticker.add((time) => lenis.raf(time * 1000));
    gsap.ticker.lagSmoothing(0);
    disposers.push(() => lenis.destroy());
  }

  // ─── Плавные переходы по якорям со смещением под фиксированную шапку ──────
  const onAnchorClick = (event: MouseEvent) => {
    const anchor = (event.target as HTMLElement | null)?.closest<HTMLAnchorElement>('a[href^="#"]');
    if (!anchor) return;

    const hash = anchor.getAttribute('href') ?? '';
    if (hash === '#' || anchor.dataset.modal) return;

    const target = document.querySelector(hash);
    if (!target) return;

    event.preventDefault();
    const top = target.getBoundingClientRect().top + window.scrollY - HEADER_OFFSET;
    window.scrollTo({ top, behavior: reducedMotion ? 'auto' : 'smooth' });
  };

  document.addEventListener('click', onAnchorClick);
  disposers.push(() => document.removeEventListener('click', onAnchorClick));

  // При уменьшенной анимации показываем всё сразу — further tweens не нужны.
  if (reducedMotion) {
    document.querySelectorAll<HTMLElement>('*').forEach((el) => {
      if (el.style.opacity === '0') el.style.opacity = '1';
    });
    return { destroy: () => disposers.forEach((fn) => fn()) };
  }

  // ─── Появление героя ──────────────────────────────────────────────────────
  const heroEntrance = gsap.timeline({ defaults: { ease: 'power3.out' } });
  const heroStages: Array<[string, { y: number, delay: number, duration: number }]> = [
    ['.cinematic-top-meta', { y: 14, delay: 0.05, duration: 0.55 }],
    ['.title-eyebrow-accent', { y: 16, delay: 0.18, duration: 0.5 }],
    ['.title-line-1', { y: 20, delay: 0.3, duration: 0.55 }],
    ['.title-line-2', { y: 20, delay: 0.42, duration: 0.55 }],
    ['.cinematic-lead', { y: 16, delay: 0.58, duration: 0.6 }],
    ['.cinematic-actions', { y: 18, delay: 0.72, duration: 0.55 }],
    ['.cinematic-stats', { y: 18, delay: 0.85, duration: 0.55 }],
  ];

  heroStages.forEach(([selector, { y, delay, duration }]) => {
    gsap.set(selector, { opacity: 0, y });
    heroEntrance.to(selector, { opacity: 1, y: 0, duration }, delay);
  });
  disposers.push(() => heroEntrance.kill());

  // ─── Заголовки секций ─────────────────────────────────────────────────────
  const animateHead = (section: Element | null) => {
    if (!section) return;
    gsap.from(section.querySelectorAll('.section-head > *'), {
      scrollTrigger: { trigger: section, start: 'top 85%' },
      opacity: 0,
      y: animOffset,
      duration: 0.7,
      stagger: 0.12,
      ease: 'power3.out',
    });
  };

  // «Проекты»
  const projects = document.querySelector('#projects');
  if (projects) {
    animateHead(projects);
    gsap.from(projects.querySelectorAll('.project-card'), {
      scrollTrigger: { trigger: '.project-grid', start: 'top 82%' },
      opacity: 0,
      y: animOffset * 1.2,
      duration: 0.75,
      stagger: 0.1,
      ease: 'power3.out',
    });
    gsap.from(projects.querySelector('.portfolio-entry'), {
      scrollTrigger: { trigger: '.portfolio-entry', start: 'top 88%' },
      opacity: 0,
      y: animOffset,
      duration: 0.7,
      ease: 'power3.out',
    });
  }

  // «Единая световая композиция»
  const composition = document.querySelector('.composition-section');
  if (composition) {
    const timeline = gsap.timeline({
      scrollTrigger: { trigger: composition, start: 'top 82%' },
    });

    timeline
      .from(composition.querySelector('.composition-photo'), {
        opacity: 0,
        scale: 0.96,
        duration: 0.8,
        ease: 'power3.out',
      })
      .from(
        composition.querySelectorAll('.composition-copy > *'),
        { opacity: 0, y: animOffset, duration: 0.65, stagger: 0.1, ease: 'power3.out' },
        '-=0.5',
      )
      .from(
        composition.querySelectorAll('.photo-pin'),
        {
          opacity: 0,
          scale: 0.5,
          duration: 0.5,
          stagger: 0.18,
          ease: 'back.out(1.7)',
          onComplete: () => {
            composition.querySelectorAll('.photo-pin').forEach((pin) => pin.classList.add('pin-active'));
          },
        },
        '-=0.3',
      );
    disposers.push(() => timeline.kill());
  }

  // «Решения», «Что входит», «Кейсы», «Как работаем»
  const solutions = document.querySelector('#solutions');
  if (solutions) {
    animateHead(solutions);
    gsap.from(solutions.querySelectorAll('.service-card'), {
      scrollTrigger: { trigger: '.services-grid', start: 'top 82%' },
      opacity: 0,
      y: animOffset * 1.1,
      duration: 0.7,
      stagger: 0.1,
      ease: 'power3.out',
    });
  }

  const included = document.querySelector('.included-section');
  if (included) {
    animateHead(included);
    gsap.from(included.querySelectorAll('.included-card'), {
      scrollTrigger: { trigger: '.included-grid', start: 'top 82%' },
      opacity: 0,
      y: animOffset * 0.8,
      duration: 0.6,
      stagger: 0.08,
      ease: 'power3.out',
    });
  }

  const cases = document.querySelector('#cases');
  if (cases) {
    animateHead(cases);
    gsap.from(cases.querySelectorAll('.case-card'), {
      scrollTrigger: { trigger: '.cases-grid', start: 'top 80%' },
      opacity: 0,
      y: animOffset * 1.1,
      duration: 0.75,
      stagger: 0.15,
      ease: 'power3.out',
    });

    // Лёгкий параллакс фото кейсов — только на десктопе.
    if (!isTouch) {
      cases.querySelectorAll('.case-photo img').forEach((img) => {
        gsap.to(img, {
          scrollTrigger: {
            trigger: img.closest('.case-card'),
            start: 'top bottom',
            end: 'bottom top',
            scrub: 1.2,
          },
          yPercent: -8,
          ease: 'none',
        });
      });
    }
  }

  const process = document.querySelector('#process');
  if (process) {
    animateHead(process);
    gsap.from(process.querySelectorAll('.process-list li'), {
      scrollTrigger: { trigger: '.process-list', start: 'top 82%' },
      opacity: 0,
      x: isTouch ? 0 : -24,
      y: isTouch ? animOffset * 0.7 : 0,
      duration: 0.65,
      stagger: 0.1,
      ease: 'power3.out',
    });
  }

  // Значение ценности: заголовок, луч и карточки шагов
  const value = document.querySelector('.value-section');
  if (value) {
    const headItems = value.querySelectorAll('.value-head-col > *');
    const beam = value.querySelector('.value-light-beam');

    gsap.fromTo(
      headItems,
      { opacity: 0.3, y: animOffset * 0.7 },
      {
        opacity: 1,
        y: 0,
        duration: 0.65,
        stagger: 0.08,
        ease: 'power3.out',
        clearProps: 'opacity,transform',
        scrollTrigger: { trigger: value, start: 'top 92%', once: true },
      },
    );

    if (beam) {
      gsap.fromTo(
        beam,
        { scaleX: 0, transformOrigin: 'left center' },
        {
          scaleX: 1,
          duration: 0.85,
          ease: 'power2.out',
          clearProps: 'transform',
          scrollTrigger: { trigger: value, start: 'top 92%', once: true },
        },
      );
    }

    gsap.fromTo(
      value.querySelectorAll('.value-item'),
      { opacity: 0.3, y: animOffset * 0.7, scale: 0.98 },
      {
        opacity: 1,
        y: 0,
        scale: 1,
        duration: 0.6,
        stagger: 0.07,
        ease: 'power2.out',
        clearProps: 'opacity,transform',
        scrollTrigger: { trigger: value, start: 'top 92%', once: true },
      },
    );
  }

  // Финальный CTA
  const finalCta = document.querySelector('.final-cta');
  if (finalCta) {
    const copyItems = finalCta.querySelectorAll('.final-copy > *');
    const formCard = finalCta.querySelector('.final-form-card');

    gsap.fromTo(
      copyItems,
      { opacity: 0.3, y: animOffset * 0.6 },
      {
        opacity: 1,
        y: 0,
        duration: 0.6,
        stagger: 0.08,
        ease: 'power3.out',
        clearProps: 'opacity,transform',
        scrollTrigger: { trigger: finalCta, start: 'top 95%', once: true },
      },
    );

    if (formCard) {
      gsap.fromTo(
        formCard,
        { opacity: 0.3, y: animOffset * 0.6, scale: 0.98 },
        {
          opacity: 1,
          y: 0,
          scale: 1,
          duration: 0.7,
          ease: 'power3.out',
          clearProps: 'opacity,transform',
          scrollTrigger: { trigger: finalCta, start: 'top 95%', once: true },
        },
      );
    }
  }

  // ─── Страховка: после загрузки всего контента пересчитываем триггеры ──────
  const onLoad = () => {
    ScrollTrigger.refresh();
    window.setTimeout(() => {
      document.querySelectorAll<HTMLElement>('.value-item, .final-form-card, .final-copy').forEach((el) => {
        if (window.getComputedStyle(el).opacity === '0') {
          el.style.opacity = '1';
          el.style.transform = 'none';
        }
      });
    }, 600);
  };

  window.addEventListener('load', onLoad);
  disposers.push(() => window.removeEventListener('load', onLoad));

  return { destroy: () => disposers.forEach((fn) => fn()) };
}

export function useCinematicMotion() {
  let handle: MotionHandle | null = null;
  let destroyed = false;

  onMounted(async () => {
    handle = await initMotion();
    if (destroyed) handle.destroy();
  });

  onBeforeUnmount(() => {
    destroyed = true;
    handle?.destroy();
  });
}
