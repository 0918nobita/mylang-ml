let () =
  print_endline @@ let open Expression in to_string @@ plus (plus (int_literal 1) (int_literal 2)) (int_literal 4)
