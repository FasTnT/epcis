CREATE SCHEMA IF NOT EXISTS subscriptions;

CREATE TABLE IF NOT EXISTS subscriptions.subscription
(
    id SERIAL NOT NULL,
    subscription_id character varying(128) COLLATE pg_catalog."default" NOT NULL,
    trigger character varying(1023) COLLATE pg_catalog."default",
    initial_record_time timestamp without time zone,
    report_if_empty boolean NOT NULL,
    destination character varying(128) COLLATE pg_catalog."default" NOT NULL,
    query_name character varying(128) COLLATE pg_catalog."default" NOT NULL,
    active boolean NOT NULL DEFAULT true,
    schedule_seconds character varying(255) COLLATE pg_catalog."default",
    schedule_minutes character varying(255) COLLATE pg_catalog."default",
    schedule_hours character varying(255) COLLATE pg_catalog."default",
    schedule_day_of_month character varying(255) COLLATE pg_catalog."default",
    schedule_month character varying(255) COLLATE pg_catalog."default",
    schedule_day_of_week character varying(255) COLLATE pg_catalog."default",
    CONSTRAINT "PK_subscription" PRIMARY KEY (id),
    CONSTRAINT uq_subscription_id UNIQUE (subscription_id)
)
WITH (OIDS = FALSE);
CREATE TABLE IF NOT EXISTS subscriptions.parameter
(
    id SERIAL NOT NULL,
    subscription_id int NOT NULL,
    name character varying(1023) COLLATE pg_catalog."default",
    CONSTRAINT "PK_parameter" PRIMARY KEY (id),
    CONSTRAINT fk_subscription_parameter FOREIGN KEY (subscription_id) REFERENCES subscriptions.subscription (id) ON DELETE CASCADE
)
WITH (OIDS = FALSE);
CREATE TABLE IF NOT EXISTS subscriptions.parameter_value
(
    id SERIAL NOT NULL,
    parameter_id int NOT NULL,
    value character varying(255) COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT "PK_parameter_value" PRIMARY KEY (id),
    CONSTRAINT fk_value_parameter FOREIGN KEY (parameter_id) REFERENCES subscriptions.parameter (id) ON DELETE CASCADE
)
WITH (OIDS = FALSE);
CREATE TABLE IF NOT EXISTS subscriptions.pendingrequest
(
    subscription_id int NOT NULL,
    request_id int NOT NULL,
    CONSTRAINT fk_pending_request FOREIGN KEY (request_id) REFERENCES epcis.request (id),
    CONSTRAINT fk_subscription_pending FOREIGN KEY (subscription_id) REFERENCES subscriptions.subscription (id) ON DELETE CASCADE
)
WITH (OIDS = FALSE);
CREATE TABLE IF NOT EXISTS subscriptions.trigger
(
    id SERIAL NOT NULL,
    subscription_id int NOT NULL,
    trigger_time timestamp without time zone NOT NULL,
    status smallint NOT NULL,
	reason character varying(255) NULL,
    CONSTRAINT fh_trigger_subscription FOREIGN KEY (subscription_id) REFERENCES subscriptions.subscription (id) ON DELETE CASCADE
)
WITH (OIDS = FALSE);

-- Subscription function
CREATE OR REPLACE FUNCTION subscriptions.update_subscriptions()
    RETURNS trigger
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE NOT LEAKPROOF 
AS $BODY$
    DECLARE
        c_subscription CURSOR FOR SELECT * FROM subscriptions.subscription WHERE active is True;
        r_subscription subscriptions.subscription%ROWTYPE;
    BEGIN
        FOR r_subscription IN c_subscription LOOP
            INSERT INTO subscriptions.pendingrequest (subscription_id, request_id) VALUES (r_subscription.id, NEW.id);
        END LOOP;

        RETURN NULL;
    END;
$BODY$;

DROP TRIGGER IF EXISTS add_pending_request ON epcis.request CASCADE;
CREATE TRIGGER add_pending_request AFTER INSERT ON epcis.request
FOR EACH ROW EXECUTE PROCEDURE subscriptions.update_subscriptions();


-- CALLBACK SCHEMA
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