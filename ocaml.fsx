#load "ninja.fsx"

Ninja.rule
    "compile"
    {| command = "ocamlopt -c $in -o $out -I ./build"
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
            [ $"build/{modInfo.name}.cmi" ]
            []
            "compile"
            [ $"{modInfo.name}.mli" ]
            (modInfo.dependencies
             |> List.map (fun name -> $"build/{name}.cmi"))

    let explicitOutputs = [ $"build/{modInfo.name}.cmx" ]

    let implicitOutputs =
        [ $"build/{modInfo.name}.o" ]
        @ (if modInfo.hasSignature then
               []
           else
               [ $"build/{modInfo.name}.cmi" ])

    let explicitInputs = [ $"{modInfo.name}.ml" ]

    let implicitInputs =
        (modInfo.dependencies
         |> List.collect (fun modName ->
             [ $"build/{modName}.cmi"
               $"build/{modName}.cmx" ]))
        @ if modInfo.hasSignature then
              [ $"build/{modInfo.name}.cmi" ]
          else
              []

    Ninja.build explicitOutputs implicitOutputs "compile" explicitInputs implicitInputs

let executable exePath modNames =
    Ninja.build [ $"build/{exePath}" ] [] "link" (modNames |> List.map (sprintf "build/%s.cmx")) []

let build () = Ninja.generate "build.ninja"
