import { InlineLink, PageContainer, Section } from "../../components/components";
import { getPosts } from "../../lib/posts";
import Head from "next/head";

const Blog = ({ posts }) => {
  return (
    <PageContainer>
      <Head>
        <title>codyq.me - Blog</title>
        <meta name='title' content='codyq.me - Blog' />
        <meta
          name='description'
          content={`Recent blog posts:\n${posts.map((post) => post.title).join("\n")}`}
        />

        <meta property='og:type' content='website' />
        <meta property='og:url' content='https://codyq.me/blog/' />
        <meta property='og:title' content='codyq.me - Blog' />
        <meta
          property='og:description'
          content={`Recent blog posts:\n${posts.map((post) => post.title).join("\n")}`}
        />

        <meta property='twitter:url' content='https://codyq.me/blog/' />
        <meta property='twitter:title' content='codyq.me - Blog' />
        <meta
          property='twitter:description'
          content={`Recent blog posts:\n${posts.map((post) => post.title).join("\n")}`}
        />
      </Head>

      <Section header='📝 My Blog'>
        <p>
          Welcome to my blog, here I will occationally write about my various projects, findings and
          honestly whatever I want. Who knows, maybe one day I'll make a post about what I had for
          breakfast.
        </p>
        <ul className='list-disc list-inside'>
          {posts.map((post) => (
            <li key={post.slug}>
              <InlineLink href={`/blog/${post.slug}`} newTab={false}>
                {post.title}
              </InlineLink>
            </li>
          ))}
        </ul>
      </Section>
    </PageContainer>
  );
};

export async function getStaticProps() {
  const posts = getPosts();

  return {
    props: {
      posts,
    },
  };
}

export default Blog;
