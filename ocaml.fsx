#load "ninja.fsx"

Ninja.rule
    "compile"
    {| command = "ocamlopt -c $in -o $out -I ./src -bin-annot"
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
    if modInfo.hasSignature then
        Ninja.build
            [ $"src/{modInfo.name}.cmi" ]
            []
            "compile"
            [ $"src/{modInfo.name}.mli" ]
            (modInfo.dependencies
             |> List.map (sprintf "src/%s.cmi"))

    let explicitOutputs = [ $"src/{modInfo.name}.cmx" ]

    let implicitOutputs =
        [ $"src/{modInfo.name}.o" ]
        @ (if modInfo.hasSignature then
               []
           else
               [ $"src/{modInfo.name}.cmi" ])

    let explicitInputs = [ $"src/{modInfo.name}.ml" ]

    let implicitInputs =
        (modInfo.dependencies
         |> List.collect (fun modName ->
             [ $"src/{modName}.cmi"
               $"src/{modName}.cmx" ]))
        @ if modInfo.hasSignature then
              [ $"src/{modInfo.name}.cmi" ]
          else
              []

    Ninja.build explicitOutputs implicitOutputs "compile" explicitInputs implicitInputs

let executable exePath modNames =
    Ninja.build [ exePath ] [] "link" (modNames |> List.map (sprintf "src/%s.cmx")) []

let generateBuildNinja () = Ninja.generate "build.ninja"
