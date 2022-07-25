#load "ninja.fsx"

Ninja.rule
    "compile"
    {| command = "ocamlopt -c $in -I ."
       depfile = None
       deps = None |}

Ninja.rule
    "link"
    {| command = "ocamlopt -o $out $in"
       depfile = None
       deps = None |}

let addModule
    (modInfo: {| name: string
                 hasSignature: bool
                 dependencies: list<string> |})
    =
    let implicitOutputs =
        [ $"{modInfo.name}.cmx"
          $"{modInfo.name}.cmi"
          $"{modInfo.name}.o" ]

    let inputs =
        (modInfo.dependencies
         |> List.map (sprintf "%s.cmx"))
        @ (if modInfo.hasSignature then
               [ $"{modInfo.name}.mli"
                 $"{modInfo.name}.ml" ]
           else
               [ $"{modInfo.name}.ml" ])

    Ninja.build [] implicitOutputs "compile" inputs

let executable exePath modNames =
    Ninja.build [ exePath ] [] "link" (modNames |> List.map (sprintf "%s.cmx"))

let generate = Ninja.generate
