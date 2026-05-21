-- =============================================
-- Biometric Evaluation Tables
-- =============================================

-- 1. BiometricEvaluation: Represents a single session/evaluation
CREATE TABLE BiometricEvaluation (
    EvaluationId         INT PRIMARY KEY IDENTITY(1,1),
    UserId               INT NOT NULL,
    EvaluationDate       DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    Notes                NVARCHAR(500) NULL,
    CreatedAt            DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    
    CONSTRAINT FK_BiometricEvaluation_Users 
        FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

-- 2. BiometricMeasure: Individual measurements within an evaluation
CREATE TABLE BiometricMeasure (
    MeasureId            INT PRIMARY KEY IDENTITY(1,1),
    EvaluationId         INT NOT NULL,
    MeasureCode          CHAR(4) NOT NULL,           -- 4-letter code (e.g., BFPT, WGTL, etc.)
    MeasureValue         DECIMAL(8,3) NOT NULL,       -- Value with 3 decimal places precision
    Unit                 NVARCHAR(20) NULL,           -- Optional: lbs, %, mmHg, etc.
    Notes                NVARCHAR(200) NULL,
    
    CONSTRAINT FK_BiometricMeasure_Evaluation 
        FOREIGN KEY (EvaluationId) REFERENCES BiometricEvaluation(EvaluationId)
);

-- =============================================
-- Sample Data
-- =============================================

-- Sample Users (assuming Users table exists)
-- INSERT INTO Users (Username, Email, PasswordHash) VALUES ... (if needed)

-- Sample Biometric Evaluations
INSERT INTO BiometricEvaluation (UserId, EvaluationDate, Notes)
VALUES 
    (1, '2025-05-01 08:15:00', 'Morning baseline'),
    (1, '2025-05-08 07:45:00', 'Post-cut check-in'),
    (2, '2025-05-05 09:30:00', 'Initial assessment');

-- Sample Biometric Measures
INSERT INTO BiometricMeasure (EvaluationId, MeasureCode, MeasureValue, Unit, Notes)
VALUES 
    -- Evaluation 1
    (1, 'WGTL',  242.5,  'lbs',   'Morning weight'),
    (1, 'BFPT',  28.4,   '%',     'Body fat percentage'),
    (1, 'BPMI',  28.7,   'kg/m2', 'BMI'),
    (1, 'WSTC',  42.5,   'in',    'Waist circumference'),

    -- Evaluation 2
    (2, 'WGTL',  231.8,  'lbs',   'After 7 days of cut'),
    (2, 'BFPT',  26.1,   '%',     'Improved body fat'),
    (2, 'BPMI',  27.4,   'kg/m2', NULL),

    -- Evaluation 3 (different user)
    (3, 'WGTL',  185.3,  'lbs',   NULL),
    (3, 'BFPT',  14.8,   '%',     'Athletic range');