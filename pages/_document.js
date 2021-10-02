import Document, { Html, Head, Main, NextScript } from "next/document";

const GA_TRACKING_ID = process.env.NEXT_PUBLIC_ANALYTICS_ID;

function MyDocument() {
  return (
    <Html>
      <Head>
        <link rel='icon' href='data:;base64,=' />

        <script
          async
          src={`https://www.googletagmanager.com/gtag/js?id=${GA_TRACKING_ID}`}
        ></script>
        <script
          dangerouslySetInnerHTML={{
            __html: `
              window.dataLayer = window.dataLayer || [];
              function gtag(){dataLayer.push(arguments);}
              gtag('js', new Date());
            
              gtag('config', '${GA_TRACKING_ID}', {
                page_path: window.location.pathname,
              });
            `,
          }}
        />
      </Head>
      <body>
        <Main />
        <NextScript />
      </body>
    </Html>
  );
}

export default MyDocument;
