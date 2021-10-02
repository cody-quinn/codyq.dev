import { CopyrightNotice, Header, OSSNotice, PageContainer } from "../../components/components";
import { getPost, getPosts } from "../../lib/posts";
import styles from "../../styles/blogpost.module.css";
import Head from "next/head";

import { unified } from "unified";
import remarkParse from "remark-parse";
import remarkRehype from "remark-rehype";
import rehypeDocument from "rehype-document";
import rehypeHighlight from "rehype-highlight";
import rehypeStringify from "rehype-stringify";

const BlogPost = ({ post }) => {
  return (
    <PageContainer>
      <Head>
        <title>codyq.me - {post.title}</title>
        <meta name='title' content={post.title} />
        <meta name='description' content={`"${post.summary}"`} />

        <meta property='og:type' content='website' />
        <meta property='og:url' content={`https://codyq.me/blog/${post.slug}`} />
        <meta property='og:title' content={post.title} />
        <meta property='og:description' content={`"${post.summary}"`} />

        <meta property='twitter:url' content={`https://codyq.me/blog/${post.slug}`} />
        <meta property='twitter:title' content={post.title} />
        <meta property='twitter:description' content={`"${post.summary}"`} />
      </Head>

      <div>
        <Header>{post.title}</Header>
        <i>"{post.summary}"</i>
        <p className='text-sm'>{post.date}</p>
      </div>
      <div className={styles.post} dangerouslySetInnerHTML={{ __html: post.content }} />
      <div>
        <CopyrightNotice />
      </div>
    </PageContainer>
  );
};

export async function getStaticPaths() {
  const posts = getPosts();

  return {
    paths: posts.map((post) => `/blog/${post.slug}`),
    fallback: false,
  };
}

export async function getStaticProps({ params }) {
  const post = getPost(params.slug);
  post.content = await unified()
    .use(remarkParse)
    .use(remarkRehype)
    .use(rehypeDocument)
    .use(rehypeHighlight)
    .use(rehypeStringify)
    .process(post.content)
    .then((file) => String(file));

  return {
    props: {
      post,
    },
  };
}

export default BlogPost;
