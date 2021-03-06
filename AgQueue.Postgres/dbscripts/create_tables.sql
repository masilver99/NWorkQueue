--This is run by the admin in the agqueue database

CREATE EXTENSION IF NOT EXISTS citext;

Create table IF NOT EXISTS transactions
(id SERIAL PRIMARY KEY,
 state SMALLINT NOT NULL,
 start_datetime timestamptz NOT NULL,
 expiry_datetimeDateTime timestamptz NOT NULL,
 end_datetime timestamptz,
 end_reason TEXT);

Create table IF NOT EXISTS queues
(id SERIAL PRIMARY KEY,
 name TEXT UNIQUE NOT NULL);

Create TABLE IF NOT EXISTS messages
(id SERIAL PRIMARY KEY,
 queue_id INT NOT NULL,
 transaction_id INT,
 transaction_action SMALLINT NOT NULL,
 state SMALLINT NOT NULL,
 add_datetime TIMESTAMPTZ NOT NULL,
 close_datetime TIMESTAMPTZ,
 priority SMALLINT NOT NULL,
 max_attempts INT NOT NULL,
 attempts INT NOT NULL,
 expiry_datetime TIMESTAMPTZ NULL,  -- Null means it won't expire.
 correlation_id INT,
 group_name TEXT,
 metadata JSONB,
 payload BYTEA,
 FOREIGN KEY(queue_id) REFERENCES queues(id),
 FOREIGN KEY(transaction_id) REFERENCES transactions(id));

Create table IF NOT EXISTS tags
(id SERIAL PRIMARY KEY,
 tag_name TEXT,
 tag_value TEXT);

CREATE UNIQUE INDEX IF NOT EXISTS idx_tag_name ON tags(tag_name);

Create table IF NOT EXISTS message_tags
(tag_id INTEGER,
 message_id INTEGER,
 PRIMARY KEY(tag_id, message_id),
 FOREIGN KEY(tag_id) REFERENCES tags(id) ON DELETE CASCADE,
 FOREIGN KEY(message_id) REFERENCES messages(id) ON DELETE CASCADE);

GRANT ALL PRIVILEGES ON DATABASE agqueue TO agqueue_user;
GRANT USAGE ON SCHEMA public to agqueue_user;
--Not needed, yet
GRANT ALL ON ALL SEQUENCES IN SCHEMA public TO agqueue_user;
GRANT ALL ON ALL TABLES IN SCHEMA public TO agqueue_user;

--This locks the next record in the DB, updates it to reflect that the record is active and returns the record
CREATE OR REPLACE PROCEDURE dequeue_message(
p_queue_id integer,
p_transaction_id integer
)
LANGUAGE plpgsql
AS $$
  DECLARE
    messageId int;
  begin
    START TRANSACTION;

    SELECT id into messageId /*, id, queue_id, transaction_id, transaction_action, state as MessageState, add_datetime, close_datetime, 
    priority, max_attempts, attempts, expiry_datetime, correlation_id, group_name, metadata, payload */
    FROM messages WHERE state = 1 /*Active*/ AND close_datetime IS NULL AND transaction_id IS NULL AND 
    queue_id = p_queue_id 
    ORDER BY priority DESC, add_datetime 
    FOR UPDATE SKIP LOCKED LIMIT 1; 

    IF messageId IS NOT NULL THEN 
      Update messages set state = 2 /*InTransaction*/, transaction_id = p_transaction_id, transaction_action = 2 /*Pull*/ 
      WHERE id = messageId;
    END IF; 

	SELECT * FROM messages where id = messageId;

    COMMIT;
  end
$$;
