/*
Navicat MySQL Data Transfer

Source Server         : Queue
Source Server Version : 50624
Source Host           : cysoft.uicp.net:3306
Source Database       : queuedb0208

Target Server Type    : MYSQL
Target Server Version : 50624
File Encoding         : 65001

Date: 2018-02-08 16:17:27
*/

ALTER table `t_appointment` add sysFlag INT comment '0:同步新增 1：同步修改 2：已同步 3：已删除' default 0;
ALTER TABLE `t_business` add sysFlag int COMMENT '0:同步新增 1：同步修改 2：已同步 3：已删除' default 0;
ALTER TABLE `t_businessattribute` add sysFlag int COMMENT '0:同步新增 1：同步修改 2：已同步 3：已删除' default 0;
ALTER TABLE `t_call` add sysFlag int COMMENT '0:同步新增 1：同步修改 2：已同步 3：已删除' default 0;
ALTER TABLE `t_dictionary` add sysFlag int COMMENT '0:同步新增 1：同步修改 2：已同步 3：已删除' default 0;
ALTER TABLE `t_employee` add sysFlag int COMMENT '0:同步新增 1：同步修改 2：已同步 3：已删除' default 0;
ALTER TABLE `t_evaluate` add sysFlag int COMMENT '0:同步新增 1：同步修改 2：已同步 3：已删除' default 0;
ALTER TABLE `t_getcard` add sysFlag int COMMENT '0:同步新增 1：同步修改 2：已同步 3：已删除' default 0;
ALTER TABLE `t_ledcontroller` add sysFlag int COMMENT '0:同步新增 1：同步修改 2：已同步 3：已删除' default 0;
ALTER TABLE `t_ledwindow` add sysFlag int COMMENT '0:同步新增 1：同步修改 2：已同步 3：已删除' default 0;
ALTER TABLE `t_lineupmaxno` add sysFlag int COMMENT '0:同步新增 1：同步修改 2：已同步 3：已删除' default 0;
ALTER TABLE `t_opratelog` add sysFlag int COMMENT '0:同步新增 1：同步修改 2：已同步 3：已删除' default 0;
ALTER TABLE `t_queue` add sysFlag int COMMENT '0:同步新增 1：同步修改 2：已同步 3：已删除' default 0;
ALTER TABLE `t_register` add sysFlag int COMMENT '0:同步新增 1：同步修改 2：已同步 3：已删除' default 0;
ALTER TABLE `t_unit` add sysFlag int COMMENT '0:同步新增 1：同步修改 2：已同步 3：已删除' default 0;
ALTER TABLE `t_user` add sysFlag int COMMENT '0:同步新增 1：同步修改 2：已同步 3：已删除' default 0;
ALTER TABLE `t_window` add sysFlag int COMMENT '0:同步新增 1：同步修改 2：已同步 3：已删除' default 0;
ALTER TABLE `t_windowarea` add sysFlag int COMMENT '0:同步新增 1：同步修改 2：已同步 3：已删除' default 0;
ALTER TABLE `t_windowbusiness` add sysFlag int COMMENT '0:同步新增 1：同步修改 2：已同步 3：已删除' default 0;
ALTER TABLE `t_windowuser` add sysFlag int COMMENT '0:同步新增 1：同步修改 2：已同步 3：已删除' default 0;


ALTER table `t_appointment` add areaCode INT comment '区域编号' default 0;
ALTER TABLE `t_business` add areaCode int COMMENT '区域编号' default 0;
ALTER TABLE `t_businessattribute` add areaCode int COMMENT '区域编号' default 0;
ALTER TABLE `t_call` add areaCode int COMMENT '区域编号' default 0;
ALTER TABLE `t_dictionary` add areaCode int COMMENT '区域编号' default 0;
ALTER TABLE `t_employee` add areaCode int COMMENT '区域编号' default 0;
ALTER TABLE `t_evaluate` add areaCode int COMMENT '区域编号' default 0;
ALTER TABLE `t_getcard` add areaCode int COMMENT '区域编号' default 0;
ALTER TABLE `t_ledcontroller` add areaCode int COMMENT '区域编号' default 0;
ALTER TABLE `t_ledwindow` add areaCode int COMMENT '区域编号' default 0;
ALTER TABLE `t_lineupmaxno` add areaCode int COMMENT '区域编号' default 0;
ALTER TABLE `t_opratelog` add areaCode int COMMENT '区域编号' default 0;
ALTER TABLE `t_queue` add areaCode int COMMENT '区域编号' default 0;
ALTER TABLE `t_register` add areaCode int COMMENT '区域编号' default 0;
ALTER TABLE `t_unit` add areaCode int COMMENT '区域编号' default 0;
ALTER TABLE `t_user` add areaCode int COMMENT '区域编号' default 0;
ALTER TABLE `t_window` add areaCode int COMMENT '区域编号' default 0;
ALTER TABLE `t_windowarea` add areaCode int COMMENT '区域编号' default 0;
ALTER TABLE `t_windowbusiness` add areaCode int COMMENT '区域编号' default 0;
ALTER TABLE `t_windowuser` add areaCode int COMMENT '区域编号' default 0;


