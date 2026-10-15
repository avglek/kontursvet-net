-- Наполнение product_cards на основе WebApp/app/utils/calculator.config.ts
-- Цены — фейковые (условные, RUB за м/шт), для демонстрации работы калькулятора.
INSERT INTO product_cards (code, name, description, group_name, price) VALUES
    -- Контур кровли (roof)
    ('roof-flex-warm', 'Тёплый флекс-неон', 'Тонкая тёплая линия', 'roof', 1450.00),
    ('roof-flex-cold', 'Холодный флекс-неон', 'Тонкая холодная линия', 'roof', 1450.00),
    ('belt-light', 'Белт-лайт', 'Отдельные тёплые лампы', 'roof', 890.00),
    -- Карниз (eaves)
    ('eaves-fringe-warm', 'Тёплая бахрома', 'Диоды на тонких подвесах', 'eaves', 980.00),
    ('eaves-fringe-cold', 'Холодная бахрома', 'Диоды на тонких подвесах', 'eaves', 980.00),
    ('eaves-fringe-warm-twinkle', 'Тёплая с мерцанием', 'Отдельные холодные точки', 'eaves', 1150.00),
    ('eaves-fringe-cold-twinkle', 'Холодная с мерцанием', 'Отдельные холодные точки', 'eaves', 1150.00),
    ('melting-icicles', 'Тающие сосульки', 'Редкие длинные световые трубки', 'eaves', 1320.00),
    -- Вертикальные линии (vertical)
    ('vertical-thread', 'Световая нить', 'На колоннах входа', 'vertical', 750.00),
    -- Световые мотивы (motifs)
    ('motifs-snowflakes', 'Световой мотив «Снежинка»', 'Диаметр 70 см', 'motifs', 2400.00),
    ('motifs-polaris', 'Световой мотив «Полярис»', 'Объёмная звезда', 'motifs', 3200.00)
ON CONFLICT (code) DO NOTHING;
