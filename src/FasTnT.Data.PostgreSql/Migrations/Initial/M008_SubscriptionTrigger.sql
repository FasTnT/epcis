CREATE OR REPLACE FUNCTION subscriptions.initial_requests()
    RETURNS trigger
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE NOT LEAKPROOF 
AS $BODY$
    DECLARE
        c_requests CURSOR FOR SELECT * FROM epcis.request WHERE record_time >= NEW.initial_record_time;
        r_request epcis.request%ROWTYPE;
    BEGIN
        FOR r_request IN c_requests LOOP
            INSERT INTO subscriptions.pendingrequest (subscription_id, request_id) VALUES (NEW.id, r_request.id);
        END LOOP;

        RETURN NULL;
    END;
$BODY$;

DROP TRIGGER IF EXISTS add_initial_request ON subscriptions.subscription CASCADE;
CREATE TRIGGER add_initial_request AFTER INSERT ON subscriptions.subscription
FOR EACH ROW EXECUTE PROCEDURE subscriptions.initial_requests();