CREATE TABLE IF NOT EXISTS "SynchronizeItems" (
    "Guid" TEXT,
    "SourcePath" TEXT,
    "DestinationPath" TEXT,
    "LastExecuteTime" TEXT,
    "LastSynchronizeTime" TEXT,
	PRIMARY KEY("Guid")
);