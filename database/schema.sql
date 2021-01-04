CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

CREATE TABLE documents (
    id uuid NOT NULL,
    created_at timestamp without time zone NOT NULL,
    file_size integer NOT NULL,
    file_type text NULL,
    CONSTRAINT "PK_documents" PRIMARY KEY (id)
);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20210104182103_CreateDocuments', '3.1.7');
