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

CREATE TABLE claims (
    id uuid NOT NULL,
    created_at timestamp without time zone NOT NULL,
    document_id uuid NOT NULL,
    service_area_created_by text NULL,
    user_created_by text NULL,
    api_created_by text NOT NULL,
    retention_expires_at timestamp without time zone NOT NULL,
    CONSTRAINT "PK_claims" PRIMARY KEY (id),
    CONSTRAINT "FK_claims_documents_document_id" FOREIGN KEY (document_id) REFERENCES documents (id) ON DELETE CASCADE
);

CREATE INDEX "IX_claims_document_id" ON claims (document_id);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20210105112353_CreateClaims', '3.1.7');

ALTER TABLE documents ALTER COLUMN file_size TYPE bigint;
ALTER TABLE documents ALTER COLUMN file_size SET NOT NULL;
ALTER TABLE documents ALTER COLUMN file_size DROP DEFAULT;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20210108162815_ChangeFileSizeToLong', '3.1.7');


