import { useState } from "react";
import { InlineLink, PageContainer, Section } from "../components/components";
import { getPosts } from "../lib/posts";
import Head from "next/head";

const Home = ({ posts }) => (
  <PageContainer>
    <Head>
      <title>codyq.me</title>
      <meta name='title' content='codyq.me' />
      <meta
        name='description'
        content="My name is Cody! I'm a highschool student interested in Computer Science, Programming and Photography. In my freetime I work on various projects of which I have many. Most can befound on my GitHub."
      />

      <meta property='og:type' content='website' />
      <meta property='og:url' content='https://codyq.me/' />
      <meta property='og:title' content='codyq.me' />
      <meta
        property='og:description'
        content="My name is Cody! I'm a highschool student interested in Computer Science, Programming and Photography. In my freetime I work on various projects, of which I have many. Most can befound on my GitHub."
      />

      <meta property='twitter:url' content='https://codyq.me/' />
      <meta property='twitter:title' content='codyq.me' />
      <meta
        property='twitter:description'
        content="My name is Cody! I'm a highschool student interested in Computer Science, Programming and Photography. In my freetime I work on various projects, of which I have many. Most can befound on my GitHub."
      />
    </Head>

    <Section header='👋 Hello'>
      <p>
        My name is Cody! I'm a highschool student interested in Computer Science, Programming and
        Photography. In my freetime I work on various projects, of which I have many. Most can be
        found on my <InlineLink href='https://github.com/CatDevz'>GitHub</InlineLink>!
      </p>
      <p>
        I know and use various languages such as Kotlin, Java, JavaScript and Python. Many of my
        projects are related to server-side Minecraft development, however I have a strong interest
        in fullstack web development.
      </p>
    </Section>
    <Section header='📝 Recent blog posts'>
      <ul className='list-disc list-inside'>
        {posts.map((post) => (
          <li key={post.slug}>
            <InlineLink href={`/blog/${post.slug}`} newTab={false}>
              {post.title}
            </InlineLink>
          </li>
        ))}
      </ul>
      <InlineLink href='/blog' newTab={false}>
        View more...
      </InlineLink>
    </Section>
    <Section header='📭️ Contact me'>
      <p>Email: inbox@codyq.me</p>
    </Section>
  </PageContainer>
);

export async function getStaticProps() {
  const posts = getPosts().slice(0, 5);

  return {
    props: {
      posts,
    },
  };
}

export default Home;
