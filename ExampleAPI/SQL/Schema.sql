-- DROP DATABASE IF EXISTS exampleapi;

CREATE DATABASE exampleapi
    WITH
    OWNER = exampleapi
    ENCODING = 'UTF8'
    LC_COLLATE = 'en_US.utf8'
    LC_CTYPE = 'en_US.utf8'
    TABLESPACE = pg_default
    CONNECTION LIMIT = -1;

-- Table: public.companies

-- DROP TABLE IF EXISTS public.companies;

CREATE TABLE IF NOT EXISTS public.companies
(
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),
    name character varying(255) COLLATE pg_catalog."default" NOT NULL,
    line1 character varying(255) COLLATE pg_catalog."default",
    line2 character varying(255) COLLATE pg_catalog."default",
    city character varying(255) COLLATE pg_catalog."default",
    state character varying(255) COLLATE pg_catalog."default",
    zip character varying(255) COLLATE pg_catalog."default",
    CONSTRAINT companies_pkey PRIMARY KEY (id)
)

TABLESPACE pg_default;

-- Table: public.company_events

-- DROP TABLE IF EXISTS public.company_events;

CREATE TABLE IF NOT EXISTS public.company_events
(
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),
    name character varying(255) COLLATE pg_catalog."default" NOT NULL,
    data json NOT NULL,
    "timestamp" timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT company_events_pkey PRIMARY KEY (id)
)

TABLESPACE pg_default;

-- Table: public.order_events

-- DROP TABLE IF EXISTS public.order_events;

CREATE TABLE IF NOT EXISTS public.order_events
(
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),
    name character varying(255) COLLATE pg_catalog."default" NOT NULL,
    data json NOT NULL,
    "timestamp" time with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT order_events_pkey PRIMARY KEY (id)
)

TABLESPACE pg_default;

-- Table: public.orders

-- DROP TABLE IF EXISTS public.orders;

CREATE TABLE IF NOT EXISTS public.orders
(
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),
    name character varying(255) COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT orders_pkey PRIMARY KEY (id)
)

TABLESPACE pg_default;

-- Table: public.ordereditems

-- DROP TABLE IF EXISTS public.ordereditems;

CREATE TABLE IF NOT EXISTS public.ordereditems
(
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),
    orderid integer NOT NULL,
    name character varying(255) COLLATE pg_catalog."default" NOT NULL,
    qty integer,
    CONSTRAINT orderitems_pkey PRIMARY KEY (id),
    CONSTRAINT order_ordereditems FOREIGN KEY (orderid)
        REFERENCES public.orders (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE CASCADE
        NOT VALID
)

TABLESPACE pg_default;

-- Index: fki_order_ordereditems

-- DROP INDEX IF EXISTS public.fki_order_ordereditems;

CREATE INDEX IF NOT EXISTS fki_order_ordereditems
    ON public.ordereditems USING btree
    (orderid ASC NULLS LAST)
    TABLESPACE pg_default;