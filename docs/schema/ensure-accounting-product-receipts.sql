-- Run on the ACCOUNTING host database (Domain/DomainCommandConnection), not Integration.
-- Accounting-owned command receipts must commit atomically with the product write.
-- No legacy table is altered, no prior version is guessed, no business row is changed.
SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF OBJECT_ID(N'dbo.TBL_Product', N'U') IS NULL OR OBJECT_ID(N'dbo.TBL_Shop', N'U') IS NULL
    THROW 51000, 'Select the accounting database with TBL_Product and TBL_Shop.', 1;
IF OBJECT_ID(N'dbo.AccountingProductChangeReceipts', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AccountingProductChangeReceipts
    (
        ConnectionKey binary(32) NOT NULL,
        EventKey binary(32) NOT NULL,
        StreamKey binary(32) NOT NULL,
        ShopId int NOT NULL,
        TenantId nvarchar(64) NOT NULL,
        ConnectionId bigint NOT NULL,
        HyperProductId int NOT NULL,
        SourceVersion bigint NOT NULL,
        Fingerprint binary(32) NOT NULL,
        AppliedAtUtc datetime2 NOT NULL,
        CONSTRAINT PK_AccountingProductChangeReceipts PRIMARY KEY(ConnectionKey,EventKey),
        CONSTRAINT UQ_AccountingProductChangeReceipts_StreamVersion UNIQUE(StreamKey,SourceVersion),
        CONSTRAINT CK_AccountingProductChangeReceipts_Identity CHECK
            (ShopId > 0 AND ConnectionId > 0 AND HyperProductId > 0 AND SourceVersion > 0)
    );
END;
COMMIT TRANSACTION;
