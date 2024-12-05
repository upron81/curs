-- 1. Вставка должностей в таблицу StaffPosition
IF NOT EXISTS (SELECT 1 FROM StaffPosition)
BEGIN
    INSERT INTO StaffPosition (PositionName) VALUES
    ('Менеджер'),
    ('Оператор'),
    ('Техник'),
    ('Аналитик'),
    ('Администратор'),
    ('Маркетолог'),
    ('Бухгалтер'),
    ('HR-специалист'),
    ('Разработчик'),
    ('Дизайнер');
END

-- 2. Вставка тарифных планов в таблицу TariffPlans
IF NOT EXISTS (SELECT 1 FROM TariffPlans)
BEGIN
    INSERT INTO TariffPlans (TariffName, SubscriptionFee, LocalCallRate, LongDistanceCallRate, InternationalCallRate, IsPerSecond, SmsRate, MmsRate, DataRatePerMB) VALUES
    ('Базовый', 299.99, 0.10, 0.50, 1.50, 0, 0.05, 0.10, 0.20),
    ('Стандарт', 499.99, 0.08, 0.40, 1.20, 0, 0.04, 0.08, 0.15),
    ('Премиум', 799.99, 0.05, 0.30, 1.00, 1, 0.03, 0.05, 0.10),
    ('Безлимит', 999.99, 0.00, 0.00, 0.00, 1, 0.00, 0.00, 0.00),
    ('Эконом', 199.99, 0.12, 0.60, 1.80, 0, 0.06, 0.12, 0.25),
    ('Бизнес', 599.99, 0.07, 0.35, 1.05, 1, 0.035, 0.07, 0.12),
    ('Семейный', 749.99, 0.06, 0.30, 0.90, 1, 0.03, 0.06, 0.10),
    ('Про', 899.99, 0.04, 0.25, 0.80, 1, 0.025, 0.05, 0.08),
    ('Минимальный', 149.99, 0.15, 0.75, 2.00, 0, 0.07, 0.14, 0.30),
    ('Ультра', 1299.99, 0.00, 0.00, 0.00, 1, 0.00, 0.00, 0.00);
END

-- 3. Создание вспомогательных таблиц для генерации данных
DECLARE @FirstNames TABLE (Name NVARCHAR(50));
DECLARE @LastNames TABLE (Name NVARCHAR(50));
DECLARE @MiddleNames TABLE (Name NVARCHAR(50));
DECLARE @Streets TABLE (Name NVARCHAR(100));
DECLARE @Cities TABLE (Name NVARCHAR(100));

-- Заполнение таблиц именами
INSERT INTO @FirstNames (Name) VALUES 
('Иван'), ('Мария'), ('Сергей'), ('Анна'), ('Дмитрий'),
('Елена'), ('Алексей'), ('Ольга'), ('Николай'), ('Татьяна'),
('Павел'), ('Ксения'), ('Максим'), ('Екатерина'), ('Михаил'),
('Анастасия'), ('Владимир'), ('Юлия'), ('Константин'), ('Людмила');

INSERT INTO @LastNames (Name) VALUES 
('Иванов'), ('Петров'), ('Сидоров'), ('Кузнецов'), ('Михайлов'),
('Козлова'), ('Васильев'), ('Попова'), ('Лебедев'), ('Смирнова'),
('Новиков'), ('Фёдоров'), ('Морозов'), ('Волкова'), ('Алексеев'),
('Соколов'), ('Егоров'), ('Баранов'), ('Голубев'), ('Орлов');

INSERT INTO @MiddleNames (Name) VALUES 
('Иванович'), ('Алексеевич'), ('Петрович'), ('Сергеевич'), ('Владимирович'),
('Николаевич'), ('Игоревич'), ('Викторович'), ('Дмитриевич'), ('Егорович'),
('Михайлович'), ('Андреевич'), ('Юрьевич'), ('Константинович'), ('Максимович'),
('Олегович'), ('Станиславович'), ('Григорьевич'), ('Анатольевич'), ('Валерьевич');

