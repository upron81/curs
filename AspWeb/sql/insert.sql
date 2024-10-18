-- Настройки для вставки данных
DECLARE @Symbol CHAR(52)= 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz',
        @Position INT,
        @i INT,
        @NameLimit INT,
        @FullName NVARCHAR(150),
        @HomeAddress NVARCHAR(255),
        @PassportData NVARCHAR(100),
        @TariffName NVARCHAR(100),
        @TariffPlanID INT,
        @SubscriberID INT,
        @StaffID INT = 1,  -- один сотрудник
        @TariffPlanCount INT = 500,
        @SubscriberCount INT = 500,
        @ContractCount INT = 2000,
        @PhoneNumber NVARCHAR(20),
        @ContractDate DATE,
        @ContractEndDate DATE,
        @MinSymbols INT,
        @MaxSymbols INT;

-- Добавление должности для сотрудника
INSERT INTO StaffPosition (PositionName)
VALUES ('Оператор');

-- Добавление сотрудников
INSERT INTO Staff (FullName, PositionID, Education)
VALUES 
('Сергей Петров', 1, 'Среднее'),
('Александр Смирнов', 1, 'Среднее'),
('Дмитрий Соколов', 1, 'Высшее'),
('Андрей Кузнецов', 1, 'Среднее'),
('Михаил Попов', 1, 'Высшее'),
('Алексей Волков', 1, 'Высшее'),
('Николай Лебедев', 1, 'Среднее'),
('Олег Семёнов', 1, 'Среднее'),
('Владимир Павлов', 1, 'Высшее'),
('Игорь Козлов', 1, 'Среднее'),
('Максим Морозов', 1, 'Высшее'),
('Артём Новиков', 1, 'Среднее'),
('Евгений Беляев', 1, 'Высшее'),
('Антон Ершов', 1, 'Среднее'),
('Роман Фролов', 1, 'Высшее'),
('Григорий Сафонов', 1, 'Среднее'),
('Павел Гусев', 1, 'Среднее'),
('Виктор Киселёв', 1, 'Высшее'),
('Константин Фёдоров', 1, 'Среднее'),
('Станислав Орлов', 1, 'Высшее');


-- Заполнение таблицы тарифных планов
SET @i = 1;
SET @MinSymbols = 5;
SET @MaxSymbols = 15;

WHILE @i <= @TariffPlanCount
BEGIN
    -- Генерация случайного названия тарифного плана
    SET @NameLimit = @MinSymbols + RAND() * (@MaxSymbols - @MinSymbols);
    SET @TariffName = '';
    WHILE LEN(@TariffName) < @NameLimit
    BEGIN
        SET @Position = RAND() * 52;
        SET @TariffName = @TariffName + SUBSTRING(@Symbol, @Position, 1);
    END;
    
    -- Вставка записи в таблицу тарифов
    INSERT INTO TariffPlans (TariffName, SubscriptionFee, LocalCallRate, LongDistanceCallRate, InternationalCallRate, IsPerSecond, SmsRate, MmsRate, DataRatePerMB)
    VALUES (@TariffName, RAND() * 100, RAND() * 2, RAND() * 5, RAND() * 10, CAST(ROUND(RAND(), 0) AS BIT), RAND() * 2, RAND() * 5, RAND() * 10);
    
    SET @i = @i + 1;
END;

-- Заполнение таблицы абонентов
SET @i = 1;
SET @MinSymbols = 5;
SET @MaxSymbols = 15;

WHILE @i <= @SubscriberCount
BEGIN
    -- Генерация случайных данных абонента
    SET @NameLimit = @MinSymbols + RAND() * (@MaxSymbols - @MinSymbols);
    SET @FullName = '';
    SET @HomeAddress = '';
    SET @PassportData = '';
    
    -- Генерация ФИО
    WHILE LEN(@FullName) < @NameLimit
    BEGIN
        SET @Position = RAND() * 52;
        SET @FullName = @FullName + SUBSTRING(@Symbol, @Position, 1);
    END;

    -- Генерация домашнего адреса
    WHILE LEN(@HomeAddress) < @NameLimit + 5
    BEGIN
        SET @Position = RAND() * 52;
        SET @HomeAddress = @HomeAddress + SUBSTRING(@Symbol, @Position, 1);
    END;

    -- Генерация паспортных данных
    WHILE LEN(@PassportData) < 10
    BEGIN
        SET @Position = RAND() * 52;
        SET @PassportData = @PassportData + SUBSTRING(@Symbol, @Position, 1);
    END;

    -- Вставка записи в таблицу абонентов
    INSERT INTO Subscribers (FullName, HomeAddress, PassportData)
    VALUES (@FullName, @HomeAddress, @PassportData);

    SET @i = @i + 1;
END;

-- Заполнение таблицы договоров
SET @i = 1;

WHILE @i <= @ContractCount
BEGIN
    -- Выбор случайного абонента и тарифного плана
    SET @SubscriberID = CAST(1 + RAND() * (@SubscriberCount - 1) AS INT);
    SET @TariffPlanID = CAST(1 + RAND() * (@TariffPlanCount - 1) AS INT);
    
    -- Генерация уникального номера телефона
    SET @PhoneNumber = '89' + CAST(CAST(RAND() * 1000000000 AS INT) AS NVARCHAR(10));
    
    -- Генерация дат контракта
    SET @ContractDate = DATEADD(DAY, -RAND() * 3650, GETDATE());  -- дата начала
    SET @ContractEndDate = DATEADD(YEAR, 1 + RAND() * 5, @ContractDate); -- дата окончания

    -- Проверка на перекрытие контрактов для абонента
    IF NOT EXISTS (
        SELECT 1 
        FROM Contracts 
        WHERE SubscriberID = @SubscriberID 
          AND @ContractDate BETWEEN ContractDate AND ISNULL(ContractEndDate, GETDATE())
    )
    BEGIN
        -- Вставка записи в таблицу договоров
        INSERT INTO Contracts (SubscriberID, TariffPlanID, ContractDate, ContractEndDate, PhoneNumber, StaffID)
        VALUES (@SubscriberID, @TariffPlanID, @ContractDate, @ContractEndDate, @PhoneNumber, @StaffID);
        
        SET @i = @i + 1;
    END;
