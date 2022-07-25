CREATE PROCEDURE DummyProcedureTest
(
	VarCharParam varchar(16) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci
)
BEGIN
	SELECT IntColumn, CharColumn
	FROM DummyTableTest;
END