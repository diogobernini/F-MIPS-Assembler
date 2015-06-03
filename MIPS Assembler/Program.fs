type Opcode = |Add
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
                   |I of Opcode * Register * Register * float  // opc $r1, $r2, ct
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


let rec processInstruction str =
    match str with
    |"Add"::x1::x2::x3::tail -> R(Add,processRegister x1, processRegister x2, processRegister x3)
    |"Sub"::x1::x2::x3::tail -> R(Sub,processRegister x1, processRegister x2, processRegister x3)
    |"Or"::x1::x2::x3::tail -> R(Or,processRegister x1, processRegister x2, processRegister x3)
    |"Nor"::x1::x2::x3::tail -> R(Nor,processRegister x1, processRegister x2, processRegister x3)
    |"Xor"::x1::x2::x3::tail -> R(Xor,processRegister x1, processRegister x2, processRegister x3)
    |"Slt"::x1::x2::x3::tail -> R(Slt,processRegister x1, processRegister x2, processRegister x3)
    |"Beq"::x1::x2::x3::tail -> I(Beq,processRegister x1, processRegister x2, float x3)
    |"Lw"::x1::x2::x3::tail -> I(Lw,processRegister x1, processRegister x3, float x2)
    |"Sw"::x1::x2::x3::tail -> I(Sw,processRegister x1, processRegister x3, float x2)
    |"Addi"::x1::x2::x3::tail -> I(Addi,processRegister x1, processRegister x2, float x3)
    |"Andi"::x1::x2::x3::tail -> I(Andi,processRegister x1, processRegister x2, float x3)
    |"J"::x1::tail -> J(Jump,x1)
    |other -> failwith "invalid instruction"

let instructionToType (instructionString:string) =
    let listInst = List.filter (fun x -> x<> "") (Array.toList(instructionString.Split(' ',',','(')))
    processInstruction listInst


[<EntryPoint>]
let main argv = 
    let valor = instructionToType "Lw $T0,0($T0)"
    printfn "%s" (valor.ToString())
    0 // return an integer exit code
