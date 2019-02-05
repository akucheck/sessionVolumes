
open System

let onDateTimeString = "2018-12-27T15:00:00"
let rthDateTimeString = "2018-12-28T13:59:45"

let aGbxTimeOfDay = DateTime.Parse(onDateTimeString).TimeOfDay
let anRthTimeOfDay = DateTime.Parse(rthDateTimeString).TimeOfDay


let gbxOpenTime = DateTime.Parse("15:00:00").TimeOfDay
let rthOpenTime = DateTime.Parse("06:30:00").TimeOfDay
let rthCloseTime = DateTime.Parse("13:15:00").TimeOfDay
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

//tests
let gbx = getSession gbxOpenTime lastSecBeforeMidnight midnight rthOpenTime aGbxTimeOfDay 
let rth = getSession gbxOpenTime lastSecBeforeMidnight midnight rthOpenTime anRthTimeOfDay


//
let intTuple = (5, 10)   
printfn "%d %d" <|| intTuple

let anotherDateTimeString = "2018-12-27T15:00:00"
let aTradeDate = DateTime.Parse(anotherDateTimeString).ToString("yyyy-MM-dd")

// holiday array handling
let holidays = [|
        "2009-01-01";
        "2009-01-19";
        "2009-02-16"
                    |]

let holidays2D = [|
        ("2009-01-01", "New Year");
        ("2009-01-19", "MLK Day");
        ("2009-02-16", "Presidents Day")
                    |]
// let res = Array.Exists (fun elem ->  elem = "2009-01-01") holidays 

let isHoliday (dateUnderTest: string) = Array.exists (fun elem -> elem = dateUnderTest) 
let res = isHoliday "2009-01-01" holidays2D