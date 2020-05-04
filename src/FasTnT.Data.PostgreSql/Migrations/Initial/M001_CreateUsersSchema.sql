CREATE SCHEMA IF NOT EXISTS users;
CREATE TABLE IF NOT EXISTS users.user
(
    id SERIAL NOT NULL,
    username character varying(50) COLLATE pg_catalog."default" NOT NULL,
    password character varying(1023) COLLATE pg_catalog."default" NOT NULL,
    registered_on timestamp without time zone NOT NULL DEFAULT timezone('UTC'::text, now()),
    CONSTRAINT "PK_user" PRIMARY KEY (id),
    CONSTRAINT uq_user_username UNIQUE (username)
)
WITH (OIDS = FALSE);