ALTER table `t_appointment` add areaId INT comment '编号' default 0;
ALTER TABLE `t_business` add areaId int COMMENT '编号' default 0;
ALTER TABLE `t_businessattribute` add areaId int COMMENT '编号' default 0;
ALTER TABLE `t_call` add areaId int COMMENT '编号' default 0;
ALTER TABLE `t_dictionary` add areaId int COMMENT '编号' default 0;
ALTER TABLE `t_employee` add areaId int COMMENT '编号' default 0;
ALTER TABLE `t_evaluate` add areaId int COMMENT '编号' default 0;
ALTER TABLE `t_getcard` add areaId int COMMENT '编号' default 0;
ALTER TABLE `t_ledcontroller` add areaId int COMMENT '编号' default 0;
ALTER TABLE `t_ledwindow` add areaId int COMMENT '编号' default 0;
ALTER TABLE `t_lineupmaxno` add areaId int COMMENT '编号' default 0;
ALTER TABLE `t_opratelog` add areaId int COMMENT '编号' default 0;
ALTER TABLE `t_queue` add areaId int COMMENT '编号' default 0;
ALTER TABLE `t_register` add areaId int COMMENT '编号' default 0;
ALTER TABLE `t_unit` add areaId int COMMENT '编号' default 0;
ALTER TABLE `t_user` add areaId int COMMENT '编号' default 0;
ALTER TABLE `t_window` add areaId int COMMENT '编号' default 0;
ALTER TABLE `t_windowarea` add areaId int COMMENT '编号' default 0;
ALTER TABLE `t_windowbusiness` add areaId int COMMENT '编号' default 0;
ALTER TABLE `t_windowuser` add areaId int COMMENT '编号' default 0;


update  `t_appointment` set sysFlag=0;
update  `t_business` set sysFlag =0;
update  `t_businessattribute` set sysFlag =0;
update  `t_call` set sysFlag =0;
update  `t_dictionary` set sysFlag =0;
update  `t_employee` set sysFlag =0;
update  `t_evaluate`  set sysFlag =0;
update  `t_getcard`  set sysFlag =0;
update  `t_ledcontroller`  set sysFlag =0;
update  `t_ledwindow` set sysFlag =0;
update  `t_lineupmaxno` set sysFlag =0;
update  `t_opratelog` set sysFlag =0;
update `t_queue` set sysFlag =0;
update  `t_register` set sysFlag =0;
update  `t_unit` set sysFlag =0;
update  `t_user`  set sysFlag =0;
update  `t_window`  set sysFlag =0;
update  `t_windowarea`  set sysFlag =0;
update  `t_windowbusiness`  set sysFlag =0;
update  `t_windowuser` set sysFlag =0;


--添加删除记录存储表
/*
Navicat MySQL Data Transfer

Source Server         : Queue
Source Server Version : 50624
Source Host           : cysoft.uicp.net:3306
Source Database       : queuedb0208

Target Server Type    : MYSQL
Target Server Version : 50624
File Encoding         : 65001

Date: 2018-02-08 16:17:27
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for `t_delete`
-- ----------------------------
DROP TABLE IF EXISTS `t_delete`;
CREATE TABLE `t_delete` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `tableName` varchar(255) DEFAULT NULL COMMENT '表名称',
  `keyId` int(11) DEFAULT NULL COMMENT '主键id',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of t_delete
-- ----------------------------


