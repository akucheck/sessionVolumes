module SessionVolumesTypes

type InputRow =
    { DateTime : string
      SeqNum : string
      Price : float
      Volume : int
      DeltaFactor : int
      Occur : int
      AggVol : int
      AggDelta : int}

let deserializeInputRow (line : string) =
    let lineArray = line.Split(',')
    
    let currInputRow =
        { DateTime = lineArray.[0]
          SeqNum = string lineArray.[1]
          Price = float lineArray.[2]
          Volume = int lineArray.[3]
          DeltaFactor = int lineArray.[4]
          Occur = int lineArray.[5]
          AggVol = int lineArray.[6]
          AggDelta = int lineArray.[7] }
    currInputRow


