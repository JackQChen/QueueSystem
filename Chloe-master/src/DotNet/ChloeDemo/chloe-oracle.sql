/*
Navicat Oracle Data Transfer
Oracle Client Version : 11.2.0.2.0

Source Server         : localhost_1521_ORCL
Source Server Version : 110200
Source Host           : localhost:1521
Source Schema         : SYSTEM

Target Server Type    : ORACLE
Target Server Version : 110200
File Encoding         : 65001

Date: 2016-09-05 09:27:33
*/


-- ----------------------------
-- Table structure for USERS
-- ----------------------------
DROP TABLE "SYSTEM"."USERS";
CREATE TABLE "SYSTEM"."USERS" (
"ID" NUMBER NOT NULL ,
"NAME" NVARCHAR2(255) NULL ,
"GENDER" NUMBER NULL ,
"AGE" NUMBER NULL ,
"CITYID" NUMBER NULL ,
"OPTIME" DATE NULL 
)
LOGGING
NOCOMPRESS
NOCACHE

;

-- ----------------------------
-- Indexes structure for table USERS
-- ----------------------------

-- ----------------------------
-- Checks structure for table USERS
-- ----------------------------
ALTER TABLE "SYSTEM"."USERS" ADD CHECK ("ID" IS NOT NULL);

-- ----------------------------
-- Primary Key structure for table USERS
-- ----------------------------
ALTER TABLE "SYSTEM"."USERS" ADD PRIMARY KEY ("ID");

-- ----------------------------
-- CREATE SEQUENCE for table USERS
-- ----------------------------
CREATE SEQUENCE  "SYSTEM"."USERS_AUTOID"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 21 CACHE 20 NOORDER  NOCYCLE;

-- ----------------------------
-- Table structure for PROVINCE
-- ----------------------------
DROP TABLE "SYSTEM"."PROVINCE";
CREATE TABLE "SYSTEM"."PROVINCE" (
"ID" NUMBER NOT NULL ,
"NAME" NVARCHAR2(255) NULL 
)
LOGGING
NOCOMPRESS
NOCACHE

;

-- ----------------------------
-- Table structure for CITY
-- ----------------------------
DROP TABLE "SYSTEM"."CITY";
CREATE TABLE "SYSTEM"."CITY" (
"ID" NUMBER NOT NULL ,
"NAME" NVARCHAR2(255) NULL ,
"PROVINCEID" NUMBER NULL 
)
LOGGING
NOCOMPRESS
NOCACHE

;
