// sessVols.fs
(*
usage: cat inFile | sessVolComp  > outFile.txt

Expected input: the standard "shrink" format that contains one row for each
price change during the course of a session.  

dateTime, seqNum, volume, deltaFactor, occur, aggVol, aggDelta
2018-12-27T15:00:00,41,2491.50,1,-1,41,205,-5
2018-12-27T15:00:00,43,2491.25,1,-1,2,3,-2
2018-12-27T15:00:00,44,2491.00,1,-1,1,2,-1
2018-12-27T15:00:00,45,2491.25,1,1,1,2,1
2018-12-27T15:00:00,47,2491.50,1,1,2,3,0

Expected output: 
the total Globex volume, and the RTH volume
// gbxVolume, rthVolume
123456,1234567
 
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
        match (sessionIdentifier) with
        | "RTH" -> (0, currRow.AggVol)
        | _ -> (currRow.AggVol, 0)
    
    let accumulateSessionVolumes (state : int * int) (rowVols : int * int) =
        (fst state + fst rowVols, snd state + snd rowVols)
    
    [<EntryPoint>]
    let main _argv =
        let _rows =
            Seq.initInfinite readInput
            |> Seq.takeWhile ((<>) null)
            |> Seq.map classifyVolumeBySession
            |> Seq.fold accumulateSessionVolumes (0, 0)            
            ||> printfn "%d,%d" // note the tupleForwardPipe operator
        0 // return an integer exit code 
// ========================================================
// ** end main ** 
// ========================================================
