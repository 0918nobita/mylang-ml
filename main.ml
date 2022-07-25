module Expression : sig
  type t

  val int_literal : int -> t

  val plus : t -> t -> t

  val times : t -> t -> t

  val to_string : t -> string
end = struct
  type t =
    | IntLiteral of int
    | Plus of t * t
    | Times of t * t

  let int_literal i = IntLiteral i
  
  let plus lhs rhs = Plus (lhs, rhs)

  let times lhs rhs = Times (lhs, rhs)

  let is_literal = function
    | IntLiteral _ -> true
    | _ -> false

  let rec to_string = function
    | IntLiteral i -> string_of_int i
    | Plus (lhs, rhs) ->
      begin
        let lhs = if is_literal lhs then to_string lhs else "(" ^ to_string lhs ^ ")" in
        let rhs = if is_literal rhs then to_string rhs else "(" ^ to_string rhs ^ ")" in
        lhs ^ " + " ^ rhs
      end
    | Times (lhs, rhs) ->
      begin
        let lhs = if is_literal lhs then to_string lhs else "(" ^ to_string lhs ^ ")" in
        let rhs = if is_literal rhs then to_string rhs else "(" ^ to_string rhs ^ ")" in
        lhs ^ " * " ^ rhs
      end
end

let () =
  print_endline @@ let open Expression in to_string @@ plus (plus (int_literal 1) (int_literal 2)) (int_literal 4)
