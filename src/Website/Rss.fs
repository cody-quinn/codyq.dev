namespace Website.Markdown

open Falco.Markup

[<RequireQualifiedAccess>]
module Rss =
  let root =
    Elem.create "rss" [
      Attr.create "version" "2.0"
      Attr.create "xmlns:atom" "http://www.w3.org/2005/Atom"
      Attr.create "xmlns:dc" "http://purl.org/dc/elements/1.1/"
    ]

  let channel = Elem.create "channel"
  let title = Elem.create "title"
  let description = Elem.create "description"
  let language = Elem.create "language"
  let lastBuildDate = Elem.create "lastBuildDate"
  let pubDate = Elem.create "pubDate"
  let item = Elem.create "item"
  let link = Elem.create "link"
  let guid = Elem.create "guid" // Same as link, since links are perma for me
