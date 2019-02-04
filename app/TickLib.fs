module TickLib

open System
open System.IO

// TODO: dynamically determine what max width should be  
// TODO: remove MAGIC NUMBER!!!
let maxDigits = 8
let zeroPaddedFormatString = String.replicate maxDigits "0"

// ========================================================
// define all string transforms here.  
let replaceTabsWithCommas (line : string) = line.Replace("\t", ",")
// ========================================================

// deine session times in PT
let gbxOpenTime = DateTime.Parse("15:00:00").TimeOfDay
let rthOpenTime = DateTime.Parse("06:30:00").TimeOfDay
let rthCloseTime = DateTime.Parse("13:15:00").TimeOfDay

// additional times for active patterns below
let lastSecBeforeMidnight = DateTime.Parse("23:59:59").TimeOfDay
let midnight = DateTime.Parse("00:00:00").TimeOfDay

let (|GbxPriorDay|_|) (gbxOpenTime, lastSecBeforeMidnight, _midnight, _rthOpenTime, aTimeOfDay) =
    if (aTimeOfDay >= gbxOpenTime && aTimeOfDay <= lastSecBeforeMidnight) then Some()
    else None

let (|GbxSameDay|_|) (_gbxOpenTime, _lastSecBeforeMidnight, midnight, rthOpenTime, aTimeOfDay) =
    if (aTimeOfDay >= midnight && aTimeOfDay < rthOpenTime) then Some()
    else None

let getSession gbxOpenTime lastSecBeforeMidnight midnight rthOpenTime aTimeOfDay =
    match (gbxOpenTime, lastSecBeforeMidnight, midnight, rthOpenTime, aTimeOfDay) with
    | GbxPriorDay -> "GBX"
    | GbxSameDay -> "GBX"
    | _ -> "RTH"




// we use sortable "yyyy-MM-dd" format
// https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings#Sortable

let nextDay (aDateTime : DateTime) =
    aDateTime.AddDays(1.0).ToString("yyyy-MM-dd")
let sameDay (aDateTime : DateTime) = aDateTime.ToString("yyyy-MM-dd")

let tradeDate (aDateTime : DateTime) gbxOpenTime =
    if aDateTime.TimeOfDay >= gbxOpenTime then nextDay aDateTime
    else sameDay aDateTime


// define all string transforms here.  NOTE: string.Replace is 3x faster than Regex.Replace(line, "\t",",")
let replaceTabWithComma (line : string) = line.Replace("\t", ",")
// TODO: replace @ES# with regex to work for any instrument

let zeroPaddedNumber (num : int) = num.ToString(zeroPaddedFormatString)

// return a tuple w two substrings from provided line
let splitStringBeforeFirstComma (line : string) =
    let firstPart = line.Substring(0, line.IndexOf(","))
    let secondPart =
        line.Substring(line.IndexOf(","), line.Length - line.IndexOf(","))
    (firstPart, secondPart)

// makes a new line by "zipping" together two arrays
// combining labels from provided header with data from provided sequence
let makeLine (header : string) (line : string) =
    // input header: T,S,D,P,V,B,A
    // input line:   09/25/2014 13:30:00,00000001,1961.25,1,1961.25,1961.50 
    // output:       T,09/25/201,D,09/25/2014 13:30:00,S,00000001,P,1961.25,V,1,B,1961.25,A,1961.50
    let headerArray = header.Split(',')
    let lineArray = line.Split(',')
    let zipperOfBothArrays =
        Array.map2 (fun x y -> x + "," + y) headerArray lineArray
    
    let outputLine =
        zipperOfBothArrays
        |> Array.toList
        |> List.map string
        |> String.concat (",")
    outputLine

// cleanup
let flushAndClose (strWriter : StreamWriter) =
    strWriter.Flush |> ignore
    strWriter.Close |> ignore
