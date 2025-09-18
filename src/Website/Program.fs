module Website.Program

open System
open System.IO
open System.Reflection
open System.Security.Cryptography
open System.Text
open FSharp.Data
open Falco.Markup
open LibGit2Sharp
open Markdig
open Markdown.ColorCode
open YamlDotNet.Serialization

let pwd =
  Environment.GetEnvironmentVariable "PWD"
  |> Option.ofObj
  |> Option.defaultValue Environment.CurrentDirectory

let sha256Hash value =
  (value : string)
  |> Encoding.ASCII.GetBytes
  |> SHA256.HashData
  |> Array.fold (fun curr next -> curr + next.ToString("x2")) ""

let readEmbeddedResource name =
  let asm = Assembly.GetExecutingAssembly()
  use stream = asm.GetManifestResourceStream(name)
  use reader = new StreamReader(stream)
  reader.ReadToEnd()

[<RequireQualifiedAccess>]
module Json =
  let findStringProperty name default' value =
    JsonExtensions.TryGetProperty(value, name)
    |> Option.map JsonExtensions.AsString
    |> Option.defaultValue default'

  let findBooleanProperty name default' value =
    JsonExtensions.TryGetProperty(value, name)
    |> Option.map JsonExtensions.AsBoolean
    |> Option.defaultValue default'

