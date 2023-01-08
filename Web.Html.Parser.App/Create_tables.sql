CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE public.game
(
    id uuid NOT NULL,
    game_web_id integer NOT NULL,
    title text NOT NULL,
    PRIMARY KEY (id)
);

ALTER TABLE IF EXISTS public.game
    OWNER to postgres;

CREATE TABLE public.gameItem
(
    id uuid NOT NULL,
    game_id uuid NOT NULL,
    title text NOT NULL,
    PRIMARY KEY (id)
);

CREATE TABLE public.user
(
    id uuid NOT NULL,
    user_web_id integer NOT NULL,
    userName text,
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

CREATE TABLE public.itemPrice
(
    id uuid NOT NULL,
    item_id uuid NOT NULL,
	price double precision NOT NULL,
	count integer NOT NULL,
    date_time_update time with time zone NOT NULL,
    PRIMARY KEY (id)
);