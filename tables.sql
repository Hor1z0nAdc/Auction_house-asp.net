CREATE TABLE "felhasználó" (
	"id"	INTEGER UNIQUE,
	"email"	TEXT NOT NULL UNIQUE,
	"password"	TEXT NOT NULL,
	"salt"	TEXT NOT NULL,
	PRIMARY KEY("id" AUTOINCREMENT)
);

CREATE TABLE "árverés" (
	"id"	INTEGER UNIQUE,
	"leírás"	TEXT NOT NULL,
	"kikiáltási_ár"	INTEGER NOT NULL,
	"aktuális_ár"	INTEGER NOT NULL,
	"határidő"	NUMERIC NOT NULL,
	"kép"	TEXT NOT NULL,
	"név"	TEXT NOT NULL,
	"győztes"	TEXT,
	PRIMARY KEY("id" AUTOINCREMENT)
);

CREATE TABLE "licit" (
	"id"	INTEGER UNIQUE,
	"ajánlat"	INTEGER NOT NULL,
	"felhasználó_id"	INTEGER NOT NULL,
	"árverés_id"	INTEGER NOT NULL,
	FOREIGN KEY("felhasználó_id") REFERENCES felhasználó (id) ,
	FOREIGN KEY("árverés_id") REFERENCES árverés (id),
	PRIMARY KEY("id" AUTOINCREMENT)
);