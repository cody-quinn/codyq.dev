import "../styles/globals.css";
import "highlight.js/styles/atom-one-dark.css";
import { useRouter } from "next/router";
import { useEffect } from "react";

const GA_TRACKING_ID = process.env.NEXT_PUBLIC_ANALYTICS_ID;

function MyApp({ Component, pageProps }) {
  const router = useRouter();

  useEffect(() => {
    const handleRouteChange = (url) => {
      window.gtag("config", GA_TRACKING_ID, {
        page_path: url,
      });
    };

    router.events.on("routeChangeComplete", handleRouteChange);
    return () => {
      router.events.off("routeChangeComplete", handleRouteChange);
    };
  }, [router.events]);

  return <Component {...pageProps} />;
}

export default MyApp;
