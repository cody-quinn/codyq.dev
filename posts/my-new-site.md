---
date: 10/02/2021 3:30 AM
title: My new website. The good, the bad and the ugly.
summary: A deep dive into my new website, statically generated with NextJS
author: Cody
tags:
  - webdev
  - meta
---

### 👋 Hello

Welcome to the few of you reading this! This is my very first blog post on my new website. Kind of a meta post, as we will be discussing the process of actually making this blog.

### ⚙️ Static Site Generation

When I started making this website I wanted it to be **fast**! I personally have really bad internet, if I'm lucky I might get 5 mbps down, so I know what it feels like for a page to _load_. I figured the best way to have this, while still having ease of editing would be through Static Site Generation.

##### What is Static Site Generation

Static Site Generation is almost like having a compiler for your website.

Instead of the traditional approach where every single request contacts a database, gets all your posts, and renders it to a template static site generation does that upfront. It takes everything from your CMS, uses your templates and spits out all your HTML, CSS, and JavaScript ahead of time. This allows you to host the site on a CDN, reducing load times by physically caching the content closer to the users.

Sounds great? Well it isn't without downsides. Sadly as a result of Static Site Generation if you ever want dynamic content you need to re-compile your website. This is fine for simple things such as blogs where only few people will be editing, but not for platforms like social networks where data is constantly changing.

##### How are you doing it?

There are many libraries available for Static Site Generation, I personally am using [Next.js](https://nextjs.org). The main reason I decided to go for Next is because of past experience using it.

Some other SSG libraries I've heard are good, but haven't personally used are:

- [Gatsby](https://gatsbyjs.com)
- [Jekyll](https://jekyllrb.com) (Easiest)
- [Hugo](https://gohugo.io)

As for content management, even though there is alot of great software in the [Headless CMS]() space I decided to opt for plain ol markdown files and git. I personally am more comfortable in a plain text editor than a GUI text editor, which is easy enough to manage with Git. Additionally this gives me all the benifits of Git.

In order to parse the Markdown I'm using a combination of libraries. I'm using [gray-matter](https://www.npmjs.com/package/gray-matter) for getting metadata written in YAML out of the top of the markdown file, and am using [remark](https://www.npmjs.com/package/remark) & [rehype](https://www.npmjs.com/package/rehype) for parsing the actual markdown into HTML. The reason for rehype in addition to remark is mainly code-block support.

##### How are you deploying it?

I'm deploying it for **free** on [Netlify]()! This amazing platform has been supporting my websites for years, and so far I haven't been charged a penny. Keep in mind though they charge based on bandwidth, if you run out of the amount allocated for free it will begin to cost you. Though I find this unlikely for most personal websites.

Some alternatives to Netlify which are also great:

- [Vercel](https://vercel.com)
- [GitHub Pages](https://guides.github.com/features/pages/) (Perfect for Jekyll)

### 🎨 Design

I put very little thought into the design, though I do believe it came out looking decent. I used [TailwindCSS]() instead of CSS, and I liked it, alot! This was my first "real" project using it, and I can safely say I **will** be coming back.

### ⬛️ Code Blocks

Code blocks were a pain, but I got them working! This was a requirement in my eyes, for a blog that will be centered around coding not to have well supported code blocks would be an embarrasment. I'm using the library [rehype-highlight](https://www.npmjs.com/package/rehype-highlight), which is an addon to rehype, the library I'm using for parsing the markdown itself.

```javascript
function count() {
  let num = 0;
  setInterval(() => {
    console.log(++num);
  }, 1000);
}

count();
```

### 💙 Thank you!

I would like to thank everyone who took their time to read this, ya'll are great!