INSERT INTO @Streets (Name) VALUES 
('Ленина'), ('Советская'), ('Мира'), ('Победы'), ('Московская'),
('Новая'), ('Садовая'), ('Центральная'), ('Луначарского'), ('Кирова'),
('Гагарина'), ('Дорожная'), ('Северная'), ('Южная'), ('Западная'),
('Восточная'), ('Парковая'), ('Речная'), ('Лесная'), ('Фрунзенская');

INSERT INTO @Cities (Name) VALUES 
('Москва'), ('Санкт-Петербург'), ('Новосибирск'), ('Екатеринбург'), ('Казань'),
('Нижний Новгород'), ('Челябинск'), ('Самара'), ('Омск'), ('Ростов-на-Дону'),
('Уфа'), ('Красноярск'), ('Воронеж'), ('Пермь'), ('Волгоград'),
('Краснодар'), ('Саратов'), ('Тольятти'), ('Ижевск'), ('Барнаул');

-- 4. Вставка сотрудников в таблицу Staff
-- Предполагается, что StaffPosition уже заполнена

DECLARE @StaffRecords TABLE (
    FullName NVARCHAR(150),
    PositionID INT,
    Education NVARCHAR(100)
);

-- Объявляем переменные один раз
DECLARE @FirstName NVARCHAR(50);
DECLARE @LastName NVARCHAR(50);
DECLARE @MiddleName NVARCHAR(50);
DECLARE @FullName NVARCHAR(150);
DECLARE @PositionID INT;
DECLARE @Education NVARCHAR(100);

SET NOCOUNT ON;

DECLARE @StaffTotal INT = 1000;
DECLARE @StaffCounter INT = 1;

WHILE @StaffCounter <= @StaffTotal
BEGIN
    -- Выбор случайного имени
    SELECT TOP 1 @FirstName = Name FROM @FirstNames ORDER BY NEWID();
    SELECT TOP 1 @LastName = Name FROM @LastNames ORDER BY NEWID();
    SELECT TOP 1 @MiddleName = Name FROM @MiddleNames ORDER BY NEWID();
    SET @FullName = CONCAT(@LastName, ' ', @FirstName, ' ', @MiddleName);
    
    -- Случайный выбор PositionID
    SELECT @PositionID = PositionID FROM StaffPosition ORDER BY NEWID();
    
    -- Генерация образования
    SELECT TOP 1 @Education = 
        CASE (ABS(CHECKSUM(NEWID())) % 5)
            WHEN 0 THEN 'Высшее экономическое'
            WHEN 1 THEN 'Среднее специальное'
            WHEN 2 THEN 'Высшее техническое'
            WHEN 3 THEN 'Высшее математическое'
            WHEN 4 THEN 'Высшее педагогическое'
        END
    FROM sys.objects; -- Используем любую таблицу для генерации строки
    
    -- Вставка записи в временную таблицу
    INSERT INTO @StaffRecords (FullName, PositionID, Education)
    VALUES (@FullName, @PositionID, @Education);
    
    SET @StaffCounter = @StaffCounter + 1;
END

-- Вставка данных в таблицу Staff
INSERT INTO Staff (FullName, PositionID, Education)
SELECT FullName, PositionID, Education FROM @StaffRecords;

-- 5. Вставка абонентов в таблицу Subscribers

DECLARE @SubscriberRecords TABLE (
    FullName NVARCHAR(150),
    HomeAddress NVARCHAR(255),
    PassportData NVARCHAR(100)
);

