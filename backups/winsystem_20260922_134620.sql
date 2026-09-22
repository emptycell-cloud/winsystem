-- MySQL dump 10.13  Distrib 8.4.7, for Win64 (x86_64)
--
-- Host: localhost    Database: winsystem
-- ------------------------------------------------------
-- Server version	8.4.7

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Current Database: `winsystem`
--

CREATE DATABASE /*!32312 IF NOT EXISTS*/ `winsystem` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;

USE `winsystem`;

--
-- Table structure for table `sys_account`
--

DROP TABLE IF EXISTS `sys_account`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `sys_account` (
  `id` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '账号ID，如 A10001',
  `username` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '登录名（唯一）',
  `password_hash` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT 'BCrypt 哈希值，明文初始密码 admin123',
  `name` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '姓名',
  `avatar` varchar(512) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `email` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `phone` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `position` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `organization_id` varchar(24) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL COMMENT '所属组织',
  `login_type` tinyint NOT NULL DEFAULT '1' COMMENT '1=密码 2=LDAP 3=SSO 4=扫码',
  `status` tinyint NOT NULL DEFAULT '1' COMMENT '1=正常 2=锁定 3=禁用 4=待激活',
  `is_protected` tinyint NOT NULL DEFAULT '0' COMMENT '1=重点保护账号 0=普通',
  `last_login_at` datetime DEFAULT NULL,
  `last_login_ip` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `login_count` int NOT NULL DEFAULT '0',
  `password_changed_at` datetime DEFAULT NULL,
  `remark` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `token_version` int NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE KEY `uk_username` (`username`) USING BTREE,
  KEY `idx_org_id` (`organization_id`) USING BTREE,
  KEY `idx_status` (`status`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='系统账号';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `sys_account`
--

LOCK TABLES `sys_account` WRITE;
/*!40000 ALTER TABLE `sys_account` DISABLE KEYS */;
INSERT INTO `sys_account` VALUES ('A10001','admin','$2a$11$StXX62j32Agy3SkgHWCbf.ptgP9wryPMQArhxyJED6NBHgrhZ1B9C','超级管理员','','admin@nebula.com1','138100','系统管理员','10',1,1,1,'2026-09-22 12:14:33','::1',593,'2026-09-13 16:40:49','重点保护账号123','2026-08-20 03:19:21','2026-09-22 12:14:33',0),('A10002','user001','$2a$11$X4fex.fvVgC3Up//6W7YA.slFsYM.Ri6H9I3HuvauV.dbrwjVHw6e','张明轩','','zhang@nebula.com','13810010002','技术总监','1002',1,2,1,'2026-08-20 05:08:00','::1',159,'2026-09-08 02:54:48','444','2026-08-20 03:19:21','2026-09-08 02:55:34',0),('A10003','zhangsan','$2a$11$8utkOW9p8x.B7dKy8Zeeb.9qIxrKimCsWr79x7HEZj.OgvAv9BhdW','张三',NULL,'412157@qq.com','13408548112','程序员','40',1,1,0,'2026-08-29 18:12:16','::1',6,'2026-08-29 18:11:59','dsadas','2026-08-20 04:49:19','2026-08-29 18:12:16',0),('A10004','user40571000','$2a$11$QIZcjVD4Re4ptvcA.uWBWOfNMuLHiyY1Z5pygt9GvYanfKBPYwkhi','罗磊',NULL,'user40571000@nebula.com','135-37032447','法务顾问','80',1,4,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:45','2026-08-20 05:24:45',0),('A10005','user40571001','$2a$11$WsoIt/CXE/yYeHmjM4RmU.7i/M25GeHPl97AdTY6O3j81PbItHn9W','彭杰',NULL,'user40571001@nebula.com','136-60928596','行政专员','50',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:46','2026-08-20 05:35:52',0),('A10006','user40571002','$2a$11$aOTXQJtyifzUD.WI3rBX5OB3o4MNJw5FUphpqApQl5d/cgQTzy4Gq','宋念瑶',NULL,'user40571002@nebula.com','130-38748450','前端工程师','1001',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:46','2026-08-20 05:36:08',0),('A10007','user40571003','$2a$11$cGFKQTkEQWmAuzQv.mMkheTuP2cYL.WS3EJjlYgpqWJsze9.I7gtq','吴浩然',NULL,'user40571003@nebula.com','136-84039387','UI设计师','50',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:46','2026-08-20 05:24:46',0),('A10008','user40571004','$2a$11$MEfNVfadSIKlD2ya/u2hi.yuTMekNcyf5KJqOb.5ldRk2tWrDgomu','郭秀英',NULL,'user40571004@nebula.com','137-62931791','法务顾问','20',1,3,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:46','2026-08-20 05:24:46',0),('A10009','user40571005','$2a$11$6rgvAMmXTZ.sDbGTAa.Rru3U7Gduw8PLOag1SUhTJNB4r0.5132M.','宋辉',NULL,'user40571005@nebula.com','131-53676448','UI设计师','20',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:46','2026-08-20 05:24:46',0),('A10010','user40571006','$2a$11$eguUnMV9SpHq/PRxZsAjc.leaiSBXD/jFgH8H1LebWEtHEYJoMgLS','黄若曦',NULL,'user40571006@nebula.com','137-54741852','人力资源','2002',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:47','2026-08-20 05:24:47',0),('A10011','user40571007','$2a$11$q/HC697d3O3hJhlbx1yJv.NoLLYd2Q31F5n5niAUgTk7Cnc4iRS5G','唐念瑶',NULL,'user40571007@nebula.com','137-94148218','测试工程师','1003',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:47','2026-08-20 05:24:47',0),('A10012','user40571008','$2a$11$sRaPdb36TevVmV1K/n/MI.qdK3ZuRFyxXlfJpoZfluhBX.sCWZ/cu','陈桂英',NULL,'user40571008@nebula.com','137-79862199','项目经理','1003',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:47','2026-08-20 05:24:47',0),('A10013','user40571009','$2a$11$enK/qWWaUtmEVS0m2sb5q.CQ5E21KZygXxDMBUHMv6VfNtKVWjAeO','杨欣怡',NULL,'user40571009@nebula.com','135-30762881','项目经理','2002',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:47','2026-08-20 05:24:47',0),('A10014','user40571010','$2a$11$AzyKT0abtJfNB.idofV./O8cJbVOLMwigMdhXoPINkwBuePyHsofC','徐强',NULL,'user40571010@nebula.com','135-88943063','产品经理','30',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:47','2026-08-20 05:24:47',0),('A10015','user40571011','$2a$11$jEXnGh.BcexD7/DkRpbqP.DeChPce/5rG.GQEQynNQuE.LYhvklGq','李宇轩',NULL,'user40571011@nebula.com','133-41405620','UI设计师','50',1,3,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:48','2026-08-20 05:24:48',0),('A10016','user40571012','$2a$11$GYtv.jIOzPwXlS9UD1oHUuBlewJko4GtuQAC5O9De7hGrRz6DcxBK','马雅静',NULL,'user40571012@nebula.com','137-46789606','数据专员','2001',1,3,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:48','2026-08-20 05:24:48',0),('A10017','user40571013','$2a$11$kFNsyERbG/444uz6oBohmucCJsa5V8KOKkfiIMNldzQZEc5O3eQ4S','孙博文',NULL,'user40571013@nebula.com','135-51629713','行政专员','1001',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:48','2026-08-20 05:24:48',0),('A10018','user40571014','$2a$11$iyUO2FudR/8qHMi9wy19uOPO0dwVdHRKbfwEelTzOA8k8FHJETVli','胡嘉怡',NULL,'user40571014@nebula.com','135-96273989','测试工程师','30',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:48','2026-08-20 05:24:48',0),('A10019','user40571015','$2a$11$6EjWlq9DBrh41Gfsnlgx/uUKkbLZ/M9m.rS2q5UbKnUc6I7OBCouC','郑敏',NULL,'user40571015@nebula.com','138-90009109','行政专员','50',1,4,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:48','2026-08-20 05:24:48',0),('A10020','user40571016','$2a$11$k3KsZeHzt1Wf.JOg4CzeCeNfy7vYYK1/3GZoRq3XVndNkKGPQc5eG','罗娜',NULL,'user40571016@nebula.com','130-47801222','法务顾问','30',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:48','2026-08-20 05:24:48',0),('A10021','user40571017','$2a$11$YxFUsQFeFrpCxMVfCH.5fuEr6PyYWQnbny6gGLFM1GVpiJ3BYycG.','马涛',NULL,'user40571017@nebula.com','131-63401450','法务顾问','20',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:49','2026-08-20 05:24:49',0),('A10022','user40571018','$2a$11$HZpybaSaKibnau2HELldQ.W8BmNnGEg2CsPfsgGuGEou7bYzp9nzu','林艳',NULL,'user40571018@nebula.com','133-73911663','后端工程师','70',1,4,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:49','2026-08-20 05:24:49',0),('A10023','user40571019','$2a$11$98qvcpOZE6U2ud5ox7uCY.6/bdVKP70TDsiE20Sc/u.UL7vxdJgX2','周建华',NULL,'user40571019@nebula.com','138-53324774','测试工程师','60',1,2,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:49','2026-08-20 05:24:49',0),('A10024','user40571020','$2a$11$5qbhxeR8B5bnAAg1bjJ9pOjMgopKkTnt/i8uRwGTyBN4NPYT4KSzy','王博文',NULL,'user40571020@nebula.com','137-12456749','系统管理员','70',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:49','2026-08-20 05:24:49',0),('A10025','user40571021','$2a$11$iolPpRtPygWTDInOdiySrelPCoTHZvvP.jX8Ks46.eQSDKSqrEAYy','彭芳',NULL,'user40571021@nebula.com','134-79257941','测试工程师','80',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:49','2026-08-20 05:24:49',0),('A10026','user40571022','$2a$11$KsytN3ETtDxfKTexGOwxiubORZPIuAhYu4EwNVEgRxZBwSJwR5zdm','胡涛',NULL,'user40571022@nebula.com','133-21406658','前端工程师','80',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:50','2026-08-20 05:24:50',0),('A10027','user40571023','$2a$11$2HdI/k6qXsx2oYG59DNFTeun/zkoT/zDdMA6Yup9Evg2iPc.eq1mS','梁明',NULL,'user40571023@nebula.com','130-14203294','人力资源','30',1,3,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:50','2026-08-20 05:24:50',0),('A10028','user40571024','$2a$11$7P9L9hNMcsp6x1TxFchwEO7h0wfKiBSFRjpuxQte1kBdENnciajnO','邓可馨',NULL,'user40571024@nebula.com','138-42304602','运营专员','40',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:50','2026-08-20 05:24:50',0),('A10029','user40571025','$2a$11$ASELu6LQW7ytXp0Ac44Q9OxnB/PcyxtRuC4B5f84Y9d.b2dMSFLOW','梁博文',NULL,'user40571025@nebula.com','135-78162073','行政专员','1001',1,3,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:50','2026-08-20 05:24:50',0),('A10030','user40571026','$2a$11$jqzf0LMnMig9pCzXirHFq.zY1laLhNhZ4YKB0JqT/dBtJZR57i46i','李敏',NULL,'user40571026@nebula.com','136-49962151','数据专员','70',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:50','2026-08-20 05:24:50',0),('A10031','user40571027','$2a$11$dks12lG6kDmF9UB0bm9lOOLwex9Kc/TDr5qwXWaVQk6rluDNPe.C.','孙可馨',NULL,'user40571027@nebula.com','138-64005982','人力资源','2002',1,2,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:51','2026-08-20 05:24:51',0),('A10032','user40571028','$2a$11$facPk4FkCL0B0ckKgoWXu.oyuxDE5etEds3UQVN.tEJOzfEdq.qfC','黄军',NULL,'user40571028@nebula.com','135-20180627','技术总监','70',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:51','2026-08-20 05:24:51',0),('A10033','user40571029','$2a$11$5Pgg0bLZvrwdvy0NQiDE4ex5tFASo2274QmIMhaHMvH/uizrsTPEi','杨敏',NULL,'user40571029@nebula.com','131-73607657','数据专员','1001',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:51','2026-08-20 05:24:51',0),('A10034','user40571030','$2a$11$zQiDo2/3eEnLjY8bbuHyqu2iZNNAKb2fzsXlZAel9JFrqxysvMV1G','周雅静',NULL,'user40571030@nebula.com','131-74561679','技术总监','30',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:51','2026-08-20 05:24:51',0),('A10035','user40571031','$2a$11$hsrD3iEF/ytaPAMWY4a3zurSEHgJl.n2ylitpgeD712TsDhjr4Du.','吴敏',NULL,'user40571031@nebula.com','133-56169549','产品经理','30',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:51','2026-08-20 05:24:51',0),('A10036','user40571032','$2a$11$ZeshgDhXmgAhEuUub218wunZD.bJjSdhZTGmRKk7ptx2F8IUGNV/K','徐思远',NULL,'user40571032@nebula.com','133-70579074','前端工程师','20',1,4,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:52','2026-08-20 05:24:52',0),('A10037','user40571033','$2a$11$hbfvkd82gb9AN/FBETTl/e/xx4rcuTFyOaBk4u3SQxSCX0/6ACVjm','徐明轩',NULL,'user40571033@nebula.com','130-32491711','财务专员','40',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:52','2026-08-20 05:24:52',0),('A10038','user40571034','$2a$11$8VJ.zDLXa.6UDRImOV1thuWCUyc4cGb8wF0SyV59m.LdtZyVQyYZG','朱磊',NULL,'user40571034@nebula.com','134-22527062','后端工程师','1002',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:52','2026-08-20 05:24:52',0),('A10039','user40571035','$2a$11$wOBDCIGrRodHENs1MHS8WuXCe7X/Ue6KRwKwPjBzMXrmkMYR0x1dS','郑桂英',NULL,'user40571035@nebula.com','134-85897875','项目经理','2001',1,2,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:52','2026-08-20 05:24:52',0),('A10040','user40571036','$2a$11$tMl4jPcO9G/8n0rYywemVeJMZ9.9VbzYOELb2kYZhmxF7oTFKLJHm','韩明',NULL,'user40571036@nebula.com','130-66831558','前端工程师','80',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:52','2026-08-20 05:24:52',0),('A10041','user40571037','$2a$11$gM06B1oHEyAZF0XzmheJceWfpJGlRPwYuc1LOA6Iy5DFg87RZIBSG','刘桂英',NULL,'user40571037@nebula.com','130-30588663','人力资源','1001',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:53','2026-08-20 05:24:53',0),('A10042','user40571038','$2a$11$jhdmyhbnNTNdhpngMpD.N.SUNvobSc7dLF9KbAJ0u07SaizvqYV8O','曹丽',NULL,'user40571038@nebula.com','130-72001938','财务专员','40',1,4,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:53','2026-08-20 05:24:53',0),('A10043','user40571039','$2a$11$PZZNjCuzNfsPdIjLoRgaGubm2aLpnwcagBDo.3AlXisu.OaKyc.gS','彭梦琪',NULL,'user40571039@nebula.com','134-91440925','系统管理员','30',1,4,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:53','2026-08-20 05:24:53',0),('A10044','user40571040','$2a$11$UfM1G.0lEMRL9Z5VOfSw4unOCgqkJwamoIPqmfV2ocwhFaw5zqOeu','胡洋',NULL,'user40571040@nebula.com','133-50820132','项目经理','2002',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:53','2026-08-20 05:24:53',0),('A10045','user40571041','$2a$11$FwuwDA1hH7Qup2Jkex1Z7OsI6UclzMt5DNj4O.eI8ZKo.Hjt5/cze','郭辉',NULL,'user40571041@nebula.com','137-94454313','财务专员','1002',1,3,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:53','2026-08-20 05:24:53',0),('A10046','user40571042','$2a$11$qCFbCsfivRyEPqEcEJmjrOa7o50ggRr100GvGKRH7Rq0PToRqs2UO','杨建华',NULL,'user40571042@nebula.com','138-72061010','法务顾问','10',1,4,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:54','2026-08-20 05:24:54',0),('A10047','user40571043','$2a$11$3wkKZsrEKSdlWoOXln1yS.cZuyc5cKRUnAktoT9WrdCWS.mEHZteG','许晨曦',NULL,'user40571043@nebula.com','134-89940711','产品经理','20',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:54','2026-08-20 05:24:54',0),('A10048','user40571044','$2a$11$EfVgaqJcZKv6DB5nkVuGFemd8iNkJWuThM06lKIa5RnmFf4xAS7wq','邓浩然',NULL,'user40571044@nebula.com','137-43240798','市场经理','40',1,3,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:54','2026-08-20 05:24:54',0),('A10049','user40571045','$2a$11$2WRkGSOioThegn645fkUEehcUJWDtWvC9Er34LrrNHhaj/8XlRTH2','彭磊',NULL,'user40571045@nebula.com','133-10160697','项目经理','1001',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:54','2026-08-20 05:24:54',0),('A10050','user40571046','$2a$11$aWcS3yeYEYF919IYQPW0Z.bk72mTpjDJW1wV.dybju67UlWD9bjKq','曹杰',NULL,'user40571046@nebula.com','132-64288674','人力资源','60',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:54','2026-08-20 05:24:54',0),('A10051','user40571047','$2a$11$5vjBDhGI3np4/PLa2RcqmuHgJCoBPSBTMhbI7Ieu8Yc1aMoebRxkq','何明轩',NULL,'user40571047@nebula.com','138-48249067','人力资源','40',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:55','2026-08-20 05:24:55',0),('A10052','user40571048','$2a$11$4UP0V9mq36xS01u6hgNtTOqZQm1Nei4WbV.s0UADYzQvM.c69z74a','马刚',NULL,'user40571048@nebula.com','131-68842995','后端工程师','70',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:55','2026-08-20 05:24:55',0),('A10053','user40571049','$2a$11$dL8UkKbSInDcXgbeMhIIRO2721AMhNgt8f9e57RIKqVhhFwq8Wy7W','张军',NULL,'user40571049@nebula.com','137-55291436','法务顾问','50',1,2,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:55','2026-08-20 05:24:55',0),('A10054','user40571050','$2a$11$hqoltAZhFZcGVTOcw3N3RumSQun1RVpLXgs/zaEHsQGPiyu3ZWVLq','唐洋',NULL,'user40571050@nebula.com','134-44292687','产品经理','10',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:55','2026-08-20 05:24:55',0),('A10055','user40571051','$2a$11$HNW5AYoVe8dNHOInMupqKO2WsYklJOXy6jYrt5Kqqng.4Viuq3Q0a','彭静',NULL,'user40571051@nebula.com','131-93529775','技术总监','2002',1,3,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:55','2026-08-20 05:24:55',0),('A10056','user40571052','$2a$11$gAMrNOiHJYtYYUA6j1dI/OvA3BYQfOBt8w8hZQXxRg2.4vcdC0Oyy','何梓萱',NULL,'user40571052@nebula.com','135-30415590','后端工程师','70',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:56','2026-08-20 05:24:56',0),('A10057','user40571053','$2a$11$PuEQy8id4Az8MZABXwGgg.dht13d5ykypKZebq1Prkrd0GOdaxcaa','孙平',NULL,'user40571053@nebula.com','133-15942377','前端工程师','80',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:56','2026-08-20 05:24:56',0),('A10058','user40571054','$2a$11$besC5SGZRJFNzpPmlqUiMeFRdFZHTQGe6LL/YpUiyoq/crqccLXRe','彭杰',NULL,'user40571054@nebula.com','131-31287744','UI设计师','2001',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:56','2026-08-20 05:24:56',0),('A10059','user40571055','$2a$11$mx7GNCUvzhntYy.1bHRane/KIkNjjszZALTwR1CIWGdS0s3rxmRV6','王静',NULL,'user40571055@nebula.com','130-23351765','市场经理','30',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:56','2026-08-20 05:24:56',0),('A10060','user40571056','$2a$11$XrSziz2xwVyB8raiOskhqOZoCQknkshghk9ClhgAImyM/c5Nrxff2','黄子涵',NULL,'user40571056@nebula.com','132-78947779','后端工程师','80',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:56','2026-08-20 05:24:56',0),('A10061','user40571057','$2a$11$.JMzWLthNPu9u1WRwdY6seOMSKhQkkqT6b4TS9Q98rA630azQl7Se','何辉',NULL,'user40571057@nebula.com','138-98087401','后端工程师','1001',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:57','2026-08-20 05:24:57',0),('A10062','user40571058','$2a$11$G.kMdwjV479PsUixKbBoieA0LmmJIb8BvIEQMIP1pk0mX1eGZ.cDO','邓丽',NULL,'user40571058@nebula.com','136-36704350','项目经理','1003',1,2,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:57','2026-08-20 05:24:57',0),('A10063','user40571059','$2a$11$NtZtSXWMSLlFRde/KR808ubzp/T8TsFr9B0dmdY0oJFgNIua7yEQK','周敏',NULL,'user40571059@nebula.com','132-73511942','项目经理','20',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:57','2026-08-20 05:24:57',0),('A10064','user40571060','$2a$11$BiTd2pXOr2H3iKmdOsg4Ju/hrE4ga8Yh/IqdGtIbfNq9bTI74POSa','张欣怡',NULL,'user40571060@nebula.com','131-29213726','法务顾问','1001',1,2,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:57','2026-08-20 05:24:57',0),('A10065','user40571061','$2a$11$ha2C1jakrILurftP4xXOo./H1otd.FRNLWBIll2Jp68YtBurMCcpa','马辉',NULL,'user40571061@nebula.com','131-39663100','市场经理','30',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:57','2026-08-20 05:24:57',0),('A10066','user40571062','$2a$11$UvWe6hl72ZwxgaIXDtlLuO.xJ/zqtCYcA5Fv8Ar0P6EndhoSR9R7S','唐天佑',NULL,'user40571062@nebula.com','135-12463779','财务专员','80',1,4,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:57','2026-08-20 05:24:57',0),('A10067','user40571063','$2a$11$10Q3PExoOrULcgUbfklQpe5Zs6RGx0OE/8r43I0bwaZ4pzYF8Ivhy','吴建华',NULL,'user40571063@nebula.com','133-16327819','项目经理','80',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:58','2026-08-20 05:24:58',0),('A10068','user40571064','$2a$11$ZlNlvN4Tbk.ZrAB9eQ9hp.hLf70p6GvDu6xAzQAjgAYghUglU.Yfq','罗思远',NULL,'user40571064@nebula.com','133-59753859','法务顾问','40',1,3,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:58','2026-08-20 05:24:58',0),('A10069','user40571065','$2a$11$V0mv6LqijXbwaz9faeviAuPjYAecwGTs8nMxhyPVpvfc4QeaeEUpa','马建军',NULL,'user40571065@nebula.com','137-37796455','系统管理员','60',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:58','2026-08-20 05:24:58',0),('A10070','user40571066','$2a$11$tWJpFuIC7BVAogPlRQr2iurpo6iHKCiDBxtj6ZnFc5XED/B/YEAb2','朱艳',NULL,'user40571066@nebula.com','130-51837883','后端工程师','60',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:58','2026-08-20 05:24:58',0),('A10071','user40571067','$2a$11$KyqFBHFAhQH5tmZKLjxateKoB4t0GoujFxZ.esrrZd1yxnRZkKhMW','孙刚',NULL,'user40571067@nebula.com','137-26583560','UI设计师','70',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:58','2026-08-20 05:24:58',0),('A10072','user40571068','$2a$11$cjdhMwn48JE2qHGPmkvW6uT/DitmnHrx1sz95Jau1dsm0WmGLzMam','王军',NULL,'user40571068@nebula.com','130-77014956','后端工程师','2002',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:59','2026-08-20 05:24:59',0),('A10073','user40571069','$2a$11$kEGwDOyyN.4aBq0WDaqmkuj0vf1kAEi2EIQFvSJ6Ajg8uFa79WbGC','郭晨曦',NULL,'user40571069@nebula.com','131-90363098','运营专员','80',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:59','2026-08-20 05:24:59',0),('A10074','user40571070','$2a$11$.179c9SRmMHCpTEOZ56Io.QmCHKvOGaezzbjCzMLrrFPY7.EtJMBa','王博文',NULL,'user40571070@nebula.com','131-37583733','财务专员','1001',1,4,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:59','2026-08-20 05:24:59',0),('A10075','user40571071','$2a$11$upumu82skuhVmp6x9Ven4.L7yHPnuGYOJV8OfZ0xxjE..ScFhmyIa','胡雅静',NULL,'user40571071@nebula.com','138-30370808','技术总监','2002',1,3,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:59','2026-08-20 05:24:59',0),('A10076','user40571072','$2a$11$qV7svkS.aotpHUiTaA/RG.RnBUOvBlFko68DyS2A6Sle3Lew0xrba','周子墨',NULL,'user40571072@nebula.com','130-27849595','人力资源','30',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:24:59','2026-08-20 05:24:59',0),('A10077','user40571073','$2a$11$kKoxZW.Y4.GSVNZ3fl9JpOO7JWOKnM8zRXlWQebOluLst0ReUje5W','周雅静',NULL,'user40571073@nebula.com','137-99397602','系统管理员','50',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:25:00','2026-08-20 05:25:00',0),('A10078','user40571074','$2a$11$G7O1b3qU/xk7m7X7NunsYOoxcizVMQL122QYGakjoR/dPCp8zDGva','郭浩然',NULL,'user40571074@nebula.com','133-97268680','项目经理','30',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:25:00','2026-08-20 05:25:00',0),('A10079','user40571075','$2a$11$YbCEyotdjj28jLVeT6DPCuVah/fQcruw4XomlnXhR2noU7TPz1mRm','孙建华',NULL,'user40571075@nebula.com','130-63376806','运营专员','1002',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:25:00','2026-08-20 05:25:00',0),('A10080','user40571076','$2a$11$hiYhNHII66yKOr9doFhLFuOzavqmNZs3P8rTwuLmPng70FL.eWc16','马子涵',NULL,'user40571076@nebula.com','136-92145128','数据专员','60',1,4,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:25:00','2026-08-20 05:25:00',0),('A10081','user40571077','$2a$11$wBKdBx14b6XgauHm0vJGK.gLtK6iWeRcldmRJ9uaZV3dMCf/h/LQy','朱伟',NULL,'user40571077@nebula.com','132-34157263','前端工程师','20',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:25:00','2026-08-20 05:25:00',0),('A10082','user40571078','$2a$11$7NXuFJq9uWeg9eBD2A/p7.lgaGlSLIwSHKuDZqax5SDnrG42i.R9e','宋辉',NULL,'user40571078@nebula.com','137-78696077','人力资源','10',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:25:01','2026-08-20 05:25:01',0),('A10083','user40571079','$2a$11$q2Ym6ElcXt5QFif/WkJga.5dDMcK9hWYbLjJ9sLw3LYTUaufNz.Y.','罗丽',NULL,'user40571079@nebula.com','135-36951699','运营专员','1001',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:25:01','2026-08-20 05:25:01',0),('A10084','user40571080','$2a$11$YApAGUhWC3AAWeju.i3scOLHzvxs7iH.tn1HZoJBRbJhiY2JxY1gK','胡明轩',NULL,'user40571080@nebula.com','131-99454928','系统管理员','60',1,2,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:25:01','2026-08-20 05:25:01',0),('A10085','user40571081','$2a$11$nMMS2NT25.779Y41uZ3.kOmigdgRsGxqcGq1quxCCZP/5qCdJMkNO','刘思远',NULL,'user40571081@nebula.com','134-12018754','运营专员','60',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:25:01','2026-08-20 05:25:01',0),('A10086','user40571082','$2a$11$UAskcP0UzYeTNk6.w87UmObICck7Zu/IBxeFyOJQLoEk.QY1meZAe','郭俊杰',NULL,'user40571082@nebula.com','133-61326842','项目经理','20',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:25:01','2026-08-20 05:25:01',0),('A10087','user40571083','$2a$11$ll5Z1dpypJxX0YbXrWIX2eKQzMq25zWaLBCTS9zmvW0BGLmwXL5gG','胡子墨',NULL,'user40571083@nebula.com','130-37008514','财务专员','30',1,3,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:25:02','2026-08-20 05:25:02',0),('A10088','user40571084','$2a$11$vhrxQ3i2Xrpr6P1uJMPRT.IiXlZ0qRIaiaWDeqfC6tRf4tGJdlcQG','邓杰',NULL,'user40571084@nebula.com','136-65456693','项目经理','2002',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:25:02','2026-08-20 05:25:02',0),('A10089','user40571085','$2a$11$oYMaU1yY1aMxFoP4Wucv4.qbEQQIqAPRg4RuOuJKyKJ2YdML9qLga','许丽',NULL,'user40571085@nebula.com','132-49692459','财务专员','80',1,3,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:25:02','2026-08-20 05:25:02',0),('A10090','user40571086','$2a$11$CMWYDQ3WKv65aE9R0/ZnaeSgjnXz4udUqfLj4eHdH9Ibs431VkUJK','李霞',NULL,'user40571086@nebula.com','132-23288898','后端工程师','2001',1,2,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:25:02','2026-08-20 05:25:02',0),('A10091','user40571087','$2a$11$w9MRHy7.wM5VAJingclNhOyPkIKbMO/oz098tQG8zmnVSxg.Yymlu','韩明',NULL,'user40571087@nebula.com','131-30679536','技术总监','1001',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:25:02','2026-08-20 05:25:02',0),('A10092','user40571088','$2a$11$wgOL3s0fmFwgRNeakgrGnuQ4rlgexiaWbmRTucqc0KTRUGwmEJd6m','王梓萱',NULL,'user40571088@nebula.com','133-75867080','后端工程师','1002',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:25:03','2026-08-20 05:25:03',0),('A10093','user40571089','$2a$11$PKR3n/76pU8wiLAETy/yHuAUvW5O31sN6jfhi.PNfbLf6oe1Z6d2O','罗俊杰',NULL,'user40571089@nebula.com','131-45665589','行政专员','2002',1,4,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:25:03','2026-08-20 05:25:03',0),('A10094','user40571090','$2a$11$jYyT3Dpj.0zAaF4T0vSi3OBnqo5GjG5ljNXa/HYOlC.TqZmZ7EE/u','许辉',NULL,'user40571090@nebula.com','132-43558238','技术总监','10',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:25:03','2026-08-20 05:25:03',0),('A10095','user40571091','$2a$11$RrngJVk5JPR3if7B028lbeNbYicbV1Y0eglU0mtrql78x4srPkeXm','杨子涵',NULL,'user40571091@nebula.com','133-71180657','人力资源','2001',1,4,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:25:03','2026-08-20 05:25:03',0),('A10096','user40571092','$2a$11$cSOR3C5uyUD0HzjMfZZW1ehM7l/wmpqnjrSq3TEiwQse0jV4ibSyC','李念瑶',NULL,'user40571092@nebula.com','135-24221147','行政专员','30',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:25:03','2026-08-20 05:25:03',0),('A10097','user40571093','$2a$11$K6wyiZXc4uSSy5.GlGMax.Ed/JgmSgCmux.vLW1vzWQfRYuP8R3De','马宇轩',NULL,'user40571093@nebula.com','134-75795527','行政专员','2002',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:25:04','2026-08-20 05:25:04',0),('A10098','user40571094','$2a$11$tgd8FpaCTLMlohBhSG6qke8dN3TZ6jqeutAM3lmjTPpzLnI8OfW/C','黄霞',NULL,'user40571094@nebula.com','130-60280030','运营专员','70',1,3,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:25:04','2026-08-20 05:25:04',0),('A10099','user40571095','$2a$11$Vs6oMNRHfy7TChY8NI6ynuXkMTWwHUVJFlNUzTId5e.d9Kr5gpv92','彭艳',NULL,'user40571095@nebula.com','136-27822239','前端工程师','2001',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:25:04','2026-08-20 05:25:04',0),('A10100','user40571096','$2a$11$MmdavOHa7RrXOItqLpMqE.lz4JdEHMdrhEHiw2CuD0hotRXZP44fu','黄霞',NULL,'user40571096@nebula.com','136-13351932','行政专员','2002',1,2,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:25:04','2026-08-20 05:25:04',0),('A10101','user40571097','$2a$11$lWSARYMSNOOns.RrsaYDeOdcjjtrw3gZI0ssGONFZsYsycz8fxn5K','赵平',NULL,'user40571097@nebula.com','133-10773391','系统管理员','1001',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:25:04','2026-08-20 05:25:04',0),('A10102','user40571098','$2a$11$XrPviTwiGAsHxk5D1T/GHe0GxHMsrH748S1r7Wbt6mIj60uPYgji6','罗洋',NULL,'user40571098@nebula.com','131-76667813','运营专员','1003',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:25:05','2026-08-20 05:25:05',0),('A10103','user40571099','$2a$11$wLacArkjtDJgqQbtMWhbk.JS82lPK4vnX9bR3bFfT4EmKNhl.hpl2','彭静',NULL,'user40571099@nebula.com','130-79298738','后端工程师','2002',1,1,0,NULL,NULL,0,NULL,'系统自动生成','2026-08-20 05:25:05','2026-08-20 05:25:05',0),('A10104','wds','$2a$11$LnQKnl9dM0gpHT8JoY4.pua1OZICur9K61mWKCjW1kuBpUPWKsipO','11111',NULL,'','','','',1,1,0,NULL,NULL,0,NULL,'','2026-08-21 02:33:59','2026-08-21 02:38:08',0),('A10105','aaa','$2a$11$DozMlPKTgivII923991psuYfr2/obt5N2EQT99ELsAbW.2KkdUbpy','22222',NULL,'','','','30',1,1,0,'2026-09-13 16:32:03','::1',1,NULL,'','2026-08-21 02:38:39','2026-09-13 16:32:03',0);
/*!40000 ALTER TABLE `sys_account` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `sys_account_role`
--

DROP TABLE IF EXISTS `sys_account_role`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `sys_account_role` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `account_id` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '账号ID',
  `role_id` varchar(24) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE KEY `uk_account_role` (`account_id`,`role_id`) USING BTREE,
  KEY `idx_role_id` (`role_id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='账号角色关联';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `sys_account_role`
--

LOCK TABLES `sys_account_role` WRITE;
/*!40000 ALTER TABLE `sys_account_role` DISABLE KEYS */;
INSERT INTO `sys_account_role` VALUES (5,'A10105','R003','2026-08-21 02:38:39'),(6,'A10003','R003','2026-08-21 02:42:31'),(7,'A10001','R001','2026-08-21 02:47:51'),(10,'A10002','R002','2026-09-08 02:55:34');
/*!40000 ALTER TABLE `sys_account_role` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `sys_config`
--

DROP TABLE IF EXISTS `sys_config`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `sys_config` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `config_key` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '参数键，如 site.name',
  `config_value` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '参数值',
  `config_name` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '参数名称（中文）',
  `config_type` tinyint NOT NULL DEFAULT '1' COMMENT '1=string 2=number 3=boolean 4=json',
  `is_system` tinyint NOT NULL DEFAULT '0' COMMENT '1=系统内置 0=自定义',
  `remark` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL COMMENT '备注',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE KEY `uk_config_key` (`config_key`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=17 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='系统参数配置';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `sys_config`
--

LOCK TABLES `sys_config` WRITE;
/*!40000 ALTER TABLE `sys_config` DISABLE KEYS */;
INSERT INTO `sys_config` VALUES (1,'site.name','Nebula Admin','站点名称',1,1,'系统/浏览器标题栏显示的名称','2026-08-20 03:19:43','2026-09-08 03:06:29'),(2,'site.logo','http://www.xxx.com/logo.jpg','站点 Logo',1,1,'站点 Logo 图片 URL，留空使用默认','2026-08-20 03:19:43','2026-09-01 17:17:36'),(3,'site.copyright','© 2026 Nebula Admin','版权信息',1,1,'页脚显示的版权声明','2026-08-20 03:19:43','2026-08-20 03:19:43'),(4,'login.captcha','true','登录验证码',3,1,'是否开启登录图形验证码','2026-08-20 03:19:43','2026-08-20 03:19:43'),(5,'login.maxRetries','5','登录最大重试次数',2,1,'连续登录失败上限，超过锁定账号','2026-08-20 03:19:43','2026-08-20 03:19:43'),(6,'login.lockMinutes','30','账号锁定时长(分)',2,1,'达到失败上限后锁定时长（分钟）','2026-08-20 03:19:43','2026-08-20 03:19:43'),(7,'password.minLength','8','密码最小长度',2,1,'用户密码最小长度要求','2026-08-20 03:19:43','2026-08-20 03:19:43'),(8,'password.complex','true','密码复杂度校验',3,1,'是否要求包含大小写字母+数字+符号','2026-08-20 03:19:43','2026-08-20 03:19:43'),(9,'password.expireDays','90','密码过期天数',2,1,'密码过期天数，0 表示不强制','2026-08-20 03:19:43','2026-08-20 03:19:43'),(10,'session.timeout','120','会话超时(分钟)',2,1,'JWT Token 有效期（分钟）','2026-08-20 03:19:43','2026-08-20 03:19:43'),(11,'upload.maxSize','10485760','上传文件大小上限',2,1,'单文件最大字节数（默认 10MB）','2026-08-20 03:19:43','2026-08-20 03:19:43'),(12,'upload.allowedExt','jpg,jpeg,png,gif,pdf,doc,docx,xls,xlsx,zip','允许上传的扩展名',1,1,'逗号分隔的允许扩展名列表','2026-08-20 03:19:43','2026-08-20 03:19:43'),(15,'server_ip','45.234.11.128','服务器IP',1,0,'数据交换服务器IP地址','2026-09-01 17:16:27','2026-09-01 20:50:55');
/*!40000 ALTER TABLE `sys_config` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `sys_dict_data`
--

DROP TABLE IF EXISTS `sys_dict_data`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `sys_dict_data` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `dict_code` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '所属字典编码',
  `label` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '数据标签',
  `value` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '数据值',
  `color` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL COMMENT '颜色样式',
  `status` tinyint NOT NULL DEFAULT '1' COMMENT '1=正常 2=禁用',
  `sort_order` int NOT NULL DEFAULT '0' COMMENT '排序号',
  `remark` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL COMMENT '备注',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`) USING BTREE,
  KEY `idx_dict_data_code` (`dict_code`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=16 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='数据字典数据';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `sys_dict_data`
--

LOCK TABLES `sys_dict_data` WRITE;
/*!40000 ALTER TABLE `sys_dict_data` DISABLE KEYS */;
INSERT INTO `sys_dict_data` VALUES (1,'user_gender','男','1','bg-blue-500/10 text-blue-600',1,1,NULL,'2026-08-20 03:19:43','2026-08-20 03:19:43'),(2,'user_gender','女','2','bg-pink-500/10 text-pink-600',1,2,NULL,'2026-08-20 03:19:43','2026-08-20 03:19:43'),(3,'user_gender','保密','0','bg-dark-500/10 text-dark-600',1,3,'','2026-08-20 03:19:43','2026-09-01 16:19:40'),(4,'account_status','启用','1','bg-emerald-500/10 text-emerald-600',1,1,NULL,'2026-08-20 03:19:43','2026-08-20 03:19:43'),(5,'account_status','禁用','2','bg-red-500/10 text-red-600',1,2,NULL,'2026-08-20 03:19:43','2026-08-20 03:19:43'),(14,'pay_type','微信支付','weixin',NULL,1,1,'','2026-09-01 16:20:51','2026-09-01 16:20:51'),(15,'pay_type','支付宝','alipay',NULL,0,2,'','2026-09-01 16:21:08','2026-09-09 17:19:29');
/*!40000 ALTER TABLE `sys_dict_data` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `sys_dict_type`
--

DROP TABLE IF EXISTS `sys_dict_type`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `sys_dict_type` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '字典名称',
  `code` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '字典编码',
  `status` tinyint NOT NULL DEFAULT '1' COMMENT '1=正常 2=禁用',
  `sort_order` int NOT NULL DEFAULT '0' COMMENT '排序号',
  `remark` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL COMMENT '备注',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE KEY `uk_dict_type_code` (`code`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='数据字典类型';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `sys_dict_type`
--

LOCK TABLES `sys_dict_type` WRITE;
/*!40000 ALTER TABLE `sys_dict_type` DISABLE KEYS */;
INSERT INTO `sys_dict_type` VALUES (1,'用户性别','user_gender',1,1,'用户性别枚举','2026-08-20 03:19:43','2026-08-20 03:19:43'),(2,'账号状态','account_status',1,2,'账号启用/禁用状态','2026-08-20 03:19:43','2026-08-20 03:19:43'),(6,'支付类型','pay_type',1,3,'','2026-09-01 16:20:19','2026-09-01 16:20:19');
/*!40000 ALTER TABLE `sys_dict_type` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `sys_file`
--

DROP TABLE IF EXISTS `sys_file`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `sys_file` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `folder_id` bigint unsigned NOT NULL COMMENT '所属文件夹ID',
  `name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '存储文件名',
  `original_name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '原始文件名',
  `display_file_name` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `extension` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '扩展名',
  `size` bigint NOT NULL DEFAULT '0' COMMENT '文件大小（字节）',
  `mime_type` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT 'MIME类型',
  `storage_path` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '物理存储路径',
  `description` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL COMMENT '文件描述',
  `uploader` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL COMMENT '上传人',
  `uploader_id` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `visibility_scope` tinyint NOT NULL DEFAULT '1',
  `visibility_target` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `status` tinyint NOT NULL DEFAULT '1' COMMENT '1=正常 2=删除',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`) USING BTREE,
  KEY `idx_folder_id` (`folder_id`) USING BTREE,
  KEY `idx_extension` (`extension`) USING BTREE,
  KEY `idx_created_at` (`created_at`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='文件信息';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `sys_file`
--

LOCK TABLES `sys_file` WRITE;
/*!40000 ALTER TABLE `sys_file` DISABLE KEYS */;
/*!40000 ALTER TABLE `sys_file` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `sys_file_folder`
--

DROP TABLE IF EXISTS `sys_file_folder`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `sys_file_folder` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `name` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '文件夹名称',
  `parent_id` bigint unsigned DEFAULT NULL COMMENT '父级文件夹ID',
  `description` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL COMMENT '描述',
  `sort_order` int NOT NULL DEFAULT '0' COMMENT '排序',
  `status` tinyint NOT NULL DEFAULT '1' COMMENT '1=正常 2=禁用',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`) USING BTREE,
  KEY `idx_parent_id` (`parent_id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='文件文件夹目录';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `sys_file_folder`
--

LOCK TABLES `sys_file_folder` WRITE;
/*!40000 ALTER TABLE `sys_file_folder` DISABLE KEYS */;
INSERT INTO `sys_file_folder` VALUES (1,'文档资料',NULL,'存放各类文档文件',1,1,'2026-08-20 03:19:43','2026-08-20 03:19:43'),(2,'图片素材',NULL,'存放图片素材文件',2,1,'2026-08-20 03:19:43','2026-08-20 03:19:43'),(3,'其他文件',NULL,'其他文件',99,1,'2026-08-20 03:19:43','2026-08-20 03:19:43');
/*!40000 ALTER TABLE `sys_file_folder` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `sys_login_log`
--

DROP TABLE IF EXISTS `sys_login_log`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `sys_login_log` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `username` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '尝试登录的用户名',
  `account_id` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL COMMENT '关联账号 ID（失败可能为空）',
  `name` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL COMMENT '账号姓名（冗余便于展示）',
  `status` tinyint NOT NULL DEFAULT '1' COMMENT '1=成功 2=失败',
  `ip` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL COMMENT '登录 IP',
  `location` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL COMMENT '登录地点',
  `user_agent` varchar(512) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL COMMENT '浏览器 UA',
  `message` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL COMMENT '失败原因 / 成功提示',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`) USING BTREE,
  KEY `idx_username` (`username`) USING BTREE,
  KEY `idx_account_id` (`account_id`) USING BTREE,
  KEY `idx_created_at` (`created_at`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='登录日志';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `sys_login_log`
--

LOCK TABLES `sys_login_log` WRITE;
/*!40000 ALTER TABLE `sys_login_log` DISABLE KEYS */;
INSERT INTO `sys_login_log` VALUES (1,'A10001',NULL,NULL,2,'127.0.0.1',NULL,'Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9168','账号不存在','2026-08-20 03:19:54'),(2,'A10001',NULL,NULL,2,'127.0.0.1',NULL,'Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9168','账号不存在','2026-08-20 03:20:03'),(3,'admin','A10001','超级管理员',1,'127.0.0.1',NULL,'Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9168','登录成功','2026-08-20 03:21:30'),(4,'admin','A10001','超级管理员',1,'127.0.0.1',NULL,'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0','登录成功','2026-08-20 03:22:18');
/*!40000 ALTER TABLE `sys_login_log` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `sys_menu`
--

DROP TABLE IF EXISTS `sys_menu`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `sys_menu` (
  `id` varchar(24) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '菜单ID，如 M001',
  `name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '菜单名称',
  `parent_id` varchar(24) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL COMMENT '父级ID，根节点为 NULL',
  `type` tinyint NOT NULL COMMENT '1=目录 2=菜单页面 3=权限按钮',
  `path` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL COMMENT '前端路由，如 /system/users',
  `component` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL COMMENT '前端组件路径',
  `icon` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL COMMENT 'Lucide 图标名，如 UserCog',
  `permission` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL COMMENT '权限标识，如 system:users:add',
  `sort_order` int NOT NULL DEFAULT '0' COMMENT '同级排序值（升序）',
  `status` tinyint NOT NULL DEFAULT '1' COMMENT '1=启用 0=禁用',
  `is_visible` tinyint NOT NULL DEFAULT '1' COMMENT '1=显示 0=隐藏（仍可路由）',
  `remark` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`) USING BTREE,
  KEY `idx_parent_id` (`parent_id`) USING BTREE,
  KEY `idx_type` (`type`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='系统菜单';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `sys_menu`
--

LOCK TABLES `sys_menu` WRITE;
/*!40000 ALTER TABLE `sys_menu` DISABLE KEYS */;
INSERT INTO `sys_menu` VALUES ('M01','工作台',NULL,1,'/dashboard','pages/Dashboard','LayoutDashboard','dashboard',1,1,1,NULL,'2026-08-20 03:19:21','2026-09-08 03:00:17'),('M05','系统管理',NULL,1,NULL,NULL,'Settings','system',5,1,1,NULL,'2026-08-20 03:19:21','2026-08-20 03:19:21'),('M0501','菜单配置','M05',2,'/Views/MenuView.xaml','/Views/MenuView.xaml','Menu','system_menus',1,1,1,'','2026-08-20 03:19:21','2026-09-08 03:05:24'),('M0502','组织机构','M05',2,'/Views/OrganizationView.xaml','/Views/OrganizationView.xaml','Network','system_organization',2,1,1,NULL,'2026-08-20 03:19:21','2026-08-20 06:25:18'),('M0503','用户管理','M05',2,'/Views/UserView.xaml','/Views/UserView.xaml','UserCog','system_users',3,1,1,NULL,'2026-08-20 03:19:21','2026-08-20 06:15:28'),('M0504','角色权限','M05',2,'/Views/RoleView.xaml','pages/system/RolePermissions','ShieldCheck','system_roles',4,1,1,NULL,'2026-08-20 03:19:21','2026-08-21 01:15:38'),('M0506','系统参数','M05',2,'/Views/ConfigView.xaml','pages/system/SystemConfig','Settings2','system_config',6,1,1,NULL,'2026-08-20 03:19:21','2026-09-01 16:52:30'),('M0507','数据字典','M05',2,'/Views/DictTypeView.xaml','pages/system/DictManagement','BookOpen','system_dict',7,1,1,'数据字典','2026-08-20 03:19:21','2026-09-01 14:33:29'),('M06','日志管理',NULL,1,NULL,NULL,'FileText','system_logs',8,1,1,NULL,'2026-08-20 03:19:43','2026-08-20 03:19:43'),('M0601','登录日志','M06',2,'/system/login-logs','pages/system/LoginLogs','LogIn','login_log',1,1,1,NULL,'2026-08-20 03:19:43','2026-08-20 03:19:43'),('M0602','操作日志','M06',2,'/Views/OperationLogView.xaml','/Views/OperationLogView.xaml','History','operation_log',2,1,1,NULL,'2026-08-20 03:19:43','2026-09-08 02:25:09'),('M08','个人中心',NULL,1,'','','User','profile',10,1,1,'','2026-08-20 03:19:43','2026-08-21 02:56:23'),('M0801','我的消息','M08',2,'/Views/MyMessageView.xaml','pages/profile/MyMessages','Bell','my_messages',1,1,1,NULL,'2026-08-20 03:19:43','2026-08-29 18:38:37'),('M0802','个人信息','M08',2,'/Views/SettingsView.xaml','/Views/SettingsView.xaml','User','my_profile',2,1,1,'','2026-08-20 03:19:43','2026-08-21 04:05:52'),('M09','通用功能',NULL,1,NULL,NULL,'Grid','common',9,1,1,NULL,'2026-08-20 03:19:43','2026-08-20 03:19:43'),('M0902','消息通知','M09',2,'/Views/MessageView.xaml','pages/system/SystemMessages','MessageSquare','system_messages',2,1,1,NULL,'2026-08-20 03:19:43','2026-08-29 18:00:13'),('M0903','日程管理','M09',2,'/Views/ScheduleView.xaml','pages/system/ScheduleManagement','Calendar','system_schedule',3,1,1,NULL,'2026-08-20 03:19:43','2026-09-01 20:57:22');
/*!40000 ALTER TABLE `sys_menu` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `sys_message`
--

DROP TABLE IF EXISTS `sys_message`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `sys_message` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `title` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '消息标题',
  `content` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '消息内容',
  `type` tinyint NOT NULL DEFAULT '1' COMMENT '1=通知 2=公告 3=提醒',
  `status` tinyint NOT NULL DEFAULT '1' COMMENT '1=已发布 2=草稿',
  `scope` tinyint NOT NULL DEFAULT '1' COMMENT '1=全部用户 2=指定角色 3=指定部门 4=指定用户',
  `sender` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL COMMENT '发送人',
  `remark` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL COMMENT '备注',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='消息通知';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `sys_message`
--

LOCK TABLES `sys_message` WRITE;
/*!40000 ALTER TABLE `sys_message` DISABLE KEYS */;
INSERT INTO `sys_message` VALUES (1,'放假通知','放假通知\r\n打撒大大叔大婶',1,1,1,'admin','无','2026-08-29 18:00:21','2026-09-08 02:03:04'),(8,'加班通知','由于项目工期紧张，本项目所有成员周末按时加班，特殊情况提前申报。',1,1,4,'超级管理员','','2026-08-29 19:05:24','2026-09-08 03:02:53');
/*!40000 ALTER TABLE `sys_message` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `sys_message_read`
--

DROP TABLE IF EXISTS `sys_message_read`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `sys_message_read` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `message_id` bigint unsigned NOT NULL COMMENT '消息ID',
  `account_id` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '账号ID',
  `read_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '已读时间',
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE KEY `uk_message_account` (`message_id`,`account_id`) USING BTREE,
  KEY `idx_account_id` (`account_id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='消息已读记录';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `sys_message_read`
--

LOCK TABLES `sys_message_read` WRITE;
/*!40000 ALTER TABLE `sys_message_read` DISABLE KEYS */;
INSERT INTO `sys_message_read` VALUES (3,1,'A10001','2026-08-29 18:57:41'),(4,8,'A10001','2026-08-29 19:05:51');
/*!40000 ALTER TABLE `sys_message_read` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `sys_message_target`
--

DROP TABLE IF EXISTS `sys_message_target`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `sys_message_target` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `message_id` bigint unsigned NOT NULL COMMENT '消息ID',
  `target_type` tinyint NOT NULL COMMENT '2=角色 3=部门 4=用户',
  `target_id` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '目标ID',
  PRIMARY KEY (`id`) USING BTREE,
  KEY `idx_message_id` (`message_id`) USING BTREE,
  KEY `idx_target` (`target_type`,`target_id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=16 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='消息发布范围目标';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `sys_message_target`
--

LOCK TABLES `sys_message_target` WRITE;
/*!40000 ALTER TABLE `sys_message_target` DISABLE KEYS */;
INSERT INTO `sys_message_target` VALUES (10,8,4,'A10001'),(11,8,4,'A10003'),(12,8,4,'A10015'),(13,8,4,'A10024'),(14,8,4,'A10059'),(15,8,4,'A10090');
/*!40000 ALTER TABLE `sys_message_target` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `sys_operation_log`
--

DROP TABLE IF EXISTS `sys_operation_log`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `sys_operation_log` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `account_id` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL COMMENT '操作人账号 ID',
  `username` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL COMMENT '操作人用户名',
  `name` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL COMMENT '操作人姓名',
  `module` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '操作模块，如 用户管理/菜单配置',
  `action` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '操作类型：新增/编辑/删除/查询等',
  `method` varchar(16) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL COMMENT 'HTTP 方法',
  `path` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL COMMENT '请求路径',
  `detail` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL COMMENT '操作详情描述',
  `status` tinyint NOT NULL DEFAULT '1' COMMENT '1=成功 2=失败',
  `ip` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL COMMENT '操作 IP',
  `user_agent` varchar(512) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL COMMENT '浏览器 UA',
  `duration_ms` bigint NOT NULL DEFAULT '0' COMMENT '耗时（毫秒）',
  `before_data` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci COMMENT '操作前数据快照（JSON）',
  `after_data` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci COMMENT '操作后数据快照（JSON）',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`) USING BTREE,
  KEY `idx_account_id` (`account_id`) USING BTREE,
  KEY `idx_module` (`module`) USING BTREE,
  KEY `idx_created_at` (`created_at`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=187 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='操作日志';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `sys_operation_log`
--

LOCK TABLES `sys_operation_log` WRITE;
/*!40000 ALTER TABLE `sys_operation_log` DISABLE KEYS */;
INSERT INTO `sys_operation_log` VALUES (58,'A10001','admin','超级管理员','日程管理','编辑','PUT','/api/schedules/3','编辑日程：邮寄信件55',1,'::1','WinSystemClient/1.0',34,'{\"Id\":3,\"Title\":\"邮寄信件55\",\"Description\":\"55544\",\"ScheduleDate\":\"2026-09-02T00:00:00\",\"StartTime\":\"09:00\",\"EndTime\":\"18:00\",\"Color\":\"#3B82F6\",\"Location\":\"444\",\"IsCompleted\":1,\"CreatorId\":\"A10001\",\"CreatorName\":\"admin\",\"Status\":1,\"CreatedAt\":\"2026-09-08T02:03:58\",\"UpdatedAt\":\"2026-09-08T02:49:12\"}','{\"Id\":3,\"Title\":\"邮寄信件55\",\"Description\":\"55544\",\"ScheduleDate\":\"2026-09-02T00:00:00\",\"StartTime\":\"09:00\",\"EndTime\":\"18:00\",\"Color\":\"#3B82F6\",\"Location\":\"444\",\"IsCompleted\":1,\"CreatorId\":\"A10001\",\"CreatorName\":\"admin\",\"Status\":1,\"CreatedAt\":\"2026-09-08T02:03:58\",\"UpdatedAt\":\"2026-09-08T03:02:01.7021328+08:00\"}','2026-09-08 03:02:02'),(59,'A10001','admin','超级管理员','消息通知','编辑','PUT','/api/messages/8','编辑消息:加班通知',1,'::1','WinSystemClient/1.0',41,'{\"Id\":8,\"Title\":\"加班通知\",\"Content\":\"由于项目工期紧张，本项目所有成员周末按时加班，特殊情况提前申报。\",\"Type\":1,\"Status\":1,\"Scope\":4,\"Sender\":\"超级管理员\",\"Remark\":\"\",\"CreatedAt\":\"2026-08-29T19:05:24\",\"UpdatedAt\":\"2026-08-29T19:05:24\"}','{\"Id\":8,\"Title\":\"加班通知\",\"Content\":\"由于项目工期紧张，本项目所有成员周末按时加班，特殊情况提前申报。\",\"Type\":1,\"Status\":1,\"Scope\":4,\"Sender\":\"超级管理员\",\"Remark\":\"\",\"CreatedAt\":\"2026-08-29T19:05:24\",\"UpdatedAt\":\"2026-09-08T03:02:52.6589944+08:00\"}','2026-09-08 03:02:53'),(60,'A10001','admin','超级管理员','菜单配置','编辑','PUT','/api/menus/M0501','编辑菜单：菜单配置',1,'::1','WinSystemClient/1.0',3,'{\"Id\":\"M0501\",\"Name\":\"菜单配置\",\"ParentId\":\"M05\",\"Type\":2,\"Path\":\"/Views/MenuView.xaml\",\"Component\":\"/Views/MenuView.xaml\",\"Icon\":\"Menu\",\"Permission\":\"system_menus\",\"SortOrder\":1,\"Status\":1,\"IsVisible\":1,\"Remark\":\"\",\"CreatedAt\":\"2026-08-20T03:19:21\",\"UpdatedAt\":\"2026-09-08T02:28:10\",\"Children\":[],\"ParentName\":null}','{\"Id\":\"M0501\",\"Name\":\"菜单配置\",\"ParentId\":\"M05\",\"Type\":2,\"Path\":\"/Views/MenuView.xaml\",\"Component\":\"/Views/MenuView.xaml\",\"Icon\":\"Menu\",\"Permission\":\"system_menus\",\"SortOrder\":1,\"Status\":1,\"IsVisible\":1,\"Remark\":\"\",\"CreatedAt\":\"2026-08-20T03:19:21\",\"UpdatedAt\":\"2026-09-08T03:05:23.5211092+08:00\",\"Children\":[],\"ParentName\":null}','2026-09-08 03:05:24'),(61,'A10001','admin','超级管理员','组织机构','编辑','PUT','/api/organizations/40','编辑组织：运营管理中心',1,'::1','WinSystemClient/1.0',44,'{\"Id\":\"40\",\"Code\":\"OPS\",\"Name\":\"运营管理中心\",\"ParentId\":\"1\",\"Type\":2,\"Manager\":\"陈志远\",\"Phone\":\"138-1040-0001\",\"Email\":\"ops@nebula.com\",\"SortOrder\":4,\"Status\":1,\"Description\":\"用户运营、内容运营、活动策划\",\"CreatedAt\":\"2026-08-20T03:19:21\",\"UpdatedAt\":\"2026-08-20T03:19:21\",\"Children\":[],\"ParentName\":null}','{\"Id\":\"40\",\"Code\":\"OPS\",\"Name\":\"运营管理中心\",\"ParentId\":\"1\",\"Type\":2,\"Manager\":\"陈志远\",\"Phone\":\"138-1040-0001\",\"Email\":\"ops@nebula.com\",\"SortOrder\":4,\"Status\":1,\"Description\":\"用户运营、内容运营、活动策划\",\"CreatedAt\":\"2026-08-20T03:19:21\",\"UpdatedAt\":\"2026-09-08T03:05:56.7996607+08:00\",\"Children\":[],\"ParentName\":null}','2026-09-08 03:05:57'),(62,'A10001','admin','超级管理员','系统参数','编辑','PUT','/api/configs/1','编辑参数:站点名称',1,'::1','WinSystemClient/1.0',42,'{\"Id\":1,\"ConfigKey\":\"site.name\",\"ConfigValue\":\"Nebula Admin\",\"ConfigName\":\"站点名称\",\"ConfigType\":1,\"IsSystem\":1,\"Remark\":\"系统/浏览器标题栏显示的名称\",\"CreatedAt\":\"2026-08-20T03:19:43\",\"UpdatedAt\":\"2026-08-20T03:19:43\"}','{\"Id\":1,\"ConfigKey\":\"site.name\",\"ConfigValue\":\"Nebula Admin\",\"ConfigName\":\"站点名称\",\"ConfigType\":1,\"IsSystem\":1,\"Remark\":\"系统/浏览器标题栏显示的名称\",\"CreatedAt\":\"2026-08-20T03:19:43\",\"UpdatedAt\":\"2026-09-08T03:06:28.711633+08:00\"}','2026-09-08 03:06:29'),(63,NULL,'',NULL,'登录认证','登录','POST','/api/auth/login','账号登录',1,'::1','WinSystemClient/1.0',616,NULL,NULL,'2026-09-09 17:18:44'),(64,'A10001','admin','超级管理员','数据字典','编辑','PUT','/api/dict-types/data/15','编辑字典数据：支付宝',1,'::1','WinSystemClient/1.0',23,'{\"Id\":15,\"DictCode\":\"pay_type\",\"Label\":\"支付宝\",\"Value\":\"alipay\",\"Color\":null,\"Status\":1,\"SortOrder\":2,\"Remark\":\"\",\"CreatedAt\":\"2026-09-01T16:21:08\",\"UpdatedAt\":\"2026-09-01T16:21:08\"}','{\"Id\":15,\"DictCode\":\"pay_type\",\"Label\":\"支付宝\",\"Value\":\"alipay\",\"Color\":null,\"Status\":0,\"SortOrder\":2,\"Remark\":\"\",\"CreatedAt\":\"2026-09-01T16:21:08\",\"UpdatedAt\":\"2026-09-09T17:19:28.8607421+08:00\"}','2026-09-09 17:19:29'),(65,NULL,'',NULL,'登录认证','登录','POST','/api/auth/login','账号登录',1,'::1','WinSystemClient/1.0',185,NULL,NULL,'2026-09-09 17:25:37'),(66,NULL,'admin',NULL,'登录认证','登录','POST','/api/auth/login','账号登录',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',552,NULL,NULL,'2026-09-13 16:32:03'),(67,NULL,'aaa',NULL,'登录认证','登录','POST','/api/auth/login','账号登录',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',215,NULL,NULL,'2026-09-13 16:32:03'),(70,NULL,'admin',NULL,'登录认证','登录','POST','/api/auth/login','账号登录',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',173,NULL,NULL,'2026-09-13 16:32:20'),(71,NULL,'',NULL,'登录认证','登录','POST','/api/auth/login','账号登录',1,'::1','WinSystemClient/1.0',185,NULL,NULL,'2026-09-13 16:32:43'),(72,NULL,'admin',NULL,'登录认证','登录','POST','/api/auth/login','账号登录',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',522,NULL,NULL,'2026-09-13 16:35:33'),(76,NULL,'admin',NULL,'登录认证','登录','POST','/api/auth/login','账号登录',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',187,NULL,NULL,'2026-09-13 16:35:46'),(77,NULL,'admin',NULL,'登录认证','登录','POST','/api/auth/login','账号登录',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',520,NULL,NULL,'2026-09-13 16:40:32'),(78,'A10001','admin','超级管理员','登录认证','新增','POST','/api/auth/change-password','/api/auth/change-password',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',225,NULL,NULL,'2026-09-13 16:40:32'),(79,'A10001','admin','超级管理员','登录认证','新增','POST','/api/auth/change-password','/api/auth/change-password',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',350,NULL,NULL,'2026-09-13 16:40:33'),(80,NULL,'admin',NULL,'登录认证','登录','POST','/api/auth/login','账号登录',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',176,NULL,NULL,'2026-09-13 16:40:33'),(81,NULL,'user001',NULL,'登录认证','登录','POST','/api/auth/login','账号登录',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',175,NULL,NULL,'2026-09-13 16:40:33'),(82,NULL,'admin',NULL,'登录认证','登录','POST','/api/auth/login','账号登录',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',214,NULL,NULL,'2026-09-13 16:40:48'),(83,'A10001','admin','超级管理员','登录认证','新增','POST','/api/auth/change-password','/api/auth/change-password',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',311,NULL,NULL,'2026-09-13 16:40:49'),(84,NULL,'admin',NULL,'登录认证','登录','POST','/api/auth/login','账号登录',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',109,NULL,NULL,'2026-09-13 16:40:49'),(85,NULL,'hacktest',NULL,'登录认证','登录','POST','/api/auth/login','账号登录',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',222,NULL,NULL,'2026-09-13 16:44:20'),(86,NULL,'hacktest',NULL,'登录认证','登录','POST','/api/auth/login','账号登录',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',46,NULL,NULL,'2026-09-13 16:44:20'),(87,NULL,'hacktest',NULL,'登录认证','登录','POST','/api/auth/login','账号登录',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',8,NULL,NULL,'2026-09-13 16:44:20'),(88,NULL,'hacktest',NULL,'登录认证','登录','POST','/api/auth/login','账号登录',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',8,NULL,NULL,'2026-09-13 16:44:20'),(89,NULL,'hacktest',NULL,'登录认证','登录','POST','/api/auth/login','账号登录',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',8,NULL,NULL,'2026-09-13 16:44:20'),(90,NULL,'hacktest',NULL,'登录认证','登录','POST','/api/auth/login','账号登录',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',7,NULL,NULL,'2026-09-13 16:44:20'),(91,NULL,'admin',NULL,'登录认证','登录','POST','/api/auth/login','账号登录',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',238,NULL,NULL,'2026-09-13 16:44:20'),(92,NULL,'hacktest',NULL,'登录认证','登录','POST','/api/auth/login','账号登录',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',8,NULL,NULL,'2026-09-13 16:44:20'),(93,NULL,'hacktest',NULL,'登录认证','登录','POST','/api/auth/login','账号登录',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',8,NULL,NULL,'2026-09-13 16:44:20'),(94,NULL,'hacktest',NULL,'登录认证','登录','POST','/api/auth/login','账号登录',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',7,NULL,NULL,'2026-09-13 16:44:20'),(95,NULL,'hacktest',NULL,'登录认证','登录','POST','/api/auth/login','账号登录',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',7,NULL,NULL,'2026-09-13 16:44:20'),(96,NULL,'hacktest',NULL,'登录认证','登录','POST','/api/auth/login','账号登录',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',8,NULL,NULL,'2026-09-13 16:44:20'),(97,NULL,'hacktest',NULL,'登录认证','登录','POST','/api/auth/login','账号登录',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',7,NULL,NULL,'2026-09-13 16:44:20'),(98,NULL,'hacktest',NULL,'登录认证','登录','POST','/api/auth/login','账号登录',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',7,NULL,NULL,'2026-09-13 16:44:20'),(99,NULL,'hacktest',NULL,'登录认证','登录','POST','/api/auth/login','账号登录',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',7,NULL,NULL,'2026-09-13 16:44:20'),(100,NULL,'hacktest',NULL,'登录认证','登录','POST','/api/auth/login','账号登录',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',8,NULL,NULL,'2026-09-13 16:44:20'),(101,NULL,'hacktest',NULL,'登录认证','登录','POST','/api/auth/login','账号登录',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',6,NULL,NULL,'2026-09-13 16:44:20'),(102,NULL,'hacktest',NULL,'登录认证','登录','POST','/api/auth/login','账号登录',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',7,NULL,NULL,'2026-09-13 16:44:20'),(103,NULL,'hacktest',NULL,'登录认证','登录','POST','/api/auth/login','账号登录',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',226,NULL,NULL,'2026-09-13 16:45:50'),(104,NULL,'hacktest',NULL,'登录认证','登录','POST','/api/auth/login','账号登录',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',45,NULL,NULL,'2026-09-13 16:45:51'),(105,NULL,'hacktest',NULL,'登录认证','登录','POST','/api/auth/login','账号登录',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',8,NULL,NULL,'2026-09-13 16:45:51'),(106,NULL,'hacktest',NULL,'登录认证','登录','POST','/api/auth/login','账号登录',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',7,NULL,NULL,'2026-09-13 16:45:51'),(107,NULL,'hacktest',NULL,'登录认证','登录','POST','/api/auth/login','账号登录',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',7,NULL,NULL,'2026-09-13 16:45:51'),(108,NULL,'hacktest',NULL,'登录认证','登录','POST','/api/auth/login','账号登录',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',6,NULL,NULL,'2026-09-13 16:45:51'),(109,NULL,'admin',NULL,'登录认证','登录','POST','/api/auth/login','账号登录',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',5,NULL,NULL,'2026-09-13 16:45:51'),(110,NULL,'hacktest',NULL,'登录认证','登录','POST','/api/auth/login','账号登录',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',6,NULL,NULL,'2026-09-13 16:45:51'),(111,NULL,'hacktest',NULL,'登录认证','登录','POST','/api/auth/login','账号登录',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',8,NULL,NULL,'2026-09-13 16:45:51'),(112,NULL,'hacktest',NULL,'登录认证','登录','POST','/api/auth/login','账号登录',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',6,NULL,NULL,'2026-09-13 16:45:51'),(113,NULL,'hacktest',NULL,'登录认证','登录','POST','/api/auth/login','账号登录',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',221,NULL,NULL,'2026-09-13 16:46:40'),(114,NULL,'hacktest',NULL,'登录认证','登录','POST','/api/auth/login','账号登录',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',46,NULL,NULL,'2026-09-13 16:46:40'),(115,NULL,'hacktest',NULL,'登录认证','登录','POST','/api/auth/login','账号登录',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',9,NULL,NULL,'2026-09-13 16:46:40'),(116,NULL,'hacktest',NULL,'登录认证','登录','POST','/api/auth/login','账号登录',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',7,NULL,NULL,'2026-09-13 16:46:40'),(117,NULL,'hacktest',NULL,'登录认证','登录','POST','/api/auth/login','账号登录',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',7,NULL,NULL,'2026-09-13 16:46:40'),(118,NULL,'hacktest',NULL,'登录认证','登录','POST','/api/auth/login','账号登录',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',6,NULL,NULL,'2026-09-13 16:46:40'),(119,NULL,'admin',NULL,'登录认证','登录','POST','/api/auth/login','账号登录',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',244,NULL,NULL,'2026-09-13 16:46:41'),(120,NULL,'hacktest',NULL,'登录认证','登录','POST','/api/auth/login','账号登录',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',7,NULL,NULL,'2026-09-13 16:46:41'),(121,NULL,'hacktest',NULL,'登录认证','登录','POST','/api/auth/login','账号登录',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',5,NULL,NULL,'2026-09-13 16:46:41'),(122,NULL,'hacktest',NULL,'登录认证','登录','POST','/api/auth/login','账号登录',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',5,NULL,NULL,'2026-09-13 16:46:41'),(123,NULL,'admin',NULL,'登录认证','登录','POST','/api/auth/login','账号登录',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',510,NULL,NULL,'2026-09-13 16:47:04'),(124,NULL,NULL,NULL,'系统认证','登录','POST','/api/auth/login','用户登录：超级管理员',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',380,NULL,NULL,'2026-09-13 16:49:26'),(126,NULL,'admin','超级管理员','系统认证','登录','POST','/api/auth/login','用户登录：超级管理员',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',367,NULL,NULL,'2026-09-13 16:50:17'),(128,NULL,'admin','超级管理员','系统认证','登录','POST','/api/auth/login','用户登录：超级管理员',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',163,NULL,NULL,'2026-09-13 16:50:31'),(129,NULL,'admin','超级管理员','系统认证','登录','POST','/api/auth/login','用户登录：超级管理员',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',403,NULL,NULL,'2026-09-16 11:55:12'),(130,'A10001','admin','超级管理员','用户管理','新增','POST','/api/users','新增用户：归属测试',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',5,NULL,'{\"Id\":\"A10106\",\"Username\":\"ownertest\",\"Name\":\"归属测试\",\"Avatar\":null,\"Email\":null,\"Phone\":null,\"Position\":null,\"OrganizationId\":null,\"LoginType\":1,\"Status\":1,\"IsProtected\":0,\"LastLoginAt\":null,\"LastLoginIp\":null,\"LoginCount\":0,\"PasswordChangedAt\":null,\"Remark\":null,\"CreatedAt\":\"2026-09-16T11:55:12.8097865+08:00\",\"UpdatedAt\":\"2026-09-16T11:55:12.8097997+08:00\"}','2026-09-16 11:55:13'),(131,NULL,'ownertest','归属测试','系统认证','登录','POST','/api/auth/login','用户登录：归属测试',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',174,NULL,NULL,'2026-09-16 11:55:13'),(132,'A10106','ownertest','归属测试','日程管理','新增','POST','/api/schedules','新增日程：自己的日程',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',9,NULL,'{\"Id\":6,\"Title\":\"自己的日程\",\"Description\":null,\"ScheduleDate\":\"2026-09-16T00:00:00\",\"StartTime\":null,\"EndTime\":null,\"Color\":\"#3B82F6\",\"Location\":null,\"IsCompleted\":0,\"CreatorId\":\"A10106\",\"CreatorName\":\"ownertest\",\"Status\":1,\"CreatedAt\":\"2026-09-16T11:55:13.1147416+08:00\",\"UpdatedAt\":\"2026-09-16T11:55:13.1147492+08:00\"}','2026-09-16 11:55:13'),(133,'A10106','ownertest','归属测试','日程管理','编辑','PUT','/api/schedules/6','编辑日程：?????(?)',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',2,'{\"Id\":6,\"Title\":\"自己的日程\",\"Description\":null,\"ScheduleDate\":\"2026-09-16T00:00:00\",\"StartTime\":null,\"EndTime\":null,\"Color\":\"#3B82F6\",\"Location\":null,\"IsCompleted\":0,\"CreatorId\":\"A10106\",\"CreatorName\":\"ownertest\",\"Status\":1,\"CreatedAt\":\"2026-09-16T11:55:13\",\"UpdatedAt\":\"2026-09-16T11:55:13\"}','{\"Id\":6,\"Title\":\"?????(?)\",\"Description\":null,\"ScheduleDate\":\"2026-09-16T00:00:00\",\"StartTime\":null,\"EndTime\":null,\"Color\":\"#3B82F6\",\"Location\":null,\"IsCompleted\":0,\"CreatorId\":\"A10106\",\"CreatorName\":\"ownertest\",\"Status\":1,\"CreatedAt\":\"2026-09-16T11:55:13\",\"UpdatedAt\":\"2026-09-16T11:55:13.1432783+08:00\"}','2026-09-16 11:55:13'),(134,'A10106','ownertest','归属测试','日程管理','删除','DELETE','/api/schedules/6','删除日程：?????(?)',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',1,'{\"Id\":6,\"Title\":\"?????(?)\",\"Description\":null,\"ScheduleDate\":\"2026-09-16T00:00:00\",\"StartTime\":null,\"EndTime\":null,\"Color\":\"#3B82F6\",\"Location\":null,\"IsCompleted\":0,\"CreatorId\":\"A10106\",\"CreatorName\":\"ownertest\",\"Status\":1,\"CreatedAt\":\"2026-09-16T11:55:13\",\"UpdatedAt\":\"2026-09-16T11:55:13\"}',NULL,'2026-09-16 11:55:13'),(135,'A10001','admin','超级管理员','日程管理','编辑','PUT','/api/schedules/2','编辑日程：发布新版系统45',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',3,'{\"Id\":2,\"Title\":\"发布新版系统45\",\"Description\":\"发布系统新版，安全维保3\",\"ScheduleDate\":\"2026-09-02T00:00:00\",\"StartTime\":\"12:00\",\"EndTime\":\"17:00\",\"Color\":\"#3B82F6\",\"Location\":\"机房3\",\"IsCompleted\":1,\"CreatorId\":\"A10001\",\"CreatorName\":\"admin\",\"Status\":1,\"CreatedAt\":\"2026-09-01T21:26:10\",\"UpdatedAt\":\"2026-09-08T02:38:30\"}','{\"Id\":2,\"Title\":\"发布新版系统45\",\"Description\":\"admin?????\",\"ScheduleDate\":\"2026-09-02T00:00:00\",\"StartTime\":\"12:00\",\"EndTime\":\"17:00\",\"Color\":\"#3B82F6\",\"Location\":\"机房3\",\"IsCompleted\":1,\"CreatorId\":\"A10001\",\"CreatorName\":\"admin\",\"Status\":1,\"CreatedAt\":\"2026-09-01T21:26:10\",\"UpdatedAt\":\"2026-09-16T11:55:13.1748219+08:00\"}','2026-09-16 11:55:13'),(136,'A10001','admin','超级管理员','用户管理','删除','DELETE','/api/users/A10106','删除用户：归属测试',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',12,'{\"Id\":\"A10106\",\"Username\":\"ownertest\",\"Name\":\"归属测试\",\"Avatar\":null,\"Email\":null,\"Phone\":null,\"Position\":null,\"OrganizationId\":null,\"LoginType\":1,\"Status\":1,\"IsProtected\":0,\"LastLoginAt\":\"2026-09-16T11:55:13\",\"LastLoginIp\":\"::1\",\"LoginCount\":1,\"PasswordChangedAt\":null,\"Remark\":null,\"CreatedAt\":\"2026-09-16T11:55:13\",\"UpdatedAt\":\"2026-09-16T11:55:13\"}',NULL,'2026-09-16 11:55:13'),(137,NULL,'admin','超级管理员','系统认证','登录','POST','/api/auth/login','用户登录：超级管理员',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',458,NULL,NULL,'2026-09-16 14:22:49'),(138,'A10001','admin','超级管理员','用户管理','新增','POST','/api/users','新增用户：数据测试',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',41,NULL,'{\"Id\":\"A10106\",\"Username\":\"datatest\",\"Name\":\"数据测试\",\"Avatar\":null,\"Email\":null,\"Phone\":null,\"Position\":null,\"OrganizationId\":null,\"LoginType\":1,\"Status\":1,\"IsProtected\":0,\"LastLoginAt\":null,\"LastLoginIp\":null,\"LoginCount\":0,\"PasswordChangedAt\":null,\"Remark\":null,\"CreatedAt\":\"2026-09-16T14:22:49.4981489+08:00\",\"UpdatedAt\":\"2026-09-16T14:22:49.4981707+08:00\"}','2026-09-16 14:22:50'),(139,NULL,'datatest','数据测试','系统认证','登录','POST','/api/auth/login','用户登录：数据测试',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',181,NULL,NULL,'2026-09-16 14:22:50'),(140,'A10001','admin','超级管理员','用户管理','删除','DELETE','/api/users/A10106','删除用户：数据测试',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',12,'{\"Id\":\"A10106\",\"Username\":\"datatest\",\"Name\":\"数据测试\",\"Avatar\":null,\"Email\":null,\"Phone\":null,\"Position\":null,\"OrganizationId\":null,\"LoginType\":1,\"Status\":1,\"IsProtected\":0,\"LastLoginAt\":\"2026-09-16T14:22:50\",\"LastLoginIp\":\"::1\",\"LoginCount\":1,\"PasswordChangedAt\":null,\"Remark\":null,\"CreatedAt\":\"2026-09-16T14:22:49\",\"UpdatedAt\":\"2026-09-16T14:22:50\"}',NULL,'2026-09-16 14:22:50'),(141,NULL,'admin','超级管理员','系统认证','登录','POST','/api/auth/login','用户登录：超级管理员',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',436,NULL,NULL,'2026-09-16 14:24:04'),(142,NULL,'admin','超级管理员','系统认证','登录','POST','/api/auth/login','用户登录：超级管理员',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',373,NULL,NULL,'2026-09-16 14:26:02'),(143,NULL,'admin','超级管理员','系统认证','登录','POST','/api/auth/login','用户登录：超级管理员',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',369,NULL,NULL,'2026-09-16 14:27:09'),(144,NULL,'admin','超级管理员','系统认证','登录','POST','/api/auth/login','用户登录：超级管理员',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',407,NULL,NULL,'2026-09-16 14:28:04'),(145,'A10001','admin','超级管理员','用户管理','新增','POST','/api/users','新增用户：DT2',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',5,NULL,'{\"Id\":\"A10106\",\"Username\":\"datatest2\",\"Name\":\"DT2\",\"Avatar\":null,\"Email\":null,\"Phone\":null,\"Position\":null,\"OrganizationId\":null,\"LoginType\":1,\"Status\":1,\"IsProtected\":0,\"LastLoginAt\":null,\"LastLoginIp\":null,\"LoginCount\":0,\"PasswordChangedAt\":null,\"Remark\":null,\"CreatedAt\":\"2026-09-16T14:28:04.4996238+08:00\",\"UpdatedAt\":\"2026-09-16T14:28:04.49964+08:00\"}','2026-09-16 14:28:05'),(146,NULL,'datatest2','DT2','系统认证','登录','POST','/api/auth/login','用户登录：DT2',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',166,NULL,NULL,'2026-09-16 14:28:05'),(147,'A10001','admin','超级管理员','消息通知','新增','POST','/api/messages','新增消息:pub-msg',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',8,NULL,'{\"Id\":10,\"Title\":\"pub-msg\",\"Content\":\"visible to all\",\"Type\":1,\"Status\":1,\"Scope\":1,\"Sender\":\"超级管理员\",\"Remark\":null,\"CreatedAt\":\"2026-09-16T14:28:04.7477164+08:00\",\"UpdatedAt\":\"2026-09-16T14:28:04.747737+08:00\"}','2026-09-16 14:28:05'),(148,'A10001','admin','超级管理员','消息通知','新增','POST','/api/messages','新增消息:draft-msg',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',2,NULL,'{\"Id\":11,\"Title\":\"draft-msg\",\"Content\":\"draft\",\"Type\":1,\"Status\":2,\"Scope\":1,\"Sender\":\"超级管理员\",\"Remark\":null,\"CreatedAt\":\"2026-09-16T14:28:04.7839968+08:00\",\"UpdatedAt\":\"2026-09-16T14:28:04.7839969+08:00\"}','2026-09-16 14:28:05'),(149,'A10001','admin','超级管理员','消息通知','删除','DELETE','/api/messages/10','删除消息:pub-msg',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',13,'{\"Id\":10,\"Title\":\"pub-msg\",\"Content\":\"visible to all\",\"Type\":1,\"Status\":1,\"Scope\":1,\"Sender\":\"超级管理员\",\"Remark\":null,\"CreatedAt\":\"2026-09-16T14:28:05\",\"UpdatedAt\":\"2026-09-16T14:28:05\"}',NULL,'2026-09-16 14:28:05'),(150,'A10001','admin','超级管理员','消息通知','删除','DELETE','/api/messages/11','删除消息:draft-msg',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',3,'{\"Id\":11,\"Title\":\"draft-msg\",\"Content\":\"draft\",\"Type\":1,\"Status\":2,\"Scope\":1,\"Sender\":\"超级管理员\",\"Remark\":null,\"CreatedAt\":\"2026-09-16T14:28:05\",\"UpdatedAt\":\"2026-09-16T14:28:05\"}',NULL,'2026-09-16 14:28:05'),(151,'A10001','admin','超级管理员','用户管理','删除','DELETE','/api/users/A10106','删除用户：DT2',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',10,'{\"Id\":\"A10106\",\"Username\":\"datatest2\",\"Name\":\"DT2\",\"Avatar\":null,\"Email\":null,\"Phone\":null,\"Position\":null,\"OrganizationId\":null,\"LoginType\":1,\"Status\":1,\"IsProtected\":0,\"LastLoginAt\":\"2026-09-16T14:28:05\",\"LastLoginIp\":\"::1\",\"LoginCount\":1,\"PasswordChangedAt\":null,\"Remark\":null,\"CreatedAt\":\"2026-09-16T14:28:04\",\"UpdatedAt\":\"2026-09-16T14:28:05\"}',NULL,'2026-09-16 14:28:05'),(152,NULL,'admin','超级管理员','系统认证','登录','POST','/api/auth/login','用户登录：超级管理员',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',422,NULL,NULL,'2026-09-16 14:28:39'),(153,NULL,'admin','超级管理员','系统认证','登录','POST','/api/auth/login','用户登录：超级管理员',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',454,NULL,NULL,'2026-09-16 14:38:41'),(154,NULL,'admin','超级管理员','系统认证','登录','POST','/api/auth/login','用户登录：超级管理员',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',385,NULL,NULL,'2026-09-16 14:41:59'),(155,NULL,'admin','超级管理员','系统认证','登录','POST','/api/auth/login','用户登录：超级管理员',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',390,NULL,NULL,'2026-09-16 14:42:22'),(156,NULL,'admin','超级管理员','系统认证','登录','POST','/api/auth/login','用户登录：超级管理员',1,'::1','curl/8.21.0',372,NULL,NULL,'2026-09-16 14:43:22'),(157,NULL,'admin','超级管理员','系统认证','登录','POST','/api/auth/login','用户登录：超级管理员',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',361,NULL,NULL,'2026-09-16 14:44:29'),(158,'A10001','admin','超级管理员','用户管理','新增','POST','/api/users','新增用户：合规密码',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',42,NULL,'{\"Id\":\"A10106\",\"Username\":\"pwtest3\",\"Name\":\"合规密码\",\"Avatar\":null,\"Email\":null,\"Phone\":null,\"Position\":null,\"OrganizationId\":null,\"LoginType\":1,\"Status\":1,\"IsProtected\":0,\"LastLoginAt\":null,\"LastLoginIp\":null,\"LoginCount\":0,\"PasswordChangedAt\":null,\"Remark\":null,\"CreatedAt\":\"2026-09-16T14:44:29.8277552+08:00\",\"UpdatedAt\":\"2026-09-16T14:44:29.8277784+08:00\"}','2026-09-16 14:44:30'),(159,'A10001','admin','超级管理员','用户管理','重置密码','POST','/api/users/A10106/reset-password','重置用户密码:合规密码',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',154,'{\"Id\":\"A10106\",\"Username\":\"pwtest3\",\"Name\":\"合规密码\",\"Avatar\":null,\"Email\":null,\"Phone\":null,\"Position\":null,\"OrganizationId\":null,\"LoginType\":1,\"Status\":1,\"IsProtected\":0,\"LastLoginAt\":null,\"LastLoginIp\":null,\"LoginCount\":0,\"PasswordChangedAt\":null,\"Remark\":null,\"CreatedAt\":\"2026-09-16T14:44:30\",\"UpdatedAt\":\"2026-09-16T14:44:30\"}','{\"Id\":\"A10106\",\"Username\":\"pwtest3\",\"Name\":\"合规密码\",\"Avatar\":null,\"Email\":null,\"Phone\":null,\"Position\":null,\"OrganizationId\":null,\"LoginType\":1,\"Status\":1,\"IsProtected\":0,\"LastLoginAt\":null,\"LastLoginIp\":null,\"LoginCount\":0,\"PasswordChangedAt\":\"2026-09-16T14:44:30.0672078+08:00\",\"Remark\":null,\"CreatedAt\":\"2026-09-16T14:44:30\",\"UpdatedAt\":\"2026-09-16T14:44:30.0673146+08:00\"}','2026-09-16 14:44:30'),(160,NULL,'pwtest3','合规密码','系统认证','登录','POST','/api/auth/login','用户登录：合规密码',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',161,NULL,NULL,'2026-09-16 14:44:30'),(161,'A10001','admin','超级管理员','登录认证','新增','POST','/api/auth/change-password','/api/auth/change-password',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',171,NULL,NULL,'2026-09-16 14:44:30'),(162,'A10001','admin','超级管理员','登录认证','新增','POST','/api/auth/change-password','/api/auth/change-password',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',161,NULL,NULL,'2026-09-16 14:44:31'),(163,'A10001','admin','超级管理员','用户管理','删除','DELETE','/api/users/A10106','删除用户：合规密码',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',9,'{\"Id\":\"A10106\",\"Username\":\"pwtest3\",\"Name\":\"合规密码\",\"Avatar\":null,\"Email\":null,\"Phone\":null,\"Position\":null,\"OrganizationId\":null,\"LoginType\":1,\"Status\":1,\"IsProtected\":0,\"LastLoginAt\":\"2026-09-16T14:44:30\",\"LastLoginIp\":\"::1\",\"LoginCount\":1,\"PasswordChangedAt\":\"2026-09-16T14:44:30\",\"Remark\":null,\"CreatedAt\":\"2026-09-16T14:44:30\",\"UpdatedAt\":\"2026-09-16T14:44:30\"}',NULL,'2026-09-16 14:44:31'),(164,NULL,'admin','超级管理员','系统认证','登录','POST','/api/auth/login','用户登录：超级管理员',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',384,NULL,NULL,'2026-09-16 14:45:01'),(165,'A10001','admin','超级管理员','用户管理','新增','POST','/api/users','新增用户：改密测试',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',40,NULL,'{\"Id\":\"A10106\",\"Username\":\"pwtest4\",\"Name\":\"改密测试\",\"Avatar\":null,\"Email\":null,\"Phone\":null,\"Position\":null,\"OrganizationId\":null,\"LoginType\":1,\"Status\":1,\"IsProtected\":0,\"LastLoginAt\":null,\"LastLoginIp\":null,\"LoginCount\":0,\"PasswordChangedAt\":null,\"Remark\":null,\"CreatedAt\":\"2026-09-16T14:45:01.3728636+08:00\",\"UpdatedAt\":\"2026-09-16T14:45:01.3728792+08:00\"}','2026-09-16 14:45:01'),(166,NULL,'pwtest4','改密测试','系统认证','登录','POST','/api/auth/login','用户登录：改密测试',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',162,NULL,NULL,'2026-09-16 14:45:02'),(167,'A10106','pwtest4','改密测试','登录认证','新增','POST','/api/auth/change-password','/api/auth/change-password',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',191,NULL,NULL,'2026-09-16 14:45:02'),(168,'A10106','pwtest4','改密测试','登录认证','新增','POST','/api/auth/change-password','/api/auth/change-password',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',339,NULL,NULL,'2026-09-16 14:45:02'),(169,NULL,'pwtest4','改密测试','系统认证','登录','POST','/api/auth/login','用户登录：改密测试',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',165,NULL,NULL,'2026-09-16 14:45:02'),(170,'A10001','admin','超级管理员','用户管理','删除','DELETE','/api/users/A10106','删除用户：改密测试',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',12,'{\"Id\":\"A10106\",\"Username\":\"pwtest4\",\"Name\":\"改密测试\",\"Avatar\":null,\"Email\":null,\"Phone\":null,\"Position\":null,\"OrganizationId\":null,\"LoginType\":1,\"Status\":1,\"IsProtected\":0,\"LastLoginAt\":\"2026-09-16T14:45:02\",\"LastLoginIp\":\"::1\",\"LoginCount\":2,\"PasswordChangedAt\":\"2026-09-16T14:45:02\",\"Remark\":null,\"CreatedAt\":\"2026-09-16T14:45:01\",\"UpdatedAt\":\"2026-09-16T14:45:02\"}',NULL,'2026-09-16 14:45:02'),(171,NULL,'admin','超级管理员','系统认证','登录','POST','/api/auth/login','用户登录：超级管理员',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',379,NULL,NULL,'2026-09-16 14:45:20'),(172,NULL,'admin','超级管理员','系统认证','登录','POST','/api/auth/login','用户登录：超级管理员',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',401,NULL,NULL,'2026-09-22 12:00:11'),(173,NULL,'admin','超级管理员','系统认证','登录','POST','/api/auth/login','用户登录：超级管理员',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',211,NULL,NULL,'2026-09-22 12:00:42'),(174,'A10001','admin','超级管理员','用户管理','新增','POST','/api/users','新增用户：PW',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',6,NULL,'{\"Id\":\"A10106\",\"Username\":\"pwtest\",\"Name\":\"PW\",\"Avatar\":null,\"Email\":null,\"Phone\":null,\"Position\":null,\"OrganizationId\":null,\"LoginType\":1,\"Status\":1,\"IsProtected\":0,\"LastLoginAt\":null,\"LastLoginIp\":null,\"LoginCount\":0,\"TokenVersion\":0,\"PasswordChangedAt\":null,\"Remark\":null,\"CreatedAt\":\"2026-09-22T12:00:42.5981567+08:00\",\"UpdatedAt\":\"2026-09-22T12:00:42.5981696+08:00\"}','2026-09-22 12:00:43'),(175,NULL,'pwtest','PW','系统认证','登录','POST','/api/auth/login','用户登录：PW',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',156,NULL,NULL,'2026-09-22 12:00:43'),(176,'A10106','pwtest','PW','登录认证','新增','POST','/api/auth/change-password','/api/auth/change-password',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',342,NULL,NULL,'2026-09-22 12:00:43'),(177,NULL,'pwtest','PW','系统认证','登录','POST','/api/auth/login','用户登录：PW',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',165,NULL,NULL,'2026-09-22 12:00:43'),(178,'A10001','admin','超级管理员','用户管理','重置密码','POST','/api/users/A10106/reset-password','重置用户密码:PW',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',160,'{\"Id\":\"A10106\",\"Username\":\"pwtest\",\"Name\":\"PW\",\"Avatar\":null,\"Email\":null,\"Phone\":null,\"Position\":null,\"OrganizationId\":null,\"LoginType\":1,\"Status\":1,\"IsProtected\":0,\"LastLoginAt\":\"2026-09-22T12:00:43\",\"LastLoginIp\":\"::1\",\"LoginCount\":2,\"TokenVersion\":1,\"PasswordChangedAt\":\"2026-09-22T12:00:43\",\"Remark\":null,\"CreatedAt\":\"2026-09-22T12:00:43\",\"UpdatedAt\":\"2026-09-22T12:00:43\"}','{\"Id\":\"A10106\",\"Username\":\"pwtest\",\"Name\":\"PW\",\"Avatar\":null,\"Email\":null,\"Phone\":null,\"Position\":null,\"OrganizationId\":null,\"LoginType\":1,\"Status\":1,\"IsProtected\":0,\"LastLoginAt\":\"2026-09-22T12:00:43\",\"LastLoginIp\":\"::1\",\"LoginCount\":2,\"TokenVersion\":2,\"PasswordChangedAt\":\"2026-09-22T12:00:43.5783882+08:00\",\"Remark\":null,\"CreatedAt\":\"2026-09-22T12:00:43\",\"UpdatedAt\":\"2026-09-22T12:00:43.5783905+08:00\"}','2026-09-22 12:00:44'),(179,NULL,'pwtest','PW','系统认证','登录','POST','/api/auth/login','用户登录：PW',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',165,NULL,NULL,'2026-09-22 12:00:44'),(180,'A10001','admin','超级管理员','用户管理','编辑','PUT','/api/users/A10106','编辑用户：PW',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',2,'{\"Id\":\"A10106\",\"Username\":\"pwtest\",\"Name\":\"PW\",\"Avatar\":null,\"Email\":null,\"Phone\":null,\"Position\":null,\"OrganizationId\":null,\"LoginType\":1,\"Status\":1,\"IsProtected\":0,\"LastLoginAt\":\"2026-09-22T12:00:44\",\"LastLoginIp\":\"::1\",\"LoginCount\":3,\"TokenVersion\":2,\"PasswordChangedAt\":\"2026-09-22T12:00:44\",\"Remark\":null,\"CreatedAt\":\"2026-09-22T12:00:43\",\"UpdatedAt\":\"2026-09-22T12:00:44\"}','{\"Id\":\"A10106\",\"Username\":\"pwtest\",\"Name\":\"PW\",\"Avatar\":null,\"Email\":null,\"Phone\":null,\"Position\":null,\"OrganizationId\":null,\"LoginType\":1,\"Status\":3,\"IsProtected\":0,\"LastLoginAt\":\"2026-09-22T12:00:44\",\"LastLoginIp\":\"::1\",\"LoginCount\":3,\"TokenVersion\":3,\"PasswordChangedAt\":\"2026-09-22T12:00:44\",\"Remark\":null,\"CreatedAt\":\"2026-09-22T12:00:43\",\"UpdatedAt\":\"2026-09-22T12:00:43.8006984+08:00\"}','2026-09-22 12:00:44'),(181,NULL,'pwtest',NULL,'系统认证','登录','POST','/api/auth/login','账号登录失败：PW（该账号待激活）。',2,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',161,NULL,NULL,'2026-09-22 12:00:44'),(182,'A10001','admin','超级管理员','用户管理','编辑','PUT','/api/users/A10106','编辑用户：PW',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',2,'{\"Id\":\"A10106\",\"Username\":\"pwtest\",\"Name\":\"PW\",\"Avatar\":null,\"Email\":null,\"Phone\":null,\"Position\":null,\"OrganizationId\":null,\"LoginType\":1,\"Status\":3,\"IsProtected\":0,\"LastLoginAt\":\"2026-09-22T12:00:44\",\"LastLoginIp\":\"::1\",\"LoginCount\":3,\"TokenVersion\":3,\"PasswordChangedAt\":\"2026-09-22T12:00:44\",\"Remark\":null,\"CreatedAt\":\"2026-09-22T12:00:43\",\"UpdatedAt\":\"2026-09-22T12:00:44\"}','{\"Id\":\"A10106\",\"Username\":\"pwtest\",\"Name\":\"PW\",\"Avatar\":null,\"Email\":null,\"Phone\":null,\"Position\":null,\"OrganizationId\":null,\"LoginType\":1,\"Status\":1,\"IsProtected\":0,\"LastLoginAt\":\"2026-09-22T12:00:44\",\"LastLoginIp\":\"::1\",\"LoginCount\":3,\"TokenVersion\":3,\"PasswordChangedAt\":\"2026-09-22T12:00:44\",\"Remark\":null,\"CreatedAt\":\"2026-09-22T12:00:43\",\"UpdatedAt\":\"2026-09-22T12:00:44.0003057+08:00\"}','2026-09-22 12:00:44'),(183,'A10001','admin','超级管理员','用户管理','删除','DELETE','/api/users/A10106','删除用户：PW',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',12,'{\"Id\":\"A10106\",\"Username\":\"pwtest\",\"Name\":\"PW\",\"Avatar\":null,\"Email\":null,\"Phone\":null,\"Position\":null,\"OrganizationId\":null,\"LoginType\":1,\"Status\":1,\"IsProtected\":0,\"LastLoginAt\":\"2026-09-22T12:00:44\",\"LastLoginIp\":\"::1\",\"LoginCount\":3,\"TokenVersion\":3,\"PasswordChangedAt\":\"2026-09-22T12:00:44\",\"Remark\":null,\"CreatedAt\":\"2026-09-22T12:00:43\",\"UpdatedAt\":\"2026-09-22T12:00:44\"}',NULL,'2026-09-22 12:00:44'),(184,NULL,'admin','超级管理员','系统认证','登录','POST','/api/auth/login','用户登录：超级管理员',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',420,NULL,NULL,'2026-09-22 12:01:03'),(185,NULL,'admin','超级管理员','系统认证','登录','POST','/api/auth/login','用户登录：超级管理员',1,'::1','Mozilla/5.0 (Windows NT; Windows NT 10.0; zh-CN) WindowsPowerShell/5.1.26100.9444',409,NULL,NULL,'2026-09-22 12:14:26'),(186,NULL,'admin','超级管理员','系统认证','登录','POST','/api/auth/login','用户登录：超级管理员',1,'::1','WinSystemClient/1.0',231,NULL,NULL,'2026-09-22 12:14:33');
/*!40000 ALTER TABLE `sys_operation_log` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `sys_organization`
--

DROP TABLE IF EXISTS `sys_organization`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `sys_organization` (
  `id` varchar(24) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '组织ID，如 10 20 30',
  `code` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '组织编码，如 TECH / PRODUCT',
  `name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '组织名称',
  `parent_id` varchar(24) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL COMMENT '父级ID',
  `type` tinyint NOT NULL COMMENT '1=公司 2=中心/部门 3=子部门 4=小组',
  `manager` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL COMMENT '负责人姓名',
  `phone` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `email` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `sort_order` int NOT NULL DEFAULT '0',
  `status` tinyint NOT NULL DEFAULT '1' COMMENT '1=启用 0=禁用',
  `description` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE KEY `uk_code` (`code`) USING BTREE,
  KEY `idx_parent_id` (`parent_id`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='组织机构';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `sys_organization`
--

LOCK TABLES `sys_organization` WRITE;
/*!40000 ALTER TABLE `sys_organization` DISABLE KEYS */;
INSERT INTO `sys_organization` VALUES ('1','GRP','星云集团总部',NULL,1,'张明轩','010-88888888','admin@nebula.com',0,1,'集团总部，最高管理层级','2026-08-20 03:19:21','2026-08-20 03:19:21'),('10','TECH','技术研发中心','1',2,'林墨白','138-1010-0001','tech@nebula.com',1,1,'负责技术研发、架构设计、产品交付','2026-08-20 03:19:21','2026-08-20 03:19:21'),('1001','TECH-FE','前端开发部','10',3,'苏雨晴','138-1010-0002','fe@nebula.com',1,1,'Web/H5/小程序前端开发','2026-08-20 03:19:21','2026-08-20 03:19:21'),('1002','TECH-BE','后端开发部','10',3,'王梓涵','138-1010-0003','be@nebula.com',2,1,'微服务/数据库/API','2026-08-20 03:19:21','2026-08-20 03:19:21'),('1003','TECH-QA','测试部','10',3,'李思琪','138-1010-0004','qa@nebula.com',3,1,'功能/性能/自动化测试','2026-08-20 03:19:21','2026-08-20 03:19:21'),('20','PROD','产品设计中心','1',2,'赵明轩','138-1020-0001','prod@nebula.com',2,1,'产品经理与UI/UX设计','2026-08-20 03:19:21','2026-08-20 03:19:21'),('2001','PROD-PM','产品部','20',3,'周文博','138-1020-0002','pm@nebula.com',1,1,'产品规划与需求管理','2026-08-20 03:19:21','2026-08-20 03:19:21'),('2002','PROD-UI','设计部','20',3,'吴佳怡','138-1020-0003','ui@nebula.com',2,1,'UI/UX与品牌设计','2026-08-20 03:19:21','2026-08-20 03:19:21'),('30','MKT','市场营销中心','1',2,'郑浩然','138-1030-0001','mkt@nebula.com',3,1,'品牌、市场、增长','2026-08-20 03:19:21','2026-08-20 03:19:21'),('40','OPS','运营管理中心','1',2,'陈志远','138-1040-0001','ops@nebula.com',4,1,'用户运营、内容运营、活动策划','2026-08-20 03:19:21','2026-09-08 03:05:57'),('50','HR','人力资源中心','1',2,'黄思琪','138-1050-0001','hr@nebula.com',5,1,'招聘、薪酬、绩效、培训','2026-08-20 03:19:21','2026-08-20 03:19:21'),('60','FIN','财务管理中心','1',2,'徐梦琪','138-1060-0001','fin@nebula.com',6,1,'总账、应收应付、税务','2026-08-20 03:19:21','2026-08-20 03:19:21'),('70','LEGAL','法务合规中心','1',2,'何子墨','138-1070-0001','legal@nebula.com',7,1,'合同、合规、知识产权','2026-08-20 03:19:21','2026-08-20 03:19:21'),('80','ADM','行政管理中心','1',2,'罗一凡','138-1080-0001','adm@nebula.com',8,1,'行政、办公、固定资产','2026-08-20 03:19:21','2026-08-20 03:19:21');
/*!40000 ALTER TABLE `sys_organization` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `sys_organization_member`
--

DROP TABLE IF EXISTS `sys_organization_member`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `sys_organization_member` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `organization_id` varchar(24) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `user_id` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '对应 sys_account.id',
  `position` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL COMMENT '岗位，如 技术总监',
  `joined_at` date DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE KEY `uk_org_user` (`organization_id`,`user_id`) USING BTREE,
  KEY `idx_user_id` (`user_id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='组织成员';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `sys_organization_member`
--

LOCK TABLES `sys_organization_member` WRITE;
/*!40000 ALTER TABLE `sys_organization_member` DISABLE KEYS */;
INSERT INTO `sys_organization_member` VALUES (1,'10','A10002','技术总监','2024-01-10','2026-08-20 03:19:21');
/*!40000 ALTER TABLE `sys_organization_member` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `sys_role`
--

DROP TABLE IF EXISTS `sys_role`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `sys_role` (
  `id` varchar(24) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '角色ID，如 R001',
  `name` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '角色名称',
  `code` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '角色编码（唯一）',
  `description` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `color` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL COMMENT '样式类名，Tailwind class',
  `data_scope` tinyint NOT NULL DEFAULT '2' COMMENT '1=全部 2=本部门及以下 3=本部门 4=仅本人 5=自定义',
  `is_built_in` tinyint NOT NULL DEFAULT '0' COMMENT '1=内置不可删 0=自定义',
  `status` tinyint NOT NULL DEFAULT '1' COMMENT '1=启用 0=停用',
  `sort_order` int NOT NULL DEFAULT '0',
  `remark` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE KEY `uk_code` (`code`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='系统角色';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `sys_role`
--

LOCK TABLES `sys_role` WRITE;
/*!40000 ALTER TABLE `sys_role` DISABLE KEYS */;
INSERT INTO `sys_role` VALUES ('R001','超级管理员','super_admin','拥有系统所有权限，可执行任何操作','bg-red-500/10 text-red-600 dark:text-red-400 border-red-500/20',1,1,1,1,'系统内置角色，不可删除','2026-08-20 03:19:21','2026-08-20 03:19:21'),('R002','系统管理员','admin','负责系统日常管理，可管理用户、菜单、组织架构','bg-brand-500/10 text-brand-600 dark:text-brand-400 border-brand-500/20',1,1,1,2,'系统内置角色','2026-08-20 03:19:21','2026-08-20 03:19:21'),('R003','部门主管','manager','管理部门业务，可查看本部门数据并审批','bg-blue-500/10 text-blue-600 dark:text-blue-400 border-blue-500/20',2,1,1,3,'','2026-08-20 03:19:21','2026-08-20 03:19:21'),('R004','运营人员','operator','执行日常业务操作，处理订单与商品','bg-emerald-500/10 text-emerald-600 dark:text-emerald-400 border-emerald-500/20',3,1,1,4,'','2026-08-20 03:19:21','2026-08-20 03:19:21'),('R005','审计人员','auditor','查看系统操作日志与数据，不可修改','bg-purple-500/10 text-purple-600 dark:text-purple-400 border-purple-500/20',1,1,1,5,'只读审计角色','2026-08-20 03:19:21','2026-08-20 03:19:21'),('R006','只读用户','viewer','只能查看数据，无任何修改权限','bg-amber-500/10 text-amber-600 dark:text-amber-400 border-amber-500/20',4,1,1,6,'','2026-08-20 03:19:21','2026-08-20 03:19:21'),('R007','财务专员','finance','负责订单财务审核与退款审批','bg-pink-500/10 text-pink-600 dark:text-pink-400 border-pink-500/20',1,0,1,7,'','2026-08-20 03:19:21','2026-08-20 03:19:21'),('R008','客服专员','service','处理用户咨询与订单问题','bg-cyan-500/10 text-cyan-600 dark:text-cyan-400 border-cyan-500/20',3,0,0,8,'已停用角色','2026-08-20 03:19:21','2026-08-20 03:19:21'),('R009','运维人员','yw','系统运维',NULL,1,0,1,9,'','2026-08-28 11:11:53','2026-08-28 11:11:53'),('R010','财务主管','finance_manager','财务部主管',NULL,2,0,1,10,'财务部','2026-08-28 11:12:41','2026-08-28 11:12:41'),('R011','行政专员','xzzy','行政普通岗',NULL,2,0,1,11,'','2026-08-28 11:13:34','2026-08-28 11:13:34'),('R012','人事经理','hr','人事部主管',NULL,2,0,1,12,'','2026-08-28 11:14:00','2026-08-28 11:14:00');
/*!40000 ALTER TABLE `sys_role` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `sys_role_menu`
--

DROP TABLE IF EXISTS `sys_role_menu`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `sys_role_menu` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `role_id` varchar(24) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `menu_key` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '菜单权限标识，如 system_users',
  `permission_state` tinyint NOT NULL DEFAULT '1' COMMENT '1=查看 2=编辑',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE KEY `uk_role_menu` (`role_id`,`menu_key`) USING BTREE,
  KEY `idx_menu_key` (`menu_key`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=219 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='角色功能权限';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `sys_role_menu`
--

LOCK TABLES `sys_role_menu` WRITE;
/*!40000 ALTER TABLE `sys_role_menu` DISABLE KEYS */;
INSERT INTO `sys_role_menu` VALUES (24,'R007','user_list',2,'2026-08-21 01:15:53'),(25,'R007','system_roles',2,'2026-08-21 01:15:53'),(26,'R007','dashboard',2,'2026-08-21 01:15:53'),(27,'R008','system_dict',2,'2026-08-21 02:09:55'),(28,'R008','system_dict:add',2,'2026-08-21 02:09:55'),(29,'R008','system_dict:edit',2,'2026-08-21 02:09:55'),(30,'R008','system_dict:delete',2,'2026-08-21 02:09:55'),(31,'R008','system_dict:data_add',2,'2026-08-21 02:09:55'),(32,'R008','system_dict:data_edit',2,'2026-08-21 02:09:55'),(33,'R008','system_dict:data_delete',2,'2026-08-21 02:09:55'),(34,'R008','common',2,'2026-08-21 02:09:55'),(35,'R008','system_files',2,'2026-08-21 02:09:55'),(36,'R008','system_files:upload',2,'2026-08-21 02:09:55'),(37,'R008','system_files:edit',2,'2026-08-21 02:09:55'),(38,'R008','system_files:delete',2,'2026-08-21 02:09:55'),(39,'R008','system_files:folder_add',2,'2026-08-21 02:09:55'),(40,'R008','system_files:folder_delete',2,'2026-08-21 02:09:55'),(41,'R008','system_messages',2,'2026-08-21 02:09:55'),(42,'R008','system_messages:add',2,'2026-08-21 02:09:55'),(43,'R008','system_messages:edit',2,'2026-08-21 02:09:55'),(44,'R008','system_messages:delete',2,'2026-08-21 02:09:55'),(45,'R008','system_messages:publish',2,'2026-08-21 02:09:55'),(46,'R008','system_schedule',2,'2026-08-21 02:09:55'),(47,'R008','system_schedule:add',2,'2026-08-21 02:09:55'),(48,'R008','system_schedule:edit',2,'2026-08-21 02:09:55'),(49,'R008','system_schedule:delete',2,'2026-08-21 02:09:55'),(57,'R001','dashboard',2,'2026-08-21 02:47:22'),(58,'R001','users',2,'2026-08-21 02:47:22'),(59,'R001','user_list',2,'2026-08-21 02:47:22'),(60,'R001','user_list:add',2,'2026-08-21 02:47:22'),(61,'R001','user_list:edit',2,'2026-08-21 02:47:22'),(62,'R001','user_list:delete',2,'2026-08-21 02:47:22'),(63,'R001','user_list:export',2,'2026-08-21 02:47:22'),(64,'R001','user_roles',2,'2026-08-21 02:47:22'),(65,'R001','user_roles:edit',2,'2026-08-21 02:47:22'),(66,'R001','user_levels',2,'2026-08-21 02:47:22'),(67,'R001','user_levels:add',2,'2026-08-21 02:47:22'),(68,'R001','user_levels:edit',2,'2026-08-21 02:47:22'),(69,'R001','user_levels:delete',2,'2026-08-21 02:47:22'),(70,'R001','orders',2,'2026-08-21 02:47:22'),(71,'R001','order_list',2,'2026-08-21 02:47:22'),(72,'R001','order_list:edit',2,'2026-08-21 02:47:22'),(73,'R001','order_list:export',2,'2026-08-21 02:47:22'),(74,'R001','order_refunds',2,'2026-08-21 02:47:22'),(75,'R001','order_refunds:approve',2,'2026-08-21 02:47:22'),(76,'R001','order_refunds:reject',2,'2026-08-21 02:47:22'),(77,'R001','order_shipping',2,'2026-08-21 02:47:22'),(78,'R001','order_shipping:ship',2,'2026-08-21 02:47:22'),(79,'R001','products',2,'2026-08-21 02:47:22'),(80,'R001','product_list',2,'2026-08-21 02:47:22'),(81,'R001','product_list:add',2,'2026-08-21 02:47:22'),(82,'R001','product_list:edit',2,'2026-08-21 02:47:22'),(83,'R001','product_list:delete',2,'2026-08-21 02:47:22'),(84,'R001','product_categories',2,'2026-08-21 02:47:22'),(85,'R001','product_categories:add',2,'2026-08-21 02:47:22'),(86,'R001','product_categories:edit',2,'2026-08-21 02:47:22'),(87,'R001','product_categories:delete',2,'2026-08-21 02:47:22'),(88,'R001','product_inventory',2,'2026-08-21 02:47:22'),(89,'R001','product_inventory:adjust',2,'2026-08-21 02:47:22'),(90,'R001','system',2,'2026-08-21 02:47:22'),(91,'R001','system_menus',2,'2026-08-21 02:47:22'),(92,'R001','system_organization',2,'2026-08-21 02:47:22'),(93,'R001','system_users',2,'2026-08-21 02:47:22'),(94,'R001','system_users:add',2,'2026-08-21 02:47:22'),(95,'R001','system_users:edit',2,'2026-08-21 02:47:22'),(96,'R001','system_users:delete',2,'2026-08-21 02:47:22'),(97,'R001','system_users:reset_password',2,'2026-08-21 02:47:22'),(98,'R001','system_users:export',2,'2026-08-21 02:47:22'),(99,'R001','system_roles',2,'2026-08-21 02:47:22'),(100,'R001','system_roles:add',2,'2026-08-21 02:47:22'),(101,'R001','system_roles:edit',2,'2026-08-21 02:47:22'),(102,'R001','system_roles:delete',2,'2026-08-21 02:47:22'),(103,'R001','system_roles:assign',2,'2026-08-21 02:47:22'),(104,'R001','system_config',2,'2026-08-21 02:47:22'),(105,'R001','system_config:add',2,'2026-08-21 02:47:22'),(106,'R001','system_config:edit',2,'2026-08-21 02:47:22'),(107,'R001','system_config:delete',2,'2026-08-21 02:47:22'),(108,'R001','system_dict',2,'2026-08-21 02:47:22'),(109,'R001','system_dict:add',2,'2026-08-21 02:47:22'),(110,'R001','system_dict:edit',2,'2026-08-21 02:47:22'),(111,'R001','system_dict:delete',2,'2026-08-21 02:47:22'),(112,'R001','system_dict:data_add',2,'2026-08-21 02:47:22'),(113,'R001','system_dict:data_edit',2,'2026-08-21 02:47:22'),(114,'R001','system_dict:data_delete',2,'2026-08-21 02:47:22'),(115,'R001','system_logs',2,'2026-08-21 02:47:22'),(116,'R001','operation_log',2,'2026-08-21 02:47:22'),(117,'R001','profile',2,'2026-08-21 02:47:22'),(118,'R001','my_messages',2,'2026-08-21 02:47:22'),(119,'R001','my_messages:read',2,'2026-08-21 02:47:22'),(120,'R001','my_messages:delete',2,'2026-08-21 02:47:22'),(121,'R001','my_profile',2,'2026-08-21 02:47:22'),(122,'R001','my_profile:edit',2,'2026-08-21 02:47:22'),(123,'R001','common',2,'2026-08-21 02:47:22'),(124,'R001','system_files',2,'2026-08-21 02:47:22'),(125,'R001','system_files:upload',2,'2026-08-21 02:47:22'),(126,'R001','system_files:edit',2,'2026-08-21 02:47:22'),(127,'R001','system_files:delete',2,'2026-08-21 02:47:22'),(128,'R001','system_files:folder_add',2,'2026-08-21 02:47:22'),(129,'R001','system_files:folder_delete',2,'2026-08-21 02:47:22'),(130,'R001','system_messages',2,'2026-08-21 02:47:22'),(131,'R001','system_messages:add',2,'2026-08-21 02:47:22'),(132,'R001','system_messages:edit',2,'2026-08-21 02:47:22'),(133,'R001','system_messages:delete',2,'2026-08-21 02:47:22'),(134,'R001','system_messages:publish',2,'2026-08-21 02:47:22'),(135,'R001','system_schedule',2,'2026-08-21 02:47:22'),(136,'R001','system_schedule:add',2,'2026-08-21 02:47:22'),(137,'R001','system_schedule:edit',2,'2026-08-21 02:47:22'),(138,'R001','system_schedule:delete',2,'2026-08-21 02:47:22'),(169,'R003','system',2,'2026-08-21 02:50:55'),(170,'R003','system_menus',2,'2026-08-21 02:50:55'),(171,'R003','system_organization',2,'2026-08-21 02:50:55'),(172,'R003','system_users',2,'2026-08-21 02:50:55'),(173,'R003','system_users:add',2,'2026-08-21 02:50:55'),(174,'R003','system_users:edit',2,'2026-08-21 02:50:55'),(175,'R003','system_users:delete',2,'2026-08-21 02:50:55'),(176,'R003','system_users:reset_password',2,'2026-08-21 02:50:55'),(177,'R003','system_users:export',2,'2026-08-21 02:50:55'),(178,'R003','system_roles',2,'2026-08-21 02:50:55'),(179,'R003','system_roles:add',2,'2026-08-21 02:50:55'),(180,'R003','system_roles:edit',2,'2026-08-21 02:50:55'),(181,'R003','system_roles:delete',2,'2026-08-21 02:50:55'),(182,'R003','system_roles:assign',2,'2026-08-21 02:50:55'),(183,'R003','system_config',2,'2026-08-21 02:50:55'),(184,'R003','system_config:add',2,'2026-08-21 02:50:55'),(185,'R003','system_config:edit',2,'2026-08-21 02:50:55'),(186,'R003','system_config:delete',2,'2026-08-21 02:50:55'),(187,'R003','system_dict',2,'2026-08-21 02:50:55'),(188,'R003','system_dict:add',2,'2026-08-21 02:50:55'),(189,'R003','system_dict:edit',2,'2026-08-21 02:50:55'),(190,'R003','system_dict:delete',2,'2026-08-21 02:50:55'),(191,'R003','system_dict:data_add',2,'2026-08-21 02:50:55'),(192,'R003','system_dict:data_edit',2,'2026-08-21 02:50:55'),(193,'R003','system_dict:data_delete',2,'2026-08-21 02:50:55'),(194,'R003','profile',2,'2026-08-21 02:50:55'),(195,'R003','my_messages',2,'2026-08-21 02:50:55'),(196,'R003','my_messages:read',2,'2026-08-21 02:50:55'),(197,'R003','my_messages:delete',2,'2026-08-21 02:50:55'),(198,'R003','my_profile',2,'2026-08-21 02:50:55'),(199,'R003','my_profile:edit',2,'2026-08-21 02:50:55'),(200,'R003','common',2,'2026-08-21 02:50:55'),(201,'R003','system_files',2,'2026-08-21 02:50:55'),(202,'R003','system_files:upload',2,'2026-08-21 02:50:55'),(203,'R003','system_files:edit',2,'2026-08-21 02:50:55'),(204,'R003','system_files:delete',2,'2026-08-21 02:50:55'),(205,'R003','system_files:folder_add',2,'2026-08-21 02:50:55'),(206,'R003','system_files:folder_delete',2,'2026-08-21 02:50:55'),(207,'R003','system_messages',2,'2026-08-21 02:50:55'),(208,'R003','system_messages:add',2,'2026-08-21 02:50:55'),(209,'R003','system_messages:edit',2,'2026-08-21 02:50:55'),(210,'R003','system_messages:delete',2,'2026-08-21 02:50:55'),(211,'R003','system_messages:publish',2,'2026-08-21 02:50:55'),(212,'R003','system_schedule',2,'2026-08-21 02:50:55'),(213,'R003','system_schedule:add',2,'2026-08-21 02:50:55'),(214,'R003','system_schedule:edit',2,'2026-08-21 02:50:55'),(215,'R003','system_schedule:delete',2,'2026-08-21 02:50:55');
/*!40000 ALTER TABLE `sys_role_menu` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `sys_schedule`
--

DROP TABLE IF EXISTS `sys_schedule`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `sys_schedule` (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `title` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '日程标题',
  `description` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci COMMENT '日程描述',
  `schedule_date` date NOT NULL COMMENT '日程日期',
  `start_time` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL COMMENT '开始时间 HH:mm',
  `end_time` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL COMMENT '结束时间 HH:mm',
  `color` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT '#3B82F6' COMMENT '日程颜色标记',
  `location` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL COMMENT '地点',
  `is_completed` tinyint(1) NOT NULL DEFAULT '0' COMMENT '是否完成',
  `creator_id` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL COMMENT '创建人ID',
  `creator_name` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL COMMENT '创建人',
  `status` tinyint NOT NULL DEFAULT '1' COMMENT '1=正常 2=删除',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`) USING BTREE,
  KEY `idx_schedule_date` (`schedule_date`) USING BTREE,
  KEY `idx_creator_id` (`creator_id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='日程信息';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `sys_schedule`
--

LOCK TABLES `sys_schedule` WRITE;
/*!40000 ALTER TABLE `sys_schedule` DISABLE KEYS */;
INSERT INTO `sys_schedule` VALUES (2,'发布新版系统45','admin?????','2026-09-02','12:00','17:00','#3B82F6','机房3',1,'A10001','admin',1,'2026-09-01 21:26:10','2026-09-16 11:55:13'),(3,'邮寄信件55','55544','2026-09-02','09:00','18:00','#3B82F6','444',1,'A10001','admin',1,'2026-09-08 02:03:58','2026-09-08 03:02:02'),(4,'????-????(???)','??????','2026-09-21','14:00','15:00','#10B981','???A',0,'A10001','admin',2,'2026-09-08 02:32:40','2026-09-08 02:32:41'),(5,'中文测试日程(修改)','修改后的中文描述','2026-09-23','09:00','10:00','#F59E0B','测试地点',0,'A10001','admin',2,'2026-09-08 02:35:50','2026-09-08 02:35:50'),(6,'?????(?)',NULL,'2026-09-16',NULL,NULL,'#3B82F6',NULL,0,'A10106','ownertest',2,'2026-09-16 11:55:13','2026-09-16 11:55:13');
/*!40000 ALTER TABLE `sys_schedule` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-09-22 13:46:20
