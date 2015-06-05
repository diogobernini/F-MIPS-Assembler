﻿type Opcode = |Add
              |Sub
              |And
              |Or
              |Nor
              |Xor
              |Slt
              |Beq
              |Lw
              |Sw
              |Addi
              |Andi
              |Jump
            
type Register = |Zero
                |At
                |V0 |V1
                |A0 |A1 |A2 |A3
                |T0 |T1 |T2 |T3 |T4 |T5 |T6 |T7
                |S0 |S1 |S2 |S3 |S4 |S5 |S6 |S7
                |T8 |T9
                |K0 |K1
                |GP |SP |FP |RA

type Instruction = |R of Opcode * Register * Register * Register  // opc $r1, $r2, $r3
                   |I of Opcode * Register * Register * int  // opc $r1, $r2, ct
                   |J of Opcode * string   // opc $label



let processRegister (register:string) =
    let regUpper = register.ToUpper().TrimEnd(')')
    match regUpper with
    |"$ZERO" -> Zero
    |"$AT" -> At
    |"$V0" -> V0
    |"$V1" -> V1
    |"$A0" -> A0
    |"$A1" -> A1
    |"$A2" -> A2
    |"$A3" -> A3
    |"$T0" -> T0
    |"$T1" -> T1
    |"$T2" -> T2
    |"$T3" -> T3
    |"$T4" -> T4
    |"$T5" -> T5
    |"$T6" -> T6
    |"$T7" -> T7
    |"$S0" -> S0
    |"$S1" -> S1
    |"$S2" -> S2
    |"$S3" -> S3
    |"$S4" -> S4
    |"$S5" -> S5
    |"$S6" -> S6
    |"$S7" -> S7
    |"$T8" -> T8
    |"$T9" -> T9
    |"$K0" -> K0 
    |"$K1" -> K1
    |"$GP" -> GP
    |"$SP" -> SP
    |"$FP" -> FP
    |"$RA" -> RA
    |_ -> failwith "invalid register"


let processInstruction str =
    match str with
    |"Add"::x1::x2::x3::tail -> R(Add,processRegister x1, processRegister x2, processRegister x3)
    |"Sub"::x1::x2::x3::tail -> R(Sub,processRegister x1, processRegister x2, processRegister x3)
    |"Or"::x1::x2::x3::tail -> R(Or,processRegister x1, processRegister x2, processRegister x3)
    |"Nor"::x1::x2::x3::tail -> R(Nor,processRegister x1, processRegister x2, processRegister x3)
    |"Xor"::x1::x2::x3::tail -> R(Xor,processRegister x1, processRegister x2, processRegister x3)
    |"Slt"::x1::x2::x3::tail -> R(Slt,processRegister x1, processRegister x2, processRegister x3)
    |"Beq"::x1::x2::x3::tail -> I(Beq,processRegister x1, processRegister x2, int x3)
    |"Lw"::x1::x2::x3::tail -> I(Lw,processRegister x1, processRegister x3, int x2)
    |"Sw"::x1::x2::x3::tail -> I(Sw,processRegister x1, processRegister x3, int x2)
    |"Addi"::x1::x2::x3::tail -> I(Addi,processRegister x1, processRegister x2, int x3)
    |"Andi"::x1::x2::x3::tail -> I(Andi,processRegister x1, processRegister x2, int x3)
    |"J"::x1::tail -> J(Jump,x1)
    |other -> failwith "invalid instruction"

let opcodeToBinary (opcode:Opcode) =
    match opcode with
    |Add -> "000000"
    |Sub -> "000001"
    |And -> "000010"
    |Or -> "000011"
    |Nor -> "000100"
    |Xor -> "000101"
    |Slt -> "000110"
    |Beq -> "000111"
    |Lw -> "001000"
    |Sw -> "010000"
    |Addi -> "111000"
    |Andi -> "111010"
    |Jump -> "100000"

let registerToBinary (register:Register) =
    match register with
    |Zero -> "00000"
    |At -> "00001"
    |V0 -> "00010"
    |V1 -> "00011"
    |A0 -> "00100"
    |A1 -> "00101"
    |A2 -> "00110"
    |A3 -> "00111"
    |T0 -> "01000"
    |T1 -> "01001" 
    |T2 -> "01010"
    |T3 -> "01011"
    |T4 -> "01100"
    |T5 -> "01101"
    |T6 -> "01110"
    |T7 -> "01111"
    |S0 -> "10000"
    |S1 -> "10001"
    |S2 -> "10010"
    |S3 -> "10011"
    |S4 -> "10100"
    |S5 -> "10101"
    |S6 -> "10110"
    |S7 -> "10111"
    |T8 -> "11000"
    |T9 -> "11001"
    |K0 -> "11010"
    |K1 -> "11011"
    |GP -> "11100"
    |SP -> "11101"
    |FP -> "11110"
    |RA -> "11111"

let cteToBinary cte size =
    let rec cteToBinaryIn i =
        match i with
        | 0 | 1 -> string i
        | _ ->
            let bit = string (i % 2)
            (cteToBinaryIn (i / 2)) + bit
    let rec add0 string =
        if(String.length(string)<size) then add0 ("0" + string)
        else string
    add0 (cteToBinaryIn cte)

let findLabelPosition (address:string) =
    cteToBinary (int address) 26

let instructionToBinary (instruction:Instruction) =
    match instruction with
    |Instruction.R(opcode,reg1,reg2,reg3) -> opcodeToBinary(opcode) + registerToBinary(reg1) + registerToBinary(reg2) + registerToBinary(reg3) + "00000000000"
    |Instruction.I(opcode,reg1,reg2,cte) -> opcodeToBinary(opcode) + registerToBinary(reg1) + registerToBinary(reg2) + cteToBinary cte 16
    |Instruction.J(opcode,address) ->  opcodeToBinary(opcode) + findLabelPosition(address)


let instructionToType (instructionString:string) =
    let listInst = List.filter (fun x -> x<> "") (Array.toList(instructionString.Split(' ',',','(')))
    processInstruction listInst

let readLines (filePath:string) = seq {
    use sr = new System.IO.StreamReader (filePath)
    while not sr.EndOfStream do
        yield sr.ReadLine ()
}

let processLines sequence =
    let list = List.ofSeq(sequence)
    let processedList = list |> List.map (fun x -> instructionToBinary(instructionToType x))
    processedList

[<EntryPoint>]
let main argv = 
    let seqFile = readLines "C:\\Users\\DiogoBernini\\Documents\\Visual Studio 2013\\Projects\\MIPS Assembler\\MIPS Assembler\\assembly.asm"
    let pl = processLines seqFile
    printfn "%s" (pl.ToString())
    0 // return an integer exit code
