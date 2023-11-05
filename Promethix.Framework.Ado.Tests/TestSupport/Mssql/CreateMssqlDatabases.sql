﻿-- Creates two databases AdoScopeTest1 and AdoScopeTest2.
-- If you really need to test with MSSQL. You can use this script to create the databases.
USE [master]
GO

/****** Object:  Database [AdoScopeTest1]    Script Date: 17/10/2023 22:14:25 ******/
CREATE DATABASE [AdoScopeTest1]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'AdoScopeTest1', FILENAME = N'D:\Program Files\Microsoft SQL Server\MSSQL13.MSSQLSERVER\MSSQL\DATA\AdoScopeTest1.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'AdoScopeTest1_log', FILENAME = N'D:\Program Files\Microsoft SQL Server\MSSQL13.MSSQLSERVER\MSSQL\DATA\AdoScopeTest1_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [AdoScopeTest1].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO

ALTER DATABASE [AdoScopeTest1] SET ANSI_NULL_DEFAULT OFF 
GO

ALTER DATABASE [AdoScopeTest1] SET ANSI_NULLS OFF 
GO

ALTER DATABASE [AdoScopeTest1] SET ANSI_PADDING OFF 
GO

ALTER DATABASE [AdoScopeTest1] SET ANSI_WARNINGS OFF 
GO

ALTER DATABASE [AdoScopeTest1] SET ARITHABORT OFF 
GO

ALTER DATABASE [AdoScopeTest1] SET AUTO_CLOSE OFF 
GO

ALTER DATABASE [AdoScopeTest1] SET AUTO_SHRINK OFF 
GO

ALTER DATABASE [AdoScopeTest1] SET AUTO_UPDATE_STATISTICS ON 
GO

ALTER DATABASE [AdoScopeTest1] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO

ALTER DATABASE [AdoScopeTest1] SET CURSOR_DEFAULT  GLOBAL 
GO

ALTER DATABASE [AdoScopeTest1] SET CONCAT_NULL_YIELDS_NULL OFF 
GO

ALTER DATABASE [AdoScopeTest1] SET NUMERIC_ROUNDABORT OFF 
GO

ALTER DATABASE [AdoScopeTest1] SET QUOTED_IDENTIFIER OFF 
GO

ALTER DATABASE [AdoScopeTest1] SET RECURSIVE_TRIGGERS OFF 
GO

ALTER DATABASE [AdoScopeTest1] SET  DISABLE_BROKER 
GO

ALTER DATABASE [AdoScopeTest1] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO

ALTER DATABASE [AdoScopeTest1] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO

ALTER DATABASE [AdoScopeTest1] SET TRUSTWORTHY OFF 
GO

ALTER DATABASE [AdoScopeTest1] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO

ALTER DATABASE [AdoScopeTest1] SET PARAMETERIZATION SIMPLE 
GO

ALTER DATABASE [AdoScopeTest1] SET READ_COMMITTED_SNAPSHOT OFF 
GO

ALTER DATABASE [AdoScopeTest1] SET HONOR_BROKER_PRIORITY OFF 
GO

ALTER DATABASE [AdoScopeTest1] SET RECOVERY FULL 
GO

ALTER DATABASE [AdoScopeTest1] SET  MULTI_USER 
GO

ALTER DATABASE [AdoScopeTest1] SET PAGE_VERIFY CHECKSUM  
GO

ALTER DATABASE [AdoScopeTest1] SET DB_CHAINING OFF 
GO

ALTER DATABASE [AdoScopeTest1] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO

ALTER DATABASE [AdoScopeTest1] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO

ALTER DATABASE [AdoScopeTest1] SET DELAYED_DURABILITY = DISABLED 
GO

ALTER DATABASE [AdoScopeTest1] SET QUERY_STORE = OFF
GO

USE [AdoScopeTest1]
GO

ALTER DATABASE SCOPED CONFIGURATION SET LEGACY_CARDINALITY_ESTIMATION = OFF;
GO

ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 0;
GO

ALTER DATABASE SCOPED CONFIGURATION SET PARAMETER_SNIFFING = ON;
GO

ALTER DATABASE SCOPED CONFIGURATION SET QUERY_OPTIMIZER_HOTFIXES = OFF;
GO

