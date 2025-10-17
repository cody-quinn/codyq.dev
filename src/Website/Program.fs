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

let inputDirectory = Path.Join(pwd, "data")
let outputDirectory = Path.Join(pwd, "dist")

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
        "friends/emortal.png", "Emortal", (Some "https://emortaldev.github.io/")
        "friends/ollie.webp", "Ollie", (Some "https://ollie.lol") ] ]

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

type Page =
  { Contents : string // Represented as markdown
    Properties : JsonValue
    SourcePath : string
    DestinationPath : string
    LatestAffectingCommit : Commit option
    LatestCommit : Commit option }

type Context = { AllPages : Page list }

module Templates =
  let pageBase title header body footer =
    Templates.html5 "en" [
      yield! header
      Header.Link.stylesheet "/styles.css"
      Header.Link.icon "data:;base64,iVBORw0KGgo="
    ] [
      Elem.header [] [
        Elem.span [ Attr.id "logo" ] [ Text.raw title ]
        Elem.nav [] [
          Elem.a [ Attr.href "/" ] [ Text.raw "Home" ]
          Elem.a [ Attr.href "/projects" ] [ Text.raw "Projects" ]
          Elem.a [ Attr.href "/journal" ] [ Text.raw "Journal" ]
        ]
      ]

      Elem.main [] body

      Elem.footer [] [
        yield! footer
        Elem.p [] [
          Text.raw "Site design stolen from GoldenStack @ "
          Elem.a [ Attr.href "https://goldenstack.net" ] [ Text.raw "goldenstack.net" ]
          Text.raw "."
          Elem.br []
          Text.raw "Site content is licensed under "
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

  let genericWrapper page body =
    let props = page.Properties

    let header = Json.findStringProperty "pageHeader" "Cody's Homepage" props
    let footer = Json.findStringProperty "pageFooter" "" props
    let title = Json.findStringProperty "pageTitle" header props
    let showBadges = Json.findBooleanProperty "showBadges" true props
    let showHistory = Json.findBooleanProperty "showHistory" true props

    let history =
      page.LatestAffectingCommit
      |> Option.map (fun commit ->
        let relativePath = page.SourcePath.Substring(pwd.Length + 1)

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

    pageBase header [ Header.title title ] body [
      if showBadges then
        badges
      if String.IsNullOrWhiteSpace footer |> not then
        Text.p footer
      if showHistory then
        history
    ]

  type PageTemplate = Context -> Page -> string -> XmlNode

  let generic : PageTemplate =
    fun ctx page content -> genericWrapper page [ Text.raw content ]

  module Journal =
    let private getTimeDisplay (entry : Page) =
      let path =
        Path.GetRelativePath(Path.Join(inputDirectory, "journal"), entry.SourcePath)

      let parts = path.Split("/")

      let entryYear = parts[0]
      let entryMonth = parts[1]
      let entryDay = Path.GetFileNameWithoutExtension(path)

      let dayText =
        match entryDay.ToCharArray() with
        | [| '0'; '1' |] -> $"1st"
        | [| '0'; '2' |] -> $"2nd"
        | [| '0'; '3' |] -> $"3rd"
        | [| '1'; s |] -> $"1{s}th"
        | [| s; '1' |] -> $"{s}1st"
        | [| s; '2' |] -> $"{s}2nd"
        | [| s; '3' |] -> $"{s}3rd"
        | [| s1; s2 |] -> $"{s1}{s2}th"
        | _ -> entryDay

      let monthText =
        match entryMonth with
        | "01" -> "January"
        | "02" -> "February"
        | "03" -> "March"
        | "04" -> "April"
        | "05" -> "May"
        | "06" -> "June"
        | "07" -> "July"
        | "08" -> "August"
        | "09" -> "September"
        | "10" -> "October"
        | "11" -> "November"
        | "12" -> "December"
        | _ -> entryMonth

      $"{monthText} {dayText}, {entryYear}"

    let aggregate : PageTemplate =
      fun ctx page content ->
        // Pipeline to convert markdown to HTML
        let pipeline =
          MarkdownPipelineBuilder().UseAdvancedExtensions().UseColorCode(HtmlFormatterType.Style).Build()

        let entryDisplay (entry : Page) =
          let url = "/" + Path.GetRelativePath(outputDirectory, entry.DestinationPath)
          let timeText = getTimeDisplay entry

          Elem.div [] [
            Elem.h3 [] [
              Text.raw timeText
              Text.raw " ["
              Elem.a [ Attr.href url ] [ Text.raw "Permalink" ]
              Text.raw "]"
            ]

            Text.raw (Markdown.ToHtml(entry.Contents, pipeline))
          ]

        let entries =
          ctx.AllPages
          |> List.filter (fun it -> Path.GetRelativePath(inputDirectory, it.SourcePath).StartsWith("journal/"))
          |> List.filter (fun it -> Json.findStringProperty "template" "generic" it.Properties = "journal-entry")
          |> List.sortByDescending (_.SourcePath)
          |> List.map (entryDisplay >> renderHtml)
          |> List.reduce (+)

        let content = content.Replace("<journal-entries />", entries)

        genericWrapper page [ Text.raw content ]

    let entry : PageTemplate =
      fun ctx page content ->
        let timeText = getTimeDisplay page

        genericWrapper page [
          Text.h3 timeText
          Text.raw content
        ]

let processPage (gitRepo : Repository) src dest =
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
    if src.Length >= 3 then
      let end' = src.IndexOf("---", 3)

      if src.StartsWith "---" && end' <> -1 then
        src.Substring(3, end' - 3).Trim() |> yamlToJson, src.Substring(end' + 3).Trim()
      else
        JsonValue.Record [||], src
    else
      JsonValue.Record [||], src

  let document = File.ReadAllText src
  let properties, content = splitDocument document

  // Page properties can be resolved from multiple areas. If a file `_properties.yaml` exists in a
  // given directory the properties inside will be applied to every document inside that directory
  // and child directories. The header, then the nearest file takes priority.
  let mergeProperties (super : (string * JsonValue) list) (child : (string * JsonValue) list) =
    let super = Map.ofList super
    let child = Map.ofList child
    Map.fold (fun acc k v -> Map.add k v acc) super child |> Map.toList

  let rec inheritedProperties (child : (string * JsonValue) list) (currentPath : string) =
    let root = Path.Join(pwd, "data")
    let directory = Path.GetDirectoryName currentPath
    let path = Path.Join(directory, ".properties.yaml")

    let props =
      if File.Exists path then
        let super = File.ReadAllText path |> yamlToJson |> _.Properties() |> List.ofArray
        mergeProperties super child
      else
        child

    // If we're not yet at the root, continue scanning upwards
    if root <> directory then
      inheritedProperties props directory
    else
      props

  let properties =
    inheritedProperties (List.ofArray <| properties.Properties()) src
    |> List.toArray
    |> JsonValue.Record

  let gitCommit =
    let relativePath = src.Substring(pwd.Length + 1)
    let gitFileHistory = gitRepo.Commits.QueryBy(relativePath).GetEnumerator()

    if gitFileHistory.MoveNext() then
      Some gitFileHistory.Current.Commit
    else
      None

  { Contents = content
    Properties = properties
    SourcePath = src
    DestinationPath = dest
    LatestAffectingCommit = gitCommit
    LatestCommit = Some gitRepo.Head.Tip }

let writePage (ctx : Context) (page : Page) =
  // Pipeline to convert markdown to HTML
  let pipeline =
    MarkdownPipelineBuilder().UseAdvancedExtensions().UseColorCode(HtmlFormatterType.Style).Build()

  let template =
    match Json.findStringProperty "template" "generic" page.Properties with
    | "generic" -> Templates.generic
    | "journal-aggregate" -> Templates.Journal.aggregate
    | "journal-entry" -> Templates.Journal.entry
    | value ->
      eprintfn $"Unknown template '{value}'"
      exit 1

  let contentHtml = Markdown.ToHtml(page.Contents, pipeline)
  let pageHtml = template ctx page contentHtml |> renderHtml

  File.WriteAllText(page.DestinationPath, pageHtml)

[<EntryPoint>]
let main _ =
  use gitRepo = new Repository(pwd)

  // Delete the dist folder if it exists
  if Directory.Exists outputDirectory then
    Directory.Delete(outputDirectory, true)

  // Copy everything from the input folder to the output folder
  for path in Directory.GetDirectories(inputDirectory, "*", SearchOption.AllDirectories) do
    let outputPath = Path.Join(outputDirectory, path.Substring(inputDirectory.Length))
    Directory.CreateDirectory(outputPath) |> ignore

  let pages =
    [ for path in Directory.GetFiles(inputDirectory, "*.md", SearchOption.AllDirectories) do
        let outputPath = Path.Join(outputDirectory, path.Substring(inputDirectory.Length))
        let outputPath = Path.ChangeExtension(outputPath, ".html")
        processPage gitRepo path outputPath ]

  let context : Context = { AllPages = pages }

  for page in pages do
    writePage context page
    printfn $"Compiled: {page.DestinationPath}"

  for path in Directory.GetFiles(inputDirectory, "*", SearchOption.AllDirectories) do
    let outputPath = Path.Join(outputDirectory, path.Substring(inputDirectory.Length))

    if pages |> List.map (_.SourcePath) |> List.contains path then
      // Page has already been processed
      ()
    else if Path.GetFileName path = ".properties.yaml" then
      printfn $"Skipping: {path}"
    else
      File.Copy(path, outputPath)
      printfn $"Copied: {path}"

  0
