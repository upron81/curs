DROP TABLE IF EXISTS dbo.InternetUsage;
DROP TABLE IF EXISTS dbo.Messages;
DROP TABLE IF EXISTS dbo.Calls;
DROP TABLE IF EXISTS dbo.Contracts;
DROP TABLE IF EXISTS dbo.Subscribers;
DROP TABLE IF EXISTS dbo.TariffPlans;
DROP TABLE IF EXISTS dbo.Staff;
DROP TABLE IF EXISTS dbo.StaffPosition;
DROP VIEW IF EXISTS dbo.EmployeeInfo;
DROP VIEW IF EXISTS dbo.SubscriberInfo;
DROP VIEW IF EXISTS dbo.ContractCalls;
DROP VIEW IF EXISTS dbo.ContractMessages;
DROP VIEW IF EXISTS dbo.ContractInternetUsage;
DROP PROCEDURE IF EXISTS dbo.InsertCall;
DROP PROCEDURE IF EXISTS dbo.InsertInternetUsage;
DROP PROCEDURE IF EXISTS dbo.InsertMessage;

-- Таблица для хранения должностей сотрудников
CREATE TABLE StaffPosition (
    PositionID INT PRIMARY KEY IDENTITY(1,1),    -- Идентификатор должности
    PositionName NVARCHAR(100) NOT NULL          -- Название должности (например, "Менеджер", "Оператор")
);

-- Таблица для хранения информации о сотрудниках
CREATE TABLE Staff (
    StaffID INT PRIMARY KEY IDENTITY(1,1),       -- Идентификатор сотрудника
    FullName NVARCHAR(150) NOT NULL,             -- ФИО сотрудника
    PositionID INT NOT NULL FOREIGN KEY REFERENCES StaffPosition(PositionID) ON DELETE CASCADE, -- Ссылка на должность сотрудника
    Education NVARCHAR(100)                      -- Образование сотрудника
);

-- Таблица для хранения информации о тарифных планах
CREATE TABLE TariffPlans (
    TariffPlanID INT PRIMARY KEY IDENTITY(1,1),  -- Идентификатор тарифного плана
    TariffName NVARCHAR(100) NOT NULL,           -- Название тарифного плана
    SubscriptionFee DECIMAL(10, 2) NOT NULL,     -- Абонентская плата
    LocalCallRate DECIMAL(10, 2) NOT NULL,       -- Стоимость минуты разговора при местной связи
    LongDistanceCallRate DECIMAL(10, 2) NOT NULL,-- Стоимость минуты разговора при междугородной связи
    InternationalCallRate DECIMAL(10, 2) NOT NULL,-- Стоимость минуты разговора при международной связи
    IsPerSecond BIT NOT NULL,                    -- Если FALSE, то тарификация поминутная; если TRUE, то посекундная
    SmsRate DECIMAL(10, 2) NOT NULL,             -- Стоимость SMS
    MmsRate DECIMAL(10, 2) NOT NULL,             -- Стоимость MMS
    DataRatePerMB DECIMAL(10, 2) NOT NULL        -- Стоимость передачи 1 МБ данных
);

-- Таблица для хранения информации об абонентах
CREATE TABLE Subscribers (
    SubscriberID INT PRIMARY KEY IDENTITY(1,1),  -- Идентификатор абонента
    FullName NVARCHAR(150) NOT NULL,             -- ФИО абонента
    HomeAddress NVARCHAR(255),                   -- Домашний адрес абонента
    PassportData NVARCHAR(100)                   -- Паспортные данные абонента
);

-- Таблица для хранения договоров на оказание услуг
CREATE TABLE Contracts (
    ContractID INT PRIMARY KEY IDENTITY(1,1),    -- Идентификатор договора
    SubscriberID INT NOT NULL FOREIGN KEY REFERENCES Subscribers(SubscriberID) ON DELETE CASCADE,  -- Идентификатор абонента
    TariffPlanID INT NOT NULL FOREIGN KEY REFERENCES TariffPlans(TariffPlanID) ON DELETE CASCADE,  -- Идентификатор тарифного плана
    ContractDate DATE NOT NULL,                  -- Дата заключения договора
    ContractEndDate DATE NULL,                   -- Дата окончания договора (может быть NULL)
    PhoneNumber NVARCHAR(20) NOT NULL,           -- Телефонный номер, присвоенный абоненту
    StaffID INT NOT NULL FOREIGN KEY REFERENCES Staff(StaffID) ON DELETE CASCADE  -- Ссылка на сотрудника, оформившего договор
);

