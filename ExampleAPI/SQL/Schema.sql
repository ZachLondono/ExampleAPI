CREATE TABLE public.companies (
    id uuid NOT NULL,
    name character varying(255) NOT NULL,
    line1 character varying(255) DEFAULT ''::character varying NOT NULL,
    line2 character varying(255) DEFAULT ''::character varying NOT NULL,
    city character varying(255) DEFAULT ''::character varying NOT NULL,
    state character varying(255) DEFAULT ''::character varying NOT NULL,
    zip character varying(255) DEFAULT ''::character varying NOT NULL,
    CONSTRAINT companies_pkey PRIMARY KEY (id)
);

CREATE TABLE public.events (
    id uuid NOT NULL,
    streamid uuid NOT NULL,
    version integer NOT NULL,
    correlationid uuid,
    "timestamp" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    type character varying(255) NOT NULL,
    data json NOT NULL,
    CONSTRAINT events_pkey PRIMARY KEY (id)
);

CREATE TABLE public.orders (
    id uuid NOT NULL,
    name character varying(255) NOT NULL,
    CONSTRAINT orders_pkey PRIMARY KEY (id)
);

CREATE TABLE public.ordereditems (
    id uuid NOT NULL,
    orderid uuid NOT NULL,
    name character varying(255) NOT NULL,
    qty integer NOT NULL,
    CONSTRAINT ordereditems_pkey PRIMARY KEY (id),
    CONSTRAINT order_ordereditems FOREIGN KEY (orderid)
        REFERENCES public.orders (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE CASCADE
        NOT VALID
);