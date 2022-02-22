module GroupUp.BaseTypes

open System
open GroupUp.Errors

type String50 = private String50 of string
type EmailAddress = private EmailAddress of string
type PostalCode = private PostalCode of string

module Guid =
    let ParseResult (guid: string) =
        let result, id = Guid.TryParse(guid)
    
        match result with
        | true -> id |> Ok
        | false ->
            "Could not parse GUID"
            |> CannotCreateBaseType
            |> Error


module ConstrainedType =
    let createString fieldName ctor maxLen str =
        if String.IsNullOrEmpty(str) then
            let msg =
                sprintf "%s must not be null or empty" fieldName

            msg |> CannotCreateBaseType |> Error
        elif str.Length > maxLen then
            let msg =
                sprintf "%s must not be more than %i chars" fieldName maxLen

            msg |> CannotCreateBaseType |> Error
        else
            Ok(ctor str)

    let createStringOption fieldName ctor maxLen str =
        if String.IsNullOrEmpty(str) then
            Ok None
        elif str.Length > maxLen then
            let msg =
                sprintf "%s must not be more than %i chars" fieldName maxLen

            msg |> CannotCreateBaseType |> Error
        else
            Ok(ctor str |> Some)

    let createInt fieldName ctor minVal maxVal i =
        if i < minVal then
            let msg =
                sprintf "%s: Must not be less than %i" fieldName minVal

            msg |> CannotCreateBaseType |> Error
        elif i > maxVal then
            let msg =
                sprintf "%s: Must not be greater than %i" fieldName maxVal

            msg |> CannotCreateBaseType |> Error
        else
            Ok(ctor i)

    let createDecimal fieldName ctor minVal maxVal i =
        if i < minVal then
            let msg =
                sprintf "%s: Must not be less than %M" fieldName minVal

            msg |> CannotCreateBaseType |> Error
        elif i > maxVal then
            let msg =
                sprintf "%s: Must not be greater than %M" fieldName maxVal

            msg |> CannotCreateBaseType |> Error
        else
            Ok(ctor i)

    let createLike fieldName ctor pattern str =
        if String.IsNullOrEmpty(str) then
            let msg =
                sprintf "%s: Must not be null or empty" fieldName

            msg |> CannotCreateBaseType |> Error
        elif System.Text.RegularExpressions.Regex.IsMatch(str, pattern) then
            Ok(ctor str)
        else
            let msg =
                sprintf "%s: '%s' must match the pattern '%s'" fieldName str pattern

            msg |> CannotCreateBaseType |> Error

module String50 =
    let value (String50 str) = str

    let create fieldName str =
        ConstrainedType.createString fieldName String50 50 str

    let createOption fieldName str =
        ConstrainedType.createStringOption fieldName String50 50 str

module EmailAddress =
    let value (EmailAddress str) = str

    let create fieldName str =
        let pattern = ".+@.+"
        ConstrainedType.createLike fieldName EmailAddress pattern str

module PostalCode =
    let value (PostalCode str) = str

    let create fieldName str =
        ConstrainedType.createString fieldName PostalCode 10 str
