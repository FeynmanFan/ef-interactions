UPDATE Users SET PasswordHash = 'password123' WHERE UserId = 1
UPDATE Users SET PasswordHash = 'MySecret456!' WHERE UserId = 2
UPDATE Users SET PasswordHash = 'LetMeIn789' WHERE UserId = 3

SELECT * FROM Users