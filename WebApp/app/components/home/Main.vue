<template>
  <main id="top">
    <section class="hero" aria-labelledby="hero-title">
      <div class="hero-copy">
        <div class="eyebrow">Архитектура света</div>
        <h1 id="hero-title" class="hero-title">
          Световое оформление домов <span>и участков</span>
        </h1>
        <p class="hero-subtitle">
          Контурная подсветка, бахрома, оформление деревьев и комплексные
          световые решения для частных домов.
        </p>
        <p class="location-line">
          <span aria-hidden="true">⌖</span> Санкт-Петербург и Ленинградская
          область
        </p>
        <div class="hero-actions">
          <a class="button button-primary" href="/lead"
            >Отправить фото для оценки <span aria-hidden="true">→</span></a
          ><a class="button button-ghost" href="#projects"
            >Смотреть проекты <span aria-hidden="true">↓</span></a
          >
        </div>
        <div class="phone-row">
          <a href="tel:+79650473553">+7 (965) 047-35-53</a
          ><a href="tel:+78127150505">8 (812) 715-05-05</a>
        </div>
        <p class="hero-note">
          Для предварительной оценки достаточно фотографий объекта.
        </p>
      </div>
      <div class="hero-media">
        <NuxtImg
          src="/images/home/hero_02.webp"
          alt="Дом с контурной подсветкой и световым оформлением кровли"
        /><span class="image-label">Реальный объект · кровля и фасад</span>
      </div>
    </section>
    <section class="value-section" aria-labelledby="value-title">
      <div class="container value-layout">
        <div>
          <div class="eyebrow">Архитектура света</div>
          <h2 id="value-title" class="value-title">
            Создаём цельный световой образ дома и участка
          </h2>
        </div>
        <div class="value-track" aria-label="Состав светового образа">
          <div class="value-item">
            <span>01</span><strong>Архитектура</strong>
          </div>
          <div class="value-item"><span>02</span><strong>Фасад</strong></div>
          <div class="value-item"><span>03</span><strong>Кровля</strong></div>
          <div class="value-item"><span>04</span><strong>Участок</strong></div>
          <div class="value-item">
            <span>05</span><strong>Световой сценарий</strong>
          </div>
        </div>
      </div>
    </section>
    <section class="section" id="projects">
      <div class="container">
        <div class="section-head">
          <div>
            <div class="eyebrow">Реальные объекты</div>
            <h2 class="section-title">
              Свет, который меняет архитектуру<span class="glow-line"></span>
            </h2>
          </div>
          <p class="section-lead">
            Не декоративный эффект в вакууме, а реальные дома и участки после
            монтажа.
          </p>
        </div>
        <div class="project-grid">
          <article class="project-card project-card-featured">
            <NuxtImg
              src="/images/home/001.webp"
              alt="Световое оформление, Гатчина"
              style="object-position: 50% 48%"
            />
            <div class="project-shade"></div>
            <div class="project-caption">
              <strong>Гатчина</strong
              ><span>Два дома · flex neon · бахрома · световые мотивы</span>
            </div>
          </article>

          <article class="project-card">
            <NuxtImg
              src="/images/home/002.webp"
              alt="Световое оформление, Васкелово"
              style="object-position: 50% 50%"
            />
            <div class="project-shade"></div>
            <div class="project-caption">
              <strong>Васкелово</strong><span>Дом, зона барбекю и деревья</span>
            </div>
          </article>
          <article class="project-card">
            <NuxtImg
              src="/images/home/003.webp"
              alt="Световое оформление, Репино"
              style="object-position: 50% 42%"
            />
            <div class="project-shade"></div>
            <div class="project-caption">
              <strong>Репино</strong
              ><span>Контурное освещение частного дома</span>
            </div>
          </article>
          <article class="project-card">
            <NuxtImg
              src="/images/home/004.webp"
              alt="Световое оформление, Всеволожск"
              style="object-position: 50% 46%"
            />
            <div class="project-shade"></div>
            <div class="project-caption">
              <strong>Всеволожск</strong
              ><span>Дом, барбекю и световые растяжки</span>
            </div>
          </article>
          <article class="project-card">
            <NuxtImg
              src="/images/home/005.webp"
              alt="Световое оформление, Flex neon"
              style="object-position: 50% 50%"
            />
            <div class="project-shade"></div>
            <div class="project-caption">
              <strong>Flex neon</strong
              ><span>Усадьба и ландшафт в единой линии света</span>
            </div>
          </article>
        </div>
        <a class="portfolio-entry" href="/portfolio" data-open-portfolio>
          <div class="portfolio-entry-copy">
            <span class="eyebrow">Полное онлайн-портфолио</span>
            <h3>Смотрите проекты подробно</h3>
            <p>
              Задача, объём работ, сроки и полная фотосерия каждого
              реализованного объекта.
            </p>
          </div>
          <div class="portfolio-entry-stats">
            <span><strong>7</strong> кейсов</span
            ><span><strong>41</strong> фотография</span>
          </div>
          <span class="portfolio-entry-action"
            >Открыть портфолио <b aria-hidden="true">→</b></span
          >
        </a>
      </div>
    </section>
    <section class="section composition-section">
      <div class="container composition-card">
        <div class="composition-photo">
          <NuxtImg
            src="/images/home/006.webp"
            alt="Дом с оформленной кровлей, фасадом и террасой"
          /><span class="photo-pin pin-roof">Кровля</span
          ><span class="photo-pin pin-facade">Фасад</span
          ><span class="photo-pin pin-terrace">Терраса</span>
        </div>
        <div class="composition-copy">
          <div class="eyebrow">Единая световая композиция</div>
          <h2 class="section-title">Свет подчинён архитектуре</h2>
          <p>
            Мы смотрим на объект целиком: что подчеркнуть, где оставить темноту
            и как связать дом с участком.
          </p>
          <ol class="architecture-list">
            <li>
              <span>01</span>
              <div>
                <strong>Читаем архитектуру</strong>
                <p>Определяем главные линии дома и точки акцента.</p>
              </div>
            </li>
            <li>
              <span>02</span>
              <div>
                <strong>Строим сценарий</strong>
                <p>Соединяем контур, бахрому и локальные решения.</p>
              </div>
            </li>
            <li>
              <span>03</span>
              <div>
                <strong>Собираем образ</strong>
                <p>Дом и территория работают как одна композиция.</p>
              </div>
            </li>
          </ol>
        </div>
      </div>
    </section>
    <section class="section" id="solutions">
      <div class="container">
        <div class="section-head">
          <div>
            <div class="eyebrow">Форматы оформления</div>
            <h2 class="section-title">
              Четыре направления света<span class="glow-line"></span>
            </h2>
          </div>
          <p class="section-lead">
            Сначала оцениваем объект и желаемый результат, затем предлагаем
            подходящее решение.
          </p>
        </div>
        <div class="services-grid">
          <article class="service-card">
            <div class="service-media">
              <NuxtImg
                src="/images/home/007.webp"
                alt="Контурная подсветка flex neon"
                style="object-position: 50% 44%"
              />
            </div>
            <div class="service-body">
              <span class="service-number">01 · КРОВЛЯ / ФАСАД</span>
              <h3>Контурная подсветка flex neon</h3>
              <p>
                Чёткая световая линия подчёркивает форму кровли и архитектурную
                геометрию дома, не перегружая фасад.
              </p>
              <!-- <a href="#try" data-select-mode="flex"
                >Примерить направление <span aria-hidden="true">→</span></a
              > -->
            </div>
          </article>
          <article class="service-card">
            <div class="service-media">
              <NuxtImg
                src="/images/home/008.webp"
                alt="Гирлянда «бахрома»"
                style="object-position: 50% 48%"
              />
            </div>
            <div class="service-body">
              <span class="service-number">02 · КАРНИЗ / ТЕРРАСА</span>
              <h3>Гирлянда «бахрома»</h3>
              <p>
                Декоративное оформление кровли, карнизов и отдельных зон фасада
                добавляет глубину вечернему образу.
              </p>
              <!-- <a href="#try" data-select-mode="fringe"
                >Примерить направление <span aria-hidden="true">→</span></a
              > -->
            </div>
          </article>
          <article class="service-card">
            <div class="service-media">
              <NuxtImg
                src="/images/home/009.webp"
                alt="Световое оформление деревьев"
                style="object-position: 50% 55%"
              />
            </div>
            <div class="service-body">
              <span class="service-number">03 · ДЕРЕВЬЯ / ЛАНДШАФТ</span>
              <h3>Световое оформление деревьев</h3>
              <p>
                Свет на стволах, кроне и отдельных акцентах связывает
                архитектуру дома с пространством участка.
              </p>
              <!-- <a href="#try" data-select-mode="complex"
                >Примерить направление <span aria-hidden="true">→</span></a
              > -->
            </div>
          </article>
          <article class="service-card">
            <div class="service-media">
              <NuxtImg
                src="/images/home/010.webp"
                alt="Комплексное оформление"
                style="object-position: 50% 50%"
              />
            </div>
            <div class="service-body">
              <span class="service-number">04 · ПОД КЛЮЧ</span>
              <h3>Комплексное оформление</h3>
              <p>
                Дом, дополнительные строения, деревья и территория работают как
                единая световая композиция.
              </p>
              <!-- <a href="#try" data-select-mode="complex"
                >Примерить направление <span aria-hidden="true">→</span></a
              > -->
            </div>
          </article>
        </div>
      </div>
    </section>
    <section class="section included-section">
      <div class="container">
        <div class="section-head">
          <div>
            <div class="eyebrow">За что вы платите</div>
            <h2 class="section-title">
              Что входит в проект<span class="glow-line"></span>
            </h2>
          </div>
          <p class="section-lead">
            Продуманное решение, материалы, монтаж и дальнейшая поддержка — не
            только метры светового материала.
          </p>
        </div>
        <div class="included-grid">
          <article class="included-card">
            <span>01</span>
            <h3>Анализ объекта</h3>
            <p>
              Архитектура дома, участок, точки монтажа и особенности объекта.
            </p>
          </article>
          <article class="included-card">
            <span>02</span>
            <h3>Подбор решения</h3>
            <p>
              Определяем, где уместны flex neon, бахрома, деревья или комплекс.
            </p>
          </article>
          <article class="included-card">
            <span>03</span>
            <h3>Материалы</h3>
            <p>Комплектующие и световые решения для наружной эксплуатации.</p>
          </article>
          <article class="included-card">
            <span>04</span>
            <h3>Профессиональный монтаж</h3>
            <p>Работа бригады, в том числе на сложных и высотных точках.</p>
          </article>
          <article class="included-card">
            <span>05</span>
            <h3>Гарантия 1 год</h3>
            <p>
              Гарантия на выполненные работы в течение года после реализации.
            </p>
          </article>
          <article class="included-card">
            <span>06</span>
            <h3>Постобслуживание</h3>
            <p>После гарантии доступно постгарантийное обслуживание объекта.</p>
          </article>
        </div>
      </div>
    </section>
    <section class="section" id="cases">
      <div class="container">
        <div class="section-head">
          <div>
            <div class="eyebrow">Кейсы</div>
            <h2 class="section-title">
              Два проекта — две задачи<span class="glow-line"></span>
            </h2>
          </div>
          <p class="section-lead">
            Конкретный объём, состав оформления и реальный результат.
          </p>
        </div>
        <div class="cases-grid">
          <article class="case-card">
            <div class="case-photo">
              <NuxtImg
                src="/images/home/13.webp"
                alt="Кейс КонтурСвет: Гатчина"
              />
            </div>
            <div class="case-body">
              <div class="eyebrow">Гатчина</div>
              <h3>Два дома в одной световой стилистике</h3>
              <div class="case-facts">
                <span>2 дня</span><span>6 человек</span
                ><span>flex neon + бахрома</span>
              </div>
              <ul>
                <li>
                  Большой дом: около 100 м flex neon и около 100 м бахромы
                </li>
                <li>12 мотивов «Полярис»</li>
                <li>Малый дом: около 24 м flex neon и около 24 м бахромы</li>
                <li>Снежинки и световые мотивы</li>
              </ul>
              <!-- <a href="#try" data-select-mode="flex"
                >Примерить похожее решение →</a
              > -->
            </div>
          </article>
          <article class="case-card">
            <div class="case-photo">
              <NuxtImg
                src="/images/home/14.webp"
                alt="Кейс КонтурСвет: Васкелово"
              />
            </div>
            <div class="case-body">
              <div class="eyebrow">Васкелово</div>
              <h3>Дом и участок в единой композиции</h3>
              <div class="case-facts">
                <span>2 дня</span><span>5 человек</span
                ><span>точки выше 15 м</span>
              </div>
              <ul>
                <li>Более 170 м flex neon и бахромы на доме</li>
                <li>35 м на зоне барбекю</li>
                <li>Световые занавесы</li>
                <li>70 м гирлянды-нити на ели</li>
              </ul>
              <!-- <a href="#try" data-select-mode="complex"
                >Примерить похожее решение →</a
              > -->
            </div>
          </article>
        </div>
      </div>
    </section>
    <section class="section process-section" id="process">
      <div class="container">
        <div class="section-head">
          <div>
            <div class="eyebrow">Как работаем</div>
            <h2 class="section-title">
              От фотографии до готового света<span class="glow-line"></span>
            </h2>
          </div>
          <p class="section-lead">
            Для первого шага не нужно считать метраж и выбирать оборудование.
          </p>
        </div>
        <ol class="process-list">
          <li>
            <span class="process-number">01</span>
            <div>
              <strong>Вы отправляете фото</strong>
              <p>Дом, участок и населённый пункт.</p>
            </div>
          </li>
          <li>
            <span class="process-number">02</span>
            <div>
              <strong>Александр уточняет задачу</strong>
              <p>Что хотите оформить и какой результат ожидаете.</p>
            </div>
          </li>
          <li>
            <span class="process-number">03</span>
            <div>
              <strong>Подбираем решение</strong>
              <p>Формируем направление и предварительный объём.</p>
            </div>
          </li>
          <li>
            <span class="process-number">04</span>
            <div>
              <strong>Согласуем проект</strong>
              <p>Фиксируем состав работ и дальнейшие действия.</p>
            </div>
          </li>
          <li>
            <span class="process-number">05</span>
            <div>
              <strong>Выполняем монтаж</strong>
              <p>Реализуем согласованное световое оформление.</p>
            </div>
          </li>
        </ol>
      </div>
    </section>
    <section class="final-cta" id="contact">
      <div class="container final-layout">
        <div>
          <div class="eyebrow">Следующий шаг</div>
          <h2>Покажите дом — начнём с фотографии</h2>
          <p>Для предварительной оценки достаточно фотографий объекта.</p>
          <div class="phone-row">
            <a href="tel:+79650473553">+7 (965) 047-35-53</a
            ><a href="tel:+78127150505">8 (812) 715-05-05</a>
          </div>
        </div>
        <div class="final-actions">
          <NuxtLink href="/lead" class="button button-primary"
            >Отправить фото для оценки →</NuxtLink
          ><a href="tel:+79650473553" class="button button-ghost"
            >Позвонить Александру</a
          >
        </div>
      </div>
    </section>
  </main>
</template>

<script lang="ts" setup></script>

<style></style>
