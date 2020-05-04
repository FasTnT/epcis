CREATE SCHEMA IF NOT EXISTS cbv;
CREATE TABLE IF NOT EXISTS cbv.masterdata
(
    id character varying(128) COLLATE pg_catalog."default" NOT NULL,
    type character varying(128) COLLATE pg_catalog."default" NOT NULL,
    created_on timestamp without time zone NOT NULL DEFAULT timezone('UTC'::text, now()),
    last_update timestamp without time zone NOT NULL DEFAULT timezone('UTC'::text, now()),
    CONSTRAINT pk_cbv_masterdata PRIMARY KEY (id, type)
)
WITH (OIDS = FALSE);
CREATE TABLE IF NOT EXISTS cbv.attribute
(
    masterdata_id character varying(128) COLLATE pg_catalog."default" NOT NULL,
    masterdata_type character varying(128) COLLATE pg_catalog."default" NOT NULL,
    id character varying(128) COLLATE pg_catalog."default" NOT NULL,
    value character varying(128) COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT pk_cbv_masterdata_attribute PRIMARY KEY (masterdata_id, masterdata_type, id),
    CONSTRAINT fl_cbv_attribute_to_masterdata FOREIGN KEY (masterdata_id, masterdata_type) REFERENCES cbv.masterdata (id, type)
)
WITH (OIDS = FALSE);
CREATE TABLE IF NOT EXISTS cbv.attribute_field
(
    internal_id int NOT NULL,
    internal_parent_id int NULL,
    masterdata_type character varying(128) COLLATE pg_catalog."default" NOT NULL,
	masterdata_id character varying(128) COLLATE pg_catalog."default" NOT NULL,
    parent_id character varying(128) COLLATE pg_catalog."default" NOT NULL,
    name character varying(128) COLLATE pg_catalog."default" NOT NULL,
    namespace character varying(128) COLLATE pg_catalog."default" NULL,
    value character varying(128) COLLATE pg_catalog."default" NULL,
    CONSTRAINT pk_cbv_masterdata_attribute_field PRIMARY KEY (masterdata_type, masterdata_id, internal_id)
)
WITH (OIDS = FALSE);
CREATE TABLE IF NOT EXISTS cbv.hierarchy
(
    type character varying(128) COLLATE pg_catalog."default" NOT NULL,
    parent_id character varying(128) COLLATE pg_catalog."default" NOT NULL,
    children_id character varying(128) COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT pk_cbv_hierarchy PRIMARY KEY (type, parent_id, children_id)
)
WITH (OIDS = FALSE);

CREATE MATERIALIZED VIEW IF NOT EXISTS cbv.masterdata_hierarchy 
AS
(
  WITH RECURSIVE children(type, parent_id, children_id, depth) AS 
  (
    (
  	  SELECT type, parent_id, children_id, 0 
      FROM cbv.hierarchy
    )
    UNION
    (
  	  SELECT c.type, c.parent_id, h.children_id, c.depth+1 
      FROM cbv.hierarchy h 
      JOIN children c 
      ON (c.children_id = h.parent_id AND c.type = h.type)
    )
  )
  SELECT * FROM children
);

CREATE OR REPLACE FUNCTION refresh_masterdata_hierarchy()
RETURNS TRIGGER LANGUAGE plpgsql
AS $$
BEGIN
  REFRESH MATERIALIZED VIEW cbv.masterdata_hierarchy;
  RETURN NULL;
END $$;

DROP TRIGGER IF EXISTS flatten_cbv_hierarchy ON cbv.hierarchy CASCADE;
CREATE TRIGGER flatten_cbv_hierarchy AFTER INSERT OR UPDATE OR DELETE ON cbv.hierarchy
EXECUTE PROCEDURE refresh_masterdata_hierarchy();