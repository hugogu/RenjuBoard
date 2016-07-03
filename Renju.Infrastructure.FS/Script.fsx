// Learn more about F# at http://fsharp.org. See the 'F# Tutorial' project
// for more guidance on F# programming.

#load "Util.fs"
open Renju.Util
open System

let zero = fun f -> 
               fun x -> x
let add1 n = fun f ->
               fun x -> f ((n f) x)

let Pair x y = fun f -> f x y
let First p = p (fun x y -> x)


let p = Pair 1 2
System.Console.Write(p |> First)