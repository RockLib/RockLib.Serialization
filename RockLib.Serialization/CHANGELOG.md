# RockLib.Serialization Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

#### Added

- Adds SourceLink to nuget package.

----

**Note:** Release notes in the above format are not available for earlier versions of
RockLib.Secrets. What follows below are the original release notes.

----

## 1.0.5

Adds net5.0 target

## 1.0.4

Adds icon to project and nuget package.

## 1.0.3

Updates to align with nuget conventions.

## 1.0.2

DefaultXmlSerializer emits utf-8 instead of utf-16 if no XmlWriterSettings are specified; emits the encoding specified by the XmlWriterSettings.Encoding if it is.

## 1.0.1

- Adds support for rocklib_serialization config section.
- Adds ConfigSection attribute for the Rockifier tool.

## 1.0.0

Initial release.
