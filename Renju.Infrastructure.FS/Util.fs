module Renju.Util

open System

let GetDirection(a, b) = if a = b then 0 else if a < b then 1 else -1

let generate openf compute closef = 
    seq { let r = openf() 
          try 
            let x = ref None
            while (x := compute r; (!x).IsSome) do
                yield (!x).Value
          finally
             closef r }

type IComparable<'T> with
    member this.InRang limit anotherLimit = 
        this.CompareTo(limit) <= 0 && this.CompareTo(anotherLimit) >= 0 ||
        this.CompareTo(limit) >= 0 && this.CompareTo(anotherLimit) <= 0
    member this.WithInRang limit anotherLimit = 
        this.CompareTo(limit) < 0 && this.CompareTo(anotherLimit) > 0 ||
        this.CompareTo(limit) > 0 && this.CompareTo(anotherLimit) < 0
