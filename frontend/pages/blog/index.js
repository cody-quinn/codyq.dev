import { InlineLink, PageContainer, Section } from "../../components/components";
import { getPosts } from "../../lib/posts";

const Blog = ({ posts }) => {
  return (
    <PageContainer>
      <Section header='📝 My Blog'>
        <p>
          Welcome to my blog, here I will occationally write about my various projects, findings and
          honestly whatever I want. Who knows, maybe one day I'll make a post about what I had for
          breakfast.
        </p>
        <ul className='list-disc list-inside'>
          {posts.map((post) => (
            <li>
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
