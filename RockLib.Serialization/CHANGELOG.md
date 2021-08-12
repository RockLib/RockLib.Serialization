# RockLib.Serialization Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## Unreleased

#### Changed

- Changes "Quicken Loans" to "Rocket Mortgage".
- Updates RockLib.Configuration to latest version, [2.5.3](https://github.com/RockLib/RockLib.Configuration/blob/main/RockLib.Configuration/CHANGELOG.md#253---2021-08-11).
- Updates RockLib.Configuration.ObjectFactory to latest version, [1.6.9](https://github.com/RockLib/RockLib.Configuration/blob/main/RockLib.Configuration.ObjectFactory/CHANGELOG.md#169---2021-08-11).

## 1.0.6 - 2021-05-10

#### Added

- Adds SourceLink to nuget package.

#### Changed

- Updates RockLib.Configuration and RockLib.Configuration.ObjectFactory packages to latest versions, which include SourceLink.
- Updates Newtonsoft.Json package to latest version.

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
