CREATE SCHEMA IF NOT EXISTS callback;

CREATE TABLE IF NOT EXISTS callback.query_callback
(
    id SERIAL NOT NULL,
    request_id int NULL,
    subscription_id character varying(128) COLLATE pg_catalog."default",
    callback_type smallint NOT NULL,
    CONSTRAINT pk_query_callback PRIMARY KEY (id),
    CONSTRAINT fk_query_callback_request FOREIGN KEY (request_id) REFERENCES epcis.request (id)
)
WITH (OIDS = FALSE);