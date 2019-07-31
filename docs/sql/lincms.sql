/*
 Navicat Premium Data Transfer

 Source Server         : localhost
 Source Server Type    : MySQL
 Source Server Version : 50725
 Source Host           : localhost:3306
 Source Schema         : lincms

 Target Server Type    : MySQL
 Target Server Version : 50725
 File Encoding         : 65001

 Date: 31/07/2019 19:52:00
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for book
-- ----------------------------
DROP TABLE IF EXISTS `book`;
CREATE TABLE `book`  (
  `author` varchar(30) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `image` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `summary` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `title` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `create_user_id` bigint(20) NULL DEFAULT NULL,
  `create_time` datetime(3) NULL,
  `is_deleted` bit(1) NOT NULL,
  `delete_user_id` bigint(20) NULL DEFAULT NULL,
  `delete_time` datetime(3) NULL DEFAULT NULL,
  `update_user_id` bigint(20) NULL DEFAULT NULL,
  `update_time` datetime(3) NULL,
  `id` int(11) NOT NULL AUTO_INCREMENT,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 3 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of book
-- ----------------------------
INSERT INTO `book` VALUES ('1221', '1', '2', '3', NULL, '0001-01-01 00:00:00.000', b'0', NULL, NULL, NULL, '0001-01-01 00:00:00.000', 2);

-- ----------------------------
-- Table structure for lin_auth
-- ----------------------------
DROP TABLE IF EXISTS `lin_auth`;
CREATE TABLE `lin_auth`  (
  `group_id` int(11) NOT NULL,
  `auth` varchar(60) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `module` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `id` int(11) NOT NULL AUTO_INCREMENT,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 33 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of lin_auth
-- ----------------------------
INSERT INTO `lin_auth` VALUES (2, '查询日志记录的用户', '日志', 20);
INSERT INTO `lin_auth` VALUES (2, '搜索日志', '日志', 21);
INSERT INTO `lin_auth` VALUES (2, '删除图书', '图书', 22);
INSERT INTO `lin_auth` VALUES (2, '查看lin的信息', '信息', 23);
INSERT INTO `lin_auth` VALUES (2, '查询所有日志', '日志', 24);
INSERT INTO `lin_auth` VALUES (15, '删除图书', '图书', 32);

-- ----------------------------
-- Table structure for lin_file
-- ----------------------------
DROP TABLE IF EXISTS `lin_file`;
CREATE TABLE `lin_file`  (
  `extension` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `md5` varchar(40) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `name` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `path` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `size` int(11) NULL DEFAULT NULL,
  `type` smallint(6) NULL DEFAULT NULL,
  `create_user_id` bigint(20) NULL DEFAULT NULL,
  `create_time` datetime(3) NULL,
  `is_deleted` bit(1) NOT NULL,
  `delete_user_id` bigint(20) NULL DEFAULT NULL,
  `delete_time` datetime(3) NULL DEFAULT NULL,
  `update_user_id` bigint(20) NULL DEFAULT NULL,
  `update_time` datetime(3) NULL,
  `id` int(11) NOT NULL AUTO_INCREMENT,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 7 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of lin_file
-- ----------------------------
INSERT INTO `lin_file` VALUES ('.png', '87cd112413b9979753ac7119c9266b5f', '1555844046442-84200e65-7dac-452c-83db-30ea73317782.png', '2019/07/31/61fd9345-052b-4c76-bbfc-b168161ab983.png', 1936251, 1, NULL, '2019-07-31 09:13:29.833', b'0', NULL, NULL, NULL, '0001-01-01 00:00:00.000', 1);
INSERT INTO `lin_file` VALUES ('.jpg', 'fd4cdd5d931a430975c51ed0ba34012e', 'avatar.jpg', '2019/07/31/5f533562-0614-4f6f-9cc3-cc62d919cdc1.jpg', 3195, 1, NULL, '2019-07-31 09:14:35.275', b'0', NULL, NULL, NULL, '0001-01-01 00:00:00.000', 2);
INSERT INTO `lin_file` VALUES ('.jpg', '7aa613b4b32544141120d3027a8606b9', 'avatar.jpg', '2019/07/31/c706747b-a360-434c-b50f-8fa79bfc2818.jpg', 6468, 1, NULL, '2019-07-31 09:59:21.547', b'0', NULL, NULL, NULL, '0001-01-01 00:00:00.000', 3);
INSERT INTO `lin_file` VALUES ('.jpg', 'c6166852e4be8657f5d41560da6103d2', 'avatar.jpg', '2019/07/31/0f40818c-ddb1-4c48-9e09-6332357a38df.jpg', 11641, 1, NULL, '2019-07-31 10:00:32.578', b'0', NULL, NULL, NULL, '0001-01-01 00:00:00.000', 4);
INSERT INTO `lin_file` VALUES ('.jpg', '3feb060714c5c18d80eddd7947396ec5', 'avatar.jpg', '2019/07/31/40c33485-9e30-435e-8664-76ea1ad579b0.jpg', 4346, 1, NULL, '2019-07-31 10:11:27.167', b'0', NULL, NULL, NULL, '0001-01-01 00:00:00.000', 5);
INSERT INTO `lin_file` VALUES ('.png', 'fab966fc5a5a9f6938402b3c81b6d90a', '1555843960160-99ea3f39-f119-40ee-b89a-97e1548cbfb8.png', '2019/07/31/05d32e3f-40fd-4b1e-b5c1-35bbe1f01c8f.png', 1525724, 1, NULL, '2019-07-31 12:06:53.904', b'0', NULL, NULL, NULL, '0001-01-01 00:00:00.000', 6);

-- ----------------------------
-- Table structure for lin_group
-- ----------------------------
DROP TABLE IF EXISTS `lin_group`;
CREATE TABLE `lin_group`  (
  `name` varchar(60) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `info` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `id` int(11) NOT NULL AUTO_INCREMENT,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 16 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of lin_group
-- ----------------------------
INSERT INTO `lin_group` VALUES ('管理员分组', '管理员分组', 2);
INSERT INTO `lin_group` VALUES ('图书分组管理', '图书分组管理', 15);

-- ----------------------------
-- Table structure for lin_log
-- ----------------------------
DROP TABLE IF EXISTS `lin_log`;
CREATE TABLE `lin_log`  (
  `authority` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `message` varchar(450) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `method` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `path` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `status_code` int(11) NULL DEFAULT NULL,
  `time` datetime(0) NULL DEFAULT NULL,
  `user_id` int(11) NOT NULL,
  `user_name` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `other_message` varchar(5000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 813 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of lin_log
-- ----------------------------
INSERT INTO `lin_log` VALUES ('', '', 'GET', '/cms/admin/authority', 200, '2019-07-28 22:23:39', 1, 'admin', 384, '参数：{}\n耗时：3809.8604 毫秒');
INSERT INTO `lin_log` VALUES ('', '', 'GET', '/cms/test/get', 200, '2019-07-28 22:24:55', 0, NULL, 385, '参数：{}\n耗时：10.9577 毫秒');
INSERT INTO `lin_log` VALUES ('', '', 'GET', '/cms/test/get', 200, '2019-07-28 22:24:58', 0, NULL, 386, '参数：{}\n耗时：11.5204 毫秒');
INSERT INTO `lin_log` VALUES ('', '', 'GET', '/cms/test/get', 200, '2019-07-28 22:25:05', 0, NULL, 387, '参数：{}\n耗时：11.4711 毫秒');
INSERT INTO `lin_log` VALUES ('', '', 'GET', '/cms/test/get', 200, '2019-07-28 22:25:12', 0, NULL, 388, '参数：{}\n耗时：11.8337 毫秒');
INSERT INTO `lin_log` VALUES ('', '', 'GET', '/cms/test/get', 200, '2019-07-28 22:25:15', 0, NULL, 389, '参数：{}\n耗时：12.43 毫秒');
INSERT INTO `lin_log` VALUES ('', '', 'GET', '/cms/test/get', 200, '2019-07-28 22:25:18', 0, NULL, 390, '参数：{}\n耗时：12.1163 毫秒');
INSERT INTO `lin_log` VALUES ('', '', 'GET', '/cms/test/get', 200, '2019-07-28 22:25:23', 0, NULL, 391, '参数：{}\n耗时：11.2141 毫秒');
INSERT INTO `lin_log` VALUES ('', '', 'GET', '/cms/test/get', 200, '2019-07-28 22:25:43', 0, NULL, 392, '参数：{}\n耗时：12.1327 毫秒');
INSERT INTO `lin_log` VALUES ('', 'LinCms.Zero.Security.CurrentUser', 'GET', '/cms/test/get', 200, '2019-07-28 22:26:30', 0, NULL, 393, '参数：{}\n耗时：10.8117 毫秒');
INSERT INTO `lin_log` VALUES ('', '', 'GET', '/cms/test/get', 200, '2019-07-28 22:26:32', 0, NULL, 394, '参数：{}\n耗时：10.7415 毫秒');
INSERT INTO `lin_log` VALUES ('', '', 'GET', '/cms/test/get', 200, '2019-07-28 22:27:12', 0, NULL, 395, '参数：{}\n耗时：12.9085 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问$', 'GET', '/cms/test/get', 200, '2019-07-28 22:27:15', 0, NULL, 396, '参数：{}\n耗时：13.4618 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/test/get,', 'GET', '/cms/test/get', 200, '2019-07-28 22:27:18', 0, NULL, 397, '参数：{}\n耗时：11.5529 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/test/get,', 'GET', '/cms/test/get', 200, '2019-07-28 22:27:21', 0, NULL, 398, '参数：{}\n耗时：10.8548 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/test/get,耗时：11.4616 毫秒', 'GET', '/cms/test/get', 200, '2019-07-28 22:27:36', 0, NULL, 399, '参数：{}\n耗时：11.4616 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/test/get,耗时：{Stopwatch.Elapsed.TotalMilliseconds} 毫秒', 'GET', '/cms/test/get', 200, '2019-07-28 22:27:38', 0, NULL, 400, '参数：{}\n耗时：12.4138 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/test/get,耗时：11.1537 毫秒', 'GET', '/cms/test/get', 200, '2019-07-28 22:27:42', 0, NULL, 401, '参数：{}\n耗时：11.1537 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/test/get,耗时：11.0256 毫秒\n', 'GET', '/cms/test/get', 200, '2019-07-28 22:28:09', 0, NULL, 402, '参数：{}\n耗时：11.0256 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/test/get,耗时：21.2369 毫秒', 'GET', '/cms/test/get', 200, '2019-07-28 22:28:17', 0, NULL, 403, '参数：{}\n耗时：21.2369 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/test/get,耗时：10.9626 毫秒', 'GET', '/cms/test/get', 200, '2019-07-28 22:28:20', 0, NULL, 404, '参数：{}\n耗时：10.9626 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/test/get,耗时：11.2875 毫秒', 'GET', '/cms/test/get', 200, '2019-07-28 22:29:58', 0, NULL, 405, '参数：{}\n耗时：11.2875 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/test/get,耗时：10.7802 毫秒', 'GET', '/cms/test/get', 200, '2019-07-28 22:30:10', 0, NULL, 406, '参数：{}\n耗时：10.7802 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/test/get,耗时：10.9919 毫秒', 'GET', '/cms/test/get', 200, '2019-07-28 22:30:23', 0, NULL, 407, '参数：{}\n耗时：10.9919 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/user/login,耗时：615.349 毫秒', 'POST', '/cms/user/login', 200, '2019-07-28 22:40:05', 0, NULL, 408, '参数：{\"loginInputDto\":{\"Nickname\":\"admin\",\"Password\":\"123qwe\"}}\n耗时：615.349 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/user/login,耗时：159.182 毫秒', 'POST', '/cms/user/login', 200, '2019-07-28 22:40:30', 0, NULL, 409, '参数：{\"loginInputDto\":{\"Nickname\":\"admin\",\"Password\":\"123qwe\"}}\n耗时：159.182 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/user/login,耗时：134.772 毫秒', 'POST', '/cms/user/login', 200, '2019-07-28 22:40:41', 0, NULL, 410, '参数：{\"loginInputDto\":{\"Nickname\":\"admin\",\"Password\":\"123qwe\"}}\n耗时：134.772 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/user/login,耗时：122.1026 毫秒', 'POST', '/cms/user/login', 200, '2019-07-28 22:40:49', 0, NULL, 411, '参数：{\"loginInputDto\":{\"Nickname\":\"admin\",\"Password\":\"123qwe\"}}\n耗时：122.1026 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/test/get,耗时：12.6786 毫秒', 'GET', '/cms/test/get', 200, '2019-07-28 22:43:20', 0, NULL, 412, '参数：{}\n耗时：12.6786 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/test/get,耗时：18.0426 毫秒', 'GET', '/cms/test/get', 200, '2019-07-28 22:43:28', 0, NULL, 413, '参数：{}\n耗时：18.0426 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/test/get,耗时：11.8709 毫秒', 'GET', '/cms/test/get', 200, '2019-07-28 22:43:36', 0, NULL, 414, '参数：{}\n耗时：11.8709 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/user/login,耗时：4897.2734 毫秒', 'POST', '/cms/user/login', 200, '2019-07-28 22:43:44', 0, NULL, 415, '参数：{\"loginInputDto\":{\"Nickname\":\"super\",\"Password\":\"123456\"}}\n耗时：4897.2734 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/test/get,耗时：12.3414 毫秒', 'GET', '/cms/test/get', 200, '2019-07-28 22:43:49', 0, NULL, 416, '参数：{}\n耗时：12.3414 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/user/login,耗时：782.2965 毫秒', 'POST', '/cms/user/login', 200, '2019-07-28 22:43:53', 0, NULL, 417, '参数：{\"loginInputDto\":{\"Nickname\":\"super\",\"Password\":\"123456\"}}\n耗时：782.2965 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/user/login,耗时：347.5534 毫秒', 'POST', '/cms/user/login', 200, '2019-07-28 22:44:12', 0, NULL, 418, '参数：{\"loginInputDto\":{\"Nickname\":\"admin\",\"Password\":\"123qwe\"}}\n耗时：347.5534 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/user/auths,耗时：26.7351 毫秒', 'GET', '/cms/user/auths', 200, '2019-07-28 22:44:12', 1, 'admin', 419, '参数：{}\n耗时：26.7351 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/test/get,耗时：11.6134 毫秒', 'GET', '/cms/test/get', 200, '2019-07-28 22:46:30', 0, NULL, 420, '参数：{}\n耗时：11.6134 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/user/login,耗时：1258.4913 毫秒', 'POST', '/cms/user/login', 200, '2019-07-28 22:50:16', 0, NULL, 421, '参数：{\"loginInputDto\":{\"Nickname\":\"super\",\"Password\":\"123456\"}}\n耗时：1258.4913 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/user/login,耗时：709.3049 毫秒', 'POST', '/cms/user/login', 200, '2019-07-28 22:50:56', 0, NULL, 422, '参数：{\"loginInputDto\":{\"Nickname\":\"super\",\"Password\":\"123456\"}}\n耗时：709.3049 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/user/login,耗时：304.8782 毫秒', 'POST', '/cms/user/login', 200, '2019-07-28 22:51:32', 0, NULL, 423, '参数：{\"loginInputDto\":{\"Nickname\":\"admin\",\"Password\":\"123qwe\"}}\n耗时：304.8782 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/user/auths,耗时：45.9579 毫秒', 'GET', '/cms/user/auths', 200, '2019-07-28 22:51:32', 1, 'admin', 424, '参数：{}\n耗时：45.9579 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/all,耗时：56.6112 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-28 22:51:40', 1, 'admin', 425, '参数：{}\n耗时：56.6112 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/users,耗时：49.4258 毫秒', 'GET', '/cms/admin/users', 200, '2019-07-28 22:51:43', 1, 'admin', 426, '参数：{\"searchDto\":{\"GroupId\":null,\"Count\":10,\"Page\":0}}\n耗时：49.4258 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/all,耗时：31.0467 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-28 22:51:43', 1, 'admin', 427, '参数：{}\n耗时：31.0467 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/all,耗时：30.9453 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-28 22:51:51', 1, 'admin', 428, '参数：{}\n耗时：30.9453 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/all,耗时：29.8119 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-28 22:51:54', 1, 'admin', 429, '参数：{}\n耗时：29.8119 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/2,耗时：192.2461 毫秒', 'DELETE', '/cms/admin/group/2', 200, '2019-07-28 22:51:58', 1, 'admin', 430, '参数：{\"id\":2}\n耗时：192.2461 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/users,耗时：36.1862 毫秒', 'GET', '/cms/admin/users', 200, '2019-07-28 22:52:02', 1, 'admin', 431, '参数：{\"searchDto\":{\"GroupId\":null,\"Count\":10,\"Page\":0}}\n耗时：36.1862 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/all,耗时：34.7599 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-28 22:52:02', 1, 'admin', 432, '参数：{}\n耗时：34.7599 毫秒');
INSERT INTO `lin_log` VALUES ('', '操作成功', 'PUT', '/cms/admin/1', 200, '2019-07-28 22:52:11', 1, 'admin', 433, '参数：{\"id\":1,\"updateUserDto\":{\"Email\":\"\",\"GroupId\":2}}\n耗时：77.8369 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/users,耗时：37.6288 毫秒', 'GET', '/cms/admin/users', 200, '2019-07-28 22:52:11', 1, 'admin', 434, '参数：{\"searchDto\":{\"GroupId\":null,\"Count\":10,\"Page\":0}}\n耗时：37.6288 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/all,耗时：29.463 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-28 22:52:15', 1, 'admin', 435, '参数：{}\n耗时：29.463 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/authority,耗时：109.1647 毫秒', 'GET', '/cms/admin/authority', 200, '2019-07-28 22:52:16', 1, 'admin', 436, '参数：{}\n耗时：109.1647 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group,耗时：93.5976 毫秒', 'POST', '/cms/admin/group', 200, '2019-07-28 22:52:27', 1, 'admin', 437, '参数：{\"inputDto\":{\"Auths\":[\"查询日志记录的用户\",\"查询所有日志\",\"查看lin的信息\"],\"Name\":\"分组1\",\"Info\":\"分组描1\"}}\n耗时：93.5976 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group,耗时：173.4271 毫秒', 'POST', '/cms/admin/group', 200, '2019-07-28 22:52:36', 1, 'admin', 438, '参数：{\"inputDto\":{\"Auths\":[\"查询日志记录的用户\",\"查询所有日志\",\"查看lin的信息\"],\"Name\":\"分组1\",\"Info\":\"分组描1\"}}\n耗时：173.4271 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/all,耗时：31.0141 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-28 22:52:43', 1, 'admin', 439, '参数：{}\n耗时：31.0141 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/authority,耗时：32.7209 毫秒', 'GET', '/cms/admin/authority', 200, '2019-07-28 22:52:45', 1, 'admin', 440, '参数：{}\n耗时：32.7209 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/13,耗时：58.7698 毫秒', 'GET', '/cms/admin/group/13', 200, '2019-07-28 22:52:45', 1, 'admin', 441, '参数：{\"id\":13}\n耗时：58.7698 毫秒');
INSERT INTO `lin_log` VALUES ('', '删除分组成功', 'DELETE', '/cms/admin/group/13', 200, '2019-07-28 22:54:04', 1, 'admin', 442, '参数：{\"id\":13}\n耗时：373.3465 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/all,耗时：51.1691 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-28 22:54:05', 1, 'admin', 443, '参数：{}\n耗时：51.1691 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/authority,耗时：113.5224 毫秒', 'GET', '/cms/admin/authority', 200, '2019-07-28 22:54:08', 1, 'admin', 444, '参数：{}\n耗时：113.5224 毫秒');
INSERT INTO `lin_log` VALUES ('', '新建分组成功', 'POST', '/cms/admin/group', 200, '2019-07-28 22:54:11', 1, 'admin', 445, '参数：{\"inputDto\":{\"Auths\":[\"查询日志记录的用户\"],\"Name\":\"1\",\"Info\":\"11\"}}\n耗时：71.0026 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/all,耗时：90.2991 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-28 22:54:12', 1, 'admin', 446, '参数：{}\n耗时：90.2991 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/authority,耗时：73.5951 毫秒', 'GET', '/cms/admin/authority', 200, '2019-07-28 22:54:12', 1, 'admin', 447, '参数：{}\n耗时：73.5951 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/all,耗时：36.8903 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-28 22:54:12', 1, 'admin', 448, '参数：{}\n耗时：36.8903 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/all,耗时：38.4594 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-28 22:54:12', 1, 'admin', 449, '参数：{}\n耗时：38.4594 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/all,耗时：36.211 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-28 22:54:12', 1, 'admin', 450, '参数：{}\n耗时：36.211 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/all,耗时：35.1485 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-28 22:54:12', 1, 'admin', 451, '参数：{}\n耗时：35.1485 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/authority,耗时：34.108 毫秒', 'GET', '/cms/admin/authority', 200, '2019-07-28 22:54:13', 1, 'admin', 452, '参数：{}\n耗时：34.108 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/14,耗时：58.6536 毫秒', 'GET', '/cms/admin/group/14', 200, '2019-07-28 22:54:13', 1, 'admin', 453, '参数：{\"id\":14}\n耗时：58.6536 毫秒');
INSERT INTO `lin_log` VALUES ('', '更新分组成功', 'PUT', '/cms/admin/group/14', 200, '2019-07-28 22:54:17', 1, 'admin', 454, '参数：{\"id\":14,\"updateGroupDto\":{\"Name\":\"1\",\"Info\":\"112\"}}\n耗时：59.0299 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/all,耗时：36.8737 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-28 22:54:17', 1, 'admin', 455, '参数：{}\n耗时：36.8737 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/authority,耗时：35.0456 毫秒', 'GET', '/cms/admin/authority', 200, '2019-07-28 22:54:19', 1, 'admin', 456, '参数：{}\n耗时：35.0456 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/14,耗时：46.1827 毫秒', 'GET', '/cms/admin/group/14', 200, '2019-07-28 22:54:19', 1, 'admin', 457, '参数：{\"id\":14}\n耗时：46.1827 毫秒');
INSERT INTO `lin_log` VALUES ('', '添加权限成功', 'POST', '/cms/admin/dispatch/patch', 200, '2019-07-28 22:54:21', 1, 'admin', 458, '参数：{\"authDto\":{\"GroupId\":14,\"Auths\":[\"查询日志记录的用户\"]}}\n耗时：38.3085 毫秒');
INSERT INTO `lin_log` VALUES ('', '删除权限成功', 'POST', '/cms/admin/remove', 200, '2019-07-28 22:54:21', 1, 'admin', 459, '参数：{\"removeAuthDto\":{\"GroupId\":14,\"Auths\":[null,null,null,null]}}\n耗时：46.9656 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/authority,耗时：36.7741 毫秒', 'GET', '/cms/admin/authority', 200, '2019-07-28 22:54:26', 1, 'admin', 460, '参数：{}\n耗时：36.7741 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/14,耗时：45.1984 毫秒', 'GET', '/cms/admin/group/14', 200, '2019-07-28 22:54:26', 1, 'admin', 461, '参数：{\"id\":14}\n耗时：45.1984 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/all,耗时：263.4286 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-28 23:12:50', 1, 'admin', 462, '参数：{}\n耗时：263.4286 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/authority,耗时：110.7504 毫秒', 'GET', '/cms/admin/authority', 200, '2019-07-28 23:12:52', 1, 'admin', 463, '参数：{}\n耗时：110.7504 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/2,耗时：121.5813 毫秒', 'GET', '/cms/admin/group/2', 200, '2019-07-28 23:12:53', 1, 'admin', 464, '参数：{\"id\":2}\n耗时：121.5813 毫秒');
INSERT INTO `lin_log` VALUES ('', '添加权限成功', 'POST', '/cms/admin/dispatch/patch', 200, '2019-07-28 23:12:56', 1, 'admin', 465, '参数：{\"authDto\":{\"GroupId\":2,\"Auths\":[\"查询日志记录的用户\"]}}\n耗时：39.3414 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/authority,耗时：37.0975 毫秒', 'GET', '/cms/admin/authority', 200, '2019-07-28 23:12:58', 1, 'admin', 466, '参数：{}\n耗时：37.0975 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/2,耗时：60.7557 毫秒', 'GET', '/cms/admin/group/2', 200, '2019-07-28 23:12:58', 1, 'admin', 467, '参数：{\"id\":2}\n耗时：60.7557 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/all,耗时：260.7088 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-28 23:15:29', 1, 'admin', 468, '参数：{}\n耗时：260.7088 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/authority,耗时：119.8949 毫秒', 'GET', '/cms/admin/authority', 200, '2019-07-28 23:16:30', 1, 'admin', 469, '参数：{}\n耗时：119.8949 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/2,耗时：105.7163 毫秒', 'GET', '/cms/admin/group/2', 200, '2019-07-28 23:16:30', 1, 'admin', 470, '参数：{\"id\":2}\n耗时：105.7163 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/all,耗时：42.7325 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-28 23:16:35', 1, 'admin', 471, '参数：{}\n耗时：42.7325 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/authority,耗时：33.071 毫秒', 'GET', '/cms/admin/authority', 200, '2019-07-28 23:16:37', 1, 'admin', 472, '参数：{}\n耗时：33.071 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/14,耗时：43.8178 毫秒', 'GET', '/cms/admin/group/14', 200, '2019-07-28 23:16:38', 1, 'admin', 473, '参数：{\"id\":14}\n耗时：43.8178 毫秒');
INSERT INTO `lin_log` VALUES ('', '添加权限成功', 'POST', '/cms/admin/dispatch/patch', 200, '2019-07-28 23:16:40', 1, 'admin', 474, '参数：{\"authDto\":{\"GroupId\":14,\"Auths\":[\"查询所有日志\"]}}\n耗时：38.221 毫秒');
INSERT INTO `lin_log` VALUES ('', '删除权限成功', 'POST', '/cms/admin/remove', 200, '2019-07-28 23:16:40', 1, 'admin', 475, '参数：{\"removeAuthDto\":{\"GroupId\":14,\"Auths\":[\"查询日志记录的用户\"]}}\n耗时：34.152 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/authority,耗时：34.3289 毫秒', 'GET', '/cms/admin/authority', 200, '2019-07-28 23:16:42', 1, 'admin', 476, '参数：{}\n耗时：34.3289 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/14,耗时：44.2974 毫秒', 'GET', '/cms/admin/group/14', 200, '2019-07-28 23:16:42', 1, 'admin', 477, '参数：{\"id\":14}\n耗时：44.2974 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/authority,耗时：36.3868 毫秒', 'GET', '/cms/admin/authority', 200, '2019-07-28 23:16:48', 1, 'admin', 478, '参数：{}\n耗时：36.3868 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/14,耗时：40.5397 毫秒', 'GET', '/cms/admin/group/14', 200, '2019-07-28 23:16:48', 1, 'admin', 479, '参数：{\"id\":14}\n耗时：40.5397 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/all,耗时：325.0164 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-29 00:00:15', 1, 'admin', 480, '参数：{}\n耗时：325.0164 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/authority,耗时：128.6998 毫秒', 'GET', '/cms/admin/authority', 200, '2019-07-29 00:00:17', 1, 'admin', 481, '参数：{}\n耗时：128.6998 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/all,耗时：30.7697 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-29 00:00:18', 1, 'admin', 482, '参数：{}\n耗时：30.7697 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/authority,耗时：30.9631 毫秒', 'GET', '/cms/admin/authority', 200, '2019-07-29 00:00:19', 1, 'admin', 483, '参数：{}\n耗时：30.9631 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/2,耗时：4396.2845 毫秒', 'GET', '/cms/admin/group/2', 200, '2019-07-29 00:00:23', 1, 'admin', 484, '参数：{\"id\":2}\n耗时：4396.2845 毫秒');
INSERT INTO `lin_log` VALUES ('', '添加权限成功', 'POST', '/cms/admin/dispatch/patch', 200, '2019-07-29 00:00:28', 1, 'admin', 485, '参数：{\"authDto\":{\"GroupId\":2,\"Auths\":[\"查询所有日志\",\"查询日志记录的用户\"]}}\n耗时：48.4502 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/authority,耗时：171.687 毫秒', 'GET', '/cms/admin/authority', 200, '2019-07-29 00:01:20', 1, 'admin', 486, '参数：{}\n耗时：171.687 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/2,耗时：2877.3009 毫秒', 'GET', '/cms/admin/group/2', 200, '2019-07-29 00:01:24', 1, 'admin', 487, '参数：{\"id\":2}\n耗时：2877.3009 毫秒');
INSERT INTO `lin_log` VALUES ('', '添加权限成功', 'POST', '/cms/admin/dispatch/patch', 200, '2019-07-29 00:01:31', 1, 'admin', 488, '参数：{\"authDto\":{\"GroupId\":2,\"Auths\":[\"查询所有日志\",\"查询日志记录的用户\"]}}\n耗时：39.4023 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/authority,耗时：41.4779 毫秒', 'GET', '/cms/admin/authority', 200, '2019-07-29 00:01:33', 1, 'admin', 489, '参数：{}\n耗时：41.4779 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/2,耗时：52.4995 毫秒', 'GET', '/cms/admin/group/2', 200, '2019-07-29 00:01:33', 1, 'admin', 490, '参数：{\"id\":2}\n耗时：52.4995 毫秒');
INSERT INTO `lin_log` VALUES ('', '添加权限成功', 'POST', '/cms/admin/dispatch/patch', 200, '2019-07-29 00:01:43', 1, 'admin', 491, '参数：{\"authDto\":{\"GroupId\":2,\"Auths\":[\"搜索日志\"]}}\n耗时：33.9194 毫秒');
INSERT INTO `lin_log` VALUES ('', '删除权限成功', 'POST', '/cms/admin/remove', 200, '2019-07-29 00:01:43', 1, 'admin', 492, '参数：{\"removeAuthDto\":{\"GroupId\":2,\"Auths\":[\"查询所有日志\"]}}\n耗时：38.0912 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/authority,耗时：33.7819 毫秒', 'GET', '/cms/admin/authority', 200, '2019-07-29 00:01:45', 1, 'admin', 493, '参数：{}\n耗时：33.7819 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/2,耗时：39.8404 毫秒', 'GET', '/cms/admin/group/2', 200, '2019-07-29 00:01:45', 1, 'admin', 494, '参数：{\"id\":2}\n耗时：39.8404 毫秒');
INSERT INTO `lin_log` VALUES ('', '添加权限成功', 'POST', '/cms/admin/dispatch/patch', 200, '2019-07-29 00:01:50', 1, 'admin', 495, '参数：{\"authDto\":{\"GroupId\":2,\"Auths\":[\"删除图书\",\"查看lin的信息\",\"查询所有日志\"]}}\n耗时：34.5892 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/authority,耗时：32.896 毫秒', 'GET', '/cms/admin/authority', 200, '2019-07-29 00:01:51', 1, 'admin', 496, '参数：{}\n耗时：32.896 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/2,耗时：45.5655 毫秒', 'GET', '/cms/admin/group/2', 200, '2019-07-29 00:01:51', 1, 'admin', 497, '参数：{\"id\":2}\n耗时：45.5655 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/authority,耗时：36.6443 毫秒', 'GET', '/cms/admin/authority', 200, '2019-07-29 00:01:56', 1, 'admin', 498, '参数：{}\n耗时：36.6443 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/2,耗时：40.4612 毫秒', 'GET', '/cms/admin/group/2', 200, '2019-07-29 00:01:56', 1, 'admin', 499, '参数：{\"id\":2}\n耗时：40.4612 毫秒');
INSERT INTO `lin_log` VALUES ('', '更新分组成功', 'PUT', '/cms/admin/group/2', 200, '2019-07-29 00:01:58', 1, 'admin', 500, '参数：{\"id\":2,\"updateGroupDto\":{\"Name\":\"23\",\"Info\":\"23\"}}\n耗时：54.6646 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/all,耗时：36.744 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-29 00:01:58', 1, 'admin', 501, '参数：{}\n耗时：36.744 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/authority,耗时：30.3225 毫秒', 'GET', '/cms/admin/authority', 200, '2019-07-29 00:02:00', 1, 'admin', 502, '参数：{}\n耗时：30.3225 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/14,耗时：37.6305 毫秒', 'GET', '/cms/admin/group/14', 200, '2019-07-29 00:02:00', 1, 'admin', 503, '参数：{\"id\":14}\n耗时：37.6305 毫秒');
INSERT INTO `lin_log` VALUES ('', '添加权限成功', 'POST', '/cms/admin/dispatch/patch', 200, '2019-07-29 00:02:02', 1, 'admin', 504, '参数：{\"authDto\":{\"GroupId\":14,\"Auths\":[\"查询日志记录的用户\"]}}\n耗时：43.948 毫秒');
INSERT INTO `lin_log` VALUES ('', '删除分组成功', 'DELETE', '/cms/admin/group/14', 200, '2019-07-29 00:02:05', 1, 'admin', 505, '参数：{\"id\":14}\n耗时：107.7703 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/all,耗时：33.316 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-29 00:02:05', 1, 'admin', 506, '参数：{}\n耗时：33.316 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/users,耗时：73.3335 毫秒', 'GET', '/cms/admin/users', 200, '2019-07-29 00:02:08', 1, 'admin', 507, '参数：{\"searchDto\":{\"GroupId\":null,\"Count\":10,\"Page\":0}}\n耗时：73.3335 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/all,耗时：31.6186 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-29 00:02:08', 1, 'admin', 508, '参数：{}\n耗时：31.6186 毫秒');
INSERT INTO `lin_log` VALUES ('', '操作成功', 'DELETE', '/cms/admin/1', 200, '2019-07-29 00:02:10', 1, 'admin', 509, '参数：{\"id\":1}\n耗时：54.9009 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/users,耗时：39.5614 毫秒', 'GET', '/cms/admin/users', 200, '2019-07-29 00:02:10', 1, 'admin', 510, '参数：{\"searchDto\":{\"GroupId\":null,\"Count\":10,\"Page\":0}}\n耗时：39.5614 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/all,耗时：34.1868 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-29 00:02:14', 1, 'admin', 511, '参数：{}\n耗时：34.1868 毫秒');
INSERT INTO `lin_log` VALUES ('', '用户创建成功', 'POST', '/cms/user/register', 200, '2019-07-29 00:02:28', 1, 'admin', 512, '参数：{\"userInput\":{\"Nickname\":\"123\",\"Password\":\"123qwe\",\"ConfirmPassword\":\"123qwe\",\"Email\":\"\",\"GroupId\":2}}\n耗时：63.8588 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/users,耗时：41.2596 毫秒', 'GET', '/cms/admin/users', 200, '2019-07-29 00:02:37', 1, 'admin', 513, '参数：{\"searchDto\":{\"GroupId\":null,\"Count\":10,\"Page\":0}}\n耗时：41.2596 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/all,耗时：37.8167 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-29 00:02:37', 1, 'admin', 514, '参数：{}\n耗时：37.8167 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/all,耗时：31.2591 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-29 00:02:51', 1, 'admin', 515, '参数：{}\n耗时：31.2591 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/users,耗时：49.3798 毫秒', 'GET', '/cms/admin/users', 200, '2019-07-29 00:02:53', 1, 'admin', 516, '参数：{\"searchDto\":{\"GroupId\":null,\"Count\":10,\"Page\":0}}\n耗时：49.3798 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/all,耗时：36.3577 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-29 00:02:54', 1, 'admin', 517, '参数：{}\n耗时：36.3577 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/password/6,耗时：248.3634 毫秒', 'PUT', '/cms/admin/password/6', 200, '2019-07-29 00:03:56', 1, 'admin', 518, '参数：{\"id\":6,\"resetPasswordDto\":{\"NewPassword\":\"123qwe\",\"ConfirmPassword\":\"123qwe\"}}\n耗时：248.3634 毫秒');
INSERT INTO `lin_log` VALUES ('', '操作成功', 'PUT', '/cms/admin/6', 200, '2019-07-29 00:07:09', 1, 'admin', 519, '参数：{\"id\":6,\"updateUserDto\":{\"Email\":\"qwqqwQ@qq.com\",\"GroupId\":2}}\n耗时：405.1976 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/users,耗时：51.0801 毫秒', 'GET', '/cms/admin/users', 200, '2019-07-29 00:07:09', 1, 'admin', 520, '参数：{\"searchDto\":{\"GroupId\":null,\"Count\":10,\"Page\":0}}\n耗时：51.0801 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/users,耗时：58.1262 毫秒', 'GET', '/cms/admin/users', 200, '2019-07-29 00:07:21', 1, 'admin', 521, '参数：{\"searchDto\":{\"GroupId\":null,\"Count\":10,\"Page\":0}}\n耗时：58.1262 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/all,耗时：70.7305 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-29 00:07:21', 1, 'admin', 522, '参数：{}\n耗时：70.7305 毫秒');
INSERT INTO `lin_log` VALUES ('', '操作成功', 'DELETE', '/cms/admin/6', 200, '2019-07-29 00:07:24', 1, 'admin', 523, '参数：{\"id\":6}\n耗时：60.4072 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/users,耗时：50.8908 毫秒', 'GET', '/cms/admin/users', 200, '2019-07-29 00:07:24', 1, 'admin', 524, '参数：{\"searchDto\":{\"GroupId\":null,\"Count\":10,\"Page\":0}}\n耗时：50.8908 毫秒');
INSERT INTO `lin_log` VALUES ('', '操作成功', 'PUT', '/cms/admin/2', 200, '2019-07-29 00:07:31', 1, 'admin', 525, '参数：{\"id\":2,\"updateUserDto\":{\"Email\":\"112@qq.com\",\"GroupId\":2}}\n耗时：56.3015 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/users,耗时：50.0204 毫秒', 'GET', '/cms/admin/users', 200, '2019-07-29 00:07:31', 1, 'admin', 526, '参数：{\"searchDto\":{\"GroupId\":null,\"Count\":10,\"Page\":0}}\n耗时：50.0204 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/users,耗时：344.5846 毫秒', 'GET', '/cms/admin/users', 200, '2019-07-29 00:10:11', 1, 'admin', 527, '参数：{\"searchDto\":{\"GroupId\":null,\"Count\":10,\"Page\":0}}\n耗时：344.5846 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/all,耗时：57.7581 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-29 00:10:11', 1, 'admin', 528, '参数：{}\n耗时：57.7581 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/users,耗时：51.2791 毫秒', 'GET', '/cms/admin/users', 200, '2019-07-29 00:10:18', 1, 'admin', 529, '参数：{\"searchDto\":{\"GroupId\":null,\"Count\":10,\"Page\":0}}\n耗时：51.2791 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/all,耗时：32.0142 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-29 00:10:18', 1, 'admin', 530, '参数：{}\n耗时：32.0142 毫秒');
INSERT INTO `lin_log` VALUES ('', '操作成功', 'PUT', '/cms/admin/2', 200, '2019-07-29 00:10:30', 1, 'admin', 531, '参数：{\"id\":2,\"updateUserDto\":{\"Email\":\"222112@qq.com\",\"GroupId\":2}}\n耗时：3798.7866 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/users,耗时：35.4342 毫秒', 'GET', '/cms/admin/users', 200, '2019-07-29 00:10:30', 1, 'admin', 532, '参数：{\"searchDto\":{\"GroupId\":null,\"Count\":10,\"Page\":0}}\n耗时：35.4342 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/all,耗时：31.6321 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-29 00:10:46', 1, 'admin', 533, '参数：{}\n耗时：31.6321 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/authority,耗时：125.3781 毫秒', 'GET', '/cms/admin/authority', 200, '2019-07-29 00:10:47', 1, 'admin', 534, '参数：{}\n耗时：125.3781 毫秒');
INSERT INTO `lin_log` VALUES ('', '新建分组成功', 'POST', '/cms/admin/group', 200, '2019-07-29 00:10:56', 1, 'admin', 535, '参数：{\"inputDto\":{\"Auths\":[\"查询日志记录的用户\"],\"Name\":\"1\",\"Info\":\"22\"}}\n耗时：4141.0706 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/all,耗时：35.5058 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-29 00:10:56', 1, 'admin', 536, '参数：{}\n耗时：35.5058 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/authority,耗时：69.1176 毫秒', 'GET', '/cms/admin/authority', 200, '2019-07-29 00:10:56', 1, 'admin', 537, '参数：{}\n耗时：69.1176 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/all,耗时：30.042 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-29 00:10:56', 1, 'admin', 538, '参数：{}\n耗时：30.042 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/users,耗时：35.9008 毫秒', 'GET', '/cms/admin/users', 200, '2019-07-29 00:11:01', 1, 'admin', 539, '参数：{\"searchDto\":{\"GroupId\":null,\"Count\":10,\"Page\":0}}\n耗时：35.9008 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/all,耗时：31.7224 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-29 00:11:01', 1, 'admin', 540, '参数：{}\n耗时：31.7224 毫秒');
INSERT INTO `lin_log` VALUES ('', '操作成功', 'PUT', '/cms/admin/2', 200, '2019-07-29 00:11:05', 1, 'admin', 541, '参数：{\"id\":2,\"updateUserDto\":{\"Email\":\"222112@qq.com\",\"GroupId\":15}}\n耗时：52.6683 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/users,耗时：37.1871 毫秒', 'GET', '/cms/admin/users', 200, '2019-07-29 00:11:05', 1, 'admin', 542, '参数：{\"searchDto\":{\"GroupId\":null,\"Count\":10,\"Page\":0}}\n耗时：37.1871 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/all,耗时：33.5713 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-29 00:11:11', 1, 'admin', 543, '参数：{}\n耗时：33.5713 毫秒');
INSERT INTO `lin_log` VALUES ('', '用户创建成功', 'POST', '/cms/user/register', 200, '2019-07-29 00:11:18', 1, 'admin', 544, '参数：{\"userInput\":{\"Nickname\":\"12121\",\"Password\":\"123qwe\",\"ConfirmPassword\":\"123qwe\",\"Email\":\"\",\"GroupId\":2}}\n耗时：60.8287 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/authority,耗时：35.9246 毫秒', 'GET', '/cms/admin/authority', 200, '2019-07-29 00:11:23', 1, 'admin', 545, '参数：{}\n耗时：35.9246 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/users,耗时：40.7379 毫秒', 'GET', '/cms/admin/users', 200, '2019-07-29 00:11:24', 1, 'admin', 546, '参数：{\"searchDto\":{\"GroupId\":null,\"Count\":10,\"Page\":0}}\n耗时：40.7379 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/all,耗时：31.7393 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-29 00:11:24', 1, 'admin', 547, '参数：{}\n耗时：31.7393 毫秒');
INSERT INTO `lin_log` VALUES ('', '操作成功', 'PUT', '/cms/admin/2', 200, '2019-07-29 00:11:29', 1, 'admin', 548, '参数：{\"id\":2,\"updateUserDto\":{\"Email\":\"222112@qq.com\",\"GroupId\":2}}\n耗时：38.179 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/users,耗时：36.4936 毫秒', 'GET', '/cms/admin/users', 200, '2019-07-29 00:11:30', 1, 'admin', 549, '参数：{\"searchDto\":{\"GroupId\":null,\"Count\":10,\"Page\":0}}\n耗时：36.4936 毫秒');
INSERT INTO `lin_log` VALUES ('', '密码修改成功', 'PUT', '/cms/admin/password/7', 200, '2019-07-29 00:11:50', 1, 'admin', 550, '参数：{\"id\":7,\"resetPasswordDto\":{\"NewPassword\":\"123qwe\",\"ConfirmPassword\":\"123qwe\"}}\n耗时：41.9789 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/user/login,耗时：928.5947 毫秒', 'POST', '/cms/user/login', 200, '2019-07-29 00:12:05', 0, NULL, 551, '参数：{\"loginInputDto\":{\"Nickname\":\"12121\",\"Password\":\"123qwe1\"}}\n耗时：928.5947 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/user/login,耗时：242.5238 毫秒', 'POST', '/cms/user/login', 200, '2019-07-29 00:12:08', 0, NULL, 552, '参数：{\"loginInputDto\":{\"Nickname\":\"12121\",\"Password\":\"123qwe\"}}\n耗时：242.5238 毫秒');
INSERT INTO `lin_log` VALUES ('', '12121访问/cms/user/auths,耗时：53.7541 毫秒', 'GET', '/cms/user/auths', 200, '2019-07-29 00:12:08', 7, '12121', 553, '参数：{}\n耗时：53.7541 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/user/login,耗时：165.9152 毫秒', 'POST', '/cms/user/login', 200, '2019-07-29 00:12:51', 0, NULL, 554, '参数：{\"loginInputDto\":{\"Nickname\":\"12121\",\"Password\":\"123qwe\"}}\n耗时：165.9152 毫秒');
INSERT INTO `lin_log` VALUES ('', '12121访问/cms/user/auths,耗时：39.4399 毫秒', 'GET', '/cms/user/auths', 200, '2019-07-29 00:12:51', 7, '12121', 555, '参数：{}\n耗时：39.4399 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/user/login,耗时：1036.9908 毫秒', 'POST', '/cms/user/login', 200, '2019-07-29 00:13:55', 0, NULL, 556, '参数：{\"loginInputDto\":{\"Nickname\":\"super\",\"Password\":\"123456\"}}\n耗时：1036.9908 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/user/login,耗时：9223.7661 毫秒', 'POST', '/cms/user/login', 200, '2019-07-29 00:16:17', 0, NULL, 557, '参数：{\"loginInputDto\":{\"Nickname\":\"super\",\"Password\":\"123456\"}}\n耗时：9223.7661 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/user/login,耗时：20826.9879 毫秒', 'POST', '/cms/user/login', 200, '2019-07-29 00:18:01', 0, NULL, 558, '参数：{\"loginInputDto\":{\"Nickname\":\"super\",\"Password\":\"123456\"}}\n耗时：20826.9879 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/user/login,耗时：1201.0258 毫秒', 'POST', '/cms/user/login', 200, '2019-07-29 00:21:49', 0, NULL, 559, '参数：{\"loginInputDto\":{\"Nickname\":\"super\",\"Password\":\"123456\"}}\n耗时：1201.0258 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/user/login,耗时：958.6255 毫秒', 'POST', '/cms/user/login', 200, '2019-07-29 00:27:14', 0, NULL, 560, '参数：{\"loginInputDto\":{\"Nickname\":\"super\",\"Password\":\"123456\"}}\n耗时：958.6255 毫秒');
INSERT INTO `lin_log` VALUES ('', '用户不存在', 'POST', '/cms/user/login', 200, '2019-07-29 00:28:51', 0, NULL, 561, '参数：{\"loginInputDto\":{\"Nickname\":\"super\",\"Password\":\"123456\"}}\n耗时：1155.5684 毫秒');
INSERT INTO `lin_log` VALUES ('', '用户不存在', 'POST', '/cms/user/login', 200, '2019-07-29 00:29:04', 0, NULL, 562, '参数：{\"loginInputDto\":{\"Nickname\":\"super\",\"Password\":\"123456\"}}\n耗时：767.4105 毫秒');
INSERT INTO `lin_log` VALUES ('', '用户不存在', 'POST', '/cms/user/login', 200, '2019-07-29 00:33:51', 0, NULL, 563, '参数：{\"loginInputDto\":{\"Nickname\":\"super\",\"Password\":\"123456\"}}\n耗时：5250.7647 毫秒');
INSERT INTO `lin_log` VALUES ('', '用户不存在', 'POST', '/cms/user/login', 200, '2019-07-29 00:34:10', 0, NULL, 564, '参数：{\"loginInputDto\":{\"Nickname\":\"super\",\"Password\":\"123456\"}}\n耗时：3011.3733 毫秒');
INSERT INTO `lin_log` VALUES ('', '用户不存在访问/cms/user/login,耗时：3099.0582 毫秒', 'POST', '/cms/user/login', 200, '2019-07-29 00:41:18', 0, NULL, 565, '参数：{\"loginInputDto\":{\"Nickname\":\"super\",\"Password\":\"123456\"}}\n耗时：3099.0582 毫秒');
INSERT INTO `lin_log` VALUES ('', '用户不存在访问/cms/user/login,耗时：3806.2145 毫秒', 'POST', '/cms/user/login', 200, '2019-07-29 00:41:32', 0, NULL, 566, '参数：{\"loginInputDto\":{\"Nickname\":\"super\",\"Password\":\"123456\"}}\n耗时：3806.2145 毫秒');
INSERT INTO `lin_log` VALUES ('', '用户不存在访问/cms/user/login,耗时：3184.3639 毫秒', 'POST', '/cms/user/login', 200, '2019-07-29 00:41:57', 0, NULL, 567, '参数：{\"loginInputDto\":{\"Nickname\":\"admin\",\"Password\":\"123qwer\"}}\n耗时：3184.3639 毫秒');
INSERT INTO `lin_log` VALUES ('', '用户不存在访问/cms/user/login,耗时：737.9628 毫秒', 'POST', '/cms/user/login', 200, '2019-07-29 00:42:03', 0, NULL, 568, '参数：{\"loginInputDto\":{\"Nickname\":\"admin\",\"Password\":\"123qwer\"}}\n耗时：737.9628 毫秒');
INSERT INTO `lin_log` VALUES ('', '密码错误，请输入正确密码!访问/cms/user/login,耗时：770.7431 毫秒', 'POST', '/cms/user/login', 200, '2019-07-29 00:42:39', 0, NULL, 569, '参数：{\"loginInputDto\":{\"Nickname\":\"admin\",\"Password\":\"123qwer\"}}\n耗时：770.7431 毫秒');
INSERT INTO `lin_log` VALUES ('', '密码错误，请输入正确密码!访问/cms/user/login,耗时：1256.7861 毫秒', 'POST', '/cms/user/login', 200, '2019-07-29 00:49:40', 0, NULL, 570, '参数：{\"loginInputDto\":{\"Nickname\":\"admin\",\"Password\":\"123qwer\"}}\n耗时：1256.7861 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/user/login,耗时：744.7586 毫秒', 'POST', '/cms/user/login', 200, '2019-07-29 00:52:46', 0, NULL, 571, '参数：{\"loginInputDto\":{\"Nickname\":\"admin\",\"Password\":\"123qwe\"}}\n耗时：744.7586 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/user/login,耗时：157.3563 毫秒', 'POST', '/cms/user/login', 200, '2019-07-29 00:52:53', 0, NULL, 572, '参数：{\"loginInputDto\":{\"Nickname\":\"admin\",\"Password\":\"123qwe\"}}\n耗时：157.3563 毫秒');
INSERT INTO `lin_log` VALUES ('', '密码错误，请输入正确密码!访问/cms/user/login,耗时：936.989 毫秒', 'POST', '/cms/user/login', 200, '2019-07-29 00:53:01', 0, NULL, 573, '参数：{\"loginInputDto\":{\"Nickname\":\"admin\",\"Password\":\"123qwe1\"}}\n耗时：936.989 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/user/login,耗时：145.939 毫秒', 'POST', '/cms/user/login', 200, '2019-07-29 00:53:06', 0, NULL, 574, '参数：{\"loginInputDto\":{\"Nickname\":\"admin\",\"Password\":\"123qwe\"}}\n耗时：145.939 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/user/login,耗时：150.2067 毫秒', 'POST', '/cms/user/login', 200, '2019-07-29 00:53:34', 0, NULL, 575, '参数：{\"loginInputDto\":{\"Nickname\":\"admin\",\"Password\":\"123qwe\"}}\n耗时：150.2067 毫秒');
INSERT INTO `lin_log` VALUES ('', '密码错误，请输入正确密码!访问/cms/user/login,耗时：738.9162 毫秒', 'POST', '/cms/user/login', 200, '2019-07-29 00:53:48', 0, NULL, 576, '参数：{\"loginInputDto\":{\"Nickname\":\"admin\",\"Password\":\"123qwe2\"}}\n耗时：738.9162 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/user/login,耗时：675.9143 毫秒', 'POST', '/cms/user/login', 200, '2019-07-29 00:55:21', 0, NULL, 577, '参数：{\"loginInputDto\":{\"Nickname\":\"admin\",\"Password\":\"123qwe\"}}\n耗时：675.9143 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/user/auths,耗时：88.9604 毫秒', 'GET', '/cms/user/auths', 200, '2019-07-29 00:55:21', 7, 'admin', 578, '参数：{}\n耗时：88.9604 毫秒');
INSERT INTO `lin_log` VALUES ('', '用户不存在访问/cms/user/login,耗时：764.1262 毫秒', 'POST', '/cms/user/login', 200, '2019-07-29 00:55:47', 0, NULL, 579, '参数：{\"loginInputDto\":{\"Nickname\":\"super\",\"Password\":\"123456\"}}\n耗时：764.1262 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/user/login,耗时：138.5553 毫秒', 'POST', '/cms/user/login', 200, '2019-07-29 00:55:54', 0, NULL, 580, '参数：{\"loginInputDto\":{\"Nickname\":\"admin\",\"Password\":\"123qwe\"}}\n耗时：138.5553 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/user/auths,耗时：33.4844 毫秒', 'GET', '/cms/user/auths', 200, '2019-07-29 00:55:54', 7, 'admin', 581, '参数：{}\n耗时：33.4844 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/log/users,耗时：59.6535 毫秒', 'GET', '/cms/log/users', 200, '2019-07-29 00:55:56', 7, 'admin', 582, '参数：{\"pageDto\":{\"Count\":5,\"Page\":0}}\n耗时：59.6535 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/log/,耗时：3133.2572 毫秒', 'GET', '/cms/log/', 200, '2019-07-29 00:56:00', 7, 'admin', 583, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":null,\"Start\":null,\"End\":null,\"Count\":15,\"Page\":0}}\n耗时：3133.2572 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/log/users,耗时：37.5826 毫秒', 'GET', '/cms/log/users', 200, '2019-07-29 00:56:15', 7, 'admin', 584, '参数：{\"pageDto\":{\"Count\":5,\"Page\":0}}\n耗时：37.5826 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/log/,耗时：42.1902 毫秒', 'GET', '/cms/log/', 200, '2019-07-29 00:56:16', 7, 'admin', 585, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":null,\"Start\":null,\"End\":null,\"Count\":15,\"Page\":0}}\n耗时：42.1902 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/log/search,耗时：2419.5852 毫秒', 'GET', '/cms/log/search', 200, '2019-07-29 00:56:24', 7, 'admin', 586, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":\"admin\",\"Start\":null,\"End\":null,\"Count\":15,\"Page\":0}}\n耗时：2419.5852 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/log/search,耗时：37.4709 毫秒', 'GET', '/cms/log/search', 200, '2019-07-29 00:56:31', 7, 'admin', 587, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":\"admin\",\"Start\":null,\"End\":null,\"Count\":15,\"Page\":1}}\n耗时：37.4709 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/log/users,耗时：44.0855 毫秒', 'GET', '/cms/log/users', 200, '2019-07-29 00:56:58', 7, 'admin', 588, '参数：{\"pageDto\":{\"Count\":5,\"Page\":0}}\n耗时：44.0855 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/log/,耗时：35.5402 毫秒', 'GET', '/cms/log/', 200, '2019-07-29 00:56:58', 7, 'admin', 589, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":null,\"Start\":null,\"End\":null,\"Count\":15,\"Page\":0}}\n耗时：35.5402 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/users,耗时：40.7999 毫秒', 'GET', '/cms/admin/users', 200, '2019-07-29 00:57:52', 7, 'admin', 590, '参数：{\"searchDto\":{\"GroupId\":null,\"Count\":10,\"Page\":0}}\n耗时：40.7999 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/all,耗时：57.8948 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-29 00:57:52', 7, 'admin', 591, '参数：{}\n耗时：57.8948 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/2,耗时：89.3783 毫秒', 'PUT', '/cms/admin/2', 200, '2019-07-29 00:57:58', 7, 'admin', 592, '参数：{\"id\":2,\"updateUserDto\":{\"Email\":\"1212@qq.com\",\"GroupId\":2}}\n耗时：89.3783 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/users,耗时：42.7103 毫秒', 'GET', '/cms/admin/users', 200, '2019-07-29 00:57:58', 7, 'admin', 593, '参数：{\"searchDto\":{\"GroupId\":null,\"Count\":10,\"Page\":0}}\n耗时：42.7103 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/2,耗时：47.9469 毫秒', 'PUT', '/cms/admin/2', 200, '2019-07-29 00:58:01', 7, 'admin', 594, '参数：{\"id\":2,\"updateUserDto\":{\"Email\":\"1212@qq.com\",\"GroupId\":15}}\n耗时：47.9469 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/users,耗时：37.0023 毫秒', 'GET', '/cms/admin/users', 200, '2019-07-29 00:58:01', 7, 'admin', 595, '参数：{\"searchDto\":{\"GroupId\":null,\"Count\":10,\"Page\":0}}\n耗时：37.0023 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/users,耗时：42.4219 毫秒', 'GET', '/cms/admin/users', 200, '2019-07-29 00:58:04', 7, 'admin', 596, '参数：{\"searchDto\":{\"GroupId\":null,\"Count\":10,\"Page\":0}}\n耗时：42.4219 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/all,耗时：43.7056 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-29 00:58:04', 7, 'admin', 597, '参数：{}\n耗时：43.7056 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/all,耗时：31.017 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-29 00:58:11', 7, 'admin', 598, '参数：{}\n耗时：31.017 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/all,耗时：32.8265 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-29 00:58:16', 7, 'admin', 599, '参数：{}\n耗时：32.8265 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/authority,耗时：116.8386 毫秒', 'GET', '/cms/admin/authority', 200, '2019-07-29 00:58:17', 7, 'admin', 600, '参数：{}\n耗时：116.8386 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/15,耗时：52.6813 毫秒', 'GET', '/cms/admin/group/15', 200, '2019-07-29 00:58:18', 7, 'admin', 601, '参数：{\"id\":15}\n耗时：52.6813 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/log/users,耗时：38.7562 毫秒', 'GET', '/cms/log/users', 200, '2019-07-29 00:58:21', 7, 'admin', 602, '参数：{\"pageDto\":{\"Count\":5,\"Page\":0}}\n耗时：38.7562 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/log/,耗时：36.2642 毫秒', 'GET', '/cms/log/', 200, '2019-07-29 00:58:21', 7, 'admin', 603, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":null,\"Start\":null,\"End\":null,\"Count\":15,\"Page\":0}}\n耗时：36.2642 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/admin/group/all,耗时：292.8989 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-29 15:58:38', 0, NULL, 604, '参数：{}\n耗时：292.8989 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/admin/group/all,耗时：28.6999 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-29 15:58:40', 0, NULL, 605, '参数：{}\n耗时：28.6999 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/admin/group/all,耗时：30.2554 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-29 15:58:44', 0, NULL, 606, '参数：{}\n耗时：30.2554 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/user/login,耗时：1488.0094 毫秒', 'POST', '/cms/user/login', 200, '2019-07-29 15:59:08', 0, NULL, 607, '参数：{\"loginInputDto\":{\"Nickname\":\"admin\",\"Password\":\"123qwe\"}}\n耗时：1488.0094 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/user/login,耗时：1577.8831 毫秒', 'POST', '/cms/user/login', 200, '2019-07-29 16:00:37', 0, NULL, 608, '参数：{\"loginInputDto\":{\"Nickname\":\"admin\",\"Password\":\"123qwe\"}}\n耗时：1577.8831 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/user/login,耗时：1680.2134 毫秒', 'POST', '/cms/user/login', 200, '2019-07-29 16:02:17', 0, NULL, 609, '参数：{\"loginInputDto\":{\"Nickname\":\"admin\",\"Password\":\"123qwe\"}}\n耗时：1680.2134 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/user/login,耗时：24514.0465 毫秒', 'POST', '/cms/user/login', 200, '2019-07-29 16:03:17', 0, NULL, 610, '参数：{\"loginInputDto\":{\"Nickname\":\"admin\",\"Password\":\"123qwe\"}}\n耗时：24514.0465 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/user/login,耗时：4938.0451 毫秒', 'POST', '/cms/user/login', 200, '2019-07-29 16:04:28', 0, NULL, 611, '参数：{\"loginInputDto\":{\"Nickname\":\"string\",\"Password\":\"string\"}}\n耗时：4938.0451 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/user/login,耗时：5429.3094 毫秒', 'POST', '/cms/user/login', 200, '2019-07-29 16:06:10', 0, NULL, 612, '参数：{\"loginInputDto\":{\"Nickname\":\"string\",\"Password\":\"string\"}}\n耗时：5429.3094 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/test,耗时：70.028 毫秒', 'GET', '/cms/test', 200, '2019-07-29 16:30:10', 0, NULL, 613, '参数：{}\n耗时：70.028 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/test/get,耗时：23.1888 毫秒', 'GET', '/cms/test/get', 200, '2019-07-29 16:30:17', 0, NULL, 614, '参数：{}\n耗时：23.1888 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/test/get,耗时：74.0955 毫秒', 'GET', '/cms/test/get', 200, '2019-07-29 16:38:21', 0, NULL, 615, '参数：{}\n耗时：74.0955 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/user/login,耗时：1359.7373 毫秒', 'POST', '/cms/user/login', 200, '2019-07-29 16:38:28', 0, NULL, 616, '参数：{\"loginInputDto\":{\"Nickname\":\"string\",\"Password\":\"string\"}}\n耗时：1359.7373 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/user/login,耗时：14904.3511 毫秒', 'POST', '/cms/user/login', 200, '2019-07-29 16:39:16', 0, NULL, 617, '参数：{\"loginInputDto\":{\"Nickname\":\"string\",\"Password\":\"string\"}}\n耗时：14904.3511 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/user/login,耗时：3446.3894 毫秒', 'POST', '/cms/user/login', 200, '2019-07-29 16:39:36', 0, NULL, 618, '参数：{\"loginInputDto\":{\"Nickname\":\"admin\",\"Password\":\"123qwe\"}}\n耗时：3446.3894 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/user/login,耗时：3455.2308 毫秒', 'POST', '/cms/user/login', 200, '2019-07-29 16:40:46', 0, NULL, 619, '参数：{\"loginInputDto\":{\"Nickname\":\"admin\",\"Password\":\"123qwe\"}}\n耗时：3455.2308 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/user/login,耗时：750.6321 毫秒', 'POST', '/cms/user/login', 200, '2019-07-29 20:32:29', 0, NULL, 620, '参数：{\"loginInputDto\":{\"Nickname\":\"admin\",\"Password\":\"123qwe\"}}\n耗时：750.6321 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/user/auths,耗时：51.3823 毫秒', 'GET', '/cms/user/auths', 200, '2019-07-29 20:32:30', 7, 'admin', 621, '参数：{}\n耗时：51.3823 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/log/users,耗时：166.7255 毫秒', 'GET', '/cms/log/users', 200, '2019-07-29 20:33:15', 7, 'admin', 622, '参数：{\"pageDto\":{\"Count\":5,\"Page\":0}}\n耗时：166.7255 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/log/,耗时：31.3867 毫秒', 'GET', '/cms/log/', 200, '2019-07-29 20:33:15', 7, 'admin', 623, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":null,\"Start\":null,\"End\":null,\"Count\":15,\"Page\":0}}\n耗时：31.3867 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/log/,耗时：28.3494 毫秒', 'GET', '/cms/log/', 200, '2019-07-29 20:33:18', 7, 'admin', 624, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":null,\"Start\":null,\"End\":null,\"Count\":15,\"Page\":1}}\n耗时：28.3494 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/log/search,耗时：28.8171 毫秒', 'GET', '/cms/log/search', 200, '2019-07-29 20:33:39', 7, 'admin', 625, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":\"admin\",\"Start\":null,\"End\":null,\"Count\":15,\"Page\":0}}\n耗时：28.8171 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/log/users,耗时：36.2515 毫秒', 'GET', '/cms/log/users', 200, '2019-07-29 20:33:48', 7, 'admin', 626, '参数：{\"pageDto\":{\"Count\":5,\"Page\":0}}\n耗时：36.2515 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/log/,耗时：33.7089 毫秒', 'GET', '/cms/log/', 200, '2019-07-29 20:33:48', 7, 'admin', 627, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":null,\"Start\":null,\"End\":null,\"Count\":15,\"Page\":0}}\n耗时：33.7089 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/log/users,耗时：291.8509 毫秒', 'GET', '/cms/log/users', 200, '2019-07-29 20:36:27', 7, 'admin', 628, '参数：{\"pageDto\":{\"Count\":5,\"Page\":0}}\n耗时：291.8509 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/log/,耗时：69.0911 毫秒', 'GET', '/cms/log/', 200, '2019-07-29 20:36:27', 7, 'admin', 629, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":null,\"Start\":null,\"End\":null,\"Count\":15,\"Page\":0}}\n耗时：69.0911 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/log/search,耗时：35.3991 毫秒', 'GET', '/cms/log/search', 200, '2019-07-29 20:36:30', 7, 'admin', 630, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":\"admin\",\"Start\":null,\"End\":null,\"Count\":15,\"Page\":0}}\n耗时：35.3991 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/log/search,耗时：32.7875 毫秒', 'GET', '/cms/log/search', 200, '2019-07-29 20:36:32', 7, 'admin', 631, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":\"12121\",\"Start\":null,\"End\":null,\"Count\":15,\"Page\":0}}\n耗时：32.7875 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/log/search,耗时：31.1239 毫秒', 'GET', '/cms/log/search', 200, '2019-07-29 20:36:33', 7, 'admin', 632, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":\"12121\",\"Start\":null,\"End\":null,\"Count\":15,\"Page\":1}}\n耗时：31.1239 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/log/search,耗时：34.5605 毫秒', 'GET', '/cms/log/search', 200, '2019-07-29 20:36:35', 7, 'admin', 633, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":\"12121\",\"Start\":null,\"End\":null,\"Count\":15,\"Page\":2}}\n耗时：34.5605 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/log/search,耗时：30.309 毫秒', 'GET', '/cms/log/search', 200, '2019-07-29 20:36:41', 7, 'admin', 634, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":\"admin\",\"Start\":null,\"End\":null,\"Count\":15,\"Page\":0}}\n耗时：30.309 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/log/search,耗时：37.2096 毫秒', 'GET', '/cms/log/search', 200, '2019-07-29 20:36:43', 7, 'admin', 635, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":null,\"Start\":null,\"End\":null,\"Count\":15,\"Page\":0}}\n耗时：37.2096 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/log/search,耗时：89.602 毫秒', 'GET', '/cms/log/search', 200, '2019-07-29 20:36:44', 7, 'admin', 636, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":null,\"Start\":null,\"End\":null,\"Count\":15,\"Page\":0}}\n耗时：89.602 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/log/users,耗时：38.7214 毫秒', 'GET', '/cms/log/users', 200, '2019-07-29 20:36:44', 7, 'admin', 637, '参数：{\"pageDto\":{\"Count\":5,\"Page\":0}}\n耗时：38.7214 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/log/search,耗时：37.4037 毫秒', 'GET', '/cms/log/search', 200, '2019-07-29 20:36:44', 7, 'admin', 638, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":null,\"Start\":null,\"End\":null,\"Count\":15,\"Page\":0}}\n耗时：37.4037 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/log/,耗时：30.5291 毫秒', 'GET', '/cms/log/', 200, '2019-07-29 20:36:44', 7, 'admin', 639, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":null,\"Start\":null,\"End\":null,\"Count\":15,\"Page\":0}}\n耗时：30.5291 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/log/search,耗时：37.8693 毫秒', 'GET', '/cms/log/search', 200, '2019-07-29 20:36:48', 7, 'admin', 640, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":null,\"Start\":\"2019-07-03T00:00:00\",\"End\":\"2019-08-07T23:59:59\",\"Count\":15,\"Page\":0}}\n耗时：37.8693 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/log/search,耗时：28.7527 毫秒', 'GET', '/cms/log/search', 200, '2019-07-29 20:36:59', 7, 'admin', 641, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":null,\"Start\":\"2019-07-03T00:00:00\",\"End\":\"2019-08-07T23:59:59\",\"Count\":15,\"Page\":1}}\n耗时：28.7527 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/all,耗时：52.5337 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-29 20:37:17', 7, 'admin', 642, '参数：{}\n耗时：52.5337 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/users,耗时：80.9719 毫秒', 'GET', '/cms/admin/users', 200, '2019-07-29 20:37:18', 7, 'admin', 643, '参数：{\"searchDto\":{\"GroupId\":null,\"Count\":10,\"Page\":0}}\n耗时：80.9719 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/all,耗时：26.1936 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-29 20:37:18', 7, 'admin', 644, '参数：{}\n耗时：26.1936 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/password/2,耗时：149.04 毫秒', 'PUT', '/cms/admin/password/2', 200, '2019-07-29 20:37:28', 7, 'admin', 645, '参数：{\"id\":2,\"resetPasswordDto\":{\"NewPassword\":\"123qwe\",\"ConfirmPassword\":\"123qwe\"}}\n耗时：149.04 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/user/login,耗时：347.9051 毫秒', 'POST', '/cms/user/login', 200, '2019-07-29 20:37:37', 0, NULL, 646, '参数：{\"loginInputDto\":{\"Nickname\":\"12\",\"Password\":\"123qwe\"}}\n耗时：347.9051 毫秒');
INSERT INTO `lin_log` VALUES ('', '12访问/cms/user/auths,耗时：60.6547 毫秒', 'GET', '/cms/user/auths', 200, '2019-07-29 20:37:37', 2, '12', 647, '参数：{}\n耗时：60.6547 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/user/login,耗时：402.2262 毫秒', 'POST', '/cms/user/login', 200, '2019-07-29 20:42:28', 0, NULL, 648, '参数：{\"loginInputDto\":{\"Nickname\":\"12\",\"Password\":\"123qwe\"}}\n耗时：402.2262 毫秒');
INSERT INTO `lin_log` VALUES ('', '12访问/cms/user/auths,耗时：29.6746 毫秒', 'GET', '/cms/user/auths', 200, '2019-07-29 20:42:28', 2, '12', 649, '参数：{}\n耗时：29.6746 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/user/login,耗时：99.9524 毫秒', 'POST', '/cms/user/login', 200, '2019-07-29 20:43:38', 0, NULL, 650, '参数：{\"loginInputDto\":{\"Nickname\":\"admin\",\"Password\":\"123qwe\"}}\n耗时：99.9524 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/user/auths,耗时：24.8689 毫秒', 'GET', '/cms/user/auths', 200, '2019-07-29 20:43:38', 7, 'admin', 651, '参数：{}\n耗时：24.8689 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/all,耗时：25.6229 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-29 20:43:42', 7, 'admin', 652, '参数：{}\n耗时：25.6229 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/users,耗时：138.428 毫秒', 'GET', '/cms/admin/users', 200, '2019-07-29 20:43:45', 7, 'admin', 653, '参数：{\"searchDto\":{\"GroupId\":null,\"Count\":10,\"Page\":0}}\n耗时：138.428 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/all,耗时：63.7839 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-29 20:43:45', 7, 'admin', 654, '参数：{}\n耗时：63.7839 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/all,耗时：25.2458 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-29 20:43:48', 7, 'admin', 655, '参数：{}\n耗时：25.2458 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/users,耗时：27.9963 毫秒', 'GET', '/cms/admin/users', 200, '2019-07-29 20:43:58', 7, 'admin', 656, '参数：{\"searchDto\":{\"GroupId\":null,\"Count\":10,\"Page\":0}}\n耗时：27.9963 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/all,耗时：28.2384 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-29 20:43:59', 7, 'admin', 657, '参数：{}\n耗时：28.2384 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/all,耗时：25.9288 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-29 20:44:02', 7, 'admin', 658, '参数：{}\n耗时：25.9288 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/authority,耗时：109.0275 毫秒', 'GET', '/cms/admin/authority', 200, '2019-07-29 20:44:06', 7, 'admin', 659, '参数：{}\n耗时：109.0275 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/15,耗时：45.5009 毫秒', 'GET', '/cms/admin/group/15', 200, '2019-07-29 20:44:06', 7, 'admin', 660, '参数：{\"id\":15}\n耗时：45.5009 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/dispatch/patch,耗时：42.9807 毫秒', 'POST', '/cms/admin/dispatch/patch', 200, '2019-07-29 20:44:10', 7, 'admin', 661, '参数：{\"authDto\":{\"GroupId\":15,\"Auths\":[\"搜索日志\",\"查询所有日志\"]}}\n耗时：42.9807 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/users,耗时：29.6781 毫秒', 'GET', '/cms/admin/users', 200, '2019-07-29 20:44:12', 7, 'admin', 662, '参数：{\"searchDto\":{\"GroupId\":null,\"Count\":10,\"Page\":0}}\n耗时：29.6781 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/all,耗时：27.2272 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-29 20:44:13', 7, 'admin', 663, '参数：{}\n耗时：27.2272 毫秒');
INSERT INTO `lin_log` VALUES ('', '密码错误，请输入正确密码!访问/cms/user/login,耗时：640.1941 毫秒', 'POST', '/cms/user/login', 200, '2019-07-29 20:44:25', 0, NULL, 664, '参数：{\"loginInputDto\":{\"Nickname\":\"12\",\"Password\":\"123456\"}}\n耗时：640.1941 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/user/login,耗时：111.5468 毫秒', 'POST', '/cms/user/login', 200, '2019-07-29 20:44:28', 0, NULL, 665, '参数：{\"loginInputDto\":{\"Nickname\":\"12\",\"Password\":\"123qwe\"}}\n耗时：111.5468 毫秒');
INSERT INTO `lin_log` VALUES ('', '12访问/cms/user/auths,耗时：28.6309 毫秒', 'GET', '/cms/user/auths', 200, '2019-07-29 20:44:28', 2, '12', 666, '参数：{}\n耗时：28.6309 毫秒');
INSERT INTO `lin_log` VALUES ('', '用户不存在访问/cms/user/login,耗时：1187.3668 毫秒', 'POST', '/cms/user/login', 200, '2019-07-29 20:52:14', 0, NULL, 667, '参数：{\"loginInputDto\":{\"Nickname\":\"super\",\"Password\":\"123456\"}}\n耗时：1187.3668 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/user/login,耗时：289.9213 毫秒', 'POST', '/cms/user/login', 200, '2019-07-29 20:52:23', 0, NULL, 668, '参数：{\"loginInputDto\":{\"Nickname\":\"12\",\"Password\":\"123qwe\"}}\n耗时：289.9213 毫秒');
INSERT INTO `lin_log` VALUES ('', '12访问/cms/user/auths,耗时：84.0329 毫秒', 'GET', '/cms/user/auths', 200, '2019-07-29 20:52:24', 2, '12', 669, '参数：{}\n耗时：84.0329 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/user/login,耗时：739.5686 毫秒', 'POST', '/cms/user/login', 200, '2019-07-29 20:59:49', 0, NULL, 670, '参数：{\"loginInputDto\":{\"Nickname\":\"12\",\"Password\":\"123qwe\"}}\n耗时：739.5686 毫秒');
INSERT INTO `lin_log` VALUES ('', '12访问/cms/user/auths,耗时：80.9849 毫秒', 'GET', '/cms/user/auths', 200, '2019-07-29 20:59:50', 2, '12', 671, '参数：{}\n耗时：80.9849 毫秒');
INSERT INTO `lin_log` VALUES ('', '12访问/cms/log/users,耗时：56.3406 毫秒', 'GET', '/cms/log/users', 200, '2019-07-29 20:59:52', 2, '12', 672, '参数：{\"pageDto\":{\"Count\":5,\"Page\":0}}\n耗时：56.3406 毫秒');
INSERT INTO `lin_log` VALUES ('', '12访问/cms/log/,耗时：45.7832 毫秒', 'GET', '/cms/log/', 200, '2019-07-29 20:59:52', 2, '12', 673, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":null,\"Start\":null,\"End\":null,\"Count\":15,\"Page\":0}}\n耗时：45.7832 毫秒');
INSERT INTO `lin_log` VALUES ('', '12访问/cms/log/search,耗时：30.1223 毫秒', 'GET', '/cms/log/search', 200, '2019-07-29 20:59:57', 2, '12', 674, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":\"12\",\"Start\":null,\"End\":null,\"Count\":15,\"Page\":0}}\n耗时：30.1223 毫秒');
INSERT INTO `lin_log` VALUES ('', '12访问/cms/log/search,耗时：31.7753 毫秒', 'GET', '/cms/log/search', 200, '2019-07-29 20:59:58', 2, '12', 675, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":\"admin\",\"Start\":null,\"End\":null,\"Count\":15,\"Page\":0}}\n耗时：31.7753 毫秒');
INSERT INTO `lin_log` VALUES ('', '12访问/cms/log/search,耗时：30.6768 毫秒', 'GET', '/cms/log/search', 200, '2019-07-29 21:00:00', 2, '12', 676, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":\"12121\",\"Start\":null,\"End\":null,\"Count\":15,\"Page\":0}}\n耗时：30.6768 毫秒');
INSERT INTO `lin_log` VALUES ('', '12访问/cms/log/search,耗时：29.8346 毫秒', 'GET', '/cms/log/search', 200, '2019-07-29 21:00:02', 2, '12', 677, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":null,\"Start\":null,\"End\":null,\"Count\":15,\"Page\":0}}\n耗时：29.8346 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/user/login,耗时：124.3218 毫秒', 'POST', '/cms/user/login', 200, '2019-07-29 21:00:25', 0, NULL, 678, '参数：{\"loginInputDto\":{\"Nickname\":\"admin\",\"Password\":\"123qwe\"}}\n耗时：124.3218 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/user/auths,耗时：27.7045 毫秒', 'GET', '/cms/user/auths', 200, '2019-07-29 21:00:25', 7, 'admin', 679, '参数：{}\n耗时：27.7045 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/all,耗时：57.6518 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-29 21:00:29', 7, 'admin', 680, '参数：{}\n耗时：57.6518 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/authority,耗时：109.5446 毫秒', 'GET', '/cms/admin/authority', 200, '2019-07-29 21:00:31', 7, 'admin', 681, '参数：{}\n耗时：109.5446 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/group/15,耗时：37.4527 毫秒', 'GET', '/cms/admin/group/15', 200, '2019-07-29 21:00:31', 7, 'admin', 682, '参数：{\"id\":15}\n耗时：37.4527 毫秒');
INSERT INTO `lin_log` VALUES ('', 'admin访问/cms/admin/remove,耗时：41.5299 毫秒', 'POST', '/cms/admin/remove', 200, '2019-07-29 21:00:33', 7, 'admin', 683, '参数：{\"authDto\":{\"GroupId\":15,\"Auths\":[\"查询所有日志\"]}}\n耗时：41.5299 毫秒');
INSERT INTO `lin_log` VALUES ('', '12访问/cms/log/users,耗时：36.1138 毫秒', 'GET', '/cms/log/users', 200, '2019-07-29 21:00:36', 2, '12', 684, '参数：{\"pageDto\":{\"Count\":5,\"Page\":0}}\n耗时：36.1138 毫秒');
INSERT INTO `lin_log` VALUES ('', '12访问/cms/log/search,耗时：29.071 毫秒', 'GET', '/cms/log/search', 200, '2019-07-29 21:00:41', 2, '12', 685, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":\"admin\",\"Start\":null,\"End\":null,\"Count\":15,\"Page\":0}}\n耗时：29.071 毫秒');
INSERT INTO `lin_log` VALUES ('', '12访问/cms/log/search,耗时：27.6693 毫秒', 'GET', '/cms/log/search', 200, '2019-07-29 21:00:42', 2, '12', 686, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":\"12121\",\"Start\":null,\"End\":null,\"Count\":15,\"Page\":0}}\n耗时：27.6693 毫秒');
INSERT INTO `lin_log` VALUES ('', '12访问/cms/log/search,耗时：30.9469 毫秒', 'GET', '/cms/log/search', 200, '2019-07-29 21:00:44', 2, '12', 687, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":\"12\",\"Start\":null,\"End\":null,\"Count\":15,\"Page\":0}}\n耗时：30.9469 毫秒');
INSERT INTO `lin_log` VALUES ('', '12访问/cms/log/search,耗时：27.7288 毫秒', 'GET', '/cms/log/search', 200, '2019-07-29 21:00:46', 2, '12', 688, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":null,\"Start\":null,\"End\":null,\"Count\":15,\"Page\":0}}\n耗时：27.7288 毫秒');
INSERT INTO `lin_log` VALUES ('', '12访问/cms/log/search,耗时：77.6311 毫秒', 'GET', '/cms/log/search', 200, '2019-07-29 21:00:46', 2, '12', 689, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":null,\"Start\":null,\"End\":null,\"Count\":15,\"Page\":0}}\n耗时：77.6311 毫秒');
INSERT INTO `lin_log` VALUES ('', '12访问/cms/log/users,耗时：35.2184 毫秒', 'GET', '/cms/log/users', 200, '2019-07-29 21:00:46', 2, '12', 690, '参数：{\"pageDto\":{\"Count\":5,\"Page\":0}}\n耗时：35.2184 毫秒');
INSERT INTO `lin_log` VALUES ('', '12访问/cms/log/search,耗时：38.2073 毫秒', 'GET', '/cms/log/search', 200, '2019-07-29 21:00:46', 2, '12', 691, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":null,\"Start\":null,\"End\":null,\"Count\":15,\"Page\":0}}\n耗时：38.2073 毫秒');
INSERT INTO `lin_log` VALUES ('', '12访问/cms/log/users,耗时：37.0963 毫秒', 'GET', '/cms/log/users', 200, '2019-07-29 21:02:00', 2, '12', 692, '参数：{\"pageDto\":{\"Count\":5,\"Page\":0}}\n耗时：37.0963 毫秒');
INSERT INTO `lin_log` VALUES ('', '12访问/cms/log/search,耗时：27.8068 毫秒', 'GET', '/cms/log/search', 200, '2019-07-29 21:02:15', 2, '12', 693, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":null,\"Start\":null,\"End\":null,\"Count\":15,\"Page\":0}}\n耗时：27.8068 毫秒');
INSERT INTO `lin_log` VALUES ('', '12访问/cms/log/users,耗时：32.1848 毫秒', 'GET', '/cms/log/users', 200, '2019-07-29 21:02:24', 2, '12', 694, '参数：{\"pageDto\":{\"Count\":5,\"Page\":0}}\n耗时：32.1848 毫秒');
INSERT INTO `lin_log` VALUES ('', '12访问/cms/log/search,耗时：820.0439 毫秒', 'GET', '/cms/log/search', 200, '2019-07-29 21:02:26', 2, '12', 695, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":null,\"Start\":null,\"End\":null,\"Count\":15,\"Page\":0}}\n耗时：820.0439 毫秒');
INSERT INTO `lin_log` VALUES ('', '12访问/cms/log/search,耗时：35.2593 毫秒', 'GET', '/cms/log/search', 200, '2019-07-29 21:02:27', 2, '12', 696, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":null,\"Start\":null,\"End\":null,\"Count\":15,\"Page\":0}}\n耗时：35.2593 毫秒');
INSERT INTO `lin_log` VALUES ('', '12访问/cms/log/search,耗时：34.9185 毫秒', 'GET', '/cms/log/search', 200, '2019-07-29 21:02:58', 2, '12', 697, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":\"12\",\"Start\":null,\"End\":null,\"Count\":15,\"Page\":0}}\n耗时：34.9185 毫秒');
INSERT INTO `lin_log` VALUES ('', '12访问/cms/log/search,耗时：46.5554 毫秒', 'GET', '/cms/log/search', 200, '2019-07-29 21:03:30', 2, '12', 698, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":null,\"Start\":null,\"End\":null,\"Count\":15,\"Page\":0}}\n耗时：46.5554 毫秒');
INSERT INTO `lin_log` VALUES ('', '12访问/cms/log/users,耗时：19151.7205 毫秒', 'GET', '/cms/log/users', 200, '2019-07-29 21:03:31', 2, '12', 699, '参数：{\"pageDto\":{\"Count\":5,\"Page\":0}}\n耗时：19151.7205 毫秒');
INSERT INTO `lin_log` VALUES ('', '12访问/cms/log/search,耗时：28.8683 毫秒', 'GET', '/cms/log/search', 200, '2019-07-29 21:03:40', 2, '12', 700, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":null,\"Start\":null,\"End\":null,\"Count\":15,\"Page\":0}}\n耗时：28.8683 毫秒');
INSERT INTO `lin_log` VALUES ('', '12访问/cms/log/users,耗时：47.447 毫秒', 'GET', '/cms/log/users', 200, '2019-07-29 21:03:58', 2, '12', 701, '参数：{\"pageDto\":{\"Count\":5,\"Page\":0}}\n耗时：47.447 毫秒');
INSERT INTO `lin_log` VALUES ('', '12访问/cms/log/search,耗时：30.9293 毫秒', 'GET', '/cms/log/search', 200, '2019-07-29 21:04:14', 2, '12', 702, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":null,\"Start\":null,\"End\":null,\"Count\":15,\"Page\":0}}\n耗时：30.9293 毫秒');
INSERT INTO `lin_log` VALUES ('', '12访问/cms/log/search,耗时：62.0751 毫秒', 'GET', '/cms/log/search', 200, '2019-07-29 21:04:15', 2, '12', 703, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":null,\"Start\":null,\"End\":null,\"Count\":15,\"Page\":0}}\n耗时：62.0751 毫秒');
INSERT INTO `lin_log` VALUES ('', '12访问/cms/log/users,耗时：31.9748 毫秒', 'GET', '/cms/log/users', 200, '2019-07-29 21:04:15', 2, '12', 704, '参数：{\"pageDto\":{\"Count\":5,\"Page\":0}}\n耗时：31.9748 毫秒');
INSERT INTO `lin_log` VALUES ('', '12访问/cms/log/search,耗时：42.8465 毫秒', 'GET', '/cms/log/search', 200, '2019-07-29 21:04:16', 2, '12', 705, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":null,\"Start\":null,\"End\":null,\"Count\":15,\"Page\":0}}\n耗时：42.8465 毫秒');
INSERT INTO `lin_log` VALUES ('', '12访问/cms/log/users,耗时：39.7228 毫秒', 'GET', '/cms/log/users', 200, '2019-07-29 21:05:33', 2, '12', 706, '参数：{\"pageDto\":{\"Count\":5,\"Page\":0}}\n耗时：39.7228 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/file,耗时：327.9916 毫秒', 'POST', '/cms/file', 200, '2019-07-31 12:08:48', 0, NULL, 707, '参数：{\"file\":{\"ContentDisposition\":\"form-data; name=\\\"file\\\"; filename=\\\"1555843960160-99ea3f39-f119-40ee-b89a-97e1548cbfb8.png\\\"\",\"ContentType\":\"image/png\",\"Headers\":{\"Content-Disposition\":[\"form-data; name=\\\"file\\\"; filename=\\\"1555843960160-99ea3f39-f119-40ee-b89a-97e1548cbfb8.png\\\"\"],\"Content-Type\":[\"image/png\"]},\"Length\":1525724,\"Name\":\"file\",\"FileName\":\"1555843960160-99ea3f39-f119-40ee-b89a-97e1548cbfb8.png\"}}\n耗时：327.9916 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/test/lincms-exception,耗时：192.6907 毫秒', 'GET', '/cms/test/lincms-exception', 200, '2019-07-31 12:14:45', 0, NULL, 708, '参数：{}\n耗时：192.6907 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/test/lincms-exception,耗时：5350.0277 毫秒', 'GET', '/cms/test/lincms-exception', 200, '2019-07-31 12:15:36', 0, NULL, 709, '参数：{}\n耗时：5350.0277 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/test/lincms-exception,耗时：325.4479 毫秒', 'GET', '/cms/test/lincms-exception', 200, '2019-07-31 12:17:10', 0, NULL, 710, '参数：{}\n耗时：325.4479 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/test/lincms-exception,耗时：265.3743 毫秒', 'GET', '/cms/test/lincms-exception', 200, '2019-07-31 12:17:41', 0, NULL, 711, '参数：{}\n耗时：265.3743 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/test/lincms-exception,耗时：302.8592 毫秒', 'GET', '/cms/test/lincms-exception', 200, '2019-07-31 14:33:14', 0, NULL, 712, '参数：{}\n耗时：302.8592 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/user/auths,耗时：1000.8946 毫秒', 'GET', '/cms/user/auths', 200, '2019-07-31 14:57:23', 11, 'super', 713, '参数：{}\n耗时：1000.8946 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/log/users,耗时：724.9641 毫秒', 'GET', '/cms/log/users', 200, '2019-07-31 14:57:31', 11, 'super', 714, '参数：{\"pageDto\":{\"Count\":5,\"Page\":0}}\n耗时：724.9641 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/log/,耗时：661.9336 毫秒', 'GET', '/cms/log/', 200, '2019-07-31 14:57:34', 11, 'super', 715, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":null,\"Start\":null,\"End\":null,\"Count\":15,\"Page\":0}}\n耗时：661.9336 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/admin/group/all,耗时：148.9801 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-31 14:58:03', 11, 'super', 716, '参数：{}\n耗时：148.9801 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/admin/authority,耗时：108.6133 毫秒', 'GET', '/cms/admin/authority', 200, '2019-07-31 14:58:10', 11, 'super', 717, '参数：{}\n耗时：108.6133 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/admin/group/15,耗时：63.8686 毫秒', 'GET', '/cms/admin/group/15', 200, '2019-07-31 14:58:10', 11, 'super', 718, '参数：{\"id\":15}\n耗时：63.8686 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/admin/dispatch/patch,耗时：38.2226 毫秒', 'POST', '/cms/admin/dispatch/patch', 200, '2019-07-31 14:58:17', 11, 'super', 719, '参数：{\"authDto\":{\"GroupId\":15,\"Auths\":[\"查询日志记录的用户\"]}}\n耗时：38.2226 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/admin/authority,耗时：34.5545 毫秒', 'GET', '/cms/admin/authority', 200, '2019-07-31 14:58:19', 11, 'super', 720, '参数：{}\n耗时：34.5545 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/admin/group/15,耗时：40.1473 毫秒', 'GET', '/cms/admin/group/15', 200, '2019-07-31 14:58:19', 11, 'super', 721, '参数：{\"id\":15}\n耗时：40.1473 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/file/,耗时：71.8019 毫秒', 'POST', '/cms/file/', 200, '2019-07-31 14:58:24', 11, 'super', 722, '参数：{\"file\":{\"ContentDisposition\":\"form-data; name=\\\"file\\\"; filename=\\\"avatar.jpg\\\"\",\"ContentType\":\"image/jpeg\",\"Headers\":{\"Content-Disposition\":[\"form-data; name=\\\"file\\\"; filename=\\\"avatar.jpg\\\"\"],\"Content-Type\":[\"image/jpeg\"]},\"Length\":11641,\"Name\":\"file\",\"FileName\":\"avatar.jpg\"}}\n耗时：71.8019 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/user/avatar,耗时：42.0346 毫秒', 'PUT', '/cms/user/avatar', 200, '2019-07-31 14:58:24', 11, 'super', 723, '参数：{\"avatarDto\":{\"Avatar\":\"2019/07/31/0f40818c-ddb1-4c48-9e09-6332357a38df.jpg\"}}\n耗时：42.0346 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/user/information,耗时：30.5258 毫秒', 'GET', '/cms/user/information', 200, '2019-07-31 14:58:24', 11, 'super', 724, '参数：{}\n耗时：30.5258 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/log/users,耗时：30.1564 毫秒', 'GET', '/cms/log/users', 200, '2019-07-31 14:58:53', 11, 'super', 725, '参数：{\"pageDto\":{\"Count\":5,\"Page\":0}}\n耗时：30.1564 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/log/,耗时：31.1543 毫秒', 'GET', '/cms/log/', 200, '2019-07-31 14:58:53', 11, 'super', 726, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":null,\"Start\":null,\"End\":null,\"Count\":15,\"Page\":0}}\n耗时：31.1543 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/log/search,耗时：31.3913 毫秒', 'GET', '/cms/log/search', 200, '2019-07-31 14:58:56', 11, 'super', 727, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":\"super\",\"Start\":null,\"End\":null,\"Count\":15,\"Page\":0}}\n耗时：31.3913 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/log/search,耗时：34.2303 毫秒', 'GET', '/cms/log/search', 200, '2019-07-31 14:58:57', 11, 'super', 728, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":null,\"Start\":null,\"End\":null,\"Count\":15,\"Page\":0}}\n耗时：34.2303 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/log/users,耗时：87.2497 毫秒', 'GET', '/cms/log/users', 200, '2019-07-31 14:58:57', 11, 'super', 729, '参数：{\"pageDto\":{\"Count\":5,\"Page\":0}}\n耗时：87.2497 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/log/search,耗时：34.9252 毫秒', 'GET', '/cms/log/search', 200, '2019-07-31 14:58:57', 11, 'super', 730, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":null,\"Start\":null,\"End\":null,\"Count\":15,\"Page\":0}}\n耗时：34.9252 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/log/,耗时：31.0404 毫秒', 'GET', '/cms/log/', 200, '2019-07-31 14:58:57', 11, 'super', 731, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":null,\"Start\":null,\"End\":null,\"Count\":15,\"Page\":0}}\n耗时：31.0404 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/log/search,耗时：31.1929 毫秒', 'GET', '/cms/log/search', 200, '2019-07-31 14:58:58', 11, 'super', 732, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":\"12\",\"Start\":null,\"End\":null,\"Count\":15,\"Page\":0}}\n耗时：31.1929 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/log/search,耗时：29.4886 毫秒', 'GET', '/cms/log/search', 200, '2019-07-31 14:59:00', 11, 'super', 733, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":\"12\",\"Start\":null,\"End\":null,\"Count\":15,\"Page\":1}}\n耗时：29.4886 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/log/search,耗时：30.8297 毫秒', 'GET', '/cms/log/search', 200, '2019-07-31 14:59:02', 11, 'super', 734, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":\"12\",\"Start\":null,\"End\":null,\"Count\":15,\"Page\":2}}\n耗时：30.8297 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/log/search,耗时：31.2126 毫秒', 'GET', '/cms/log/search', 200, '2019-07-31 14:59:03', 11, 'super', 735, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":\"12\",\"Start\":null,\"End\":null,\"Count\":15,\"Page\":3}}\n耗时：31.2126 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/log/search,耗时：30.2037 毫秒', 'GET', '/cms/log/search', 200, '2019-07-31 14:59:04', 11, 'super', 736, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":\"12\",\"Start\":null,\"End\":null,\"Count\":15,\"Page\":4}}\n耗时：30.2037 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/log/search,耗时：31.4164 毫秒', 'GET', '/cms/log/search', 200, '2019-07-31 14:59:05', 11, 'super', 737, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":\"12\",\"Start\":null,\"End\":null,\"Count\":15,\"Page\":5}}\n耗时：31.4164 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/log/search,耗时：30.6127 毫秒', 'GET', '/cms/log/search', 200, '2019-07-31 14:59:07', 11, 'super', 738, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":null,\"Start\":null,\"End\":null,\"Count\":15,\"Page\":0}}\n耗时：30.6127 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/log/search,耗时：95.4292 毫秒', 'GET', '/cms/log/search', 200, '2019-07-31 14:59:10', 11, 'super', 739, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":null,\"Start\":null,\"End\":null,\"Count\":15,\"Page\":0}}\n耗时：95.4292 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/log/users,耗时：32.8028 毫秒', 'GET', '/cms/log/users', 200, '2019-07-31 14:59:10', 11, 'super', 740, '参数：{\"pageDto\":{\"Count\":5,\"Page\":0}}\n耗时：32.8028 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/log/search,耗时：35.0228 毫秒', 'GET', '/cms/log/search', 200, '2019-07-31 14:59:10', 11, 'super', 741, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":null,\"Start\":null,\"End\":null,\"Count\":15,\"Page\":0}}\n耗时：35.0228 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/log/,耗时：31.3362 毫秒', 'GET', '/cms/log/', 200, '2019-07-31 14:59:10', 11, 'super', 742, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":null,\"Start\":null,\"End\":null,\"Count\":15,\"Page\":0}}\n耗时：31.3362 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/log/search,耗时：27.859 毫秒', 'GET', '/cms/log/search', 200, '2019-07-31 14:59:11', 11, 'super', 743, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":null,\"Start\":null,\"End\":null,\"Count\":15,\"Page\":1}}\n耗时：27.859 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/log/search,耗时：31.0803 毫秒', 'GET', '/cms/log/search', 200, '2019-07-31 14:59:12', 11, 'super', 744, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":null,\"Start\":null,\"End\":null,\"Count\":15,\"Page\":2}}\n耗时：31.0803 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/admin/users,耗时：61.2601 毫秒', 'GET', '/cms/admin/users', 200, '2019-07-31 14:59:17', 11, 'super', 745, '参数：{\"searchDto\":{\"GroupId\":null,\"Count\":10,\"Page\":0}}\n耗时：61.2601 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/admin/group/all,耗时：34.5626 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-31 14:59:17', 11, 'super', 746, '参数：{}\n耗时：34.5626 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/admin/8,耗时：71.1428 毫秒', 'PUT', '/cms/admin/8', 200, '2019-07-31 14:59:23', 11, 'super', 747, '参数：{\"id\":8,\"updateUserDto\":{\"Email\":\"1221@qq.com\",\"GroupId\":15}}\n耗时：71.1428 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/admin/users,耗时：33.8487 毫秒', 'GET', '/cms/admin/users', 200, '2019-07-31 14:59:23', 11, 'super', 748, '参数：{\"searchDto\":{\"GroupId\":null,\"Count\":10,\"Page\":0}}\n耗时：33.8487 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/admin/8,耗时：44.2765 毫秒', 'PUT', '/cms/admin/8', 200, '2019-07-31 14:59:28', 11, 'super', 749, '参数：{\"id\":8,\"updateUserDto\":{\"Email\":\"1221@qq.com\",\"GroupId\":2}}\n耗时：44.2765 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/admin/users,耗时：32.5514 毫秒', 'GET', '/cms/admin/users', 200, '2019-07-31 14:59:28', 11, 'super', 750, '参数：{\"searchDto\":{\"GroupId\":null,\"Count\":10,\"Page\":0}}\n耗时：32.5514 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/admin/group/all,耗时：25.9169 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-31 14:59:31', 11, 'super', 751, '参数：{}\n耗时：25.9169 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/admin/authority,耗时：30.5288 毫秒', 'GET', '/cms/admin/authority', 200, '2019-07-31 14:59:33', 11, 'super', 752, '参数：{}\n耗时：30.5288 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/admin/group/2,耗时：36.7009 毫秒', 'GET', '/cms/admin/group/2', 200, '2019-07-31 14:59:33', 11, 'super', 753, '参数：{\"id\":2}\n耗时：36.7009 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/admin/users,耗时：30.7366 毫秒', 'GET', '/cms/admin/users', 200, '2019-07-31 14:59:40', 11, 'super', 754, '参数：{\"searchDto\":{\"GroupId\":null,\"Count\":10,\"Page\":0}}\n耗时：30.7366 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/admin/group/all,耗时：30.0692 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-31 14:59:40', 11, 'super', 755, '参数：{}\n耗时：30.0692 毫秒');
INSERT INTO `lin_log` VALUES ('', '1访问/cms/user/auths,耗时：30.599 毫秒', 'GET', '/cms/user/auths', 200, '2019-07-31 14:59:52', 8, '1', 756, '参数：{}\n耗时：30.599 毫秒');
INSERT INTO `lin_log` VALUES ('', '1访问/cms/log/users,耗时：32.5845 毫秒', 'GET', '/cms/log/users', 200, '2019-07-31 14:59:55', 8, '1', 757, '参数：{\"pageDto\":{\"Count\":5,\"Page\":0}}\n耗时：32.5845 毫秒');
INSERT INTO `lin_log` VALUES ('', '1访问/cms/log/,耗时：30.5615 毫秒', 'GET', '/cms/log/', 200, '2019-07-31 14:59:55', 8, '1', 758, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":null,\"Start\":null,\"End\":null,\"Count\":15,\"Page\":0}}\n耗时：30.5615 毫秒');
INSERT INTO `lin_log` VALUES ('', '1访问/cms/file/,耗时：27.2068 毫秒', 'POST', '/cms/file/', 200, '2019-07-31 15:00:05', 8, '1', 759, '参数：{\"file\":{\"ContentDisposition\":\"form-data; name=\\\"file\\\"; filename=\\\"avatar.jpg\\\"\",\"ContentType\":\"image/jpeg\",\"Headers\":{\"Content-Disposition\":[\"form-data; name=\\\"file\\\"; filename=\\\"avatar.jpg\\\"\"],\"Content-Type\":[\"image/jpeg\"]},\"Length\":6468,\"Name\":\"file\",\"FileName\":\"avatar.jpg\"}}\n耗时：27.2068 毫秒');
INSERT INTO `lin_log` VALUES ('', '1访问/cms/user/avatar,耗时：30.0708 毫秒', 'PUT', '/cms/user/avatar', 200, '2019-07-31 15:00:05', 8, '1', 760, '参数：{\"avatarDto\":{\"Avatar\":\"2019/07/31/c706747b-a360-434c-b50f-8fa79bfc2818.jpg\"}}\n耗时：30.0708 毫秒');
INSERT INTO `lin_log` VALUES ('', '1访问/cms/user/information,耗时：32.0007 毫秒', 'GET', '/cms/user/information', 200, '2019-07-31 15:00:05', 8, '1', 761, '参数：{}\n耗时：32.0007 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/user/auths,耗时：28.7587 毫秒', 'GET', '/cms/user/auths', 200, '2019-07-31 15:00:25', 11, 'super', 762, '参数：{}\n耗时：28.7587 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/log/users,耗时：38.3442 毫秒', 'GET', '/cms/log/users', 200, '2019-07-31 15:00:39', 11, 'super', 763, '参数：{\"pageDto\":{\"Count\":5,\"Page\":0}}\n耗时：38.3442 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/log/,耗时：36.0289 毫秒', 'GET', '/cms/log/', 200, '2019-07-31 15:00:39', 11, 'super', 764, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":null,\"Start\":null,\"End\":null,\"Count\":15,\"Page\":0}}\n耗时：36.0289 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/user/auths,耗时：31.628 毫秒', 'GET', '/cms/user/auths', 200, '2019-07-31 15:00:53', 11, 'super', 765, '参数：{}\n耗时：31.628 毫秒');
INSERT INTO `lin_log` VALUES ('', '1访问/cms/log/users,耗时：29.0805 毫秒', 'GET', '/cms/log/users', 200, '2019-07-31 15:02:06', 8, '1', 766, '参数：{\"pageDto\":{\"Count\":15,\"Page\":0}}\n耗时：29.0805 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/test/lincms-exception,耗时：257.0208 毫秒', 'GET', '/cms/test/lincms-exception', 200, '2019-07-31 15:21:25', 0, NULL, 767, '参数：{}\n耗时：257.0208 毫秒');
INSERT INTO `lin_log` VALUES ('', '1访问/cms/log/search,耗时：66.5926 毫秒', 'GET', '/cms/log/search', 200, '2019-07-31 15:21:38', 8, '1', 768, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":null,\"Start\":null,\"End\":null,\"Count\":15,\"Page\":0}}\n耗时：66.5926 毫秒');
INSERT INTO `lin_log` VALUES ('', '1访问/cms/user/information,耗时：42.0256 毫秒', 'GET', '/cms/user/information', 200, '2019-07-31 15:25:00', 8, '1', 769, '参数：{}\n耗时：42.0256 毫秒');
INSERT INTO `lin_log` VALUES ('', '1访问/cms/user/information,耗时：28.07 毫秒', 'GET', '/cms/user/information', 200, '2019-07-31 15:25:16', 8, '1', 770, '参数：{}\n耗时：28.07 毫秒');
INSERT INTO `lin_log` VALUES ('', '1访问/cms/log/search,耗时：27.2045 毫秒', 'GET', '/cms/log/search', 200, '2019-07-31 15:25:36', 8, '1', 771, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":null,\"Start\":null,\"End\":null,\"Count\":15,\"Page\":0}}\n耗时：27.2045 毫秒');
INSERT INTO `lin_log` VALUES ('', '1访问/cms/user/information,耗时：156.4983 毫秒', 'GET', '/cms/user/information', 200, '2019-07-31 15:45:10', 8, '1', 772, '参数：{}\n耗时：156.4983 毫秒');
INSERT INTO `lin_log` VALUES ('', '1访问/cms/user/information,耗时：36.4948 毫秒', 'GET', '/cms/user/information', 200, '2019-07-31 15:45:12', 8, '1', 773, '参数：{}\n耗时：36.4948 毫秒');
INSERT INTO `lin_log` VALUES ('', '1访问/cms/user/information,耗时：27.1076 毫秒', 'GET', '/cms/user/information', 200, '2019-07-31 15:45:13', 8, '1', 774, '参数：{}\n耗时：27.1076 毫秒');
INSERT INTO `lin_log` VALUES ('', '1访问/cms/user/information,耗时：289.6781 毫秒', 'GET', '/cms/user/information', 200, '2019-07-31 16:06:12', 8, '1', 775, '参数：{}\n耗时：289.6781 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/cms/test/lincms-exception,耗时：139.6209 毫秒', 'GET', '/cms/test/lincms-exception', 200, '2019-07-31 16:06:17', 0, NULL, 776, '参数：{}\n耗时：139.6209 毫秒');
INSERT INTO `lin_log` VALUES ('', '1访问/cms/user/information,耗时：27.055 毫秒', 'GET', '/cms/user/information', 200, '2019-07-31 16:06:37', 8, '1', 777, '参数：{}\n耗时：27.055 毫秒');
INSERT INTO `lin_log` VALUES ('', '1访问/cms/user/information,耗时：55.6175 毫秒', 'GET', '/cms/user/information', 200, '2019-07-31 16:36:46', 8, '1', 778, '参数：{}\n耗时：55.6175 毫秒');
INSERT INTO `lin_log` VALUES ('', '1访问/cms/user/information,耗时：27.8711 毫秒', 'GET', '/cms/user/information', 200, '2019-07-31 16:36:53', 8, '1', 779, '参数：{}\n耗时：27.8711 毫秒');
INSERT INTO `lin_log` VALUES ('', '1访问/cms/user/information,耗时：26.4121 毫秒', 'GET', '/cms/user/information', 200, '2019-07-31 16:36:54', 8, '1', 780, '参数：{}\n耗时：26.4121 毫秒');
INSERT INTO `lin_log` VALUES ('', '1访问/cms/user/information,耗时：27.5528 毫秒', 'GET', '/cms/user/information', 200, '2019-07-31 16:38:18', 8, '1', 781, '参数：{}\n耗时：27.5528 毫秒');
INSERT INTO `lin_log` VALUES ('', '1访问/cms/test/lincms-exception,耗时：147.4638 毫秒', 'GET', '/cms/test/lincms-exception', 200, '2019-07-31 16:38:51', 8, '1', 782, '参数：{}\n耗时：147.4638 毫秒');
INSERT INTO `lin_log` VALUES ('', '1访问/cms/user/auths,耗时：55.6891 毫秒', 'GET', '/cms/user/auths', 200, '2019-07-31 16:39:10', 8, '1', 783, '参数：{}\n耗时：55.6891 毫秒');
INSERT INTO `lin_log` VALUES ('', '1访问/cms/user/auths,耗时：33.4124 毫秒', 'GET', '/cms/user/auths', 200, '2019-07-31 16:41:07', 8, '1', 784, '参数：{}\n耗时：33.4124 毫秒');
INSERT INTO `lin_log` VALUES ('', '1访问/cms/user/auths,耗时：30.7801 毫秒', 'GET', '/cms/user/auths', 200, '2019-07-31 16:41:08', 8, '1', 785, '参数：{}\n耗时：30.7801 毫秒');
INSERT INTO `lin_log` VALUES ('', '1访问/cms/user/auths,耗时：28.9894 毫秒', 'GET', '/cms/user/auths', 200, '2019-07-31 16:41:08', 8, '1', 786, '参数：{}\n耗时：28.9894 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/log/users,耗时：309.4649 毫秒', 'GET', '/cms/log/users', 200, '2019-07-31 17:47:43', 11, 'super', 787, '参数：{\"pageDto\":{\"Count\":5,\"Page\":0}}\n耗时：309.4649 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/log/,耗时：58.0369 毫秒', 'GET', '/cms/log/', 200, '2019-07-31 17:47:43', 11, 'super', 788, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":null,\"Start\":null,\"End\":null,\"Count\":15,\"Page\":0}}\n耗时：58.0369 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/admin/users,耗时：124.7941 毫秒', 'GET', '/cms/admin/users', 200, '2019-07-31 17:47:52', 11, 'super', 789, '参数：{\"searchDto\":{\"GroupId\":null,\"Count\":10,\"Page\":0}}\n耗时：124.7941 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/admin/group/all,耗时：41.0169 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-31 17:47:52', 11, 'super', 790, '参数：{}\n耗时：41.0169 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/log/users,耗时：31.1509 毫秒', 'GET', '/cms/log/users', 200, '2019-07-31 17:47:59', 11, 'super', 791, '参数：{\"pageDto\":{\"Count\":5,\"Page\":0}}\n耗时：31.1509 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/log/,耗时：32.259 毫秒', 'GET', '/cms/log/', 200, '2019-07-31 17:47:59', 11, 'super', 792, '参数：{\"searchDto\":{\"Keyword\":null,\"Name\":null,\"Start\":null,\"End\":null,\"Count\":15,\"Page\":0}}\n耗时：32.259 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/v1/book,耗时：268.704 毫秒', 'POST', '/v1/book', 200, '2019-07-31 18:12:28', 0, NULL, 793, '参数：{\"createBook\":{\"Author\":\"string\",\"Image\":\"string\",\"Summary\":\"string\",\"Title\":\"string\"}}\n耗时：268.704 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/v1/book/1,耗时：144.5702 毫秒', 'GET', '/v1/book/1', 200, '2019-07-31 18:13:26', 0, NULL, 794, '参数：{\"id\":1}\n耗时：144.5702 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/v1/book/2,耗时：32.1618 毫秒', 'GET', '/v1/book/2', 200, '2019-07-31 18:13:31', 0, NULL, 795, '参数：{\"id\":2}\n耗时：32.1618 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/v1/book,耗时：27.2351 毫秒', 'GET', '/v1/book', 200, '2019-07-31 18:13:53', 0, NULL, 796, '参数：{}\n耗时：27.2351 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/v1/book/1,耗时：52.6922 毫秒', 'PUT', '/v1/book/1', 200, '2019-07-31 18:14:08', 0, NULL, 797, '参数：{\"id\":1,\"updateBook\":{\"Author\":\"12\",\"Image\":\"str22ing\",\"Summary\":\"string\",\"Title\":\"string\"}}\n耗时：52.6922 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/admin/group/all,耗时：53.6452 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-31 18:15:12', 11, 'super', 798, '参数：{}\n耗时：53.6452 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/admin/users,耗时：84.5406 毫秒', 'GET', '/cms/admin/users', 200, '2019-07-31 18:15:13', 11, 'super', 799, '参数：{\"searchDto\":{\"GroupId\":null,\"Count\":10,\"Page\":0}}\n耗时：84.5406 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/admin/group/all,耗时：31.9666 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-31 18:15:13', 11, 'super', 800, '参数：{}\n耗时：31.9666 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/admin/group/all,耗时：31.7979 毫秒', 'GET', '/cms/admin/group/all', 200, '2019-07-31 18:15:17', 11, 'super', 801, '参数：{}\n耗时：31.7979 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/admin/authority,耗时：114.5876 毫秒', 'GET', '/cms/admin/authority', 200, '2019-07-31 18:15:18', 11, 'super', 802, '参数：{}\n耗时：114.5876 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/admin/group/15,耗时：42.1706 毫秒', 'GET', '/cms/admin/group/15', 200, '2019-07-31 18:15:19', 11, 'super', 803, '参数：{\"id\":15}\n耗时：42.1706 毫秒');
INSERT INTO `lin_log` VALUES ('', 'super访问/cms/admin/dispatch/patch,耗时：34.7624 毫秒', 'POST', '/cms/admin/dispatch/patch', 200, '2019-07-31 18:15:22', 11, 'super', 804, '参数：{\"authDto\":{\"GroupId\":15,\"Auths\":[\"删除图书\"]}}\n耗时：34.7624 毫秒');
INSERT INTO `lin_log` VALUES ('', '1访问/v1/book/1,耗时：40.9928 毫秒', 'DELETE', '/v1/book/1', 200, '2019-07-31 18:15:25', 8, '1', 805, '参数：{\"id\":1}\n耗时：40.9928 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/v1/book,耗时：297.3415 毫秒', 'POST', '/v1/book', 200, '2019-07-31 19:10:35', 0, NULL, 806, '参数：{\"createBook\":{\"Author\":\"23\",\"Image\":\"23\",\"Summary\":\"string\",\"Title\":\"string\"}}\n耗时：297.3415 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/v1/book,耗时：75.9694 毫秒', 'GET', '/v1/book', 200, '2019-07-31 19:10:42', 0, NULL, 807, '参数：{}\n耗时：75.9694 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/v1/book/2,耗时：36.4855 毫秒', 'GET', '/v1/book/2', 200, '2019-07-31 19:10:53', 0, NULL, 808, '参数：{\"id\":2}\n耗时：36.4855 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/v1/book/3,耗时：28.3227 毫秒', 'GET', '/v1/book/3', 200, '2019-07-31 19:10:55', 0, NULL, 809, '参数：{\"id\":3}\n耗时：28.3227 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/v1/book/2,耗时：35.1015 毫秒', 'GET', '/v1/book/2', 200, '2019-07-31 19:11:27', 0, NULL, 810, '参数：{\"id\":2}\n耗时：35.1015 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/v1/book/2,耗时：27.9644 毫秒', 'GET', '/v1/book/2', 200, '2019-07-31 19:17:01', 0, NULL, 811, '参数：{\"id\":2}\n耗时：27.9644 毫秒');
INSERT INTO `lin_log` VALUES ('', '访问/v1/book/2,耗时：66.4741 毫秒', 'PUT', '/v1/book/2', 200, '2019-07-31 19:17:11', 0, NULL, 812, '参数：{\"id\":2,\"updateBook\":{\"Author\":\"1221\",\"Image\":\"1\",\"Summary\":\"2\",\"Title\":\"3\"}}\n耗时：66.4741 毫秒');

-- ----------------------------
-- Table structure for lin_user
-- ----------------------------
DROP TABLE IF EXISTS `lin_user`;
CREATE TABLE `lin_user`  (
  `nickname` varchar(24) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `avatar` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `password` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `email` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `admin` int(11) NOT NULL,
  `active` int(11) NOT NULL,
  `group_id` int(11) NOT NULL,
  `create_user_id` bigint(20) NULL DEFAULT NULL,
  `create_time` datetime(3) NULL,
  `is_deleted` bit(1) NOT NULL,
  `delete_user_id` bigint(20) NULL DEFAULT NULL,
  `delete_time` datetime(3) NULL DEFAULT NULL,
  `update_user_id` bigint(20) NULL DEFAULT NULL,
  `update_time` datetime(3) NULL,
  `id` int(11) NOT NULL AUTO_INCREMENT,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 13 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of lin_user
-- ----------------------------
INSERT INTO `lin_user` VALUES ('admin', NULL, '46F94C8DE14FB36680850768FF1B7F2A', '', 2, 1, 2, NULL, '0001-01-01 00:00:00.000', b'0', NULL, NULL, NULL, '0001-01-01 00:00:00.000', 7);
INSERT INTO `lin_user` VALUES ('1', '2019/07/31/c706747b-a360-434c-b50f-8fa79bfc2818.jpg', '46F94C8DE14FB36680850768FF1B7F2A', '1221@qq.com', 1, 1, 15, NULL, '0001-01-01 00:00:00.000', b'0', NULL, NULL, NULL, '0001-01-01 00:00:00.000', 8);
INSERT INTO `lin_user` VALUES ('12', NULL, '46F94C8DE14FB36680850768FF1B7F2A', '', 1, 1, 2, NULL, '0001-01-01 00:00:00.000', b'0', NULL, NULL, NULL, '0001-01-01 00:00:00.000', 9);
INSERT INTO `lin_user` VALUES ('14', NULL, '123qwe', '14@qq.com', 1, 1, 2, NULL, '0001-01-01 00:00:00.000', b'0', NULL, NULL, NULL, '0001-01-01 00:00:00.000', 10);
INSERT INTO `lin_user` VALUES ('super', '2019/07/31/0f40818c-ddb1-4c48-9e09-6332357a38df.jpg', 'BEB6B72231DAAFE7D913BAA818A63F0C', '', 2, 1, 15, NULL, '0001-01-01 00:00:00.000', b'0', NULL, NULL, NULL, '0001-01-01 00:00:00.000', 11);
INSERT INTO `lin_user` VALUES ('用户名用户名用', NULL, '46F94C8DE14FB36680850768FF1B7F2A', '', 1, 1, 2, NULL, '0001-01-01 00:00:00.000', b'0', NULL, NULL, NULL, '0001-01-01 00:00:00.000', 12);

SET FOREIGN_KEY_CHECKS = 1;
