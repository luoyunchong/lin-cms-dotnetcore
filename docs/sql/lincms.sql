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

 Date: 02/09/2019 23:59:12
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for base_item
-- ----------------------------
DROP TABLE IF EXISTS `base_item`;
CREATE TABLE `base_item`  (
  `is_deleted` bit(1) NOT NULL,
  `base_type_id` int(11) NOT NULL,
  `item_code` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `item_name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `sort_code` int(11) NULL DEFAULT NULL,
  `create_user_id` bigint(20) NULL DEFAULT NULL,
  `create_time` datetime(3) NULL,
  `delete_user_id` bigint(20) NULL DEFAULT NULL,
  `delete_time` datetime(3) NULL DEFAULT NULL,
  `update_user_id` bigint(20) NULL DEFAULT NULL,
  `update_time` datetime(3) NULL,
  `id` int(11) NOT NULL AUTO_INCREMENT,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 2 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of base_item
-- ----------------------------
INSERT INTO `base_item` VALUES (b'0', 2, '.net core', '.net core', 2, NULL, '2019-09-02 22:12:27.196', NULL, NULL, NULL, '2019-09-02 22:17:38.230', 1);

-- ----------------------------
-- Table structure for base_type
-- ----------------------------
DROP TABLE IF EXISTS `base_type`;
CREATE TABLE `base_type`  (
  `type_code` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `full_name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `sort_code` int(11) NULL DEFAULT NULL,
  `create_user_id` bigint(20) NULL DEFAULT NULL,
  `create_time` datetime(3) NULL,
  `is_deleted` bit(1) NOT NULL,
  `delete_user_id` bigint(20) NULL DEFAULT NULL,
  `delete_time` datetime(3) NULL DEFAULT NULL,
  `update_user_id` bigint(20) NULL DEFAULT NULL,
  `update_time` datetime(3) NULL,
  `id` int(11) NOT NULL AUTO_INCREMENT,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 2 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of base_type
-- ----------------------------
INSERT INTO `base_type` VALUES ('tag', '标签', 3, NULL, '2019-09-02 22:11:20.440', b'0', NULL, NULL, NULL, '2019-09-02 22:15:02.309', 1);

-- ----------------------------
-- Table structure for blog_article
-- ----------------------------
DROP TABLE IF EXISTS `blog_article`;
CREATE TABLE `blog_article`  (
  `is_deleted` bit(1) NOT NULL,
  `f_id` int(11) NULL DEFAULT NULL,
  `title` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `keywords` varchar(400) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `source` varchar(400) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `excerpt` varchar(400) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `content` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `view_hits` int(11) NOT NULL,
  `comment_quantity` int(11) NOT NULL,
  `point_quantity` int(11) NOT NULL,
  `thumbnail` varchar(400) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `is_audit` bit(1) NOT NULL,
  `recommend` bit(1) NOT NULL,
  `is_stickie` bit(1) NOT NULL,
  `archive` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `article_type` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `editor` int(11) NOT NULL,
  `create_user_id` bigint(20) NULL DEFAULT NULL,
  `create_time` datetime(3) NULL,
  `delete_user_id` bigint(20) NULL DEFAULT NULL,
  `delete_time` datetime(3) NULL DEFAULT NULL,
  `update_user_id` bigint(20) NULL DEFAULT NULL,
  `update_time` datetime(3) NULL,
  `id` int(11) NOT NULL AUTO_INCREMENT,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1425 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

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
) ENGINE = InnoDB AUTO_INCREMENT = 29 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

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
) ENGINE = InnoDB AUTO_INCREMENT = 1143 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for lin_poem
-- ----------------------------
DROP TABLE IF EXISTS `lin_poem`;
CREATE TABLE `lin_poem`  (
  `author` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `content` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL,
  `dynasty` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `image` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
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
-- Records of lin_poem
-- ----------------------------
INSERT INTO `lin_poem` VALUES ('1', '2', '3', '4', '', 8, '2019-08-01 21:11:47.268', b'0', NULL, NULL, 8, '2019-08-01 21:12:43.056', 1);
INSERT INTO `lin_poem` VALUES ('string', 'string', 'string', 'string', 'string', 8, '2019-08-01 21:18:37.128', b'0', NULL, NULL, NULL, '0001-01-01 00:00:00.000', 2);

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
) ENGINE = InnoDB AUTO_INCREMENT = 1242 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of lin_user
-- ----------------------------
INSERT INTO `lin_user` VALUES ('admin', NULL, '46F94C8DE14FB36680850768FF1B7F2A', '', 2, 1, 2, NULL, '2019-08-02 23:46:08.000', b'0', NULL, NULL, NULL, '2019-08-02 23:46:08.000', 7);
INSERT INTO `lin_user` VALUES ('super', '2019/07/31/0f40818c-ddb1-4c48-9e09-6332357a38df.jpg', 'BEB6B72231DAAFE7D913BAA818A63F0C', '', 1, 1, 15, NULL, '2019-08-02 23:46:08.000', b'0', NULL, NULL, NULL, '2019-08-02 23:46:08.000', 11);

SET FOREIGN_KEY_CHECKS = 1;
