module Website.Program

open System
open System.IO
open Falco.Markup
open Markdig
open Markdown.ColorCode

let badges =
  let badges =
    [ "https://versary.town",
      [ "antinft.gif", "This is an Anti-NFT site", None
        "piracy.gif", "Piracy Now! (Free)", None
        "lan.gif", "Screw y'all I'm going back to my LAN", None ]

      "http://88x31.nl",
      [ "seedyourtorrents.gif", "Sharing is caring... Seed your torrents", None
        "ltt.gif", "Free tech tips", None
        "linux-p.gif", "Linux powered", None
        "homesite.gif", "Homesite Now!", None
        "hair.gif", "No bad hairday on the internet!", None
        "antinft2.gif", "NFT? No fucking thanks", None
        "neocities-now.gif", "Neocities Now!", None
        "nclinux.gif", "Linux Today!", None ]

      "https://anlucas.neocities.org/88x31Buttons", [ "drpepper.gif", "Powered by Dr. Pepper", None ]

      "friends",
      [ "friends/gaffclant.png", "Gaffclant NOW", (Some "https://gaffclant.dev")
        "friends/goldenstack.png", "GoldenStack (1.0)", (Some "https://goldenstack.net")
        "friends/mudkip.png", "Mudkip", (Some "https://mudkip.dev")
        "friends/emortal.png", "Emortal", (Some "https://emortaldev.github.io/") ] ]

  let elements =
    badges
    |> List.map snd
    |> List.concat
    |> List.map (fun (path, title, url) ->
      let img =
        Elem.img [
          Attr.src $"/badges/%s{path}"
          Attr.alt title
          Attr.title title
        ]

      match url with
      | None -> img
      | Some url ->
        Elem.a [
          Attr.href url
          Attr.target "_blank"
          Attr.rel "noopener noreferrer"
        ] [ img ])

  let sources =
    badges
    |> List.map fst
    |> List.filter (String.IsNullOrEmpty >> not)
    |> List.distinct
    |> List.fold (fun curr e -> $"%s{curr}\n- %s{e}") "\n88x31 sources:"

  Elem.create "marquee" [] (Text.comment sources :: elements)

module Header =
  let meta name content =
    Elem.meta [
      Attr.name name
      Attr.content content
    ]

  let title title = Elem.title [] [ Text.raw title ]

  module Link =
    let link rel href =
      Elem.link [
        Attr.rel rel
        Attr.href href
      ]

    let stylesheet = link "stylesheet"
    let icon = link "icon"

    let preload href as' type' =

      Elem.link [
        Attr.rel "preload"
        Attr.href href
        Attr.create "as" as'
        Attr.type' type'
        Attr.crossorigin ""
      ]

module Templates =
  let page title header body footer =
    Templates.html5 "en" [
      yield! header
      Header.Link.stylesheet "/styles.css"
      Header.Link.icon "data:;base64,iVBORw0KGgo="
    ] [
      Elem.header [] [
        Elem.span [ Attr.id "logo" ] [ Text.raw title ]
        Elem.nav [] [ Elem.a [ Attr.href "/" ] [ Text.raw "Home" ] ]
      ]

      Elem.main [] body

      Elem.footer [] [
        yield! footer
        Elem.p [] [
          Text.raw "Site contents is licensed under "
          Elem.a [
            Attr.href "https://creativecommons.org/licenses/by/4.0"
            Attr.target "_blank"
            Attr.rel "license noopener noreferrer"
          ] [ Text.raw "CC BY 4.0" ]
          Text.raw "."
          Elem.br []
          Text.raw "Site "
          Elem.a [
            Attr.href "https://github.com/cody-quinn/codyq.dev"
            Attr.target "_blank"
            Attr.rel "noopener noreferrer"
          ] [ Text.raw "source code" ]
          Text.raw " is licensed under "
          Elem.a [
            Attr.href "https://github.com/cody-quinn/codyq.dev/blob/master/LICENSE"
            Attr.target "_blank"
            Attr.rel "noopener noreferrer"
          ] [ Text.raw "MIT" ]
          Text.raw "."
        ]
      ]
    ]

  type PageTemplate = string -> string -> XmlNode

  let homepage : PageTemplate =
    fun meta content ->
      page "Cody Quinn's Homepage" [ Header.title "Cody Quinn's Homepage" ] [ Text.raw content ] [ badges ]

let compilePage template src dest =
  // Function to split the document into meta & content
  let splitDocument (src : string) =
    if src.StartsWith "---" then
      let end' = src.IndexOf("---", 3)

      if end' <> -1 then
        src.Substring(3, end' - 3) |> _.Trim(), src.Substring(end' + 3) |> _.Trim()
      else
        "", src
    else
      "", src

  // Pipeline to convert markdown to HTML
  let pipeline =
    MarkdownPipelineBuilder()
      .UseAdvancedExtensions()
      .UseColorCode(HtmlFormatterType.Style)
      .Build()

  let document = File.ReadAllText src
  let meta, content = splitDocument document
  let contentHtml = Markdown.ToHtml(content, pipeline)
  let pageHtml = template meta contentHtml |> renderHtml

  File.WriteAllText(dest, pageHtml)

[<EntryPoint>]
let main _ =
  // Delete the dist folder if it exists
  if Directory.Exists "dist" then
    Directory.Delete("dist", true)

  // Copy everything from the assets folder to the dist folder
  for path in Directory.GetDirectories("assets", "*", SearchOption.AllDirectories) do
    Directory.CreateDirectory($"dist/{path.Substring(6)}") |> ignore

  for path in Directory.GetFiles("assets", "*", SearchOption.AllDirectories) do
    File.Copy(path, $"dist/{path.Substring(6)}")

  // Compile basic pages from markdown
  compilePage Templates.homepage "data/index.md" "dist/index.html"
  compilePage Templates.homepage "data/404.md" "dist/404.html"

  0
