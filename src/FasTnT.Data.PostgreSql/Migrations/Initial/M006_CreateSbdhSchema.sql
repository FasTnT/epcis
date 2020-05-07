CREATE SCHEMA IF NOT EXISTS sbdh;

CREATE TABLE IF NOT EXISTS sbdh.standardheader(
    id int NOT NULL,
	version character varying(50),
	standard character varying(128),
	type_version character varying(128),
	identifier character varying(128),
	type character varying(128),
	creation_datetime timestamp without time zone,
	CONSTRAINT pk_standardheader PRIMARY KEY (id),
	CONSTRAINT fk_standardheader_request FOREIGN KEY (id) REFERENCES epcis.request (id)
)
WITH (OIDS = FALSE);

CREATE TABLE IF NOT EXISTS sbdh.contactinformation(
    id int NOT NULL,
	header_id int NOT NULL,
	type smallint NOT NULL,
	identifier character varying(50),
	contact character varying(128),
	email character varying(128),
	fax_number character varying(128),
	phone_number character varying(128),
	type_identifier character varying(128),
	CONSTRAINT pk_contactinformation PRIMARY KEY (header_id, id),
	CONSTRAINT fk_contactinformation_standardheader FOREIGN KEY (header_id) REFERENCES sbdh.standardheader (id)
)
WITH (OIDS = FALSE);

CREATE TABLE IF NOT EXISTS sbdh.custom_field
(
	header_id int NOT NULL,
    field_id integer NOT NULL,
    parent_id integer,
    namespace character varying(128) COLLATE pg_catalog."default" NOT NULL,
    name character varying(128) COLLATE pg_catalog."default" NOT NULL,
    type smallint NOT NULL,
    text_value character varying(128) COLLATE pg_catalog."default",
    numeric_value double precision,
    date_value timestamp without time zone,
    CONSTRAINT "PK_HEADER_CUSTOMFIELD" PRIMARY KEY (header_id, field_id),
    CONSTRAINT "FK_CUSTOMFIELD_EVENT" FOREIGN KEY (header_id) REFERENCES sbdh.standardheader (id)
)
WITH (OIDS = FALSE);