-- Таблица для хранения информации о звонках
CREATE TABLE Calls (
    CallID INT PRIMARY KEY IDENTITY(1,1),        -- Идентификатор звонка
    ContractID INT NOT NULL FOREIGN KEY REFERENCES Contracts(ContractID) ON DELETE CASCADE, -- Ссылка на договор
    CallDate DATETIME NOT NULL,                  -- Дата и время звонка
    CallDuration INT NOT NULL                    -- Продолжительность разговора (в секундах)
);

-- Таблица для хранения информации о смс и ммс
CREATE TABLE Messages (
    MessageID INT PRIMARY KEY IDENTITY(1,1),     -- Идентификатор сообщения
    ContractID INT NOT NULL FOREIGN KEY REFERENCES Contracts(ContractID) ON DELETE CASCADE, -- Ссылка на договор
    MessageDate DATETIME NOT NULL,               -- Дата и время отправки сообщения (SMS или MMS)
    IsMMS BIT NOT NULL                           -- Если значение FALSE, то это SMS; если TRUE, то MMS
);

-- Таблица для хранения информации о трафике интернета
CREATE TABLE InternetUsage (
    UsageID INT PRIMARY KEY IDENTITY(1,1),       -- Идентификатор записи использования интернета
    ContractID INT NOT NULL FOREIGN KEY REFERENCES Contracts(ContractID) ON DELETE CASCADE, -- Ссылка на договор
    UsageDate DATETIME NOT NULL,                 -- Дата и время использования интернета
    DataSentMB DECIMAL(10, 2) NOT NULL,          -- Объем отправленных данных (в МБ)
    DataReceivedMB DECIMAL(10, 2) NOT NULL       -- Объем полученных данных (в МБ)
);


-- Представления

GO

CREATE VIEW EmployeeInfo AS
SELECT 
    s.StaffID,
    s.FullName,
    sp.PositionName,
    s.Education
FROM 
    Staff s
JOIN 
    StaffPosition sp ON s.PositionID = sp.PositionID

GO

CREATE VIEW SubscriberInfo AS
SELECT 
    s.SubscriberID,
    s.FullName AS SubscriberFullName,
    s.HomeAddress,
    s.PassportData,
    c.ContractID,
    c.ContractDate,
    c.ContractEndDate,
    c.PhoneNumber,
    tp.TariffName
FROM 
    Subscribers s
JOIN 
    Contracts c ON s.SubscriberID = c.SubscriberID
JOIN 
    TariffPlans tp ON c.TariffPlanID = tp.TariffPlanID;

GO

CREATE VIEW ContractCalls AS
SELECT 
    c.ContractID,
    c.PhoneNumber,
    cl.CallID,
    cl.CallDate,
    cl.CallDuration
FROM 
    Contracts c
JOIN 
    Calls cl ON c.ContractID = cl.ContractID;

GO

CREATE VIEW ContractMessages AS
SELECT 
    c.ContractID,
    c.PhoneNumber,
    m.MessageID,
    m.MessageDate,
    m.IsMMS
FROM 
    Contracts c
JOIN 
    Messages m ON c.ContractID = m.ContractID;

GO

CREATE VIEW ContractInternetUsage AS
SELECT 
    c.ContractID,
    c.PhoneNumber,
    iu.UsageID,
    iu.UsageDate,
    iu.DataSentMB,
    iu.DataReceivedMB
FROM 
    Contracts c
JOIN 
    InternetUsage iu ON c.ContractID = iu.ContractID;

GO



-- Хранимые процедуры

CREATE PROCEDURE InsertCall
    @ContractID INT,
    @CallDate DATETIME,
    @CallDuration INT
AS
BEGIN
    INSERT INTO Calls (ContractID, CallDate, CallDuration)
    VALUES (@ContractID, @CallDate, @CallDuration);
END;

GO

CREATE PROCEDURE InsertInternetUsage
    @ContractID INT,
    @UsageDate DATETIME,
    @DataSentMB DECIMAL(10, 2),
    @DataReceivedMB DECIMAL(10, 2)
AS
BEGIN
    INSERT INTO InternetUsage (ContractID, UsageDate, DataSentMB, DataReceivedMB)
    VALUES (@ContractID, @UsageDate, @DataSentMB, @DataReceivedMB);
END;

GO

CREATE PROCEDURE InsertMessage
    @ContractID INT,
    @MessageDate DATETIME,
    @IsMMS BIT
AS
BEGIN
    INSERT INTO Messages (ContractID, MessageDate, IsMMS)
    VALUES (@ContractID, @MessageDate, @IsMMS);
END;
