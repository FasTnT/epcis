CREATE SCHEMA IF NOT EXISTS epcis;
CREATE TABLE IF NOT EXISTS epcis.request
(
    id SERIAL NOT NULL,
    document_time timestamp without time zone NOT NULL,
    record_time timestamp without time zone NOT NULL DEFAULT timezone('UTC'::text, now()),
    user_id int,
    subscription_id character varying(128) COLLATE pg_catalog."default",
    CONSTRAINT "PK_request" PRIMARY KEY (id)
)
WITH (OIDS = FALSE);
CREATE TABLE IF NOT EXISTS epcis.event_type
(
    id smallint NOT NULL,
    name character varying(25) COLLATE pg_catalog."default" NOT NULL,
    is_deprecated boolean NOT NULL DEFAULT false,
    CONSTRAINT "PK_event_type" PRIMARY KEY (id)
)
WITH (OIDS = FALSE);
CREATE TABLE IF NOT EXISTS epcis.event
(
    id SERIAL NOT NULL,
    request_id int NOT NULL,
    record_time timestamp without time zone NOT NULL,
    action smallint,
    event_type smallint NOT NULL,
    event_timezone_offset smallint NOT NULL,
    event_id character varying(128) COLLATE pg_catalog."default",
    business_location character varying(128) COLLATE pg_catalog."default",
    business_step character varying(128) COLLATE pg_catalog."default",
    disposition character varying(128) COLLATE pg_catalog."default",
    read_point character varying(128) COLLATE pg_catalog."default",
    transformation_id character varying(128) COLLATE pg_catalog."default",
    CONSTRAINT "PK_event" PRIMARY KEY (id),
    CONSTRAINT "FK_EVENT_EVENTTYPE" FOREIGN KEY (event_type) REFERENCES epcis.event_type (id),
    CONSTRAINT "FK_EVENT_REQUEST" FOREIGN KEY (request_id) REFERENCES epcis.request (id)
)
WITH (OIDS = FALSE);
CREATE TABLE IF NOT EXISTS epcis.business_location
(
    event_id int NOT NULL,
    location_id character varying(128) COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT "PK_BUSINESS_LOCATION" PRIMARY KEY (event_id, location_id),
    CONSTRAINT "FK_BUSINESS_LOCATION_EVENT" FOREIGN KEY (event_id) REFERENCES epcis.event (id)
)
WITH (OIDS = FALSE);
CREATE TABLE 
IF NOT EXISTS epcis.business_transaction
(
    event_id int NOT NULL,
    transaction_type character varying(128) COLLATE pg_catalog."default" NOT NULL,
    transaction_id character varying(128) COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT "PK_BUSINESS_TRANSACTION" PRIMARY KEY (event_id, transaction_type, transaction_id),
    CONSTRAINT "FK_BUSINESS_TRANSACTION_EVENT" FOREIGN KEY (event_id) REFERENCES epcis.event (id)
)
WITH (OIDS = FALSE);
CREATE TABLE IF NOT EXISTS epcis.custom_field
(
    event_id int NOT NULL,
    field_id integer NOT NULL,
    parent_id integer,
    namespace character varying(128) COLLATE pg_catalog."default" NOT NULL,
    name character varying(128) COLLATE pg_catalog."default" NOT NULL,
    type smallint NOT NULL,
    text_value character varying(128) COLLATE pg_catalog."default",
    numeric_value double precision,
    date_value timestamp without time zone,
    CONSTRAINT "PK_CUSTOMFIELD" PRIMARY KEY (event_id, field_id),
    CONSTRAINT "FK_CUSTOMFIELD_EVENT" FOREIGN KEY (event_id) REFERENCES epcis.event (id)
)
WITH (OIDS = FALSE);
CREATE TABLE IF NOT EXISTS epcis.epc
(
    event_id int NOT NULL,
    epc character varying(128) COLLATE pg_catalog."default" NOT NULL,
    type smallint NOT NULL,
    is_quantity boolean NOT NULL,
    quantity real,
    unit_of_measure character varying(3) COLLATE pg_catalog."default",
    CONSTRAINT "PK_EPC" PRIMARY KEY (event_id, epc, type),
    CONSTRAINT "FK_EPC_EVENT" FOREIGN KEY (event_id) REFERENCES epcis.event (id)
)
WITH (OIDS = FALSE);
CREATE TABLE IF NOT EXISTS epcis.source_destination
(
    event_id int NOT NULL,
    type character varying(128) COLLATE pg_catalog."default" NOT NULL,
    source_dest_id character varying(128) COLLATE pg_catalog."default" NOT NULL,
    direction smallint NOT NULL,
    CONSTRAINT "PK_SOURCE_DESTINATION" PRIMARY KEY (event_id, direction, type),
    CONSTRAINT "FK_SOURCE_DESTINATION_EVENT" FOREIGN KEY (event_id) REFERENCES epcis.event (id)
)
WITH (OIDS = FALSE);
CREATE TABLE IF NOT EXISTS epcis.error_declaration
(
    event_id int NOT NULL,
    declaration_time timestamp without time zone NOT NULL,
    reason character varying(128) COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT "PK_error_declaration" PRIMARY KEY (event_id),
    CONSTRAINT "FK_error_declaration_EVENT" FOREIGN KEY (event_id) REFERENCES epcis.event (id)
)
WITH (OIDS = FALSE);
CREATE TABLE IF NOT EXISTS epcis.error_declaration_eventid
(
    event_id int NOT NULL,
    corrective_eventid character varying(128) COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT "PK_CORRECTIVE_EVENTID" PRIMARY KEY (event_id, corrective_eventid),
    CONSTRAINT "FK_error_declaration_eventid_error_declaration" FOREIGN KEY (event_id) REFERENCES epcis.error_declaration (event_id)
)
WITH (OIDS = FALSE);