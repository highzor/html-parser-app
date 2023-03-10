CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE public.game
(
    id uuid NOT NULL,
    game_web_id integer NOT NULL,
    title text NOT NULL,
    PRIMARY KEY (id)
);

CREATE TABLE public.game_item
(
    id uuid NOT NULL,
    game_id uuid NOT NULL,
    title text NOT NULL,
    PRIMARY KEY (id)
);

CREATE TABLE public.users
(
    id uuid NOT NULL,
    user_web_id integer NOT NULL,
    user_name text,
    PRIMARY KEY (id)
);

CREATE TABLE public.item
(
    id uuid NOT NULL,
    seller_id uuid NOT NULL,
	game_id uuid NOT NULL,
    description text NOT NULL,
    PRIMARY KEY (id)
);

CREATE TABLE public.item_price
(
    id uuid NOT NULL,
    item_id uuid NOT NULL,
	price double precision NOT NULL,
	count integer NOT NULL,
    is_single boolean NOT NULL,
    date_time_update timestamp with time zone NOT NULL,
    PRIMARY KEY (id)
);