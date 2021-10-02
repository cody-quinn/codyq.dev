import fs from "fs";
import matter from "gray-matter";
import path from "path";

const postDirectory = path.join(process.cwd(), "posts");

export const getPosts = () => {
  const filenames = fs.readdirSync(postDirectory);
  const postdata = filenames.map((filename) => {
    const slug = filename.replace(/\.md$/, "");
    const fullpath = path.join(postDirectory, filename);
    const filecontents = fs.readFileSync(fullpath, "utf-8");
    const matterresult = matter(filecontents);

    return {
      slug,
      ...matterresult.data,
    };
  });

  return postdata.sort((a, b) => (a.date < b.date ? 1 : -1));
};

export const getPost = (slug) => {
  const fullpath = path.join(postDirectory, slug + ".md");
  const filecontents = fs.readFileSync(fullpath, "utf-8");
  const matterresult = matter(filecontents);

  return {
    slug,
    content: matterresult.content,
    ...matterresult.data,
  };
};
