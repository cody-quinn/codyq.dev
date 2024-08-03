{
  inputs = {
    nixpkgs.url = "github:NixOS/nixpkgs/master";
    flake-utils.url = "github:numtide/flake-utils";
  };

  outputs = {
    self,
    nixpkgs,
    flake-utils,
    ...
  }:
    flake-utils.lib.eachDefaultSystem (system: let
      pkgs = import nixpkgs {inherit system;};
    in
      {
        devShells.default = pkgs.mkShell {
          packages = with pkgs; [
            dotnet-sdk_8
            libgit2
          ];
          DOTNET_ROOT = "${pkgs.dotnet-sdk_8}";
          LD_LIBRARY_PATH = "${pkgs.libgit2}/lib";
        };
      });
}
