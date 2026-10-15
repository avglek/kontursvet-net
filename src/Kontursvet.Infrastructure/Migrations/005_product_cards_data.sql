-- Наполнение product_cards на основе WebApp/app/utils/calculator.config.ts
-- Цены — фейковые (условные, RUB за м/шт), для демонстрации работы калькулятора.
INSERT INTO product_cards (code, name, description, group_name, price, img_src) VALUES
    -- Контур кровли (roof)
    ('roof-flex-warm', 'Тёплый флекс-неон', 'Тонкая тёплая линия', 'Контур кровли', 1450.00,'/images/products/flex-neon-white.webp'),
    ('roof-flex-cold', 'Холодный флекс-неон', 'Тонкая холодная линия', 'Контур кровли', 1450.00,'/images/products/flex-neon-cold.webp'),
    ('belt-light', 'Белт-лайт', 'Отдельные тёплые лампы', 'Контур кровли', 890.00,'/images/products/belt-light.webp'),
    -- Карниз (eaves)
    ('eaves-fringe-warm', 'Тёплая бахрома', 'Диоды на тонких подвесах', 'Карниз', 980.00,'/images/products/bachroma-white.webp'),
    ('eaves-fringe-cold', 'Холодная бахрома', 'Диоды на тонких подвесах', 'Карниз', 980.00,'/images/products/bachroma-cold.webp'),
    ('eaves-fringe-warm-twinkle', 'Тёплая с мерцанием', 'Отдельные холодные точки', 'Карниз', 1150.00,'/images/products/bachroma-white-flash.webp'),
    ('eaves-fringe-cold-twinkle', 'Холодная с мерцанием', 'Отдельные холодные точки', 'Карниз', 1150.00,'/images/products/bachroma-cold-flash.webp'),
    ('melting-icicles', 'Тающие сосульки', 'Редкие длинные световые трубки', 'Карниз', 1320.00,'/images/products/tayushchie_sosulki.webp'),
    -- Вертикальные линии (vertical)
    ('vertical-thread', 'Световая нить', 'На колоннах входа', 'Вертикальные линии', 750.00,'/images/products/light-string.webp'),
    -- Световые мотивы (motifs)
    ('motifs-snowflakes', 'Световой мотив «Снежинка»', 'Диаметр 70 см', 'Световые мотивы', 2400.00,'/images/products/snowflake.webp'),
    ('motifs-polaris', 'Световой мотив «Полярис»', 'Объёмная звезда', 'Световые мотивы', 3200.00,'/images/products/polaris-3d.webp')
ON CONFLICT (code) DO NOTHING;
