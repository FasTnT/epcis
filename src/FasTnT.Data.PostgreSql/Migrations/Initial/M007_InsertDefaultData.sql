INSERT INTO epcis.event_type (id, name, is_deprecated) SELECT 0, 'OBJECT', false WHERE NOT EXISTS (SELECT id FROM epcis.event_type WHERE id = 0);
INSERT INTO epcis.event_type (id, name, is_deprecated) SELECT 1, 'AGGREGATION', false WHERE NOT EXISTS (SELECT id FROM epcis.event_type WHERE id = 1);
INSERT INTO epcis.event_type (id, name, is_deprecated) SELECT 2, 'TRANSACTION', false WHERE NOT EXISTS (SELECT id FROM epcis.event_type WHERE id = 2);
INSERT INTO epcis.event_type (id, name, is_deprecated) SELECT 3, 'TRANSFORMATION', false WHERE NOT EXISTS (SELECT id FROM epcis.event_type WHERE id = 3);
INSERT INTO epcis.event_type (id, name, is_deprecated) SELECT 4, 'QUANTITY', true WHERE NOT EXISTS (SELECT id FROM epcis.event_type WHERE id = 4);


INSERT INTO users.user (username, password) SELECT 'admin', 'ACCFD4C270758972FCF9B17EECD9A15B951D67FF98342D9F5EE697936E31FA5E' WHERE NOT EXISTS (SELECT id FROM users.user WHERE username = 'admin');