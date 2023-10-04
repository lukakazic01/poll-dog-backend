

create table brands (
	id_brand varchar(255) not null primary key,
	brand varchar(255) not null
)

create table products (
	id_product varchar(255) not null primary key,
	product varchar(255) not null
)

create table ratings (
	id_rating varchar(255) not null primary key,
	star_rating int not null,
	comment varchar(255) not null
)

create table productDetails (
	id_brand varchar(255) not null,
	id_product varchar(255) not null,
	id_rating varchar(255) not null,
	PRIMARY KEY(id_brand, id_product, id_rating),
	FOREIGN KEY(id_brand) REFERENCES  brands(id_brand),
	FOREIGN KEY(id_product) REFERENCES  products(id_product),
	FOREIGN KEY(id_rating) REFERENCES  ratings(id_rating)

)


select * from brands
select * from products
select * from ratings
select * from productDetails