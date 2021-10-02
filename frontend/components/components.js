import Link from "next/link";

export const Navbar = () => (
  <div className='flex justify-between items-center py-4 h-16 sticky top-0 border-b-2 bg-white border-gray-100 border-solid'>
    <div>codyq.me</div>
    <div className='flex gap-x-4'>
      <Link href='/'>Home</Link>
      <Link href='/blog'>Blog</Link>
    </div>
  </div>
);

export const Container = ({ children, ...props }) => (
  <div className='min-w-screen min-h-screen bg-gray-50'>
    <div className='max-w-screen-md min-h-screen mx-auto px-6 md:px-12 bg-white' {...props}>
      {children}
    </div>
  </div>
);

export const PageContainer = ({ children, ...props }) => (
  <Container {...props}>
    <Navbar />
    <div className='my-8 flex flex-col gap-8'>{children}</div>
  </Container>
);

export const Header = ({ children }) => <p className='font-bold text-xl'>{children}</p>;

export const Section = ({ children, header, ...props }) => (
  <section {...props}>
    <Header>{header}</Header>
    <div className='flex flex-col gap-2'>{children}</div>
  </section>
);

export const InlineLink = ({ children, href = "", newTab = true, ...props }) => (
  <Link href={href} {...props}>
    <a
      target={newTab ? "_blank" : ""}
      rel={newTab ? "noreferrer" : ""}
      className='text-blue-600 cursor-pointer hover:underline'
    >
      {children}
    </a>
  </Link>
);

export const CopyrightNotice = () => (
  <p cc='http://creativecommons.org/ns#' className='text-xs'>
    This work is licensed under{" "}
    <a
      href='http://creativecommons.org/licenses/by-sa/4.0/?ref=chooser-v1'
      target='_blank'
      rel='license noopener noreferrer'
      style={{ display: "inline-flex" }}
    >
      CC BY-SA 4.0
      <img
        style={{ height: 18, marginLeft: 3, alignSelf: "center" }}
        src='https://mirrors.creativecommons.org/presskit/icons/cc.svg?ref=chooser-v1'
      />
      <img
        style={{ height: 18, marginLeft: 3, alignSelf: "center" }}
        src='https://mirrors.creativecommons.org/presskit/icons/by.svg?ref=chooser-v1'
      />
      <img
        style={{ height: 18, marginLeft: 3, alignSelf: "center" }}
        src='https://mirrors.creativecommons.org/presskit/icons/sa.svg?ref=chooser-v1'
      />
    </a>
  </p>
);
