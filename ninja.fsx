open System.IO

type VarDecl =
    | VarDecl of name: string * value: string

    override this.ToString() =
        let (VarDecl (name, value)) = this
        $"%s{name} = %s{value}"

type DepsType =
    | Gcc
    | Msvc

    override this.ToString() =
        match this with
        | Gcc -> "gcc"
        | Msvc -> "msvc"

type RuleBlock =
    | RuleBlock of rulename: string * command: string * depfile: option<string> * deps: option<DepsType>

    override this.ToString() =
        let (RuleBlock (rulename, command, depfile, deps)) = this

        let depfile =
            depfile
            |> Option.map (fun depfile -> $"    depfile = %O{depfile}\n")
            |> Option.defaultValue ""

        let deps =
            deps
            |> Option.map (fun deps -> $"    deps = %O{deps}\n")
            |> Option.defaultValue ""

        $"rule %s{rulename}\n    command = %s{command}\n%s{depfile}%s{deps}"

type BuildStmt =
    | BuildStmt of
        explicitOutputs: list<string> *
        implicitOutputs: list<string> *
        rulename: string *
        explicitInputs: list<string> *
        implicitInputs: list<string>

    override this.ToString() =
        let (BuildStmt (explicitOutputs, implicitOutputs, rulename, explicitInputs, implicitInputs)) =
            this

        let explicitOutputs = String.concat "" explicitOutputs

        let implicitOutputs =
            (if List.isEmpty implicitOutputs then
                 ""
             else
                 " | ")
            + String.concat " " implicitOutputs

        let explicitInputs = String.concat " " explicitInputs

        let implicitInputs =
            (if List.isEmpty implicitInputs then
                 ""
             else
                 " | ")
            + String.concat " " implicitInputs

        $"build %s{explicitOutputs}%s{implicitOutputs}: %s{rulename} %s{explicitInputs}%s{implicitInputs}"

type BuildFileContent =
    | BuildFileContent of varDecls: list<VarDecl> * ruleBlocks: list<RuleBlock> * buildStmts: list<BuildStmt>

    override this.ToString() =
        let (BuildFileContent (varDecls, ruleBlocks, buildStmts)) = this
        let varDecls = varDecls |> List.map string
        let ruleBlocks = ruleBlocks |> List.map string
        let buildStmts = buildStmts |> List.map string

        buildStmts
        |> List.append ruleBlocks
        |> List.append varDecls
        |> String.concat "\n"
        |> sprintf "%s\n"

let mutable varDecls = List.empty<VarDecl>

let var name value =
    varDecls <- VarDecl(name = name, value = value) :: varDecls

let mutable ruleBlocks = List.empty<RuleBlock>

let rule
    rulename
    (options: {| command: string
                 depfile: option<string>
                 deps: option<DepsType> |})
    =
    ruleBlocks <-
        RuleBlock(rulename = rulename, command = options.command, depfile = options.depfile, deps = options.deps)
        :: ruleBlocks

let mutable buildStmts = List.empty<BuildStmt>

let build explicitOutputs implicitOutputs rulename explicitInputs implicitInputs =
    buildStmts <-
        BuildStmt(
            explicitOutputs = explicitOutputs,
            implicitOutputs = implicitOutputs,
            rulename = rulename,
            explicitInputs = explicitInputs,
            implicitInputs = implicitInputs
        )
        :: buildStmts

let generate buildFile =
    let content =
        BuildFileContent(varDecls = varDecls, ruleBlocks = ruleBlocks, buildStmts = buildStmts)
        |> string

    File.WriteAllText(buildFile, content)