END;


-- Заполнение таблицы звонков
DECLARE @CallCount INT = 10000; -- количество записей о звонках
DECLARE @CallDate DATETIME;
DECLARE @CallDuration INT;

SET @i = 1;
WHILE @i <= @CallCount
BEGIN
    -- Выбор случайного контракта
    DECLARE @ContractID INT = CAST(1 + RAND() * (SELECT COUNT(*) FROM Contracts) AS INT);
    
    -- Получение даты начала и окончания контракта
    SELECT @ContractDate = ContractDate, @ContractEndDate = ContractEndDate
    FROM Contracts
    WHERE ContractID = @ContractID;

    -- Генерация случайной даты звонка в пределах действия контракта
    SET @CallDate = DATEADD(DAY, CAST(RAND() * DATEDIFF(DAY, @ContractDate, ISNULL(@ContractEndDate, GETDATE())) AS INT), @ContractDate);
    
    -- Генерация случайной длительности звонка (в секундах)
    SET @CallDuration = CAST(1 + RAND() * 3600 AS INT); -- до 1 часа

    -- Вставка записи в таблицу звонков
    INSERT INTO Calls (ContractID, CallDate, CallDuration)
    VALUES (@ContractID, @CallDate, @CallDuration);

    SET @i = @i + 1;
END;

-- Заполнение таблицы сообщений
DECLARE @MessageCount INT = 5000; -- количество записей о сообщениях
DECLARE @MessageDate DATETIME;
DECLARE @IsMMS BIT;

SET @i = 1;
WHILE @i <= @MessageCount
BEGIN
    -- Выбор случайного контракта
    SET @ContractID = CAST(1 + RAND() * (SELECT COUNT(*) FROM Contracts) AS INT);
    
    -- Получение даты начала и окончания контракта
    SELECT @ContractDate = ContractDate, @ContractEndDate = ContractEndDate
    FROM Contracts
    WHERE ContractID = @ContractID;

    -- Генерация случайной даты сообщения в пределах действия контракта
    SET @MessageDate = DATEADD(DAY, CAST(RAND() * DATEDIFF(DAY, @ContractDate, ISNULL(@ContractEndDate, GETDATE())) AS INT), @ContractDate);
    
    -- Генерация типа сообщения (SMS или MMS)
    SET @IsMMS = CAST(ROUND(RAND(), 0) AS BIT);

    -- Вставка записи в таблицу сообщений
    INSERT INTO Messages (ContractID, MessageDate, IsMMS)
    VALUES (@ContractID, @MessageDate, @IsMMS);

    SET @i = @i + 1;
END;

-- Заполнение таблицы использования интернета
DECLARE @UsageCount INT = 10000; -- количество записей о трафике
DECLARE @UsageDate DATETIME;
DECLARE @DataSentMB DECIMAL(10, 2);
DECLARE @DataReceivedMB DECIMAL(10, 2);

SET @i = 1;
WHILE @i <= @UsageCount
BEGIN
    -- Выбор случайного контракта
    SET @ContractID = CAST(1 + RAND() * (SELECT COUNT(*) FROM Contracts) AS INT);
    
    -- Получение даты начала и окончания контракта
    SELECT @ContractDate = ContractDate, @ContractEndDate = ContractEndDate
    FROM Contracts
    WHERE ContractID = @ContractID;

    -- Генерация случайной даты использования интернета в пределах действия контракта
    SET @UsageDate = DATEADD(DAY, CAST(RAND() * DATEDIFF(DAY, @ContractDate, ISNULL(@ContractEndDate, GETDATE())) AS INT), @ContractDate);
    
    -- Генерация случайного объема данных
    SET @DataSentMB = ROUND(RAND() * 500, 2); -- до 500 MB
    SET @DataReceivedMB = ROUND(RAND() * 500, 2); -- до 500 MB

    -- Вставка записи в таблицу использования интернета
    INSERT INTO InternetUsage (ContractID, UsageDate, DataSentMB, DataReceivedMB)
    VALUES (@ContractID, @UsageDate, @DataSentMB, @DataReceivedMB);

    SET @i = @i + 1;
END;


-- Количество записей в таблице StaffPosition
SELECT 'StaffPosition' AS TableName, COUNT(*) AS RecordCount
FROM StaffPosition;

-- Количество записей в таблице Staff
SELECT 'Staff' AS TableName, COUNT(*) AS RecordCount
FROM Staff;

-- Количество записей в таблице TariffPlans
SELECT 'TariffPlans' AS TableName, COUNT(*) AS RecordCount
FROM TariffPlans;

-- Количество записей в таблице Subscribers
SELECT 'Subscribers' AS TableName, COUNT(*) AS RecordCount
FROM Subscribers;

-- Количество записей в таблице Contracts
SELECT 'Contracts' AS TableName, COUNT(*) AS RecordCount
FROM Contracts;

-- Количество записей в таблице Calls
SELECT 'Calls' AS TableName, COUNT(*) AS RecordCount
FROM Calls;

-- Количество записей в таблице Messages
SELECT 'Messages' AS TableName, COUNT(*) AS RecordCount
FROM Messages;

-- Количество записей в таблице InternetUsage
SELECT 'InternetUsage' AS TableName, COUNT(*) AS RecordCount
FROM InternetUsage;