ALTER DATABASE [AdoScopeTest1] SET  READ_WRITE 
GO

USE [master]
GO

/****** Object:  Database [AdoScopeTest2]    Script Date: 17/10/2023 22:14:49 ******/
CREATE DATABASE [AdoScopeTest2]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'AdoScopeTest2', FILENAME = N'D:\Program Files\Microsoft SQL Server\MSSQL13.MSSQLSERVER\MSSQL\DATA\AdoScopeTest2.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'AdoScopeTest2_log', FILENAME = N'D:\Program Files\Microsoft SQL Server\MSSQL13.MSSQLSERVER\MSSQL\DATA\AdoScopeTest2_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [AdoScopeTest2].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO

ALTER DATABASE [AdoScopeTest2] SET ANSI_NULL_DEFAULT OFF 
GO

ALTER DATABASE [AdoScopeTest2] SET ANSI_NULLS OFF 
GO

ALTER DATABASE [AdoScopeTest2] SET ANSI_PADDING OFF 
GO

ALTER DATABASE [AdoScopeTest2] SET ANSI_WARNINGS OFF 
GO

ALTER DATABASE [AdoScopeTest2] SET ARITHABORT OFF 
GO

ALTER DATABASE [AdoScopeTest2] SET AUTO_CLOSE OFF 
GO

ALTER DATABASE [AdoScopeTest2] SET AUTO_SHRINK OFF 
GO

ALTER DATABASE [AdoScopeTest2] SET AUTO_UPDATE_STATISTICS ON 
GO

ALTER DATABASE [AdoScopeTest2] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO

ALTER DATABASE [AdoScopeTest2] SET CURSOR_DEFAULT  GLOBAL 
GO

ALTER DATABASE [AdoScopeTest2] SET CONCAT_NULL_YIELDS_NULL OFF 
GO

ALTER DATABASE [AdoScopeTest2] SET NUMERIC_ROUNDABORT OFF 
GO

ALTER DATABASE [AdoScopeTest2] SET QUOTED_IDENTIFIER OFF 
GO

ALTER DATABASE [AdoScopeTest2] SET RECURSIVE_TRIGGERS OFF 
GO

ALTER DATABASE [AdoScopeTest2] SET  DISABLE_BROKER 
GO

ALTER DATABASE [AdoScopeTest2] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO

ALTER DATABASE [AdoScopeTest2] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO

ALTER DATABASE [AdoScopeTest2] SET TRUSTWORTHY OFF 
GO

ALTER DATABASE [AdoScopeTest2] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO

ALTER DATABASE [AdoScopeTest2] SET PARAMETERIZATION SIMPLE 
GO

ALTER DATABASE [AdoScopeTest2] SET READ_COMMITTED_SNAPSHOT OFF 
GO

ALTER DATABASE [AdoScopeTest2] SET HONOR_BROKER_PRIORITY OFF 
GO

ALTER DATABASE [AdoScopeTest2] SET RECOVERY FULL 
GO

ALTER DATABASE [AdoScopeTest2] SET  MULTI_USER 
GO

ALTER DATABASE [AdoScopeTest2] SET PAGE_VERIFY CHECKSUM  
GO

ALTER DATABASE [AdoScopeTest2] SET DB_CHAINING OFF 
GO

ALTER DATABASE [AdoScopeTest2] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO

ALTER DATABASE [AdoScopeTest2] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO

ALTER DATABASE [AdoScopeTest2] SET DELAYED_DURABILITY = DISABLED 
GO

ALTER DATABASE [AdoScopeTest2] SET QUERY_STORE = OFF
GO

USE [AdoScopeTest2]
GO

ALTER DATABASE SCOPED CONFIGURATION SET LEGACY_CARDINALITY_ESTIMATION = OFF;
GO

ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 0;
GO

ALTER DATABASE SCOPED CONFIGURATION SET PARAMETER_SNIFFING = ON;
GO

ALTER DATABASE SCOPED CONFIGURATION SET QUERY_OPTIMIZER_HOTFIXES = OFF;
GO

ALTER DATABASE [AdoScopeTest2] SET  READ_WRITE 
GO