let badges =
  let badges =
    [ "https://versary.town",
      [ "antinft.gif", "This is an Anti-NFT site", None
        "lan.gif", "Screw y'all I'm going back to my LAN", None ]

      "https://88x31.nl",
      [ "ltt.gif", "Free tech tips", None
        "linux-p.gif", "Linux powered", None
        "homesite.gif", "Homesite Now!", None
        "hair.gif", "No bad hairday on the internet!", None
        "antinft2.gif", "NFT? No fucking thanks", None
        "neocities-now.gif", "Neocities Now!", None
        "nclinux.gif", "Linux Today!", None ]

      "https://anlucas.neocities.org/88x31Buttons", [ "drpepper.gif", "Powered by Dr. Pepper", None ]

      "friends",
      [ "friends/gaffclant.png", "Gaffclant NOW", (Some "https://github.com/gaffclant")
        "friends/goldenstack.png", "GoldenStack (1.0)", (Some "https://goldenstack.net")
        "friends/mat.png", "Mat does dev! :3", (Some "https://matdoes.dev")
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

  let duration = (List.length elements |> float) * 1.5 |> int

  Elem.div [ Attr.class' "marquee" ] [
    Text.comment sources
    Elem.div
      [ Attr.class' "marquee-inner"
        Attr.style $"animation-duration:%i{duration}s;height:31px" ]
      elements
  ]

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

type PageMetadata =
  { Properties : JsonValue
    SourcePath : string
    DestinationPath : string
    LatestAffectingCommit : Commit option
    LatestCommit : Commit option }

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
          Text.raw "Site design stolen from GoldenStack @ "
          Elem.a [
            Attr.href "https://goldenstack.net"
          ] [ Text.raw "goldenstack.net" ]
          Text.raw "."
          Elem.br []
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
        Elem.noscript [] [ Text.p "Some features may be unavailable without JavaScript." ]
      ]

      Elem.script [ Attr.type' "application/javascript" ] [ Text.raw (readEmbeddedResource "Website.universal.js") ]
    ]

  type PageTemplate = PageMetadata -> string -> XmlNode

  let generic : PageTemplate =
    fun meta content ->
      let props = meta.Properties

      let header = Json.findStringProperty "pageHeader" "Cody Quinn's Homepage" props
      let title = Json.findStringProperty "pageTitle" header props
      let showBadges = Json.findBooleanProperty "showBadges" true props
      let showHistory = Json.findBooleanProperty "showHistory" true props

      let history =
        meta.LatestAffectingCommit
        |> Option.map (fun commit ->
          let relativePath = meta.SourcePath.Substring(pwd.Length + 1)

          let commitHash = commit.Sha
          let pathHash = sha256Hash relativePath

          let commitDateTime = commit.Author.When.UtcDateTime
          let commitTime = commitDateTime.ToString("h:mm:ss tt")
          let commitDate = commitDateTime.ToString("M/d/yyyy")

          // Render out the element
          Elem.p [] [
            Text.raw "Page last modified on "
            Elem.span [
              Attr.dataAttr "timestamp" $"{commit.Author.When.ToUnixTimeSeconds()}"
              Attr.dataAttr "timestamp-format" "{D} at {T}"
            ] [ Text.raw $"{commitDate} at {commitTime} UTC" ]
            Text.raw ". "
            Elem.a [
              Attr.href $"https://github.com/cody-quinn/codyq.dev/commits/master/{relativePath}"
              Attr.target "_blank"
              Attr.rel "noopener noreferrer"
            ] [ Text.raw "History" ]
            Text.raw ". "
            Elem.a [
              Attr.href $"https://github.com/cody-quinn/codyq.dev/commit/{commitHash}#diff-{pathHash}"
              Attr.target "_blank"
              Attr.rel "noopener noreferrer"
            ] [ Text.raw "Diff" ]
            Text.raw "."
          ])
        |> Option.defaultValue (Text.p "Page commit info is unavailable.")

      page header [ Header.title title ] [ Text.raw content ] [
        if showBadges then
          badges
        if showHistory then
          history
      ]

let compilePage src dest =
  // Function to convert yaml to json (because the yaml library sucks)
  let yamlToJson yaml =
    if String.IsNullOrWhiteSpace yaml then
      JsonValue.Record [||]
    else
      yaml
      |> DeserializerBuilder().Build().Deserialize
      |> SerializerBuilder().JsonCompatible().Build().Serialize
      |> JsonValue.Parse

  // Function to split the document into meta & content
  let splitDocument (src : string) =
    let end' = src.IndexOf("---", 3)

    if src.StartsWith "---" && end' <> -1 then
      src.Substring(3, end' - 3).Trim() |> yamlToJson, src.Substring(end' + 3).Trim()
    else
      JsonValue.Record [||], src

  // Pipeline to convert markdown to HTML
  let pipeline =
    MarkdownPipelineBuilder().UseAdvancedExtensions().UseColorCode(HtmlFormatterType.Style).Build()

  let document = File.ReadAllText src
  let properties, content = splitDocument document

  use gitRepo = new Repository(pwd)

  let gitCommit =
    let relativePath = src.Substring(pwd.Length + 1)
    let gitFileHistory = gitRepo.Commits.QueryBy(relativePath).GetEnumerator()

    if gitFileHistory.MoveNext() then
      Some gitFileHistory.Current.Commit
    else
      None

  let meta =
    { Properties = properties
      SourcePath = src
      DestinationPath = dest
      LatestAffectingCommit = gitCommit
      LatestCommit = Some gitRepo.Head.Tip }

  let template =
    match Json.findStringProperty "template" "generic" meta.Properties with
    | "generic" -> Templates.generic
    | value ->
      eprintfn $"Unknown template '{value}'"
      exit 1

  let contentHtml = Markdown.ToHtml(content, pipeline)
  let pageHtml = template meta contentHtml |> renderHtml

  File.WriteAllText(dest, pageHtml)

[<EntryPoint>]
let main _ =
  let inputDirectory = Path.Join(pwd, "data")
  let outputDirectory = Path.Join(pwd, "dist")

  // Delete the dist folder if it exists
  if Directory.Exists outputDirectory then
    Directory.Delete(outputDirectory, true)

  // Copy everything from the input folder to the output folder
  for path in Directory.GetDirectories(inputDirectory, "*", SearchOption.AllDirectories) do
    let outputPath = Path.Join(outputDirectory, path.Substring(inputDirectory.Length))
    Directory.CreateDirectory(outputPath) |> ignore

  for path in Directory.GetFiles(inputDirectory, "*", SearchOption.AllDirectories) do
    let outputPath = Path.Join(outputDirectory, path.Substring(inputDirectory.Length))

    if Path.GetExtension path = ".md" then
      // If the page is a markdown page we need to process it
      let outputPath = Path.ChangeExtension(outputPath, ".html")
      compilePage path outputPath
      printfn $"Compiled: {path}"
    else
      File.Copy(path, outputPath)
      printfn $"Copied: {path}"

  0
