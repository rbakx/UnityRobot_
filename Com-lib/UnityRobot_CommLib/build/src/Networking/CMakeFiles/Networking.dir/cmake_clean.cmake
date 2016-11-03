file(REMOVE_RECURSE
  "libNetworking.pdb"
  "libNetworking.a"
)

# Per-language clean rules from dependency scanning.
foreach(lang )
  include(CMakeFiles/Networking.dir/cmake_clean_${lang}.cmake OPTIONAL)
endforeach()
