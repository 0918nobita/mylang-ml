type expression =
  | IntLiteral of int
  | Plus of expression * expression
  | Times of expression * expression

module Expression = struct
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

let () = print_endline @@ Expression.to_string (Plus (Plus (IntLiteral 1, IntLiteral 2), IntLiteral 4))