-- Объявляем переменные один раз
DECLARE @SubscriberFirstName NVARCHAR(50);
DECLARE @SubscriberLastName NVARCHAR(50);
DECLARE @SubscriberMiddleName NVARCHAR(50);
DECLARE @SubscriberFullName NVARCHAR(150);
DECLARE @Street NVARCHAR(100);
DECLARE @City NVARCHAR(100);
DECLARE @HouseNumber INT;
DECLARE @HomeAddress NVARCHAR(255);
DECLARE @PassportSeries INT;
DECLARE @PassportNumber INT;
DECLARE @PassportData NVARCHAR(100);

DECLARE @SubscriberTotal INT = 1000;
DECLARE @SubscriberCounter INT = 1;

WHILE @SubscriberCounter <= @SubscriberTotal
BEGIN
    -- Выбор случайного имени
    SELECT TOP 1 @SubscriberFirstName = Name FROM @FirstNames ORDER BY NEWID();
    SELECT TOP 1 @SubscriberLastName = Name FROM @LastNames ORDER BY NEWID();
    SELECT TOP 1 @SubscriberMiddleName = Name FROM @MiddleNames ORDER BY NEWID();
    SET @SubscriberFullName = CONCAT(@SubscriberLastName, ' ', @SubscriberFirstName, ' ', @SubscriberMiddleName);
    
    -- Генерация адреса
    SELECT TOP 1 @Street = Name FROM @Streets ORDER BY NEWID();
    SELECT TOP 1 @City = Name FROM @Cities ORDER BY NEWID();
    SET @HouseNumber = (ABS(CHECKSUM(NEWID())) % 200) + 1;
    SET @HomeAddress = CONCAT('г. ', @City, ', ул. ', @Street, ', д. ', @HouseNumber);
    
    -- Генерация паспортных данных
    SET @PassportSeries = (ABS(CHECKSUM(NEWID())) % 9000) + 1000;
    SET @PassportNumber = (ABS(CHECKSUM(NEWID())) % 900000) + 100000;
    SET @PassportData = CONCAT(@PassportSeries, ' ', @PassportNumber);
    
    -- Вставка записи в временную таблицу
    INSERT INTO @SubscriberRecords (FullName, HomeAddress, PassportData)
    VALUES (@SubscriberFullName, @HomeAddress, @PassportData);
    
    SET @SubscriberCounter = @SubscriberCounter + 1;
END

-- Вставка данных в таблицу Subscribers
INSERT INTO Subscribers (FullName, HomeAddress, PassportData)
SELECT FullName, HomeAddress, PassportData FROM @SubscriberRecords;

-- 6. Вставка договоров в таблицу Contracts

DECLARE @ContractRecords TABLE (
    SubscriberID INT,
    TariffPlanID INT,
    ContractDate DATE,
    ContractEndDate DATE,
    PhoneNumber NVARCHAR(20),
    StaffID INT
);

-- Объявляем переменные один раз
DECLARE @SubscriberID INT;
DECLARE @TariffPlanID INT;
DECLARE @ContractDate DATE;
DECLARE @HasEndDate BIT;
DECLARE @ContractEndDate DATE;
DECLARE @PhoneNumber NVARCHAR(20);
DECLARE @ContractStaffID INT;

DECLARE @ContractTotal INT = 1000;
DECLARE @ContractCounter INT = 1;

-- Получаем максимальные ID из связанных таблиц
DECLARE @MaxSubscriberID INT = (SELECT MAX(SubscriberID) FROM Subscribers);
DECLARE @MaxTariffPlanID INT = (SELECT MAX(TariffPlanID) FROM TariffPlans);
DECLARE @MaxStaffID INT = (SELECT MAX(StaffID) FROM Staff);

