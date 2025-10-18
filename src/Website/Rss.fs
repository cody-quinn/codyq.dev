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

  let title text = Elem.create "title" [] [ Text.raw text ]
  let link link = Elem.create "link" [] [ Text.raw link ]
  let guid link = Elem.create "guid" [] [ Text.raw link ] // Same as link, since links are perma for me
  let description text = Elem.create "description" [] [ Text.raw text ]
  let language lang = Elem.create "language" [] [ Text.raw lang ]
  let copyright text = Elem.create "copyright" [] [ Text.raw text ]
  let managingEditor email name = Elem.create "managingEditor" [] [ Text.raw $"{email} ({name})" ]
  let webMaster email name = Elem.create "webMaster" [] [ Text.raw $"{email} ({name})" ]

  let pubDate = Elem.create "pubDate"
  let lastBuildDate = Elem.create "lastBuildDate"

  let ttl time = Elem.create "ttl" [] [ Text.raw $"%i{time}" ]

  let item = Elem.create "item"
