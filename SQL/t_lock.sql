/*
Navicat MySQL Data Transfer

Source Server         : conn
Source Server Version : 50624
Source Host           : cysoft.uicp.net:3306
Source Database       : QueueDB

Target Server Type    : MYSQL
Target Server Version : 50624
File Encoding         : 65001

Date: 2018-03-13 15:48:21
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for `t_lock`
-- ----------------------------
DROP TABLE IF EXISTS `t_lock`;
CREATE TABLE `t_lock` (
  `key` varchar(255) NOT NULL DEFAULT '0',
  PRIMARY KEY (`key`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of t_lock
-- ----------------------------
INSERT INTO `t_lock` VALUES ('Call');
INSERT INTO `t_lock` VALUES ('Queue');