WHILE @ContractCounter <= @ContractTotal
BEGIN
    -- Случайный SubscriberID и TariffPlanID
    SET @SubscriberID = (ABS(CHECKSUM(NEWID())) % @MaxSubscriberID) + 1;
    SET @TariffPlanID = (ABS(CHECKSUM(NEWID())) % @MaxTariffPlanID) + 1;
    
    -- Генерация дат договора
    SET @ContractDate = DATEADD(DAY, -ABS(CHECKSUM(NEWID())) % 365, GETDATE());
    SET @HasEndDate = CASE WHEN (ABS(CHECKSUM(NEWID())) % 2) = 0 THEN 1 ELSE 0 END;
    IF @HasEndDate = 1
        SET @ContractEndDate = DATEADD(DAY, 365, @ContractDate);
    ELSE
        SET @ContractEndDate = NULL;
    
    -- Генерация телефонного номера (+7-XXX-XXX-XX-XX)
    SET @PhoneNumber = CONCAT('+7-', 
        CAST((ABS(CHECKSUM(NEWID())) % 900 + 100) AS NVARCHAR), '-', 
        CAST((ABS(CHECKSUM(NEWID())) % 900 + 100) AS NVARCHAR), '-', 
        CAST((ABS(CHECKSUM(NEWID())) % 90 + 10) AS NVARCHAR), '-', 
        CAST((ABS(CHECKSUM(NEWID())) % 90 + 10) AS NVARCHAR));
    
    -- Случайный StaffID
    SET @ContractStaffID = (ABS(CHECKSUM(NEWID())) % @MaxStaffID) + 1;
    
    -- Вставка записи в временную таблицу
    INSERT INTO @ContractRecords (SubscriberID, TariffPlanID, ContractDate, ContractEndDate, PhoneNumber, StaffID)
    VALUES (@SubscriberID, @TariffPlanID, @ContractDate, @ContractEndDate, @PhoneNumber, @ContractStaffID);
    
    SET @ContractCounter = @ContractCounter + 1;
END

-- Вставка данных в таблицу Contracts
INSERT INTO Contracts (SubscriberID, TariffPlanID, ContractDate, ContractEndDate, PhoneNumber, StaffID)
SELECT SubscriberID, TariffPlanID, ContractDate, ContractEndDate, PhoneNumber, StaffID FROM @ContractRecords;

-- 7. Вставка звонков в таблицу Calls

DECLARE @CallRecords TABLE (
    ContractID INT,
    CallDate DATETIME,
    CallDuration INT
);

-- Объявляем переменные один раз
DECLARE @CallContractID INT;
DECLARE @CallDate DATETIME;
DECLARE @CallDuration INT;

DECLARE @CallTotal INT = 1000;
DECLARE @CallCounter INT = 1;

-- Получаем максимий ContractID
DECLARE @MaxContractIDCalls INT = (SELECT MAX(ContractID) FROM Contracts);

WHILE @CallCounter <= @CallTotal
BEGIN
    -- Случайный ContractID
    SET @CallContractID = (ABS(CHECKSUM(NEWID())) % @MaxContractIDCalls) + 1;
    
    -- Генерация даты и времени звонка за последний год
    SET @CallDate = DATEADD(DAY, -ABS(CHECKSUM(NEWID())) % 365, GETDATE()) 
                  + CAST(ABS(CHECKSUM(NEWID())) % 86400 AS FLOAT) / 86400.0;
    
    -- Генерация продолжительности звонка (1 - 3600 секунд)
    SET @CallDuration = (ABS(CHECKSUM(NEWID())) % 3600) + 1;
    
    -- Вставка записи в временную таблицу
    INSERT INTO @CallRecords (ContractID, CallDate, CallDuration)
    VALUES (@CallContractID, @CallDate, @CallDuration);
    
    SET @CallCounter = @CallCounter + 1;
END

-- Вставка данных в таблицу Calls
INSERT INTO Calls (ContractID, CallDate, CallDuration)
SELECT ContractID, CallDate, CallDuration FROM @CallRecords;

-- 8. Вставка сообщений в таблицу Messages

DECLARE @MessageRecords TABLE (
    ContractID INT,
    MessageDate DATETIME,
    IsMMS BIT
);

-- Объявляем переменные один раз
DECLARE @MessageContractID INT;
DECLARE @MessageDate DATETIME;
DECLARE @IsMMS BIT;

