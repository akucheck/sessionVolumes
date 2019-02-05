// sessionVolumes.fs
(*
usage: cat inFile | sessionVolumes  > outFile.txt

Expected input: the standard "shrink" format that contains one row for each
price change during the course of a session.  

dateTime, seqNum, volume, deltaFactor, occur, aggVol, aggDelta
2018-12-27T15:00:00,41,2491.50,1,-1,41,205,-5
2018-12-27T15:00:00,43,2491.25,1,-1,2,3,-2
2018-12-27T15:00:00,44,2491.00,1,-1,1,2,-1
2018-12-27T15:00:00,45,2491.25,1,1,1,2,1
2018-12-27T15:00:00,47,2491.50,1,1,2,3,0

Expected output: 
tradeDate, total Globex volume, and total RTH volume
// tradeDate, gbxVolume, rthVolume
2018-12-27,123456,1234567
 
*)
module sessionVolumes =
    open System
    // open System.IO
    open TickLib
    open FilterIoLib
    open SessionVolumesTypes
    
    let getSessionIdent (currRow : InputRow) =
        // returns "RTH" or "GBX"
        let aTimeOfDay = DateTime.Parse(currRow.DateTime).TimeOfDay
        let sessionIdentifier =
            getSession gbxOpenTime lastSecBeforeMidnight midnight rthOpenTime 
                aTimeOfDay
        sessionIdentifier
    
    let classifyVolumeBySession (line : string) =
        let currRow = deserializeInputRow line
        let sessionIdentifier = getSessionIdent (currRow)
        let tradeDate = DateTime.Parse(currRow.DateTime).ToString("yyyy-MM-dd")
        match (sessionIdentifier) with
        | "RTH" -> (tradeDate, 0, currRow.AggVol)
        | _ -> (tradeDate, currRow.AggVol, 0)
    
    let accumulateSessionVolumes (state : string * int * int) 
        (rowValues : string * int * int) =
        let (_tradeDate, gbxVolState, rthVolState) = state
        let (tradeDate, gbxVolRow, rthVolRow) = rowValues
        (tradeDate, gbxVolState + gbxVolRow, rthVolState + rthVolRow)
    
    [<EntryPoint>]
    let main _argv =
        let _rows =
            Seq.initInfinite readInput
            |> Seq.takeWhile ((<>) null)
            |> Seq.map classifyVolumeBySession
            |> Seq.fold accumulateSessionVolumes ("", 0, 0)
            |||> printfn "%s,%d,%d" // note the tupleForwardPipe operator
        0 // return an integer exit code 
// ========================================================
// ** end main ** 
// ========================================================
