#load "ocaml.fsx"

Ocaml.addModule
    {| name = "expression"
       hasSignature = true
       dependencies = [] |}

Ocaml.addModule
    {| name = "main"
       hasSignature = false
       dependencies = [ "expression" ] |}

Ocaml.executable "main" [ "expression"; "main" ]

Ocaml.generateBuildNinja ()