DECLARE @MessageTotal INT = 1000;
DECLARE @MessageCounter INT = 1;

DECLARE @MaxContractIDMessages INT = (SELECT MAX(ContractID) FROM Contracts);

WHILE @MessageCounter <= @MessageTotal
BEGIN
    -- Случайный ContractID
    SET @MessageContractID = (ABS(CHECKSUM(NEWID())) % @MaxContractIDMessages) + 1;
    
    -- Генерация даты и времени сообщения за последний год
    SET @MessageDate = DATEADD(DAY, -ABS(CHECKSUM(NEWID())) % 365, GETDATE()) 
                     + CAST(ABS(CHECKSUM(NEWID())) % 86400 AS FLOAT) / 86400.0;
    
    -- Определение, MMS это или SMS
    SET @IsMMS = CASE WHEN (ABS(CHECKSUM(NEWID())) % 2) = 0 THEN 0 ELSE 1 END;
    
    -- Вставка записи в временную таблицу
    INSERT INTO @MessageRecords (ContractID, MessageDate, IsMMS)
    VALUES (@MessageContractID, @MessageDate, @IsMMS);
    
    SET @MessageCounter = @MessageCounter + 1;
END

-- Вставка данных в таблицу Messages
INSERT INTO Messages (ContractID, MessageDate, IsMMS)
SELECT ContractID, MessageDate, IsMMS FROM @MessageRecords;

-- 9. Вставка использования интернета в таблицу InternetUsage

DECLARE @InternetUsageRecords TABLE (
    ContractID INT,
    UsageDate DATETIME,
    DataSentMB DECIMAL(10,2),
    DataReceivedMB DECIMAL(10,2)
);

-- Объявляем переменные один раз
DECLARE @InternetContractID INT;
DECLARE @UsageDate DATETIME;
DECLARE @DataSentMB DECIMAL(10,2);
DECLARE @DataReceivedMB DECIMAL(10,2);

DECLARE @InternetUsageTotal INT = 1000;
DECLARE @InternetUsageCounter INT = 1;

DECLARE @MaxContractIDInternet INT = (SELECT MAX(ContractID) FROM Contracts);

WHILE @InternetUsageCounter <= @InternetUsageTotal
BEGIN
    -- Случайный ContractID
    SET @InternetContractID = (ABS(CHECKSUM(NEWID())) % @MaxContractIDInternet) + 1;
    
    -- Генерация даты и времени использования интернета за последний год
    SET @UsageDate = DATEADD(DAY, -ABS(CHECKSUM(NEWID())) % 365, GETDATE()) 
                  + CAST(ABS(CHECKSUM(NEWID())) % 86400 AS FLOAT) / 86400.0;
    
    -- Генерация объёма отправленных и полученных данных (до 500 МБ)
    SET @DataSentMB = CAST((ABS(CHECKSUM(NEWID())) % 5000) AS DECIMAL(10,2)) / 10.0; -- 0.00 - 500.00
    SET @DataReceivedMB = CAST((ABS(CHECKSUM(NEWID())) % 10000) AS DECIMAL(10,2)) / 10.0; -- 0.00 - 1000.00
    
    -- Вставка записи в временную таблицу
    INSERT INTO @InternetUsageRecords (ContractID, UsageDate, DataSentMB, DataReceivedMB)
    VALUES (@InternetContractID, @UsageDate, @DataSentMB, @DataReceivedMB);
    
    SET @InternetUsageCounter = @InternetUsageCounter + 1;
END

-- Вставка данных в таблицу InternetUsage
INSERT INTO InternetUsage (ContractID, UsageDate, DataSentMB, DataReceivedMB)
SELECT ContractID, UsageDate, DataSentMB, DataReceivedMB FROM @InternetUsageRecords;

-- 10. Завершение процесса
PRINT 'Заполнение таблиц завершено успешно.';
