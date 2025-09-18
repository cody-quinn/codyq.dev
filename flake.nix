{
  inputs.nixpkgs.url = "github:NixOS/nixpkgs/nixos-unstable";
  inputs.flake-utils.url = "github:numtide/flake-utils";
  inputs.treefmt-nix.url = "github:numtide/treefmt-nix";

  outputs =
    {
      self,
      nixpkgs,
      flake-utils,
      treefmt-nix,
      ...
    }:
    flake-utils.lib.eachDefaultSystem (
      system:
      let
        pkgs = import nixpkgs { inherit system; };

        formatter = treefmt-nix.lib.evalModule pkgs {
          projectRootFile = "flake.nix";
          programs.nixfmt.enable = true;
        };

        mkProjectShell =
          extraPkgs:
          pkgs.mkShell {
            packages =
              (with pkgs; [
                dotnet-sdk_8
                libgit2
              ])
              ++ extraPkgs;

            DOTNET_ROOT = "${pkgs.dotnet-sdk_8}";
            LD_LIBRARY_PATH = "${pkgs.libgit2}/lib";
          };
      in
      {
        formatter = formatter.config.build.wrapper;

        checks = {
          formatting = formatter.config.build.check self;
        };

        devShells.default = mkProjectShell [ ];
        devShells.wrangler = mkProjectShell [ pkgs.wrangler ];
      }
    );
}
