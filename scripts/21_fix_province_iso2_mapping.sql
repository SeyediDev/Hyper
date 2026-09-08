-- Fix province ISO2 codes to match jVectorMap Iran map
-- This script aligns province ISO2 codes with the correct province names based on ISO-3166-2 standard
-- Using EnglishTitle for mapping to avoid encoding issues

-- Mapping of correct ISO2 codes to English province names (based on jVectorMap)
DECLARE @CorrectMapping TABLE (
    Iso2 NVARCHAR(8) PRIMARY KEY,
    EnglishTitle NVARCHAR(128) NOT NULL
);

INSERT INTO @CorrectMapping(Iso2, EnglishTitle) VALUES
    (N'IR-01', N'East Azerbaijan'),
    (N'IR-02', N'West Azerbaijan'),
    (N'IR-03', N'Ardabil'),
    (N'IR-04', N'Isfahan'),
    (N'IR-05', N'Ilam'),
    (N'IR-06', N'Bushehr'),
    (N'IR-07', N'Tehran'),
    (N'IR-08', N'Chaharmahal and Bakhtiari'),
    (N'IR-09', N'South Khorasan'),
    (N'IR-10', N'Khuzestan'),
    (N'IR-11', N'Zanjan'),
    (N'IR-12', N'Semnan'),
    (N'IR-13', N'Sistan and Baluchestan'),
    (N'IR-14', N'Fars'),
    (N'IR-15', N'Kerman'),
    (N'IR-16', N'Kurdistan'),
    (N'IR-17', N'Kermanshah'),
    (N'IR-18', N'Kohgiluyeh and Boyer-Ahmad'),
    (N'IR-19', N'Gilan'),
    (N'IR-20', N'Lorestan'),
    (N'IR-21', N'Mazandaran'),
    (N'IR-22', N'Markazi'),
    (N'IR-23', N'Hormozgan'),
    (N'IR-24', N'Hamadan'),
    (N'IR-25', N'Yazd'),
    (N'IR-26', N'Qom'),
    (N'IR-27', N'Golestan'),
    (N'IR-28', N'Qazvin'),
    (N'IR-29', N'South Khorasan'), -- Note: IR-29 is also South Khorasan in some mappings
    (N'IR-30', N'Razavi Khorasan'),
    (N'IR-31', N'North Khorasan'),
    (N'IR-32', N'Alborz');

-- Update provinces with correct ISO2 codes based on their English titles
UPDATE p
SET p.Iso2 = m.Iso2
FROM [CoreConfig].[Provinces] AS p
INNER JOIN @CorrectMapping AS m 
    ON m.EnglishTitle COLLATE DATABASE_DEFAULT = p.EnglishTitle COLLATE DATABASE_DEFAULT
WHERE p.Iso2 COLLATE DATABASE_DEFAULT <> m.Iso2 COLLATE DATABASE_DEFAULT;

-- Show what was updated
SELECT 
    p.Id,
    p.EnglishTitle AS [Province Name],
    p.Iso2 AS [Current ISO2],
    m.Iso2 AS [Correct ISO2],
    CASE 
        WHEN p.Iso2 COLLATE DATABASE_DEFAULT = m.Iso2 COLLATE DATABASE_DEFAULT THEN N'Correct'
        ELSE N'Needs Update'
    END AS [Status]
FROM [CoreConfig].[Provinces] AS p
LEFT JOIN @CorrectMapping AS m 
    ON m.EnglishTitle COLLATE DATABASE_DEFAULT = p.EnglishTitle COLLATE DATABASE_DEFAULT
WHERE p.CountryId = (SELECT TOP 1 Id FROM [CoreConfig].[Countries] WHERE Iso2 = N'IR')
ORDER BY p.Iso2;

-- Show any provinces that don't have a mapping
SELECT 
    p.Id,
    p.EnglishTitle,
    p.Iso2,
    N'No mapping found' AS [Status]
FROM [CoreConfig].[Provinces] AS p
LEFT JOIN @CorrectMapping AS m 
    ON m.EnglishTitle COLLATE DATABASE_DEFAULT = p.EnglishTitle COLLATE DATABASE_DEFAULT
WHERE p.CountryId = (SELECT TOP 1 Id FROM [CoreConfig].[Countries] WHERE Iso2 = N'IR')
    AND m.Iso2 IS NULL;

-- Verify no duplicate ISO2 codes
SELECT 
    Iso2,
    COUNT(*) AS [Count],
    STRING_AGG(Title, N', ') AS [Provinces]
FROM [CoreConfig].[Provinces]
WHERE CountryId = (SELECT TOP 1 Id FROM [CoreConfig].[Countries] WHERE Iso2 = N'IR')
GROUP BY Iso2
HAVING COUNT(*) > 1;

